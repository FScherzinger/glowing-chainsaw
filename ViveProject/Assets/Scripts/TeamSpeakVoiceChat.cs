using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;
using anyID = System.UInt16;
using uint64 = System.UInt64;
using IntPtr = System.IntPtr;

public class TeamSpeakVoiceChat : MonoBehaviour {

    // connection parameter
    public string serverAddress = "127.0.0.1";
    public int serverPort = 9987;
    public string serverPassword = "secret";
    public string nickName = "client";
    private string[] defaultChannel = new string[] { "Channel_1", "" };
    public string defaultChannelPassword = null;

    private static TeamSpeakClient ts3_client;

    public static bool didNotFindServer = false;

    private static List<int> onTalkStatusChange_status = new List<int>();
    private static List<string> onTalkStatusChange_ClientName = new List<string>();
    private static string onTalkStatusChange_labelText = "";
    private bool con = false;

    void Start()
    {
        //Getting the client		
        ts3_client = TeamSpeakClient.GetInstance();
        
        //enabling logging of some pre defined errors.
        TeamSpeakClient.logErrors = true;
    }

    //Starting the Client
    public void Connect()
    {
        ts3_client.StartClient(serverAddress, (uint)serverPort, serverPassword, nickName, ref defaultChannel, defaultChannelPassword);
        // UI Elements
        if (TeamSpeakClient.started == true)
        {
            Debug.Log("Teamspeak started...");
        }
    }

    // Disconnect the Client
    public void Disconnect()
    {
        var _leaveMessage = "Bye bye";
        ts3_client.StopConnection(_leaveMessage);
        // UI Elements
        if (TeamSpeakClient.started == false)
        {
            Debug.Log("Teamspeak disconnected ...");
        }
    }

    public void Connect_Disconnect()
    {
        con = !con;
        if (con)
        {
            Connect();
        }
        else
        {
            Disconnect();
        }
    }

    void OnApplicationQuit()
    {
        if(con)
            Disconnect();
    }
}
