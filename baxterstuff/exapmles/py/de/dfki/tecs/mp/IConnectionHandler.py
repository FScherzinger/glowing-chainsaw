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



 #Specifies the communication logic for a client or a server.
 # The methods IConnectionHandler::connectionEstablished and IConnectionHandler::connectionLost are called
 # once. IConnectionHandler::updateConnection is called frequently while the connection is still valid.
class IConnectionHandler:

     # Called once as soon as connection could be established.
     # @param socket The socket. Use it for receiving and sending messages.
    def connectionEstablished(self, socket):
        pass


     # Add your client logic here. Use the given socket for sending and receiving messages.
     # This method is called frequently as long as the connection is valid.
     #
     # The cpu load can be reduced by adding something like:
     # msg = read();
     # if(msg == null){
     #     if(!socket.isReceivingData()){
     #          IUDThread.sleep(50);
     #     }
     #     return;
     # }
     # @param socket The socket. Use it for receiving and sending messages.
    def updateConnection(self, socket):
        pass


     # Called once as soon as the connection is lost or shutdown. In the latter case the
     # socket may still be in a valid state for sending or receiving messages 
     # @param socket The socket. Use it for receiving and sending messages.
    def connectionLost(self, socket):
        pass
