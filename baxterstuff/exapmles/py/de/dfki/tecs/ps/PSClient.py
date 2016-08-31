#!/usr/bin/python2.7
# coding: utf8
##
# The Creative Commons CC-BY-NC 4.0 License
#
# http://creativecommons.org/licenses/by-nc/4.0/legalcode
#
# Creative Commons (CC) by DFKI GmbH
#  - Christian Bürckert <Christian.Buerckert@DFKI.de>
#  - Yannick Körber <Yannick.Koerber@DFKI.de>
#  - Magdalena Kaiser <Magdalena.Kaiser@DFKI.de>

# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
# IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
# FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
# AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
# LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
# FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
# IN THE SOFTWARE.
#


import sys, glob
import socket
import re
import time
import os
import platform


from thrift import Thrift
from thrift.transport import TSocket
from thrift.transport import TTransport
from thrift.protocol import TBinaryProtocol 
from datetime import datetime
from collections import deque
from struct import *
from uritools import urisplit

from de.dfki.tecs.discovery.DiscoveryTree import *
from de.dfki.tecs.basetypes.constants import *
from de.dfki.tecs.basetypes.ttypes import *
from de.dfki.tecs.Event import *
from de.dfki.tecs.misc.RingBuffer import *
from de.dfki.tecs.misc.EventFrame import *


def current_millis():
	"""
	This function delivers a best equivivalent to Java's System.getCurrentMillis() and returns 
	the milliseconds since the unix epoch
	"""
	return int(round(time.time() * 1000))


def SerializeThriftMsg(msg):
    """
	Helps serializing a thrift message
    """
    msg.validate()
    transportOut = TTransport.TMemoryBuffer()
    protocolOut = TBinaryProtocol.TBinaryProtocol(transportOut)
    msg.write(protocolOut)
    return transportOut.getvalue()


def DeserializeThriftMsg(msg, data):
    """
	Helps deserializing a thrift message
    """
    transportIn = TTransport.TMemoryBuffer(data)
    protocolIn = TBinaryProtocol.TBinaryProtocol(transportIn)
    msg.read(protocolIn)
    msg.validate()
    return msg


# Representing the internal state of the client.
class ClientState:
	OFFLINE = 0
	CONNECT = 1
	READFRAME = 2
	READFRAMESIZE = 3
	

class PSClient:

	@staticmethod
	def discoverURI(id, interval, strategy):
		tree = DiscoveryTree(2*interval)
		discoverer = Discoverer(None, None, None, SERVICE_TYPE_TECS_SERVER)
		discoverer.setCallback(tree)
		discoverer.start()

		uri = None
		while uri == None:
			uri = tree.getBest(SERVICE_TYPE_TECS_SERVER,SCHEME_PS, strategy)
			time.sleep(interval / 1000.0)

		discoverer.shutdown()

		return uri


	def __init__(self, param):
		# Creates a PSClient with auto discovery.
		if (isinstance(param,basestring) and not param.startswith("tecs-ps://")):
			self.id = param;
			self.uri = None
			self.discover = True
		# Creates a PSClient with a specified tecs uri.
		else:			
			split = urisplit(param)

			if (split.host == None):
				raise RuntimeError("Invalid Host. URI tecs-ps://id@host:port")
			if (split.scheme != SCHEME_PS):
				raise RuntimeError("Invalid Scheme. Use tecs-ps://id@host:port")
			if (split.port == None):
				raise RuntimeError("Invalid Port. Use tecs-ps://id@host:port")
			if (split.userinfo == None):
				raise RuntimeError("Invalid Id. Use tecs-ps://id@host:port")

			self.id = split.userinfo
			self.uri = param
			self.discover = False

		self.password = ">>Empty<<"
		self.state = ClientState.OFFLINE
		self.ringbuffer = RingBuffer(5*1024*1024)
		self.currentframe = EventFrame()
		self.events = deque()
		self.socket = None
		self.lastconnect = 0
		self.connectionTimeout = 5000
		self.subscribes = None
		self.sendingOnly = False

		
		self.bytesWritten = 0
		self.bytesRead = 0
		self.eventsWritten = 0
		self.eventsRead = 0
		self.strategy = DiscoveryTree.ALL


	# Processes Reading the data.sdf
	def __loop(self):
		continueReading = True
		mustWait = False
		while continueReading:
			try:
				if (mustWait):
					time.sleep (1)
					mustWait = False

				if (self.state == ClientState.OFFLINE):
					continueReading = False
				elif (self.state == ClientState.CONNECT):
					if (self.discover):
						self.uri = self.discoverURI(self.id, 1000, self.strategy)
					split = urisplit(self.uri)
					continueReading = self.__initsocket(split.host, int(split.port))
				elif (self.state == ClientState.READFRAME):
					self.__readAll()
					continueReading = self.__readFrame()
				elif (self.state == ClientState.READFRAMESIZE):
					self.__readAll()
					continueReading = self.__readFrameSize()
			except socket.error as err:
				mustWait = True
				continueReading = True
				print ("Lost Connection. Reconnect. ",err)
				if (self.state != ClientState.OFFLINE):
					self.state = ClientState.CONNECT




	# Initializes a nonblocking reconnect.
	def __initsocket(self, host, port):	
		self.ringbuffer.reset()
		self.currentframe.reset()

		print("init socket ")
		self.lastconnect = current_millis()
		self.socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)				
		self.socket.connect((host, port))
		self.socket.setblocking(0)	#block after connecting :/ not optimal but good enough here
		print ("socket opened")
		
		login = Login()
		login.id = self.id;
		login.subscribes = self.subscribes; 
		login.password = self.password
		login.language = ProgrammingLanguage.PYTHON;
		login.languageVersion = sys.version;
		login.operatingSystem = platform.system()+" "+platform.release()+" "+os.name;
		login.validate()

		login_raw = SerializeThriftMsg(login)
		
		message = pack('!i', len(login_raw))
		message+= login_raw
		
		self.__writeAll(message)
		self.__readAck()			

		self.state = ClientState.READFRAMESIZE
		return True


	# Waits for exactly one byte
	def __readAck(self):
		while True:
		        try:
				buf = self.socket.recv(1)
				if (not buf):
					raise RuntimeError("Connection Reset");
				if (len(buf) == 1):
					self.bytesRead += 1
					break
			except socket.error:
				time.sleep (0.1)

	
	# Since the socket is blocking this method ensures all is written. If the
    # connection is lost it will throw an exception and therefore cause a
    # reconnect.
	def __writeAll(self, msg):
		#TODO (chbu02): Check networkbuffer full
		self.socket.send(msg)
		self.bytesWritten += len(msg)

	# Transmits as much stuff from the networkbuffer into the ringbuffer. But
    # at most 1024b.
	def __readAll(self):
		try:
			maxread = min(self.ringbuffer.getFree(),1024)
			if (maxread == 0):
				return
			buf = self.socket.recv(maxread)
			if (not buf):				
				state = ClientState.CONNECT
				raise RuntimeError("Connection failed");
			self.ringbuffer.write(buf, len(buf))
			self.bytesRead += len(buf)
		except socket.error:
			#temporary unavailable means nothing to read			
			return

	
	# Reads a data from the ringbuffer into the currentframe. If the frame is
    # finished it will be parsed and put into the queue. If the connection
    # fails ringbuffer and current frame are resetted.
	def __readFrame(self):
		if (self.ringbuffer.getAvailable() == 0):
			return False
		maxread = min(self.ringbuffer.getAvailable(), self.currentframe.getMissing())	
		buf = bytearray(maxread)
		read = self.ringbuffer.read(buf, maxread)		
		self.currentframe.write(buf, read)
		if (self.currentframe.isFinished()):
			evt = Event(self.currentframe)

			self.events.append(evt)
			self.state = ClientState.READFRAMESIZE
			self.currentframe.reset()
			if (len(self.events) == 100):
				print ("Events are not retreived from queue")

			while (self.sendingOnly and len(self.events) > 0):
				self.recv()


			#print ("Received %s" % evt.getHeader().event_type)
		return True
	

	# If 4 bytes are available in the ringbuffer it will read them and
    # interpret them as the size of the next frame, if not it will inform loop
    # that no data is available by returning false.
	def __readFrameSize(self):
		if self.ringbuffer.getAvailable() < 4:
			return False
		size = self.ringbuffer.readI32()
		self.currentframe.setSize(size)
		self.state = ClientState.READFRAME
		return True

	# Uses Blocking IO to exactly read one byte.
	def __blockRead(self):
		try:
			if (self.ringbuffer.getFree() == 0):
				return
			self.socket.setblocking(1)
			try:
				buf = self.socket.recv(1)
				if (not buf):				
					raise RuntimeError("Connection lost");
				self.ringbuffer.write(buf, len(buf))
			finally:
				self.socket.setblocking(0)
		except error:
			print ("Connection Terminated during a blocking read", error)
			return


	# If not connected this method allows to subscribe for channel. If the
    # connection is already established an exception is thrown.
    # If none of the subscribe methods is called the client automatically
    # requests all existing events. Channels 'PingEvent', 'PongEvent' and
    #'SheepEvent' are automatically subscribed.
    #If no subscription is called - All events will be received.
	# Will use subscribe on a channel using a class name.
    # not thread-safe
	def subscribe(self, clazz):
		if (self.isOnline()):
			raise RuntimeError("You cannot subscribe after connecting!")
		if (self.subscribes == None):
			self.subscribes = set()
		self.subscribes.add(clazz)


	# Removes all subscribed channels again. This on only possible if the
    # client is offline.
    # not thread-safe
	def resetSubscription(self):		
		if (self.isOnline()):
			raise RuntimeError("You cannot reset subscription after connecting!")
		self.subscribes = None

	
	#The client will enter the connection mode. After this it is ensured that
    # a connection is established before sending and receiving. All methods
    # will block until a connection is successfully established.
    # not thread-safe	
	def connect(self):
		if (not self.isOnline()):
			self.state = ClientState.CONNECT
			self.__loop()
			return
		else:
			raise RuntimeError("Client cannot connect because it is already connected")
	

	# Checks non-blocking if events are available.
	def canRecv(self):
		if (not self.isOnline()):
			raise RuntimeError("Client cannot check while offline")
		self.__loop()
		return len(self.events) != 0


	# Retreives events from the queue if available. If no events are available
    # this method will block. If you want to use non-blocking io check
    #canRecv() first.
    # not thread-safe
	def recv(self):
		while (len(self.events) == 0):
			while (not self.canRecv()):
				if (self.isConnected()):
					#print ("Enter blocking read")
					self.__blockRead()
		evt = self.events.popleft()
		if (evt.is_a("PingEvent")):
			ping = PingEvent()
			evt.parseData(ping)
			ping.validate()
			pong = PongEvent()
			#print("Received Ping at %d"%ping.request_time)
			pong.requestTime = ping.requestTime
			pong.responseTime = current_millis()
			self.send(".*", "PongEvent", pong)
		elif (evt.is_a("SheepEvent")):
			sheep = SheepEvent()
			evt.parseData(sheep)
			print (evt.header.source+"-Sheep: "+sheep.wool,e)
		return evt
		
		

	def send(self, target, etype, evt):
		evt.validate()
		data_buffer = TTransport.TMemoryBuffer()
		data_proto = TBinaryProtocol.TBinaryProtocol(data_buffer)
		evt.write(data_proto)
		data_raw = data_buffer.getvalue()
		self.sendPayload(target,etype, data_raw)
		

	def sendPayload(self, target, etype, payload):
		if (not self.isOnline):
			raise RuntimeError("You cannot send while offline");
		eventSent = False
		while (not eventSent):
			self.__loop()
			
			try:			
				header = EventHeader()
				header.etype = etype
				header.source = self.id
				header.target = target
				header.time = current_millis()		
				header.validate()
				
				header_buffer = TTransport.TMemoryBuffer()
				header_proto = TBinaryProtocol.TBinaryProtocol(header_buffer)
				header.write(header_proto)
					
				header_raw = header_buffer.getvalue()
				
				
				frame_size = len(header_raw) + len(payload) + 4
				header_size = len(header_raw)

				message = pack('!i', frame_size)
				message += pack('!i', header_size)
				message += header_raw
				message += payload

				#print ("Transmitted: %d "%frame_size)

				self.__writeAll(message)
				eventSent = True
			except error:
				print ("Connection failed while sending. Reconnecting. ",error)
				self.state = ClientState.CONNECT



	# Returns if the client socket is connected.
	def isConnected(self):
		return self.state != ClientState.OFFLINE and self.state != ClientState.CONNECT

	# Returns if client is in established mode such that events can be sent and
    # received.
	def isOnline(self):
		return self.state != ClientState.OFFLINE

	# Terminates the connection.
	def disconnect(self):
		if (not self.isOnline()):
			raise RuntimeError("You cannot disconnect while offline");
		try:
			self.__loop()		
			self.state = ClientState.OFFLINE
			self.socket.close()
		except error:
			print ("Ignored Socket Exception while disconnecting", error)


	# Sends a PingEvent and waits for it's according pong event to determine
    # the time.
    # not thread-safe
    # @return ping or timeout if the timeout happens before a pong is received
	def ping(self, target, timeout):
		firstrequest = current_millis()
		nextPing = current_millis()
		others = deque()
		try:
			while (current_millis() < firstrequest + timeout):
				if (nextPing < current_millis()):
					self.send(target, "PingEvent", PingEvent(firstrequest))
					nextPing = current_millis() + 100
				if (self.canRecv()):
					event = self.recv()
					others.appendleft(event)
					if (event.is_a("PongEvent")):
						pong = PongEvent()
						event.parseData(pong)
						if (pong.requestTime == firstrequest):
							return pong.responseTime - pong.requestTime;
			return timeout
		finally:
			while (len(others)!=0):
				self.events.appendleft(others.pop())
	

	def sheep(self, wool):
		self.send(target,"SheepEvent", SheepEvent(wool));
	
	# How many bytes have been read from the socket.
    # not thread-safe
	def getBytesRead(self):
		return self.bytesRead

	# How many bytes have been written from the socket.
    # not thread-safe
	def getBytesWritten(self):
		return self.bytesWritten

	# How many events have been written.
    # not thread-safe
	def getEventsWritten(self):
		return self.eventsWritten

	# How many events have been read.
    # not thread-safe
	def getEventsRead(self):
		return self.eventsRead

	#not thread-safe
	def getClientId(self):
		return self.id

	# Sets the timeout to establish connections.
	def setConnectionTimeout(self, timeout):
		self.connectionTimeout = timeout

	# Sets the DiscoveryStrategy.
	def setDiscoveryStrategy(self, strategy):
		if (not self.isDiscoveryMode()):
			raise RuntimeError("The client is not in discovery mode")
		self.strategy = strategy

	def getHost(self):
		split = urisplit(self.uri)
		return split.host

	def getPort(self):
		split = urisplit(self.uri)
		return split.port

	# Reconfigure the password.
	def setPassword(self, password):
		self.password = password

	# Configures to automatically reply ping events and drop all other events.
	def setSendingOnly(self):
		self.resetSubscription()
		self.subscribe("NOTHING")
		self.sendingOnly = True

	# Get a full PSClient URI.
	def getUri(self):
		return self.uri


