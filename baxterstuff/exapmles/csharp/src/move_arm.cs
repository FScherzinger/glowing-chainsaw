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


class SenderExample {
   
   static int Main(string[] args)
   {
      // connect to server
      // PSFactory.CreateURI([ClientName], [ServerAddress], [ServerPort]);
      Uri uri = PSFactory.CreateURI("csharp-client-ma", "localhost", 9000);
      PSClient client = PSFactory.CreatePSClient(uri);
      // subscribe before connecting
      client.Subscribe("DoneEvent");
      client.Subscribe("MoveArmEvent");
      // connect
      client.Connect();

      // create event
      MoveArmEvent move = new MoveArmEvent();
      move.Limb = Limb.LEFT; 

      // define position from where to pick the object
      move.Pos = new Position();
      move.Pos.X_left = "1";
      move.Pos.Y_left = "2";
      move.Pos.Z_left = "3";

      // define the orientation how to pick the object
      move.Ori = new Orientation();
      move.Ori.Yaw_left = "1";
      move.Ori.Pitch_left = "1";
      move.Ori.Roll_left = "1";

      move.Speed = new Speed();
      move.Speed.Speed_left = "0.4"; // possible values x: 0.0 <= x <= 1.0

      move.Angls = new Angles();
      move.Mode = Reference_sys.ABSOLUTE;
      move.Kin = Kinematics.INVERSE;
      
      bool active = true;

      // DEBUG output
      Console.WriteLine("Sending MoveArmEvent\n");

      // Send event to client "receiver"
      // client.Send([ClientAddress], [Event = Struct Name], [Struct])
      client.Send("baxter_dummy", "MoveArmEvent", move);

      // check if client is still connected to server
      while (client.IsConnected() && active){
         // check if there is something to receive
      	while (client.CanRecv()){
            // get received event
      		Event evt = client.Recv();
            // DEBUG output
      		Console.WriteLine("Received {0} from {1}\n", evt.Etype, evt.Source);
      		
            // check which event was received
      		if (evt.Is("DoneEvent")){
               // Get values of received event
      			DoneEvent de = new DoneEvent();
      			evt.ParseData(de);

               // DEBUG output
               if (de.Error)
                  Console.WriteLine("ERROR: {0}\n", de.Message); 
               else
      			   Console.WriteLine("Message: {0}\n", de.Message); 
      			

               active = false;

      			break;
      		} 

      	}

      }

      // disconnect client and terminate
      client.Disconnect();
      
      // DEBUG output
      Console.WriteLine("Successfully disconnected and shutdown\n");
      return 0; 
   }

}
