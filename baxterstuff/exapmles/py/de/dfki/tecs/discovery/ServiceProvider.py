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


from __future__ import with_statement
from uritools import *

import sys, glob
import socket
import re
import time
import os
import platform
import time
import threading

from thrift import Thrift
from thrift.transport import TSocket
from thrift.transport import TTransport
from thrift.protocol import TBinaryProtocol 

from de.dfki.tecs.misc.IUDThread import IUDThread
from de.dfki.tecs.misc.UDPMulticastSocket import UDPMulticastSocket
from de.dfki.tecs.misc.UDPMessage import UDPMessage
from de.dfki.tecs.basetypes.constants import *
from de.dfki.tecs.basetypes.ttypes import *
from de.dfki.tecs.discovery.DiscoverySettings import DiscoverySettings
from de.dfki.tecs.misc.Serializer import Serializer
from de.dfki.tecs.misc.Deserializer import Deserializer



 # Offers a service via udp multicast / broadcast.
 # Use the Service class to define the information that is propagated.
 # Services are identified by a type:string, whereby the regex of a request is matched against this
 # type.
 # The ServiceProvider will initially send an announcement message and afterwards
 # response on requests that can be fulfilled by a given service.
 #
 # Usage:
 # [1] In own thread: Create a ServiceProvider and call ServiceProvider::start. ServiceProvider::shutdown to stop it.
 # [2] Single thread: Create a ServiceProvider and call ServiceProvider::initialize, ServiceProvider::update
 # (multiple times to update the internal state) and ServiceProvider::deinitialize to stop it

class ServiceProvider(IUDThread):

	__DEFAULT_SLEEP_TIME_IF_NO_REQUEST = 50

	# Creates a ServiceProvider that will join the given multicast group for
    # discovery and offers the given services.
	def __init__(self, multicastGroup = None, port=None, *services):
		IUDThread.__init__(self)
		if multicastGroup:
			self.__multicastGroup = multicastGroup
		else:
			self.__multicastGroup = DiscoverySettings.DEFAULT_MULTICAST_ADDRESS
		if port:
			self.__socket = UDPMulticastSocket(port)
		else:
			self.__socket = UDPMulticastSocket(DiscoverySettings.DEFAULT_MULTICAST_PORT)
		if len(services) >= 1:
			self.__services = services
		else:
			self.__services = None
		self.__sleepTime = self.__DEFAULT_SLEEP_TIME_IF_NO_REQUEST
		self.__initialized = False
		self.__serviceLock = threading.RLock()
		self.serializer = Serializer()
		self.deserializer = Deserializer()

		
 	#return The duration how long the thread sleeps if no new data is available
	def getSleepTime(self):
		return self.__sleepTime


    #Sets the duration how long the thread sleeps if no new data is available
	def setSleepTime(self, sleepT):
		 self.__sleepTime = sleepT
	

	#bind socket and join multicast group
	def initialize(self):
		try:
			self.__socket.bind()
			self.__socket.joinMulticastGroup(self.__multicastGroup)
			self.__initialized = True
            #Announce the service availability via multicast -> faster discovery
			for service in self.__services:
				self._sendServiceResponse(self.__multicastGroup, self.__socket.getPort(), "Announce", service)
		except IOError as e:
			print "Couldn't initialize ServiceProvider. Trying again after one second" + e
			time.sleep(1000)


	def _reinitialize(self):
		self.__socket.close()
		self.__initialized = False
		self.initialize()
 
	#receive and handle different types of messages 
	def update(self):
		if not self.__initialized:
			self._reinitialize()
			return
		msg = self.__socket.recv()
		if msg is None:
			time.sleep(self.__sleepTime)
			return
		try:
			#deserialize message
			discMessage = DiscoveryMessage()
			discoveryMessage = self.deserializer.deserialize(discMessage, msg.getBuffer())
		except Thrift.TException as tx:
			print tx
			return
		#check type of message
		if discoveryMessage.serviceRequest:
			try:
				#message was a service request
				self._updateServiceRequest(discoveryMessage, msg.getHost(), msg.getPort())
			except IOError as e:
				print e
				return
		if discoveryMessage.ipResolveRequest:
			try:
				#message was an IP resolving request
				self._updateIpRequest(discoveryMessage.ipResolveRequest.requestId, msg.getHost(), msg.getPort())
			except IOError as e:
				print e
				return

	#answer ip resolving request
	# @throws Runtimerror if serialization failed
	def _updateIpRequest(self, requestId, srcAddr, srcPort):
		#lookup ip address
		srcIp = socket.gethostbyname(srcAddr)
		disMsg = DiscoveryMessage.ipResolveResponse(IPResolveResponse(requestId, srcIp))
		try:
			# serialize and send answer
			serializeDisMsg = self.serializer.serialize(disMsg)
			outMsg = UDPMessage(serializeDisMsg, srcIp, srcPort)
			self.__socket.send(outMsg)
		except Thrift.TException as tx:
			raise RuntimeError(str(tx) + " " + str(disMsg))


	#searches for requested services among all known available services
	def _updateServiceRequest(self, discoveryMessage, srcAddr, srcPort):
		for service in self.__services:
			self._updateSpecificServiceRequest(discoveryMessage, srcAddr, srcPort, service)


	#checks if @param service is the requested service in the discoveryMessage
	def _updateSpecificServiceRequest(self, discoveryMessage, srcAddr, srcPort, service):
		serviceReq = discoveryMessage.serviceRequest
		#check if the service has the requested type
		if serviceReq.serviceType:
			m = re.search(serviceReq.serviceType, service.type)
			if m is None:
				print "Can't answer service request for regex " + str(serviceReq.serviceType)
				return
		#check if the service has the requested id
		if serviceReq.serviceId and (not serviceReq.serviceId == service.id):
			return
		if not serviceReq.port is None:
            #unicast back
			port = serviceReq.port
			host = srcAddr
		else:
            #else multicast
			port = self.__socket.getPort()
			host = self.__multicastGroup
		responseId = discoveryMessage.serviceRequest.requestId
		self._sendServiceResponse(host, port, responseId, service)

       
	def deinitialize(self):
		self.__socket.unbind()


	#send back service response to requesting client 
	# @throws RuntimeError if serialization failed
	def _sendServiceResponse(self, host, port, responseId, service):
		disMsg = DiscoveryMessage()
		servResp = ServiceResponse(responseId)
		servResp.service_ = service
		disMsg.serviceResponse = servResp
		serializeDisMsg = None
		try:
			serializeDisMsg = self.serializer.serialize(disMsg)
		except Thrift.TException as tx:
			raise RuntimeError(str(tx) + " " + str(disMsg))
		#create UDPMessage and sends message via the socket
		outMsg = UDPMessage(serializeDisMsg, host, port)
		self.__socket.send(outMsg)
 	

	#add a new service to the service provider
	def addService(self, service):
		with self.__serviceLock:
			self.__services.append(service)


	#remove a service from current available ones
	def removeService(self, serviceId):
		with self.__serviceLock:
			newServiceList = []
			for service in self.__services:
				if not service.id == serviceId:
					newServiceList.append(service)
			self.__service = newServiceList


	def getServiceIds(self):
		serviceIds = []
		with self.__serviceLock:
			for service in self.__services:
				serviceIds.append(service.id)
		return serviceIds

    
    # Updates the port of all URIs if the port is invalid (== -1)
	def updateInvalidPort(self, portNbr):
		with self.__serviceLock:
			for service in self.__services:
				newUris = []
				#get the uris of the service
				uris = service.URIs
				for uriStr in uris:
					try:
						#uri = uriencode(uriStr, safe='', encoding='utf-8', errors='strict')
						uri = uriStr
						parts = urisplit(uri)
						if parts.port is None: #invalid port, update it
							uri = uricompose(scheme=parts.scheme, authority=parts.authority, path=parts.path, query=parts.query, fragment=parts.fragment, userinfo=parts.userinfo, host=parts.host, port=portNbr, encoding='utf-8')
						newUris.append(str(uri))
					except Exception as e:
						print "Could not set port of uri " + uriStr + " Cause: " + e.message
						newUris.append(uriStr)
				#set updated uris for all services
				service.URIs = newUris
           

	def toString(self):
		with self.__serviceLock:
			sb = ""
			for service in self.__services:
				uriSB = "";
				for uriStr in service.URIs:
					uriStr = str(uriStr)
					uriSB +="\n     " + uriStr
				sb += "\n    " + uriSB
		return "ServiceProvider: " + sb


    #@return A copy of the services
	def copyServices(self):
		servicesCopy = []
		with self.__serviceLock:
			for service in self.__services:
				servicesCopy.append(Service(service))
		return servicesCopy
  



