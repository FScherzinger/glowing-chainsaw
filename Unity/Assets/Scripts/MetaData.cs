using UnityEngine;
using System.Collections;
using de.dfki.events;

public class MetaData : MonoBehaviour {

	public int id {get; set;}
    public ObjType ObjType { get; set; }

	public MetaData(int id, ObjType objType){
		this.id = id;
        this.ObjType = objType;
	}

}
