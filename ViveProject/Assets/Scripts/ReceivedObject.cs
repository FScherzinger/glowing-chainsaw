using UnityEngine;
using System;
using System.Collections;
using System.Threading;

using de.dfki.tecs;
using de.dfki.tecs.ps;
using de.dfki.tecs.basetypes;
using System.Runtime.InteropServices;

using de.dfki.events;
using System.Collections.Generic;
using UnityEngine.UI;

public class ReceivedObject : MonoBehaviour {
    public bool noUpdate { get; set; }

    void Start()
	{
        noUpdate = false;
	}

	void Update(){
	}

	public void updatePosition(PositionEvent pe ){
			Vector3 newpos = new Vector3((float)pe.Position.X,(float)pe.Position.Y,(float)pe.Position.Z);
			if ( (transform.position - newpos).sqrMagnitude > 0.0025 & !noUpdate)
				transform.position = newpos;
	}
		
	public void updateDirection(DirectionEvent de){
			Quaternion newrot = new Quaternion((float)de.Direction.X, (float)de.Direction.Y, (float)de.Direction.Z, (float)de.Direction.W);
		if ( Quaternion.Angle(transform.rotation,newrot) > 1 & !noUpdate)
				transform.rotation = newrot;
	}
	
}
  
