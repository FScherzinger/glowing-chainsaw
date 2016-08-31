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
import threading
from time import sleep
from de.dfki.tecs.misc.MyLog import MyLog


class IUDThread:
    # Overwrite update(), initialize() and deinitialize()

    __STATE_INACTIVE = 0
    __STATE_ACTIVE = 1
    __STATE_SHUTDOWN = 2

  
    def __init__(self):
        self.__state = IUDThread.__STATE_INACTIVE
        self.__stateLock = threading.RLock()
        self.__daemon = True
        self.__thread = None
        self.log = MyLog("IUDThread")


    # Initialize Update Deinitialize a Thread.
    def _run(self):
        self.initialize()
        while self.isRunning():
            self.update()
        self.deinitialize()
        with self.__stateLock:
            self.__state = IUDThread.__STATE_INACTIVE
     

    #start new thread
    # @throws ValueError if not ready (shutting down or already running)
    def start(self):
        if not self.isReady():
            raise ValueError("Can't start thread: Thread is not ready.. old instance still active?")
        #create new thread
        self.__thread = threading.Thread(target=self._run)
        #set some attributes
        with self.__stateLock:
            self.__state = IUDThread.__STATE_ACTIVE
            self.__thread.daemon = self.__daemon
        #call the thread's run method
        self.__thread.start()
        return self



    #Starts this thread. If it is already running or net yet ready this call
    # will stop the old thread and block until it is ready
    def restart(self):
        while not self.isReady():
            self.shutdown() 
        self.start()

   
    # @param ms timeout in milliseconds
    def join(self, ms=None):
        with self.__stateLock:
            if self.__thread is None:
                return
        if ms:
            self.__thread.join(ms)
        else:
            self.__thread.join()
   

    def initialize(self):

        # Called once as the thread is started

        pass

    def deinitialize(self):

        # Called once as the thread is stopped

        pass

    def update(self):

        # Overwrite me

        pass


    def isRunning(self):
        with self.__stateLock:
            return self.__state == IUDThread.__STATE_ACTIVE
     
        
    def isReady(self):
        with self.__stateLock:
            return self.__state == IUDThread.__STATE_INACTIVE
        
    def isShuttingDown(self):
        with self.__stateLock:
            return self.__state == IUDThread.__STATE_SHUTDOWN
        

    def shutdown(self):
        with self.__stateLock:
            if not self.isRunning():
                self.log.LOG_INFO("Can't shutdown: Not running")
                return
            self.__state = IUDThread.__STATE_SHUTDOWN


    def setDaemon(self, daemon):
        self.__daemon = daemon
  


    # Causes the current thread to sleep for the given amount of time.
    # Exceptions are silently ignored. Interrupt flags are kept if the thread
    # is interrupted during sleeping.
    @staticmethod
    def sleep(durationMS):
        if durationMS <= 0:
            return
        sleep(durationMS / 1000.0)
