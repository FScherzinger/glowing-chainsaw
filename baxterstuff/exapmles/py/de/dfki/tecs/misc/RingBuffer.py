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

class RingBuffer:
	"""
		Typical fast RingBuffer implementation (FIFO). It does 1 or 2 array copies depending
		on the position of the pointer. 
	"""
	def __init__(self, capacity):
		"""
			Creates the Buffer with capacity (int) of bytes
		"""
		self.data = bytearray(capacity);
		self.capacity = capacity;
		self.beginIndex = 0;
		self.endIndex = 0;
		self.available = 0;

	def getAvailable(self):
		"""
			Returns the amout of bytes written into the buffer.
			So the "available" bytes
		"""
		return self.available

	def getFree(self):
		"""
			Returns the amout of free bytes in the ring buffer
		"""
		return self.getCapacity() - self.getAvailable();

	def getCapacity(self):
		"""
			Returns the initial set capacity
		"""
		return self.capacity

	def reset(self):
		self.available = 0;
		self.endIndex = 0;
		self.beginIndex = 0;

	def write (self, data, bytes) :
		"""
			writes 'bytes' (int) bytes of data (bytearray) into the ringbuffer.
			If the buffer has not enough space left the maximum possible bytes
			are written.
			It returns the amout of bytes actually written.
		"""
		if (bytes == 0):
			return 0
		btw = min(bytes, self.getFree())
		
		if (btw <= self.getCapacity() - self.endIndex):
			self.data[self.endIndex:self.endIndex+btw] = data[0:btw];
			self.endIndex += btw;
			if (self.endIndex == self.getCapacity()):
				self.endIndex = 0
		else:
			size_1 = self.getCapacity() - self.endIndex
			self.data[self.endIndex:self.endIndex+size_1] = data[0:size_1]
			size_2 = btw - size_1
			self.data[0:size_2] = data[size_1:size_1+size_2]
			self.endIndex = size_2
		self.available += btw
		return btw

	def read(self, data, bytes):
		"""
			Reads 'bytes' (int) bytes into 'data' (bytearray) from the ringbuffer.
			If there are less then 'bytes' bytes available the maximum amount is read into 'data'.
			It returns the amount of bytes actually read.
		"""
		if (bytes == 0):
			return 0
		btr = min(bytes, self.getAvailable())
		
		if (btr <= self.getCapacity() - self.beginIndex):
			data[0:btr] = self.data[self.beginIndex:self.beginIndex + btr]
			self.beginIndex += btr
			if (self.beginIndex == self.getCapacity()):
				self.beginIndex = 0
		else:
			size_1 = self.getCapacity() - self.beginIndex
			size_2 = btr - size_1
			data[0:size_1] = self.data[self.beginIndex:self.beginIndex+size_1]
			data[size_1:size_1+size_2] = self.data[0:size_2]
			self.beginIndex = size_2
		self.available -= btr
		return btr

	def readI32(self):
		"""
			Reads one int 32bit from this ringbuffer. It is interpreted in NetworkByteOrder and translated
			to Host Byte Order. (TBS sends frame sizes in 32bit NBO signed integers)
		"""
		data = bytearray(4)
		self.read(data, 4)
		return unpack('!i', data)[0]
