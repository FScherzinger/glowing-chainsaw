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

from __future__ import with_statement

import sys, glob
import socket
import re
import time
import os
import platform
import time
import threading



class ReconnectSettings:

	DEFAULT_TIME_BETWEEN_CONNECT_ATTEMPTS = 1500
    

     # Always tries to reconnect
	def __init__(self, numOfReconnects=None, reconnectInterval=None):
		if numOfReconnects is None:
			self.__reconnectAttempts = -1
		else:
			self.__reconnectAttempts = numOfReconnects
		if reconnectInterval is None:
			self.__reconnectInterval = self.DEFAULT_TIME_BETWEEN_CONNECT_ATTEMPTS
		else:
			self.__reconnectInterval = reconnectInterval
		self.__remainingAttempts = 0
		self.__reconnectLock = threading.RLock()
		self.reset()
		
        
   
  
	def shouldReconnect(self):
		with self.__reconnectLock:
			if self.__reconnectAttempts < 0:
				return True
			if self.__remainingAttempts > 0:
				self.__remainingAttempts = self.__remainingAttempts - 1
				return True
			else:
				return False
	       

	def reset(self):
		with self.__reconnectLock:
			self.__remainingAttempts = self.__reconnectAttempts
    
	
	def toString(self):
		with self.__reconnectLock:
			if self.__reconnectAttempts < 0:
				return "Remaining reconnect attempts: infinite"
			else:
				return "Remaining reconnect attempts: " + str(self.__remainingAttempts)
    

	def getReconnectAttempts(self):
		with self.__reconnectLock:
			return self.__reconnectAttempts
    

  
	def getRemainingAttempts(self):
		with self.__reconnectLock:
			return self.__remainingAttempts

    
	def setRemainingAttempts(self, remainingAttempts):
		with self.__reconnectLock:
			self.__remainingAttempts = remainingAttempts
   

	def getReconnectInterval(self):
		with self.__reconnectLock:
			return self.__reconnectInterval;
    

	def setReconnectAttempts(self, reconnectAttempts):
		with self.__reconnectLock:
			self.__reconnectAttempts = reconnectAttempts;
    

	def setReconnectInterval(self, reconnectInterval):
		with self.__reconnectLock:
			self.__reconnectInterval = reconnectInterval;
    

