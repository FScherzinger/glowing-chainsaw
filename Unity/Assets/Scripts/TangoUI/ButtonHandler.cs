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

    private enum SwipeDirection
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        NONE
    }

    private bool cubeSelected;

    //GameObject for pick and place
    private GameObject go;
    private GameObject movingCube;
    [SerializeField] private GameObject movingCubeModel;
    private int id = 0;

    //position Cube should be moved to
    private Vector3 moveto;

    //fields needed to detect Swipe Direction
    private bool swiping = false;
    private bool eventSent = false;
    private Vector2 lastPosition;

    RaycastHit hit;

    SelectedButton currentButton = SelectedButton.none;
    SwipeDirection swipeDirection = SwipeDirection.NONE;

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
                select();
                rotate();
                break;
        }
    }

    private void pickAndPlace()
    {
        if (cubeSelected)
        {
            moveto = hit.point;
            Debug.Log("Move" + movingCube.transform.position + "to" + go);
            PositionEvent posEvent = new PositionEvent(Device.TANGO, ObjType.CUBE, new Position(moveto.x, moveto.y, moveto.z), id);
            if (!RPCClient.client.Move(posEvent))
                Debug.Log("Could not move cube");
            Destroy(movingCube);
            movingCube = null;
            cubeSelected = false;
        }
        else
        {
            select();
        }
    }

    private void rotate()
    {
        if (cubeSelected)
        {
            swipe();
            switch (swipeDirection)
            {
                case SwipeDirection.DOWN:
                case SwipeDirection.NONE:
                case SwipeDirection.UP:
                    return;
                case SwipeDirection.LEFT:
                    if (RPCClient.client.Can_Interact(id))
                    {
                        RPCClient.client.LockGameObject(id);
                        movingCube = Instantiate(movingCubeModel);
                        movingCube.SetActive(true);
                        movingCube.transform.position = this.gameObject.transform.position;
                        movingCube.transform.rotation = this.gameObject.transform.rotation;
                        this.gameObject.SetActive(false);
                        movingCube.transform.RotateAround(movingCube.transform.position, Vector3.down, 10);
                    }
                    break;
                case SwipeDirection.RIGHT:
                    if (RPCClient.client.Can_Interact(id))
                    {
                        RPCClient.client.LockGameObject(id);
                        movingCube = Instantiate(movingCubeModel);
                        movingCube.SetActive(true);
                        movingCube.transform.position = this.gameObject.transform.position;
                        movingCube.transform.rotation = this.gameObject.transform.rotation;
                        this.gameObject.SetActive(false);
                        movingCube.transform.RotateAround(movingCube.transform.position, Vector3.up, 10);
                    }
                    break;
            }
            Vector3 pos = this.gameObject.transform.position;
            PositionEvent posEvent = new PositionEvent(Device.GEARVR, ObjType.CUBE, new Position(pos.x, pos.y, pos.z), id);
            Quaternion dir = movingCube.transform.rotation;
            DirectionEvent dirEvent = new DirectionEvent(Device.GEARVR, ObjType.CUBE, new Direction(dir.x, dir.y, dir.z, dir.w), id);
            Destroy(movingCube);
            movingCube = null;
            this.gameObject.SetActive(true);
            if (!RPCClient.client.Move_And_Rotate(posEvent, dirEvent))
                Debug.Log("Could not rotate cube");
        }
    }

    private void select()
    {
        if (Input.GetMouseButtonDown(0))//TODO:replace mouse input action by Tango click (see next line)
            //if (Input.GetTouch(0))
            {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//TODO:Tango click position -> Input.GetTouch(0).position
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
                            movingCube = Instantiate(go);
                            movingCube.SetActive(true);
                            Debug.Log("GameObject at " + movingCube.transform.position + " selected");
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

    void swipe()
    {
        if (Input.touchCount == 0)
            return;

        if (Input.GetTouch(0).deltaPosition.sqrMagnitude != 0)
        {
            if (swiping == false)
            {
                swiping = true;
                lastPosition = Input.GetTouch(0).position;
                return;
            }
            else
            {
                if (!eventSent)
                {
                    if (swipeDirection != SwipeDirection.NONE)
                    {
                        Vector2 direction = Input.GetTouch(0).position - lastPosition;

                        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                        {
                            if (direction.x > 0)
                                swipeDirection = SwipeDirection.RIGHT;
                            else
                                swipeDirection = SwipeDirection.LEFT;
                        }
                        else
                        {
                            if (direction.y > 0)
                                swipeDirection = SwipeDirection.UP;
                            else
                                swipeDirection = SwipeDirection.DOWN;
                        }

                        eventSent = true;
                    }
                }
            }
        }
        else
        {
            swiping = false;
            eventSent = false;
        }
    }
}
