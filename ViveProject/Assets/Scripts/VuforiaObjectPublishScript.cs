using UnityEngine;
using System.Collections;

public class VuforiaObjectPublishScript : MonoBehaviour {

    private SceneHandler s_handler;
    public GameObject server;
    private bool active;

    // Use this for initialization
    void Start()
    {
        s_handler = server.GetComponent(typeof(SceneHandler)) as SceneHandler;
        active = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.GetComponent<MeshRenderer>().enabled && !active)
        {
            active = true;
            s_handler.addToSceneObjects(gameObject);
        }
    }
}
