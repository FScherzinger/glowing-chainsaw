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
uri = PSFactory.createURI('python-client-pap', 'localhost', 9000)
client = PSFactory.createPSClient(uri)
# subscribe before connecting
client.subscribe("DoneEvent")
client.subscribe("PickAndPlaceEvent")
# connect
client.connect()


# create event
event = PickAndPlaceEvent()
event.limb = Limb.LEFT
# define position from where to pick the object
event.initial_pos = Position()
event.initial_pos.X_left = "1"
event.initial_pos.Y_left = "2"
event.initial_pos.Z_left = "3"

# define the orientation how to pick the object
event.initial_ori = Orientation()
event.initial_ori.Yaw_left = "1"
event.initial_ori.Pitch_left = "1"
event.initial_ori.Roll_left = "1"

# define position where to place the object
event.final_pos = Position()
event.final_pos.X_left = "1"
event.final_pos.Y_left = "2"
event.final_pos.Z_left = "3"
# define orientation how to place the object
event.final_ori = Orientation()
event.final_ori.Yaw_left = "1"
event.final_ori.Pitch_left = "1"
event.final_ori.Roll_left = "1"

# define speed of movement
event.speed = Speed()
event.speed.Speed_left = "0.2"	# possible values x: 0.0 <= x <= 1.0

# define mode and kinematics
event.angls = Angles()
event.mode = Reference_sys.ABSOLUTE
event.kin = Kinematics.INVERSE

# DEBUG output
print ("Sending PickAndPlaceEvent");
# Send event to client (consumer)
# client.send([ClientAddress], [Event = Struct Name], [Struct])
client.send("baxter_dummy", "PickAndPlaceEvent", event)

# check if client is still connected to server
while (client.isConnected()):
	# check if there is something to receive
	while (client.canRecv()):
		# get received event
		evt = client.recv()
		# DEBUG output
		print ("Received: %s from %s" % (evt.getEtype() ,evt.getSource()));
		
		# check which event was received
		if (evt.is_a("DoneEvent")):
			# Get values of received event
			de = DoneEvent()
			evt.parseData(de)

			# DEBUG output
			print ("Message: %s\n" % de.message)

			# disconnect client and terminate
			client.disconnect()
			break

# DEBUG output	
print ("Successfully shutdown and stopped")

