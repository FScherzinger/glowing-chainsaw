#!/usr/bin/python2.7
# coding: utf8

# The Creative Commons CC-BY-NC 4.0 License
#
# http://creativecommons.org/licenses/by-nc/4.0/legalcode
#
# Creative Commons (CC) by DFKI GmbH
#  - Christian Buerckert <Christian.Buerckert@DFKI.de>
#  - Yannick Koerber <Yannick.Koerber@DFKI.de>
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
import time
import os
import platform
import threading

from thrift import Thrift

from de.dfki.tecs.misc.IUDThread import IUDThread
from de.dfki.tecs.mp.IConnectionHandler import IConnectionHandler
from de.dfki.tecs.basetypes.constants import *
from de.dfki.tecs.basetypes.ttypes import *
from de.dfki.tecs.Event import Event



 # A simple connection handler that saves some cpu cycle (by sleeping) if no new data arrived.
class DefaultConnectionHandler(IConnectionHandler):

    def connectionEstablished(self, socket):
        print "Connection established: " + socket
    
    
    def updateConnection(self, socket):
        event = socket.recv()
        if event is None:
            if not socket.isReceivingData():
                IUDThread.sleep(50);
            return
        else:
            self.onEvent(event)
      

    def connectionLost(self, socket):
        print "Connection lost: " + socket
   

    def onEvent(self, event):
        pass
