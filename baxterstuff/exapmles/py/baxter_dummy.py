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
import time

# connect to server
# PSFactory.createURI([ClientName], [ServerAddress], [ServerPort]);
uri = PSFactory.createURI('baxter_dummy', 'localhost', 9000)
client = PSFactory.createPSClient(uri)
# subscribe before connecting
client.subscribe("DoneEvent")
client.subscribe("MoveArmEvent")
client.subscribe("PickAndPlaceEvent")
client.subscribe("RetrievePoseEvent")
client.subscribe("RetrieveAnglesEvent")
client.subscribe("GripperEvent")

# connect
client.connect()

# DEBUG output
print ("Waiting for Events\n")

# check if client is still connected to server
while (client.isConnected()):
	# check if there is something to receive
	while (client.canRecv()):
		# get received event
		evt = client.recv()
		# DEBUG output
		print ("Received: %s from %s" % (evt.getEtype() ,evt.getSource()))
	
		# check which event was received
		if (evt.is_a("MoveArmEvent")):
			# Get values of received event
			mae = MoveArmEvent()
			evt.parseData(mae)

			# DEBUG output
			print ("MoveArmEvent")
			print ("Message: Limb: %s, X: %s, Y: %s, Z: %s ...\n" % (mae.limb, mae.pos.X_left, mae.pos.Y_left, mae.pos.Z_left)) 
			print ("Performing Event!")

			
			de = DoneEvent()
			if (mae.mode != Reference_sys.ABSOLUTE and mae.mode != Reference_sys.RELATIVE):
				de.error = True
				de.message = ("Error in MoveArmEvent. Unknown mode: %s" % (mae.mode))
			else:
				# perform action (DUMMY)
				time.sleep(3)

			# Send DoneEvent to client that sends the event
			client.send(evt.getSource(), "DoneEvent", de) 


		elif (evt.is_a("PickAndPlaceEvent")):
			# Get values of received event
			pap = PickAndPlaceEvent()
			evt.parseData(pap)

			# DEBUG output
			print ("PickAndPlaceEvent")
			print ("Message: Limb: %s, X_init: %s, Y_init: %s, Z_init: %s ...\n" % (pap.limb, pap.initial_pos.X_left, pap.initial_pos.Y_left, pap.initial_pos.Z_left)) 
			print ("Performing Event!")

			de = DoneEvent()
			if (pap.mode != Reference_sys.ABSOLUTE and pap.mode != Reference_sys.RELATIVE):
				de.error = True
				de.message = ("Error in PickAndPlaceEvent. Unknown mode: %s" % (pap.mode))
			else:
				# perform action (DUMMY)
				time.sleep(3)
				
			# Send DoneEvent to client that sends the event
			client.send(evt.getSource(), "DoneEvent", de) 

		elif (evt.is_a("GripperEvent")):
			# Get values of received event
			ge = GripperEvent()
			evt.parseData(ge)

			# DEBUG output
			print ("GripperEvent")
			print ("Message: Limb: %s, action: %s\n" % (ge.limb, ge.action)) 
			print ("Performing Event!")

			de = DoneEvent()
			if (ge.action != Gripper_state.OPEN and ge.action != Gripper_state.CLOSE):
				de.error = True
				de.message = ("Error in GripperEvent. Unknown action: %s" % (ge.action))
			else:
				# perform action (DUMMY)
				time.sleep(1)

			# Send DoneEvent to client that sends the event
			client.send(evt.getSource(), "DoneEvent", de) 


		elif (evt.is_a("RetrievePoseEvent")):
			# DEBUG output
			print ("RetrievePoseEvent")
			print ("Message: Client asced for position and orientation") 
			print ("Performing Event!")

			# perform action (DUMMY)
			# Create RetrievePoseEvent (Dummy values)
			rpe = RetrievePoseEvent()
			rpe.pos = Position()
			rpe.ori = Orientation()
			rpe.pos.X_left = "1.1"
			rpe.pos.Y_left = "1.2"
			rpe.pos.Z_left = "1.3"
			rpe.pos.X_right = "2.1"
			rpe.pos.Y_right = "2.2"
			rpe.pos.Z_right = "2.3"
			rpe.ori.Yaw_left = "0"
			rpe.ori.Pitch_left = "1.57"
			rpe.ori.Roll_left = "3.14"
			rpe.ori.Yaw_right = "0"
			rpe.ori.Pitch_right = "1.57"
			rpe.ori.Roll_right = "3.14"

			# Send RetrievePoseEvent to client that asced for the information
			client.send(evt.getSource(), "RetrievePoseEvent", rpe)

		elif (evt.is_a("RetrieveAnglesEvent")):
			# DEBUG output
			print ("RetrieveAnglesEvent")
			print ("Message: Client asced for position and orientation") 
			print ("Performing Event!")

			# perform action (DUMMY)
			# Create RetrievePoseEvent (Dummy values)
			rae = RetrieveAnglesEvent()
			rae.angles = Angles()
			rae.angles.left_s0 = "1"
			rae.angles.left_s1 = "1"
			rae.angles.left_e0 = "1"
			rae.angles.left_e1 = "1"
			rae.angles.left_w0 = "1"
			rae.angles.left_w1 = "1"
			rae.angles.left_w2 = "1"
			rae.angles.right_s0 = "1"
			rae.angles.right_s1 = "1"
			rae.angles.right_e0 = "1"
			rae.angles.right_e1 = "1"
			rae.angles.right_w0 = "1"
			rae.angles.right_w1 = "1"
			rae.angles.right_w2 = "1"

			# Send RetrievePoseEvent to client that asced for the information
			client.send(evt.getSource(), "RetrieveAnglesEvent", rae)
		
		elif (evt.is_a("PingEvent") or evt.is_a("PongEvent")):
			continue

		else:
			print ("!!!\tEvent not supported!" % (evt))

		# DEBUG output
		print ("Done!")
		print ("\nWaiting for Events\n")


# disconnect client and terminate
client.disconnect()

# DEBUG output	
print ("Successfully shutdown and stopped")


