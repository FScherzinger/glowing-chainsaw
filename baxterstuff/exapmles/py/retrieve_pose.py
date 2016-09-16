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
uri = PSFactory.createURI('python-sender', '192.168.1.101', 9000)
client = PSFactory.createPSClient(uri)
# subscribe before connecting
client.subscribe("RetrievePoseEvent")
# connect
client.connect()

# create event
event = RetrievePoseEvent()
event.pos = Position()
event.ori = Orientation()

# DEBUG output
print ("Sending RetrievePoseEvent");
# Send event to client "receiver"
# client.send([ClientAddress], [Event = Struct Name], [Struct])
client.send("receiver_right", "RetrievePoseEvent", event)

# check if client is still connected to server
while (client.isConnected()):
	# check if there is something to receive
	while (client.canRecv()):
		# get received event
		evt = client.recv()
		# DEBUG output
		print ("Received: %s from %s addressed to %s at %d" % (evt.getEtype() ,evt.getSource(), evt.getTarget(), evt.getTime()));
		
		# check which event was received
		if (evt.is_a("RetrievePoseEvent")):
			# Get values of received event
			e = RetrievePoseEvent()
			evt.parseData(e)

			# DEBUG output
			print ("Position: X %s, Y %s, Z %s ...\n" % (e.pos.X_right, e.pos.Y_right, e.pos.Z_right)) 
			print("Orientation: Pitch %s, Yaw %s, Roll %s ...\n" % (e.ori.Pitch_right, e.ori.Yaw_right, e.ori.Roll_right))
			# disconnect client and terminate
			client.disconnect()
			break

# DEBUG output	
print ("Successfully shutdown and stopped")

