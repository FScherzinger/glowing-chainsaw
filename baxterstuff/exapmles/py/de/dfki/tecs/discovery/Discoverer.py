# !/usr/bin/python2.7
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

import sys, glob
import socket
import re
import time
import os
import platform
import threading
import time

from thrift import Thrift
from thrift.transport import TSocket
from thrift.transport import TTransport
from thrift.protocol import TBinaryProtocol

from de.dfki.tecs.misc.IUDThread import IUDThread
from de.dfki.tecs.misc.IntervalHelper import IntervalHelper
from de.dfki.tecs.misc.UDPMulticastSocket import UDPMulticastSocket
from de.dfki.tecs.misc.UDPMessage import UDPMessage
from de.dfki.tecs.basetypes.constants import *
from de.dfki.tecs.basetypes.ttypes import *
from de.dfki.tecs.discovery.ServiceProvider import ServiceProvider
from de.dfki.tecs.discovery.DiscoverySettings import DiscoverySettings
from de.dfki.tecs.misc.Serializer import Serializer
from de.dfki.tecs.misc.Deserializer import Deserializer


# Looks via UDP multicast (and broadcast [optional]) for available for servers / services.
# The latter are defined via type:string, whereby a discoverer sends a regex that is matched against the
# type.
#
# Usage [1] - in own thread:
#      Discoverer discoverer = new Discoverer(".*"); //looks for all services
#      dis.setCallback(new Callback() { ... } //react on feedback
#      dis.start(); //start thread
#      ...
#      dis.shutdown(); //finish
#
# Usage [2] - same Thread:
#      Discoverer discoverer = new Discoverer(".*"); //looks for all services
#      dis.setCallback(new Callback() { ... } //react on feedback
#      dis.initialize();
#      dis.update(); //call frequently
#      ...
#      dis.deinitialize(); //finish


class Discoverer(IUDThread):
    __DEFAULT_SLEEP_TIME = None
    __DEFAULT_SEARCH_INTERVAL = None
    callback = None

    def __init__(self, multicastAddress=None, multicastPort=None, searchInterval=None, *serviceTypeRegex):
        IUDThread.__init__(self)
        self.__DEFAULT_SLEEP_TIME = 100
        self.__DEFAULT_SEARCH_INTERVAL = 1000
        self.__sleepTime = self.__DEFAULT_SLEEP_TIME
        if multicastAddress:
            self.__multicastAddress = multicastAddress
        else:
            self.__multicastAddress = DiscoverySettings.DEFAULT_MULTICAST_ADDRESS
        if multicastPort:
            self.__multicastSocket = UDPMulticastSocket(multicastPort, True)
        else:
            self.__multicastSocket = UDPMulticastSocket(DiscoverySettings.DEFAULT_MULTICAST_PORT, True)
        if searchInterval:
            self.__searchIH = IntervalHelper(searchInterval)
        else:
            self.__searchIH = IntervalHelper(self.__DEFAULT_SEARCH_INTERVAL)
        if len(serviceTypeRegex) >= 1:
            self.__serviceTypeRegexes = serviceTypeRegex
        else:
            #search for any type of service
            self.__serviceTypeRegexes = ".*"
        self.__rebindIH = IntervalHelper(-1)
        self.__unicastSocket = UDPMulticastSocket(1234, False)
        self.__initialized = False
        self.__discLock = threading.RLock()
        self.serializer = Serializer()
        self.deserializer = Deserializer()
    
    
    def getSleepTime(self):
        with self.__discLock:
            return self.__sleepTime

    def setSleepTime(self, sleepT):
        with self.__discLock:
            self.__sleepTime = sleepT


    #binds to socket, joins multicast group
    def initialize(self):
        try:
            self.__multicastSocket.bind()
            self.__unicastSocket.bind()
            self.__multicastSocket.joinMulticastGroup(self.__multicastAddress)
            self.__initialized = True
        except IOError as e:
            print "Error during initialization: " + str(e)
            return
    

    #periodically sends request for the services to discover
    #and receives and handles service response
    def update(self):
        if self.__searchIH.shouldExecute():
            #sends service request for each type of requested service
            for serviceTypeRegex in self.__serviceTypeRegexes:
                self.sendRequest(serviceTypeRegex)
        #check for service response received by the multicast socket
        noWork = self._updateSocket(self.__multicastSocket)
        if not self.__initialized:
            # prevents the exception thrown when the callback called Deinitialize on the Discoverer
            return
        #check for service response received by the unicast socket
        noWork &= self._updateSocket(self.__unicastSocket)
        if noWork:
            time.sleep(self.__sleepTime)


    def deinitialize(self):
        self.__multicastSocket.unbind()
        self.__unicastSocket.unbind()
        self.__initialized = False


    # sends request for certain service type
    # @throws RuntimeError If serialization fails
    def sendRequest(self, serviceType):
        with self.__discLock:
            print "Sending service request for service type: " + str(serviceType)
            #create service request
            serviceRequest = ServiceRequest()
            if not serviceType is None:
                serviceRequest.serviceType = serviceType
            serviceRequest.requestId = ""  # we don't care about the response id;
            outMsg = DiscoveryMessage()
            outMsg.serviceRequest = serviceRequest
            outMsg.serviceRequest.port = self.__unicastSocket.getPort()  # discoverable will unicast back
            try:
                #create and send UDP message
                byteBuffer = self.serializer.serialize(outMsg)
                udpMsg = UDPMessage(byteBuffer, self.__multicastAddress, self.__multicastSocket.getPort())
                self.__multicastSocket.send(udpMsg)
            except Thrift.TException as tx:
                raise RuntimeError(str(tx))
            except IOError as e:
                print str(e)
           



    # The callback is called after a service instance was found
    def setCallback(self, cb):
        with self.__discLock:
            self.callback = cb


    def getResponsePort(self):
        return self.__unicastSocket.getPort()


    #creates and runs a new Discoverer
    def requestServices(timeoutMs, *serviceTypeRegex):
        discoverer = Discoverer(serviceTypeRegex)
        discoverer.callback = ServiceCallback()
        now = int(round(time.time() * 1000))
        endTime = now + timeoutMs
        discoverer.initialize()
        while now < endTime:
            discoverer.update()
            now = int(round(time.time() * 1000))
        res = discoverer.callback.result
        discoverer.deinitialize()
        return res


    #check socket for response
    def _updateSocket(self, socket):
        if not socket.isBound():
            return False
        msg = socket.recv()
        if msg is None:
            return True
        try:
            #deserialize message
            discMessage = DiscoveryMessage()
            discoveryMessage = self.deserializer.deserialize(discMessage, msg.getBuffer())
        except Thrift.TException as tx:
            print tx
            return False
        #if message is a response to the service request -> further process message
        if discoveryMessage.serviceResponse:
            self._handleServiceResponse(discoveryMessage, msg.getHost(), msg.getPort())
        return False


    def _handleServiceResponse(self, disMsg, srcHost, srcPort):
        serviceResponse = disMsg.serviceResponse
        if not serviceResponse.service_:
            print "no service in serviceResponse"
            return
        #retrieve service from service response
        service = serviceResponse.service_
        newURIs = []
        #set correct host
        for uri in service.URIs:
            newuri = uri.replace(URI_HOST, srcHost)
            newURIs.append(newuri)
        service.URIs = newURIs
        if self.callback is None:
            print "Callback not set: " + str(service)
        else:
            #further handle discovered service
            self.callback.onDiscovered(self, service)


    # Use a negative value to disable automic rebinding
    def setRebindInterval(self, intervalMs):
        self.__rebindIH.setExecuteInterval(intervalMs)


class Callback:
    def __init__(self, arg=None):
        pass

        def onDiscovered(discoverer, service):
            pass


class ServiceCallback(Callback):
    def __init(self):
        self.result = []

    #put discovered services into a list
    def onDiscovered(self, discoverer, service):
        self.result.append(service)
