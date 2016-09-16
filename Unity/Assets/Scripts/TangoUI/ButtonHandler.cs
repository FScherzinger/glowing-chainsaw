using UnityEngine;
using System.Collections;
using de.dfki.events;

public class ButtonHandler : MonoBehaviour {

    private int state;
    private GameObject movefrom;
    private Vector3 moveto;

    public enum SelectedButton
    {
        pickAndPlace,
        inspect,
        point,
        annotate,
        rotate,
        none
    };

    SelectedButton currentButton=SelectedButton.none;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        switch (currentButton) {
            case SelectedButton.annotate:
                Debug.Log("annotating mode");
                break;
            case SelectedButton.inspect:
                Debug.Log("inpecting mode");
                break;
            case SelectedButton.pickAndPlace:
                PickPlace();
                break; 
            case SelectedButton.point:
                Debug.Log("pointing mode");
                break;
            case SelectedButton.rotate:
                Debug.Log("rotating mode");
                break;
            }
	}

    public void setCurrent(SelectedButton current)
    {
        currentButton = current;
        state = 0;
    }

    private void PickPlace()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {
                    if (state == 0)
                    {
                        if (hit.collider.GetType() == typeof(MeshCollider))
                        {
                            Debug.Log("No valid object");
                        }
                        else
                        {
                            movefrom = hit.collider.gameObject;
                            Debug.Log("GameObject at " + movefrom.transform.position + " selected");
                            state++;
                        }
                    }
                    else
                   {
                        moveto = hit.point;
                        Debug.Log("Move" + movefrom.transform.position + "to" + moveto);
                        int id=movefrom.gameObject.GetComponent<MetaData>().id;
                        PositionEvent posEvent = new PositionEvent(Device.TANGO, ObjType.CUBE, new Position(moveto.x, moveto.y, moveto.z), id);
                        state = 0;
                    }
                }
            }
            else
            {
                Debug.Log("Not clicked in valid area");
            }
        }
    }
}
