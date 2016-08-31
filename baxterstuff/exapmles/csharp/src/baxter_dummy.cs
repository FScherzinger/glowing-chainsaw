/*
 * The MIT License
 *
 * Copyright 2016 Winfried.Schuffert@DFKI.de.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

 /**
  *
  * @Author: Winfried.Schuffert@DFKI.de
  *
  **/

// necessary libraries
using System;
using System.Collections.Generic;
using de.dfki.tecs.basetypes;
using Thrift.Protocol;
using Thrift.Transport;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;


//libtecs namespace
using de.dfki.tecs.ps;
using de.dfki.tecs;

//Baxter Namespace
using de.dfki.tecs.robot.baxter;


class ReceiverExample {
	
	static int Main(string[] args)
	{
		// connect to server
		// PSFactory.CreateURI([ClientName], [ServerAddress], [ServerPort]);
		Uri uri = PSFactory.CreateURI("baxter_dummy", "localhost", 9000);
		PSClient client = PSFactory.CreatePSClient(uri);
		// subscribe before connecting
		client.Subscribe("DoneEvent");
		client.Subscribe("MoveArmEvent");
		client.Subscribe("PickAndPlaceEvent");
		client.Subscribe("RetrievePoseEvent");
		client.Subscribe("RetrieveAnglesEvent");
		client.Subscribe("GripperEvent");
		
		// connect
		client.Connect();

		// DEBUG output
		Console.WriteLine("Waiting for Events\n");

		// check if client is still connected to server
		while (client.IsConnected()){
			// check if there is something to receive
			while (client.CanRecv()){
				// get received event
				Event evt = client.Recv();
				// DEBUG output
				Console.WriteLine("Received {0} from {1}\n", evt.Etype, evt.Source);
			
				// check which event was received
				if (evt.Is("MoveArmEvent")){
					// Get values of received event
					MoveArmEvent mae = new MoveArmEvent();
					evt.ParseData(mae);

					// DEBUG output
					Console.WriteLine("MoveArmEvent");
					Console.WriteLine("Message: Limb: {0}, X: {1}, Y: {2}, Z: {3} ...\n", mae.Limb, mae.Pos.X_left, mae.Pos.Y_left, mae.Pos.Z_left); 
					Console.WriteLine("Performing Event!");

					// Send DoneEvent to client that sends the event
					DoneEvent de = new DoneEvent();
					if (mae.Mode != Reference_sys.ABSOLUTE && mae.Mode != Reference_sys.RELATIVE){
						de.Error = true;
						de.Message = "Error in MoveArmEvent. Unknown mode: " + mae.Mode;
					}
					else{
						// perform action (DUMMY)
						System.Threading.Thread.Sleep(3000);
					}

					client.Send(evt.Source, "DoneEvent", de);

				}
				else if (evt.Is("PickAndPlaceEvent")){
					// Get values of received event
					PickAndPlaceEvent pap = new PickAndPlaceEvent();
					evt.ParseData(pap);

					// DEBUG output
					Console.WriteLine("PickAndPlaceEvent");
					Console.WriteLine("Message: Limb: {0}, X_init: {1}, Y_init: {2}, Z_init: {3} ...\n", pap.Limb, pap.Initial_pos.X_left, pap.Initial_pos.Y_left, pap.Initial_pos.Z_left); 
					Console.WriteLine("Performing Event!");

					// Send DoneEvent to client that sends the event
					DoneEvent de = new DoneEvent();
					if (pap.Mode != Reference_sys.ABSOLUTE && pap.Mode != Reference_sys.RELATIVE){
						de.Error = true;
						de.Message = "Error in MoveArmEvent. Unknown mode: " + pap.Mode;
					}
					else {
						// perform action (DUMMY)
						System.Threading.Thread.Sleep(3000);
					}

					client.Send(evt.Source, "DoneEvent", de);

				}

				else if (evt.Is("GripperEvent")){
					// Get values of received event
					GripperEvent ge = new GripperEvent();
					evt.ParseData(ge);

					// DEBUG output
					Console.WriteLine("GripperEvent");
					Console.WriteLine("Message: Limb: {0}, action: {1}\n", ge.Limb, ge.Action);
					Console.WriteLine("Performing Event!");

					DoneEvent de = new DoneEvent();
					if (ge.Action != Gripper_state.OPEN && ge.Action != Gripper_state.CLOSE){
						de.Error = true;
						de.Message = ("Error in GripperEvent. Unknown action: " + ge.Action);
					}
					else {
						// perform action (DUMMY)
						System.Threading.Thread.Sleep(1000);
					}

					// Send DoneEvent to client that sends the event
					client.Send(evt.Source, "DoneEvent", de);
				}

				else if (evt.Is("RetrievePoseEvent")){
					// DEBUG output
					Console.WriteLine("PickAndPlaceEvent");
					Console.WriteLine("Message: Client asced for position and orientation"); 
					Console.WriteLine("Performing Event!");

					// perform action (DUMMY)
					// Create RetrievePoseEvent (Dummy values)
					RetrievePoseEvent rpe = new RetrievePoseEvent();
					rpe.Pos = new Position();
					rpe.Ori = new Orientation();
					rpe.Pos.X_left = "1.1";
					rpe.Pos.Y_left = "1.2";
					rpe.Pos.Z_left = "1.3";
					rpe.Pos.X_right = "2.1";
					rpe.Pos.Y_right = "2.2";
					rpe.Pos.Z_right = "2.3";
					rpe.Ori.Yaw_left = "0";
					rpe.Ori.Pitch_left = "1.57";
					rpe.Ori.Roll_left = "3.14";
					rpe.Ori.Yaw_right = "0";
					rpe.Ori.Pitch_right = "1.57";
					rpe.Ori.Roll_right = "3.14";

					// Send RetrievePoseEvent to client that asced for the information
					client.Send(evt.Source, "RetrievePoseEvent", rpe);

				}

				else if (evt.Is("RetrieveAnglesEvent")){
					// DEBUG output
					Console.WriteLine("RetrieveAnglesEvent");
					Console.WriteLine("Message: Client asced for position and orientation"); 
					Console.WriteLine("Performing Event!");

					// perform action (DUMMY)
					// Create RetrieveAnglesEvent (Dummy values)
					RetrieveAnglesEvent rae = new RetrieveAnglesEvent();
					rae.Angles = new Angles();
					rae.Angles.Left_s0 = "1";
					rae.Angles.Left_s1 = "1";
					rae.Angles.Left_e0 = "1";
					rae.Angles.Left_e1 = "1";
					rae.Angles.Left_w0 = "1";
					rae.Angles.Left_w1 = "1";
					rae.Angles.Left_w2 = "1";
					rae.Angles.Right_s0 = "1";
					rae.Angles.Right_s1 = "1";
					rae.Angles.Right_e0 = "1";
					rae.Angles.Right_e1 = "1";
					rae.Angles.Right_w0 = "1";
					rae.Angles.Right_w1 = "1";
					rae.Angles.Right_w2 = "1";

					// Send RetrieveAnglesEvent to client that asced for the information
					client.Send(evt.Source, "RetrieveAnglesEvent", rae);

				}

				else if (evt.Is("PingEvent") || evt.Is("PongEvent")){
					continue;
				}

				else {
					Console.WriteLine("!!!\tEvent not supported!");
				}


				// DEBUG output
				Console.WriteLine("Done");
				Console.WriteLine("Waiting for Events\n");
			}
		}

		// disconnect client and terminate
		client.Disconnect();

		// DEBUG output
		Console.WriteLine("Successfully disconnected and shutdown\n");
		return 0; 
	}

}
