using UnityEngine;
using System;

using de.dfki.events;


public class ReceivedObject : MonoBehaviour {
    public bool noUpdate=false;

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
		
}
  
