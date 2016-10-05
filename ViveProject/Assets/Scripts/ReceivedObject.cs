using UnityEngine;
using System;

using de.dfki.events;


public class ReceivedObject : MonoBehaviour {
    public bool noUpdate=false;
    public Device dev=Device.VUFORIA;

    void Start()
	{
	}


	void Update(){
	}

	public void updatePosition(PositionEvent pe ){
        if (!noUpdate)
        {
            Vector3 newpos = new Vector3((float)pe.Position.X, (float)pe.Position.Y, (float)pe.Position.Z);
            if ((transform.position - newpos).sqrMagnitude > 0.00025)
                transform.position = newpos;
        }

	}
		
	public void updateDirection(DirectionEvent de){
        if(!noUpdate)
        {
            Quaternion newrot = new Quaternion((float)de.Direction.X, (float)de.Direction.Y, (float)de.Direction.Z, (float)de.Direction.W);
            //if ( Quaternion.Angle(transform.rotation,newrot) > 1)
            transform.rotation = newrot;
        }
	}
    public void updateAnnotation(Annotation an)
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        Material newMat = renderer.material;
        Color color = Color.green;
        dev = an.Device;
        switch (an.Device)
        {
            case Device.GEARVR:
                color = Color.blue;
                //newMat = Resources.Load("GearAnno", typeof(Material)) as Material;
                break;
            case Device.TANGO:
                color = Color.cyan;
                //newMat = Resources.Load("TangoAnno", typeof(Material)) as Material;
                break;
            case Device.VIVE:
                color = Color.grey;
                //newMat = Resources.Load("ViveAnno", typeof(Material)) as Material;
                break;
            case Device.PC:
                color = Color.white;
                //newMat = Resources.Load("PCAnno", typeof(Material)) as Material;
                break;
        }
        renderer.material.color = color;
    }

}
  
