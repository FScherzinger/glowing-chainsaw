using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;

using Vuforia;

public class PublishObjPosRot : MonoBehaviour {

    PublishPosRot.Publisher publisher;
    Thread publishThread;
    public de.dfki.events.Device device;
    public String serverAddr = "localhost";
    public int serverPort = 9000;
    public int id { get; set; }

    void Start()
    {
        System.Random rnd = new System.Random();
        id = rnd.Next();
        publisher = new PublishPosRot.Publisher{
            device = this.device,
            serverAddr = this.serverAddr,
            serverPort = this.serverPort,
            id = this.id
        };
        publishThread = new Thread(publisher.Connect);
        publishThread.Start();
    }


    void FixedUpdate()
    {
        StateManager sm = TrackerManager.Instance.GetStateManager();

        IEnumerable<TrackableBehaviour> activeTrackables = sm.GetActiveTrackableBehaviours();

        foreach (TrackableBehaviour tb in activeTrackables)
        {
            if (tb.gameObject == gameObject)
            {
                publisher.SendPosition(gameObject);
                publisher.SendRotation(gameObject);
            }
        }
    }


    void OnApplicationQuit()
    {
        publisher.close();
        publishThread.Abort();
    }
}
