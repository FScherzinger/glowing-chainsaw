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
import uuid
import errno

from thrift import Thrift

from de.dfki.tecs.misc.IntervalHelper import IntervalHelper
from de.dfki.tecs.basetypes.constants import *
from de.dfki.tecs.basetypes.ttypes import *
from de.dfki.tecs.Event import Event
from de.dfki.tecs.misc.IUDThread import IUDThread
from de.dfki.tecs.mp.MPSocket import MPSocket
from de.dfki.tecs.discovery.ServiceProvider import ServiceProvider


 #  A socket for the server-side communication for MP
 # Usage: bind(); com = accept(); msg = com.recv() com.send(..) unbind();
 # If Service instances are given via constructor it also offers the services via discovery.
 # Use createService to create a suitable service.
class MPServerSocket:

	# Creates a new server socket that will bind to the given port.
    # If at least one service is given, a ServiceProvider will be started
    # that announces the services via discovery.
	def __init__(self, **kwargs):
		self.__port = kwargs.get('port', 0)
		self.__services = kwargs.get('services', [])
		if not self.__services == []:
			self.__serviceProvider = ServiceProvider(None, None, self.__services)
		else:
			self.__serviceProvider = None
		self.__socket = None
		self.__bound = False


	def bind(self, host=None):
		if self.isBound():
			raise ValueError("Can't bind: Already bound")
		if host is None:
			#Binds the server socket to a free port and all network interfaces
			host = "0.0.0.0"
		try:
			self.__socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
			self.__socket.bind((host,self.__port))
			self.__socket.listen(1)
			self.__socket.setblocking(1)
			self._startDiscovery(self.getPort())
			self.setBound(True)
			print "Sucessfully bound TCP socket to port " + str(self.__port)
		except IOError as e:
			try:
				#try to close the socket in case of error
				self.__socket.close()
			except socket.error as ex:
				print ex
			print e


	#start the ServiceProvider and set the correct port
	def _startDiscovery(self, port):
		if self.__serviceProvider is None:
			return
		self.__serviceProvider.updateInvalidPort(port)
		self.__serviceProvider.start()
		print "Started discovery with " + self.__serviceProvider.toString()
		
		
	#shutdown the ServiceProvider
	def _stopDiscovery(self):
		if self.__serviceProvider is None:
			return
		self.__serviceProvider.shutdown()
		try:
			self.__serviceProvider.join()
		except Exception as e:
			print e

      
	# @throws SocketError if closing the socket fails.
    # @throws ValueError if the socket is not bound.
	def unbind(self):
		if not self.isBound():
			raise ValueError("Can't shutdown: Not running")
		try:
			self.__socket.close()
			self.setBound(False)
			self.__socket = None
			print "Closed TCP socket on port " + str(self.__port)
		except socket.error as e:
			print str(e)
		self._stopDiscovery()
  
			
     # Accept a new client for communication.
     # @return A MPSocket representing the connection. Or None if no new connection is available (non-blocking mode)
     # @throws socket.error If the accept operation fails. If errno = eagain: then no new client to accept, return.
     # @throws ValueError If the server socket is not bound.
	def accept(self):
		if not self.isBound():
			raise ValueError("Can't accept: Socket is not bound")
		try:
			clientsock, addr = self.__socket.accept()
			clientsock.setblocking(0)
			socketHandler =  MPSocket(clientsock)
			socketHandler.setConnected(True)
			return socketHandler
		except socket.error as e:
			if errno.EAGAIN or errno.EWOULDBLOCK:
				return None
			print e
  

	#@return The port the socket is or will be bound to.
	def getPort(self):
		if self.isBound():
			return self.__socket.getsockname()[1]
		return self.__port

	# @return True iff the server is bound to a bound.
	def isBound(self):
		return self.__bound
 

	def setBound(self, bound):
		self.__bound = bound


	def setBlocking(self, block):
		if not self.isBound():
			raise ValueError("Can't set blocking mode: Not bound")
		try:
			self.__socket.setblocking(block)
		except socket.error as e:
			print "Can't set blocking mode." + str(e)
     


 	# @return A copy of the services that are offered or will be offered via discovery
	def getOfferedServices(self):
		if self.__serviceProvider is None:
			newServiceList = []
			return newServiceList
		else:
		    return self.__serviceProvider.copyServices();

       

	def toString(self):
		sb = "MPServerSocket { "
		sb += "port=" + str(self.getPort())
		sb += ", bound=" + str(isBound())
		if not self.__serviceProvider is None:
			sb += ", offered services=["
			for service in self.__serviceProvider.copyServices():
				sb += service.toString()  + ", "           
		sb += "]"
		sb += " }"
		return sb
 

    # Creates a new service instance suitable for MP services. It will
    # use tecs-mp as scheme and a placeholder for the host. The port is left empty and will be set by
    # the according server implementation. The id is set to a random UUID.
    # @param serviceType The service type. Clients may send a regex that is matched against this type.
	@staticmethod
	def createService(serviceType):
		service = Service(str(uuid.uuid4()), serviceType)
		service.URIs.append(str(SCHEME_MP) + "://" + str(URI_HOST) + "/")
		return service






