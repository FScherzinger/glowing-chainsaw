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
from struct import *

class EventFrame:
	"""Used as a buffer of a full event frame. 
	   Data is transfered from socket into the ringbuffer. Then from the ringbuffer into an EventFrame.
	   If the EventFrame contains exactly one Event it is interpreted and forwarded to the client's queue.
	"""
	def __init__(self):
		self.size = 0
		self.write_ = 0
		self.read_ = 0
		self.data = None
	
	def reset(self):		
		self.size = 0
		self.write_ = 0
		self.read_ = 0
		self.data = None

	def setSize(self, size):
		if (size < 8):
			raise RuntimeError("Size must be at least 8 bytes")
		self.size = size
		self.write_ = 0
		self.read_ = 0
		self.data = bytearray(size)

	def getSize(self):
		return self.size
	
	def getMissing(self):
		return self.size - self.write_

	def getAvailable(self):
		return self.size - self.read_

	def isFinished(self):
		return self.getMissing() == 0

	def write(self, data, bytes):
		if (bytes == 0):
			return
		if (self.getMissing() >= bytes):
			self.data[self.write_:self.write_+bytes] = data[0:bytes]
			self.write_ += bytes
			return
		raise RuntimeError("Frame Size exceeded " + self.getMissing() + " " + self.getAvailable() + " " + self.getSize())

	def read(self, data, bytes):
		if (bytes == 0):
			return
		if (not self.isFinished()):
			raise RuntimeError("Frame is not completely written")
		if (self.getAvailable() >= bytes):
			data[0:bytes] = self.data[self.read_:self.read_+bytes]
			self.read_ += bytes
			return
		raise RuntimeError("Frame has not enough data to read")

	def readI32(self):
		data = bytearray(4)
		self.read(data, 4)
		return unpack('!i', data)[0]
	


