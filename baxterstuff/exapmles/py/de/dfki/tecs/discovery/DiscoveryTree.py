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

import sys, glob
import socket
import re
import time
import os
import platform
import threading
import random
import netifaces
import time

from uritools import urisplit

from thrift import Thrift
from thrift.transport import TSocket
from thrift.transport import TTransport
from thrift.protocol import TBinaryProtocol

from de.dfki.tecs.basetypes import constants
from de.dfki.tecs.basetypes.ttypes import *
from de.dfki.tecs.discovery.Discoverer import Discoverer
from de.dfki.tecs.discovery.Discoverer import Callback
from de.dfki.tecs.misc.Ping import PingService



# Adapts service to add a timestamp and functionallity for merging URIs.


class DiscoveredService():
    def __init__(self, service, timestamp):
        self.__service = service
        self.__timesstamp = timestamp

    def getService(self):
        return self.__service

    def getTimestamp(self):
        return self.__timesstamp

    def setTimestamp(self, timestamp):
        self.__timesstamp = timestamp

    def addURIs(self, uris):
        newUris = uris
        oldUris = self.__service.URIs
        for uri in newUris:
            if not uri in oldUris:
                oldUris.append(uri)
        self.__service.URIs = oldUris



        # Safe API to implement Strategy that copies a service and filters the uris
        # by modifying the uri list.


class URIFilterStrategy():
    # Modify the URIs
    # @param uris
    # @return if scheme matches
    def _filter(self, uris):
        pass

    def getResult(self, scheme, service):
        uris = DiscoveryTree.schemeFilter(scheme, self._filter(service.URIs))
        if not uris:
            return None
        result = Service(service.id, service.type)
        result.URIs = uris
        if service.data1:
            result.data1 = service.data1
        if service.data2:
            result.data2 = service.data2
        return result


# Simplifies the URIFilterStrategy to a check function wich returns true if
# the uri shall be in the target list and false if not.
class SimpleURIFilterStrategy(URIFilterStrategy):
    def _check(self, uri):
        pass

    def _filter(self, uris):
        checkedUris = []
        for uri in uris:
            try:
                if self._check(uri):
                    checkedUris.append(uri)
            except Exception as e:
                print "Filtered uri " + str(uri) + ". Use valid URIs!" + str(e)
        return checkedUris


class ALLURIFilterStrategy(URIFilterStrategy):
    # Used internally to filter the scheme. Since this is done automatically by
    # the getResult() function the list of uris ist just returned.
    def _filter(self, uris):
        # Scheme filtering is done in getResult() therefore just return the list again.
        return uris


# Filters for valid Host URIS where the host is a local address.
class LOCALONLYURIFilterStrategy(SimpleURIFilterStrategy):
    def _check(self, uri):
        try:
            parts = urisplit(uri)
            hostname = parts.host
            addr = socket.gethostbyname(hostname)
            # Check if the address is a valid special local or loop back
            # addr.isAnyLocalAddress() is currently ignored
            if addr.startswith("127.") or addr == "::1":
                return True
            # Check if the address is defined on any interface
            interface_list = netifaces.interfaces()
            for interface in interface_list:
                result = netifaces.ifaddresses(interface).values()
                for x in result:
                    for y in x:
                        if addr in y.values():
                            return True
        except Exception as e:
            print e
            return False
        return False


# Filters for all URIs that are not local (invalid uris are not filtered)
class REMOTEONLYURIFilterStrategy(SimpleURIFilterStrategy):
    def _check(self, uri):
        try:
            parts = urisplit(uri)
            hostname = parts.host
            addr = socket.gethostbyname(hostname)
            # Check if the address is a valid special local or loop back
            # addr.isAnyLocalAddress() is currently ignored
            if addr.startswith("127.") or addr == "::1":
                return False
            # Check if the address is defined on any interface
            interface_list = netifaces.interfaces()
            for interface in interface_list:
                result = netifaces.ifaddresses(interface).values()
                for x in result:
                    for y in x:
                        if addr in y.values():
                            return False
        except Exception as e:
            print e
            return True
        return True


# Filters for all URIs which are valid uris and the host is reachable
# within timeout milliseconds.
# @param timeout
class REACHABLEURIFilterStrategy(SimpleURIFilterStrategy):
    def __init__(self, timeout=5000):
        self.timeout = timeout

    def _check(self, uri):
        parts = urisplit(uri)
        hostname = parts.host
        addr = socket.gethostbyname(hostname)
        p = PingService(addr, 1.0, self.timeout)
        p.start()
        if p.isup():
            p.stop()
            return True
        return False


# Filters all but a single random uri.
class RANDOMURIFilterStrategy(URIFilterStrategy):
    def _filter(self, uris):
        if(len(uris) == 0):
            return uris

        result = random.sample(uris, 1)
        return [result]



class DiscoveryTree(Callback):
    ALL = ALLURIFilterStrategy()
    RANDOM = RANDOMURIFilterStrategy()
    REACHABLE = REACHABLEURIFilterStrategy()
    REMOTEONLY = REMOTEONLYURIFilterStrategy()
    LOCALONLY = LOCALONLYURIFilterStrategy()
    SCHEMES_ALL = ""
    SCHEMES_TECS = "tecs-"
    SCHEMES_TECS_PS = constants.SCHEME_PS
    SCHEMES_TECS_MP = constants.SCHEME_MP
    SCHEMES_TECS_RPC = constants.SCHEME_RPC
    SCHEMES_TECS_RPC_HTTP = constants.SCHEME_RPC_HTTP


    # Creates a new DiscoveryTree as a Callback for Discoverer. This class
    # helps unifying discovered udp services. The URIs are merged and some
    # strategies to find the best uri are provided. Old services (defined by
    # the servieTimeout) are automatically removed.
    #
    # This class is Thread-Safe.

    def __init__(self, serviceTimeout=5000):
        self.__serviceTimeout = serviceTimeout
        self.__discoveryIndex = {}
        self._discoveries = []
        self._lock = threading.RLock()

    # Filters all uris that does not start with scheme.
    @staticmethod
    def schemeFilter(scheme, uris):
        newUris = []
        for uri in uris:
            if uri.startswith(scheme):
                newUris.append(uri)
        return newUris


        # removes old services.

    def cleanUp(self):
        removed = 0
        newlist = []
        for service in self._discoveries:
            now = int(round(time.time()*1000))
            if (now - service.getTimestamp()) > self.__serviceTimeout:
                del self.__discoveryIndex[service.getService().id]
                # print "CleanUp routine removed old service: " + service.getService()
                removed = removed + 1
            else:
                newlist.append(service)
        self._discoveries = newlist
        # print "CleanUp routine removed  " + str(removed) + " old services from DiscoveryTree"


        # Returns a service
        #
        # @param id - id specified by the service
        # @param schemeFilter - string the scheme must start with e.g.
        # DiscoveryTree.SCHEMES_ALL or DiscoveryTree.SCHEMES_TECS.
        # @param strategy - a strategy to filter the uris even further.

    def getById(self, idString, schemeFilter=None, strategy=None):
        with self._lock:
            if schemeFilter is None:
                schemeFilter = DiscoveryTree.SCHEMES_ALL
            if strategy is None:
                strategy = DiscoveryTree.ALL
            self.cleanUp()
            if idString in self.__discoveryIndex:
                # SCHEMEFILTER may return null;
                return strategy.getResult(schemeFilter, self.__discoveryIndex[idString].service)
            return None




    def getByTyp(self, typ, schemeFilter=None, strategy=None):
        """
        Returns a service with the given type
        @param typ - typ specified by the service
        @param schemeFilter - string the scheme must start with e.g.
        DiscoveryTree.SCHEMES_ALL or DiscoveryTree.SCHEMES_TECS.
        @param strategy - a strategy to filter the uris even further.
        """

        with self._lock:
            if schemeFilter is None:
                schemeFilter = DiscoveryTree.SCHEMES_ALL
            if strategy is None:
                strategy = DiscoveryTree.ALL
            self.cleanUp()
            services = []
            for ds in self._discoveries:
                if ds.getService().type == typ:
                    service = strategy.getResult(schemeFilter, ds.getService())
                    if service:
                        services.append(service)
            return services




            # Returns a service
            #
            # @param typ - id specified by the service
            # @param schemeFilter - string the scheme must start with e.g.
            # DiscoveryTree.SCHEMES_ALL or DiscoveryTree.SCHEMES_TECS.
            # @param strategy - a strategy to filter the uris even further.

    def getBest(self, typ, schemeFilter, strategy):
        with self._lock:
            self.cleanUp()
            print "Trying to find best URI typ: " + str(typ) + " schemeFilter: " + str(schemeFilter) + " Strategy: " + str(strategy)
            services = []
            for ds in self._discoveries:
                if ds.getService().type == typ:
                    service = strategy.getResult(schemeFilter, ds.getService())
                    if service:
                        services.append(service)
            if len(services) == 0:
                return None

            # try:
            if len(services) > 1:
                print "SortStrategy results in more than one service. Take random one."
            finalServiceL = random.sample(services, 1)
            finalService = finalServiceL[0]
            if len(finalService.URIs) > 1:
                print "Different URIs are available. Using random one."
            finaluriL = random.sample(finalService.URIs, 1)
            finaluri = finaluriL[0]
            print "Best Solution is: " + str(finaluri)
            return finaluri
            # TODO sth similiar in python?
            # catch (URISyntaxException ex) {

    def getAll(self, schemeFilter=None, strategy=None):
        with self._lock:
            if schemeFilter is None:
                schemeFilter = DiscoveryTree.SCHEMES_ALL
            if strategy is None:
                strategy = DiscoveryTree.ALL
            self.cleanUp()
            services = []
            for ds in self._discoveries:
                service = strategy.getResult(schemeFilter, ds.getService())
                if service:
                    services.append(service)
            return services


        # Implements the callback and fills the datastructure.
        # Hint: If overwritten super should be called at first.

    def onDiscovered(self, discoverer, service):
        with self._lock:
            #print "DiscoveryTree received: " + str(service)
            # print "Available: " + str(len(self._discoveries))
            if service.id in self.__discoveryIndex:
                ds = self.__discoveryIndex[service.id]
                if not (ds.getService().type == service.type):
                    print "Discovered and ignored a service same id (" + service.id + ") but different types! type1: " + service.type + " type2: " + ds.getService().type
                    return
            else:
                now = int(round(time.time()*1000))
                ds = DiscoveredService(service, now)
                self._discoveries.append(ds)
                self.__discoveryIndex[service.id] = ds

            now = int(round(time.time()*1000))
            ds.setTimestamp(now)
            ds.addURIs(service.URIs)
			# copy newer data if available
            if service.data1:
                ds.getService().data1 = service.data1
            if service.data2:
                ds.getService().data2 = service.data2
            self.cleanUp()




