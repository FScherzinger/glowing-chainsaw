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

import sys,glob
import socket
import os
import platform


from uritools import *
from thrift import Thrift
from de.dfki.tecs.discovery.Discoverer import Discoverer
from de.dfki.tecs.discovery.Discoverer import Callback
from de.dfki.tecs.mp.MPSocket import MPSocket
from de.dfki.tecs.mp.MPSocket import TCPSocketState
from de.dfki.tecs.mp.MPSocket import ReadState
from de.dfki.tecs.misc.IUDThread import IUDThread



class ConnectType:
	HOST_PORT = 1
	SERVICE_TYPE_REGEX = 2
	SERVICE = 3

# This socket for the client-side offers the minimal functionality to communicate with
# a DirectServerSocket. It offers functionality for (dis-)connecting and message receiving and sending.

 # Usage:
 # connect(...)
 # send(..) / recv()
 # disconnect()
class MPClientSocket(MPSocket):
	
	#define correct connection type acording to given arguments
	def __init__(self, host=None, port=None, service=None, serviceTypeRegex=None, cService=None):
		MPSocket.__init__(self)
		if host and port:
			self._connectType = ConnectType.HOST_PORT
			self._host = host
			self._port = port
		elif service:
			self._connectType = ConnectType.SERVICE
			self._service = service
		elif serviceTypeRegex:
			self._connectType = ConnectType.SERVICE_TYPE_REGEX
			self._serviceTypeRegex = serviceTypeRegex
			if cService:
				self._confirmservice = cService
			else:
				self._confirmservice = ConfirmTrueService()
	

	# Connects to a server as specified in the constructor. 
    # @throws ValueError If unhandled connection type
    # @throws SocketError If connecting failed
	def connect(self):
		if self._connectType == ConnectType.HOST_PORT:
			self._connectHostPort(self._host, self._port)
		elif self._connectType == ConnectType.SERVICE:
			self._connectService(self._service)
		elif self._connectType == ConnectType.SERVICE_TYPE_REGEX:
			self._connectServiceTypeRegex(self._serviceTypeRegex, self._confirmservice)
		else:
			raise ValueError("Unhandled connectType: " + str(self._connectType))


	# @throws ValueError If already connected
	# @throws SocketError If connecting failed
	def _connectHostPort(self, dstHost, dstPort):
		if self.isConnected():
			raise ValueError("Can't connect: Already running")
	#	try:
		sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
		sock.setblocking(1)
		sock.connect((dstHost, int(dstPort)))
		#set socket into non-blocking mode
		sock.setblocking(0)
		self.setSocket(sock)
		#resets reading state and corresponding buffers
		self.reset()
		self.setConnected(True)
	#	except socket.error as e:
	#		print e
		print "Connected to " + str(dstHost) + ":" + str(dstPort)


	# @throws ValueError If already connected
	# @throws SocketError If connecting failed
	# @throws Exception  If the service does not contain a valid endpoint.
	#Tries to connect to one of the endpoints defined by the given service.
	def _connectService(self, service):
		if self.isConnected():
			raise ValueError("Can't connect: Already running")

		uris = service.URIs
		containsValidURI = False
		for uriStr in uris:
			try:
				print "Trying to connect to " + str(uriStr)			
			#	uri = uriencode(uriStr, safe='', encoding='utf-8', errors='strict')
				uri = uriStr
				#split uri into its components
				parts = urisplit(uri)
				containsValidURI = True
				try:
					self._connectHostPort(parts.host, parts.port)
					return
				except socket.error as e:
					print "Connecting to " + uriStr + " failed: " + str(e)
			except Exception as ex:
				print "Connecting to " + uriStr + " failed: Illegal URI in service " + str(service)
				print ex
		if not containsValidURI:
			raise Exception("Given service does not contain a valid URI. Service was " + str(service))
		raise Exception("Could not connect to any endpoint defined in " + str(service))


	# @throws ValueError If already connected
	# @throws SocketError If connecting failed
	#Uses discovery to find services with the given type
	def _connectServiceTypeRegex(self, serviceTypeRegex, confirmservice):
		if self.isConnected():
			raise ValueError("Can't connect: Already running")

		#param confirmService: Decides whether you want to connect to a discovered service.
		discoverer = Discoverer(None, None, None, serviceTypeRegex)
		#create an set service callback
		confirmServicecallback = ConfirmServiceCallback(self, confirmservice)
		discoverer.setCallback(confirmServicecallback)	
		#discover requested service
		discoverer.start()
		while discoverer.isRunning():
			try:
				discoverer.join()
			except Exception as e:
				discoverer.shutdown()
				IUDThread.sleep(500)
				print e



	def _setService(self, service):
		self._service = service
		self._connectType = ConnectType.SERVICE


	def toString(self):
		sb = "MPClientSocket{ "
		sb += "connected=" + str(self.isConnected())
		sb += ", connectType=" + str(self._connectType) + " "
		if self._connectType == ConnectType.Host_Port:
			sb += str(self._host) + ":" + str(self._port)
		elif self._connectType == ConnectType.SERVICE_TYPE_REGEX:
			sb+= str(self._serviceTypeRegex)
		elif self._connectType == ConnectType.SERVICE:
			sb += str(self.service)
		else:
			sb += "unknown"
		sb += " }"
		return sb



class ConfirmServiceInterface:
        
	def confirmService(self, service):
		pass


class ConfirmTrueService(ConfirmServiceInterface):
	def confirmService(self, service):
			return True


class ConfirmServiceCallback(Callback):

	def __init__(self, clientSockInstance, confirmservice):
		self.__mpclientsocket = clientSockInstance
		self._callconfirmservice = confirmservice
		
			
	def onDiscovered(self, discoverer, service):
		if self._callconfirmservice.confirmService(service):
			#print "Discovered service " + str(service)
			try:
				#set discovererd service and call corresponding connect method
				self.__mpclientsocket._setService(service)
				self.__mpclientsocket._connectService(service)
				discoverer.shutdown()
			except Exception as e:
				print e
		


 
