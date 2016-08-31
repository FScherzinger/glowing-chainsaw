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
import time
import sys, glob
import os
import platform


__author__ = 'yk'


#Helps to execute a command at most once during a given time interval.
class IntervalHelper(object):
	def __init__(self, executeInterval):
		self._executeInterval = executeInterval
		self.last = 0

	# Returns true iff the time elapsed since the last call it returned true
    # is higher than the execution interval given in the constructor.
    # Otherwise it returns false.
	def shouldExecute(self):
		if self._executeInterval < 0:
			return False
		now = int(round(time.time()*1000))
		diff = now - self.last
		if diff >= self._executeInterval:
			self.last = now
			return True
		else:
			return False


	# IntervalHelper::shouldExecute will return at most once true during the given time interval.
    # Given executeInterval < 0 IntervalHelper::shouldExecute will always return false.
	def setExecuteInterval(self, executeInterval):
		self._executeInterval = executeInterval
    
	
	def getExecuteInterval(self):
		return self._executeInterval
  

	#Rewinds the timer such that whole interval has to elapse again such that IntervalHelper::shouldExecute returns true again.
	def reset(self):
		lastTime = int(round(time.time()*1000))



