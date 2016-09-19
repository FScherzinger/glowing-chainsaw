using UnityEngine;
using System.Collections;
using de.dfki.events;

public class ButtonHandler : MonoBehaviour
{

    public enum SelectedButton
    {
        pickAndPlace,
        inspect,
        point,
        annotate,
        rotate,
        none
    };

    private bool cubeSelected;

    //GameObject for pick and place
    private GameObject go;
    private GameObject goCopy;
    private int id = 0;

    //position Cube should be moved to
    private Vector3 moveto;

    RaycastHit hit;

    SelectedButton currentButton = SelectedButton.none;

    // Use this for initialization
    void Start()
    {
    }

    public void setCurrent(SelectedButton current)
    {
        currentButton = current;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentButton)
        {
            case SelectedButton.annotate:
                Debug.Log("annotating mode");
                break;
            case SelectedButton.inspect:
                Debug.Log("inpecting mode");
                break;
            case SelectedButton.pickAndPlace:
                Debug.Log("picking mode");
                pickAndPlace();
                break;
            case SelectedButton.point:
                Debug.Log("pointing mode");
                break;
            case SelectedButton.rotate:
                Debug.Log("rotating mode");
                rotate();
                break;
        }
    }

    private void pickAndPlace()
    {
        if (cubeSelected)
        {
            moveto = hit.point;
            Debug.Log("Move" + goCopy.transform.position + "to" + go);
            PositionEvent posEvent = new PositionEvent(Device.TANGO, ObjType.CUBE, new Position(moveto.x, moveto.y, moveto.z), id);
            if (!RPCClient.client.Move(posEvent))
                Debug.Log("Could not move cube");
            Destroy(goCopy);
            goCopy = null;
            cubeSelected = false;
        }
        else
        {
            select();
        }
    }

    private void rotate()
    {

    }

    private void select()
    {
        if (Input.GetMouseButtonDown(0))//TODO:replace mouse input action by Tango click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject != null
                        && hit.collider.gameObject.GetComponent<MetaData>() != null
                        && hit.collider.gameObject.GetType() != typeof(MeshCollider)
                        && hit.collider.gameObject.GetComponent<MetaData>().ObjType == ObjType.CUBE)
                    {
                        go = hit.collider.gameObject;
                        id = go.GetComponent<MetaData>().id;
                        if (RPCClient.client.Can_Interact(id))
                        {
                            RPCClient.client.LockGameObject(id);
                            goCopy = Instantiate(go);
                            goCopy.SetActive(true);
                            Debug.Log("GameObject at " + goCopy.transform.position + " selected");
                            cubeSelected = true;
                        }
                    }
                    else
                    {
                        Debug.Log("No valid object or no metadata attached to cube");
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
