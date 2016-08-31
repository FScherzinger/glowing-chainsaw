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
import socket
import struct
import sys
import errno

from socket import error
from thrift.transport import TTransport
from thrift.protocol import TBinaryProtocol

from de.dfki.tecs.misc.UDPMessage import UDPMessage

#UDP socket: blocking (timeout) + multicast

class UDPMulticastSocket():
	MAX_DATAGRAM_SIZE = 65536

	def __init__(self, port= None, reuseAddr= None):
		if port and not reuseAddr:
			self.__port = port
			self.__reuseAddr = True
		if not port and reuseAddr:
			self.__port = 0
			self.__reuseAddr = reuseAddr
		if port and reuseAddr:
			self.__port = port
			self.__reuseAddr = reuseAddr
		if not port and not reuseAddr:
			self.__port = 0
			self.__reuseAddr = False
		self.__bound = False
		self.__socket = None
		self.__recvBuffer = None

	
	def getPort(self):
		return self.__port


	def joinMulticastGroup(self,  hostName):		
		host = socket.gethostbyname(hostName)
		group = socket.inet_aton(host)
		mreq = struct.pack("4sl", group, socket.INADDR_ANY)
		self.__socket.setsockopt(socket.IPPROTO_IP, socket.IP_ADD_MEMBERSHIP, mreq)
		print "Joined multicast group: " + str(host) + "."


	def leaveMulticastGroup(self, host):
		self.__socket.close()


	def isBound(self):
		return self.__bound


	def setBound(self, bound):
		self.__bound = bound


	def bind(self):
 		if self.isBound():
			print "Can't bind: Already running."
			return
		#self.__socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM, socket.IPPROTO_UDP)
		self.__socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
		if self.__reuseAddr:
			self.__socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEPORT , 1) 
			self.__socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR , 1) 
		else:
			self.__socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEPORT, 1) 
		print ("Binding 0.0.0.0 %d \n" % self.__port)
		self.__socket.bind(('0.0.0.0', self.__port))	
		self.setBound(True)
		

	def unbind(self):
		if not self.isBound():
			print "Can't shutdown: Not running."
			return
		self.__socket.close()
		self.setBound(False)
		print "Stopped."


	#Sends the given msg to the message's address. See UdpMessage::setHost
	def send(self, udpMsg):
		try:
			self.__socket.sendto(udpMsg.getBuffer(),(udpMsg.getHost(), udpMsg.getPort()))
		except error as e:
			print "Could not send message: Sending failed: ", "errno=",e.errno, " msg=", e.strerror
  

	# @return UdpMessage if a udp msg was received. The content of the UdpMessage may still
    # be None if e.g. an error happened during parsing. If no UdpMessage could be received this
    # function returns None. The UdpMessage contains the address and port of the sender.
	def recv(self):
		if not self.isBound():
			print "Can't receive: Not bound"
			return None
		try:
			self.__socket.setblocking(0)
			self.__recvBuffer = self.__socket.recvfrom(UDPMulticastSocket.MAX_DATAGRAM_SIZE)
		except socket.error as e:
			if not errno.EWOULDBLOCK:
				print e
				sys.exit(1)
		if not self.__recvBuffer:
			return None
		else:	
			data = self.__recvBuffer[0]
			host = self.__recvBuffer[1][0]
			port = self.__recvBuffer[1][1]
			return UDPMessage(data, host, port)
				


