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

from array import *

# connect to server
uri = PSFactory.createURI('python-client-ma', 'localhost', 9000)
client = PSFactory.createPSClient(uri)

# subscribe before connecting
client.subscribe("DoneEvent")
client.subscribe("MoveArmEvent")
# connect
client.connect()

# 
# for i in range(5):
# 	print i

Pos_x = ["0.75", "0.9", "0.6", "0.9", "0.6"]
Pos_y = ["0.4", "0.2", "0.2", "0.5", "0.5"]
Pos_z = ["0.0", "0.3", "0.0", "0.3", "0.0"]

Ori_y = ["0.0", 	"", 	"", 	"", 	""]
Ori_p = ["0.0", "1.57", "0.0", "1.57", "0.0"]
Ori_r = ["3.14", 	"", 	"", 	"", 	""]

# create event
event = MoveArmEvent()
event.limb = Limb.LEFT
# define position from where to pick the object
event.pos = Position()

# define the orientation how to pick the object
event.ori = Orientation()

# define speed of movement
event.speed = Speed()
event.speed.Speed_left = "0.4"	# possible values x: 0.0 <= x <= 1.0

# define mode and kinematics
event.angls = Angles()
event.mode = Reference_sys.ABSOLUTE
event.kin = Kinematics.INVERSE

active = True

for i in range(5):
	print i
	active = True

	# define position from where to pick the object
	event.pos.X_left = Pos_x[i]
	event.pos.Y_left = Pos_y[i]
	event.pos.Z_left = Pos_z[i]

	# define the orientation how to pick the object
	event.ori.Yaw_left = Ori_y[i]
	event.ori.Pitch_left = Ori_p[i]
	event.ori.Roll_left = Ori_r[i]

	# DEBUG output
	print ("Sending MoveArmEvent %d" % (range(1,6)[i]));
	# Send event to client (consumer)
	# client.send([ClientAddress], [Event = Struct Name], [Struct])
	# client.send("baxter_dummy", "MoveArmEvent", event)
	client.send("baxter_dummy", "MoveArmEvent", event)

	# check if client is still connected to server
	while (client.isConnected() and active):
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
				if de.error:
					print ("ERROR: %s\n" % de.message)
				else:
					print ("Message: %s\n" % de.message)

				active = False
				break

				
# disconnect client and terminate
client.disconnect()

# DEBUG output	
print ("Successfully shutdown and stopped")

