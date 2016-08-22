//======= Copyright (c) Valve Corporation, All rights reserved. ===============
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class FloorColorChanger : MonoBehaviour
{

    SteamVR_TrackedObject trackedObj;
    FixedJoint joint;

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    void FixedUpdate()
    {
        var go = GameObject.Find("Plane");
        Debug.Log("Test");
        var device = SteamVR_Controller.Input((int)trackedObj.index);
        if (joint == null && device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("tets");
            Renderer rend = go.GetComponent<Renderer>();
            rend.material.shader = Shader.Find("Specular");
            rend.material.SetColor("_SpecColor", Color.red);
        }
        else if (joint != null && device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger)) {

            Renderer rend = go.GetComponent<Renderer>();
            rend.material.shader = Shader.Find("Specular");
            rend.material.SetColor("_SpecColor", Color.blue);

        }
    }
}
