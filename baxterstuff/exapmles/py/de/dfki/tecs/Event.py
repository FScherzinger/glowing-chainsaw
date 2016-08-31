##
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

from thrift import Thrift

from de.dfki.tecs.basetypes.constants import *
from de.dfki.tecs.basetypes.ttypes import *
from de.dfki.tecs.misc.Deserializer import Deserializer
from de.dfki.tecs.misc.EventFrame import EventFrame


class Event:

	def __init__(self, frame):
		try:
			header_size = frame.readI32()
			header_raw = bytearray(header_size)
			frame.read(header_raw, header_size)
			try:
				data_size = frame.getSize() - header_size - 4
				self.__data = bytearray(data_size)
				frame.read(self.__data, data_size)
			except Exception as e:
				raise Exception("Cannot read data size due to () header was  ", e)
			self.__header = EventHeader()
			self.deserializer = Deserializer()
			self.deserializer.deserialize(self.__header, header_raw)
		except Exception as e:
			raise RuntimeError("Cannot read header size due to", e)


    # Returns the source.
	def getSource(self):
		return self.__header.source

    
    # Returns the target.
	def getTarget(self):
		return self.__header.target

    
    # Returns the time.
	def getTime(self):
		return self.__header.time

 
    # Returns the Event Type. Better check etype using this.is(...)
	def getEtype(self):
		return self.__header.etype


    # Sets the source of this event.
	def setSource(self, source):
		self.__header.source = source


     # Sets the target. This must be a valid regular expression.
	def setTarget(self, target):
		self.__header.target = target


    # Sets the event type.
	def setEtype(self, etype):
		self.__header.etype = etype

   
    # Sets the time in ms since unix epoch.
	def setTime(self,time):
		self.__header.time = time
  

	def getData(self):
		return self.__data
    

	def is_(self, name):
		return self.__header.etype == name

	def is_a(self, name):
		return self.__header.etype == name


	#@throws RuntimeError If deserialization failed
	def parseData(self, evt):
		try:
			self.deserializer.deserialize(evt, self.__data)
		except Thrift.TException as tx:
			raise RuntimeError(tx)	
		
 
