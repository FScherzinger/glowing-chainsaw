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
using VRStandardAssets.Utils;
using UnityEngine.UI;

public class ReceivedObject : MonoBehaviour {


    void Start()
	{
	}


	void Update(){
	}

	public void updatePosition(PositionEvent pe ){
		Vector3 newpos = new Vector3((float)pe.Position.X,(float)pe.Position.Y,(float)pe.Position.Z);
		if ( (transform.position - newpos).sqrMagnitude > 0.00025)
			transform.position = newpos;
	}
		
	public void updateDirection(DirectionEvent de){
		Quaternion newrot = new Quaternion((float)de.Direction.X, (float)de.Direction.Y, (float)de.Direction.Z, (float)de.Direction.W);
		transform.rotation = newrot;
	}

	public void updateAnnotation(Annotation an){
		Renderer renderer = gameObject.GetComponent<Renderer>();
		Material newMat = renderer.material;
		switch(an.Device){
		case Device.GEARVR:
			newMat = Resources.Load("GearAnno", typeof(Material)) as Material;
			break;
		case Device.TANGO:
			newMat = Resources.Load("TangoAnno", typeof(Material)) as Material;
			break;
		case Device.VIVE:
			newMat = Resources.Load("ViveAnno", typeof(Material)) as Material;
			break;
		case Device.PC:
			newMat = Resources.Load("PCAnno", typeof(Material)) as Material;
			break;
		}
		renderer.material = newMat;
	}
		
}
  
