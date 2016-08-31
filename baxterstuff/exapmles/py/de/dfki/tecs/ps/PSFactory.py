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
from uritools import *


from de.dfki.tecs.discovery.DiscoveryTree import *
from de.dfki.tecs.discovery.Discoverer import *
from de.dfki.tecs.basetypes.constants import *
from de.dfki.tecs.ps.PSClient import *

	

class PSFactory:

	@staticmethod
	def createPSClient(param): 
		#param is either uri or basestring id
		return PSClient(param)

	# Creates a URI for a specified tecs-server.
	@staticmethod
	def createURI(id, host, port):
		return uricompose(scheme=SCHEME_PS, authority="", path="", userinfo=id, host=host, port=port, encoding='utf-8')
	

	# Automatically finds a tecs-server in the network and associates the client's id.
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
