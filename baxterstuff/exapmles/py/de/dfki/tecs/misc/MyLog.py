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
from __future__ import print_function
import sys



def LOG_INFO(*args):
    print("[INFO] ", *args)

def LOG_ERR(*args):
    print("[ERR] ", *args, file=sys.stderr)

def LOG_WARN(*args):
    print("[WARN] ", *args, file=sys.stderr)

def LOG_DEBUG(*args):
    print("[WARN] ", *args, file=sys.stderr)

#TODO rename to TecsLog
class MyLog:
    def __init__(self, TAG):
        self.TAG = TAG

    def LOG_INFO(self, *args):
        LOG_INFO(self.TAG, *args)

    def LOG_ERR(self, *args):
        LOG_ERR(self.TAG, *args)

    def LOG_WARN(self, *args):
        LOG_WARN(self.TAG, *args)

    def LOG_DEBUG(self, *args):
        LOG_DEBUG(self.TAG, *args)

    def info(self, *args):
        LOG_INFO(self.TAG, *args)

    def err(self, *args):
        LOG_ERR(self.TAG, *args)

    def warn(self, *args):
        LOG_WARN(self.TAG, *args)

    def debug(self, *args):
        LOG_DEBUG(self.TAG, *args)
