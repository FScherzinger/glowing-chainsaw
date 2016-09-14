using UnityEngine;
using System.Collections;
using de.dfki.events;

public class PickPlaceButton : MonoBehaviour {

	public void PickandPlace(GameObject go)
    {
        Vector3 position = transform.position;
        PositionEvent e = new PositionEvent(Device.TANGO, ObjType.CUBE, new Position(position.x, position.y, position.z), go.GetComponent<MetaData>().id);
        RPCClient.client.Move(e);
    }
}
