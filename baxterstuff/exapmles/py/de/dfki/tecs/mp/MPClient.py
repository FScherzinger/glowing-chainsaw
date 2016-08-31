#!/usr/bin/python2.7
# coding: utf8

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
import time
import threading

from thrift import Thrift
from thrift.transport import TSocket
from thrift.transport import TTransport
from thrift.protocol import TBinaryProtocol 

from de.dfki.tecs.misc.IUDThread import IUDThread
from de.dfki.tecs.misc.ReconnectSettings import ReconnectSettings
from de.dfki.tecs.mp.MPClientSocket import MPClientSocket
from de.dfki.tecs.basetypes.constants import *
from de.dfki.tecs.basetypes.ttypes import *


 # Threaded TCP client for TECS-MP communication that is suitable
 # Implement your client logic either as IConnectionHandler or overwrite the functions
 # MPClient::connectionEstablished, MPClient::connectionList and MPClient::connectionUpdate.
 # The function MPClient::setReconnectSettings allows to adapt behavior on disconnect

class MPClient(IUDThread):


    def __init__(self, clientSocket, connectionHandler):
        IUDThread.__init__(self)
        self.__clientSocket = clientSocket
        self.__connectionHandler = connectionHandler
        self.__callback = Callback()
        self.__reconnectSettings = ReconnectSettings(-1)


    #call the connectionEstablished method of the handler 
    def connectionEstablished(self, socket):
        if self.__connectionHandler:
            self.__connectionHandler.connectionEstablished(socket)
     
    #call the connectonLost method of the handler 
    def connectionLost(self, socket):
        if self.__connectionHandler:
            self.__connectionHandler.connectionLost(socket)
    
    #call the updateConnection method of the handler  
    def updateConnection(self, socket):
        if self.__connectionHandler:
            self.__connectionHandler.updateConnection(socket)
    

    def initialize(self):
        pass


    def deinitialize(self):
        pass

    #update method regualarly called from IUDThread
    def update(self):
        #check if reconnection should be performed
        if not self.__reconnectSettings.shouldReconnect():
            print "Stop trying to connect"
            self.shutdown()
            return
     
        if not self.__clientSocket.isConnected():
            try:
                #connect
                self.__clientSocket.connect()
            except Exception as e:   # SocketAccessException e
                print "Could not connect: " + str(e)
                IUDThread.sleep(self.__reconnectSettings.getReconnectInterval())
                return
        self.connectionEstablished(self.__clientSocket)
        if self.__callback:
            self.__callback.notifyConnected(self)
        while self.isRunning() and self.__clientSocket.isConnected():
            try:
                self.updateConnection(self.__clientSocket)
            except Exception as ex: #(SocketAccessException ex) 
                print "Disconnected: " + str(ex)
                break
            except RuntimeError as e:
                print str(e)
                break

        if self.__clientSocket.isConnected():
            try:
                self.__clientSocket.disconnect()
            except Exception as e: # (SocketAccessException e) 
                print str(e)
           
        self.connectionLost(self.__clientSocket)
        if self.__callback:
            self.__callback.notifyDisconnected(self)
        IUDThread.sleep(self.__reconnectSettings.getReconnectInterval())



    def getReconnectSettings(self):
        return self.__reconnectSettings
    

    def setReconnectSettings(self, reconnectSettings):
        if reconnectSettings is None:
            raise ValueError("ReconnectSettings can't be none")
        self.__reconnectSettings = reconnectSettings
    


     # Sets the callback that is used to notify on client (dis-)connects.
    def setCallback(self, callback):
        self.__callback = callback



# Used for notifying about (dis-)connects.
class Callback:


    # Called once for each new connection.
    def notifyConnected(self, client):
        pass


    # Called once for each lost connection.
    def notifyDisconnected(self,client):
        pass
    