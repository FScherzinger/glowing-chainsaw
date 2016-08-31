#!/usr/bin/python2.7
#-------------------------------------------------------------------------------
# The MIT License
#
# Copyright 2016 Winfried.Schuffert@DFKI.de.
#
# Permission is hereby granted, free of charge, to any person obtaining a copy
# of this software and associated documentation files (the "Software"), to deal
# in the Software without restriction, including without limitation the rights
# to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
# copies of the Software, and to permit persons to whom the Software is
# furnished to do so, subject to the following conditions:
#
# The above copyright notice and this permission notice shall be included in
# all copies or substantial portions of the Software.
#
# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
# IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
# FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
# AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
# LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
# OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
# THE SOFTWARE.
#-------------------------------------------------------------------------------
#
# @author Winfried.Schuffert@DFKI.de
#
#------------------------------------------------------------------------------- 

from de.dfki.tecs.ps.PSFactory import PSFactory

#import generated user space source
from genpy.de.dfki.tecs.robot.baxter.constants import *
from genpy.de.dfki.tecs.robot.baxter.ttypes import *

# connect to server
# PSFactory.createURI([ClientName], [ServerAddress], [ServerPort]);
uri = PSFactory.createURI('python-client-gripper', 'localhost', 9000)
client = PSFactory.createPSClient(uri)
# subscribe before connecting
client.subscribe("DoneEvent")
client.subscribe("GripperEvent")
# connect
client.connect()

# create event
event = GripperEvent()
event.limb = Limb.LEFT
event.action = Gripper_state.OPEN


# DEBUG output
print ("Sending GripperEvent");
# Send event to client (consumer)
# client.send([ClientAddress], [Event = Struct Name], [Struct])
client.send("baxter_dummy", "GripperEvent", event)

# check if client is still connected to server
while (client.isConnected()):
	# check if there is something to receive
	while (client.canRecv()):
		# get received event
		evt = client.recv()
		# DEBUG output
		print ("Received: %s from %s" % (evt.getEtype() ,evt.getSource()))
		
		# check which event was received
		if (evt.is_a("DoneEvent")):
			# Get values of received event
			de = DoneEvent()
			evt.parseData(de)

			# DEBUG output
			if(de.error):
				print "Error:"
			else:
				print "Message:"
			print ("%s\n" % de.message)

			# disconnect client and terminate
			client.disconnect()
			break

# DEBUG output	
print ("Shutdown and stopped")

