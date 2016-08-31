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
import threading
import struct
import errno


from thrift import Thrift
from thrift.protocol import TBase

from de.dfki.tecs.misc.IntervalHelper import IntervalHelper
from de.dfki.tecs.basetypes.constants import *
from de.dfki.tecs.basetypes.ttypes import *
from de.dfki.tecs.misc.Serializer import Serializer
from de.dfki.tecs.misc.EventFrame import EventFrame
from de.dfki.tecs.Event import Event

class ReadState:
	HEADER = 1
	CONTENT = 2

class TCPSocketState:
	DISCONNECTED = 0	
	CONNECTED = 1


 # Implements the protocol for communication with TECS-Message Passing
 # Wraps around a socket and handles (non-blocking) receiving and sending of messages.
 # Furthermore, heartbeats are sent frequently to check for disconnects.
 # Use send(), recv() for communication.
 # Call update() frequently.
 # Your process may sleep if isReceivingData() returns false

class MPSocket:

	def __init__(self, socket=None):
		if socket:
			self.__socket = socket
		else:
			self.__socket = None
		self.__DEFAULT_HEARTBEAT_INTERVAL = 1000
		self.__HEADER_SIZE = 4
		self.__BUFFER_SIZE = 5 * 1024 * 1024 #for reading buffer and ring buffer
		self.__heartbeatIH = IntervalHelper(self.__DEFAULT_HEARTBEAT_INTERVAL)
		self.__recvContentBuffer = bytearray(self.__BUFFER_SIZE) 
		self.__recvHeaderBuffer = bytearray(self.__HEADER_SIZE)
		self.__socketState = None
		self.__readingState = ReadState.HEADER
		self.__receivingData = False
		self.__eventFrame = EventFrame()
		self.serializer = Serializer()


     
     # Checks whether the last update call expects more incoming messages.
     # If it returns true, recv() should be called. 
	def isReceivingData(self):
		return self.__receivingData


     # Resets the internal state of the writer and reader. 
     # Should only be called if no connection is established
	def reset(self):
		self.__readingState = ReadState.HEADER
   

     # Disconnects this socket from a remote endpoint.
     # @throws ValueError if not connected
     # @throws SocketError if closing of the socket failed
	def disconnect(self):
		if not self.isConnected():
			print "Can't disconnect: Not connected"
			raise ValueError
		
		try:
			self.__socket.shutdown(socket.SHUT_RDWR)
			self.__socket.close()
		except socket.error as e:
			print "Error while closing socket: " + str(e)

		self.__socketState = TCPSocketState.DISCONNECTED
		self.reset()
		#print "Socket closed"


     # Checks for incoming events and in case returns them. If blocking is configured
 	 # this may block for some time but will return
     # if heartbeat or message is returned. If you want to block until a new event is available
     # see TCPSocketHandler::recv(true) (java version)
     # with block param is set: Changes the blocking mode and waits until a new event is available
     # Blocks until an event is available. 
     # @return A received event if available, otherwise None
     # @return A received event if available, otherwise null
     # @throws ValueError If not connected
     # @throws SocketError: On network error

	def recv(self, block= None):
		if not self.isConnected():
			print "Can't receive message: Not Connected"
			raise ValueError
		self.updateHeartbeat()
		if not block or block is None:
			try:
				return self._recv_()
			except socket.error as e:
				self.disconnect()
				print e
				return None
		else:
			self.setBlocking(block)
			event = None
			while event is None:
				self.updateHeartbeat()
				try:
					event = self._recv_()
				except socket.error as e:
					self.disconnect()
					print e
					return None
			return event


	 # Sends the given event to the remote endpoint.
     # throws Thrift.TException if the given event couldn't be serialized
     # throws SocketError on network error (e.g. disconnect)
	def send(self, stringType, event):
		try:		
			data_raw = self.serializer.serialize(event)
			self.sendBytes(stringType, data_raw)
		except Thrift.TException as tx:
			raise RuntimeError(str(tx))
		except socket.error as e:
			self.disconnect()
			print str(e)


 	#  Sends the given data to the remote endpoint.
	def sendBytes(self, stringType, content):
		if not self.isConnected():
			print "Can't send message: Not Connected"
			raise ValueError
		self.updateHeartbeat()
		ev = EventHeader(None, None, None, stringType)
		try:
			header_raw = self.serializer.serialize(ev)
			framesize = 4 + len(header_raw) + len(content)
			out = struct.pack('!i', framesize)
			out += struct.pack('!i', len(header_raw))
			out += header_raw
			self._write(out)
			out = content
			self._write(out)
		except Exception as e:
			raise RuntimeError(e.message)
   

  	# handles the receiving
	# throws SocketError
	def _recv_(self):
		#read header
		if self.__readingState == ReadState.HEADER:
			self.__recvHeaderBuffer = self._read(self.__HEADER_SIZE)
			#check if data was received
			if not self.__recvHeaderBuffer is None:
				self.__receivingData = True
			else:
				self.__receivingData = False
				return None
			#check if the required amount of bytes have been read
			if len(self.__recvHeaderBuffer) == self.__HEADER_SIZE:
				resTuple = struct.unpack_from('!i', self.__recvHeaderBuffer)
				frameSize = resTuple[0]
				if frameSize == 0:
					#heartbeat
					return None
				else:
					self.__eventFrame.setSize(frameSize)
					self.__readingState = ReadState.CONTENT
		
		# receive content
		if self.__readingState == ReadState.CONTENT:
			self.__recvContentBuffer = self._read(self.__eventFrame.getMissing())
			#check if data was received
			if self.__recvContentBuffer is None:
				return None
			#write bytes to eventFrame
			n = min(self.__eventFrame.getMissing(), len(self.__recvContentBuffer))
			if n > 0:
				self.__eventFrame.write(self.__recvContentBuffer, n)
			# create event
			if self.__eventFrame.isFinished():
				self.__readingState = ReadState.HEADER
				event = Event(self.__eventFrame)
				return event
		return None


	#recv data (read number of @param length bytes from socket)
	def _read(self, length):
		bytebuffer = None
		try:
			bytebuffer = self.__socket.recv(length)
		except socket.error as e:
			if not errno.EWOULDBLOCK:
				print e
				sys.exit(1)
		if bytebuffer is None:
			return None
			print "Socket closed: " + str(e)
		else:
			return bytebuffer

	#send data from socket
	def _write(self, bytebuffer):
		try:
			return self.__socket.send(bytebuffer);
		except socket.error as e:
			print "Socket closed: " + str(e)




    # Sends a heartbeat (four zero bytes) to the remote. 
    # @throws SocketError if sending fails.
    # @throws ValueError if not connected
	def sendHeartbeat(self):
		if not self.isConnected():
			print "Can't send message: Not Connected"
			raise ValueError
		byteBuffer = bytearray(self.__HEADER_SIZE)
		for byte in byteBuffer:
			byte = 0
		try:
			self._write(byteBuffer)
		except socket.error as e:
			self.disconnect()
			raise e


	# Sends a heartbeat if a predefined amount time has passed
	def updateHeartbeat(self):
		if self.__heartbeatIH.shouldExecute():
			self.sendHeartbeat()
     

	def setHeartbeatInterval(self, ms):
		self.__heartbeatIH.setExecuteInterval(ms)
  
	#True iff this socket is connected to remote endpoint
	def isConnected(self):
		return (self.__socketState == TCPSocketState.CONNECTED)
		


	def setConnected(self, connected):
		if connected:
			self.__socketState = TCPSocketState.CONNECTED
		else:
			self.__socketState = TCPSocketState.DISCONNECTED


	#TODO: can this be checked in python?
	def isBlocking():
		return None 
  

	# Sets the blocking mode of this socket.
	def setBlocking(self, block):
		if self.__socket is None:
			raise ValueError("Can't change blocking mode: Socket is null.")
		try:
		    self.__socket.setblocking(block)
		    if block:
		        print "Successfully switched to blocking mode"
		    else:
		        print "Successfully switched to non-blocking mode"
		except socket.error as e:
		    print "Could not change blocking mode: " + str(e)
		    raise RuntimeException(e);
     
  

     # Updates the socket of this handler and resets the internal state
     # of the writer and reader. Remaining messages are discarded.
	def setSocket(self, sock):
		self.__socket = sock
		self.reset()
 

	def toString():
		string = "TCPSocketHandler{ " 
		string +="socket=" + str(self.__socket) 
		string +=", connected=" + str(self.isConnected())
		string += '}'
		return string
    

	# @return The port of the remote endpoint if connected. Otherwise -1.
	def getRemotePort(self):
		if self.isConnected():
			ipaddr, port = self.__socket.getpeername()
			return port
		else:
			return -1
 
	# @return The host of the remote endpoint if connected. Otherwise an empty string.
	def getRemoteHost(self):
		if self.isConnected():
			ipaddr, port = self.__socket.getpeername()
			return ipaddr
		else:
			return ""
     

