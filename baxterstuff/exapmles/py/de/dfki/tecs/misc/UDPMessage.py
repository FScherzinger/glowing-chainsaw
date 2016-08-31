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

from thrift.transport import TTransport
from thrift.protocol import TBinaryProtocol

class UDPMessage():

	def __init__(self, buf, host, port):	
		self.__buf = buf
		self.__host = host
		self.__port = port


	def getBuffer(self):
		return self.__buf


	def setBuffer(self, buf):
		self.__buf = buffer
	
	
	def getPort(self):
		return self.__port


	def setPort(self, port):
		self.__port = port


	def getHost(self):
		return self.__host
   
	def setHost(host):
		self.__host = host

	def toString(self):
		str = "UDPMessage {" 
		str += "buffer=" + str(buffer) 
		str +=  ", port=" + str(port) 
		str +=  ", host=" + str(host) 
		str +=  '}'
		return str
  
