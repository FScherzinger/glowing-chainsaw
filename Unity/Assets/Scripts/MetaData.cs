using UnityEngine;
using System.Collections;
using de.dfki.events;

public class MetaData : MonoBehaviour {

	public int id {get; set;}
    public ObjType ObjType;

	public MetaData(int id){
		this.id = id;
	}

}
