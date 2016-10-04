using UnityEngine;
using System.Collections;
using de.dfki.events;

public class ButtonHandler : MonoBehaviour
{

    public enum SelectedButton
    {
        pickAndPlace,
        //inspect,
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
    private bool annotated = false;

    //GameObject for pick and place
    private GameObject go;
    private GameObject movingCube;
    [SerializeField]
    private GameObject movingCubeModel;
    private int id = 0;

    //position Cube should be moved to
    private Vector3 moveto;

    //fields needed to detect Swipe Direction
    private bool swiping = false;
    private bool eventSent = false;
    private Vector2 lastPosition;
    private bool tap = false;

    SelectedButton currentButton = SelectedButton.none;
    SwipeDirection swipeDirection = SwipeDirection.NONE;

    [SerializeField] Material normalMaterial;

    // Use this for initialization
    void Start()
    {
    }

    public void setCurrent(SelectedButton current)
    {
        currentButton = current;
        cubeSelected = false;
        if (go != null)
        {
            if (!annotated)
                go.gameObject.GetComponent<Renderer>().material = normalMaterial;
            else
                go.gameObject.GetComponent<Renderer>().material.color = new Color32(0xB3, 0x00, 0x98, 0xFF);
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentButton)
        {
            case SelectedButton.annotate:
                annotate();
                break;
            /*case SelectedButton.inspect:
                select();
                inspect();
                break;*/
            case SelectedButton.pickAndPlace:
                pickAndPlace();
                break;
            case SelectedButton.rotate:
                rotate();
                break;
        }
    }

    private void pickAndPlace()
    {
        if (cubeSelected)
        {
            if (Input.GetMouseButtonDown(0))
            //if (Input.touchCount==1)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider != null)
                    {
                        moveto = hit.point;
                        Position pos = new Position(moveto.x, moveto.y + 0.05, moveto.z);
                        Debug.Log("Move " + go + " to " + moveto);
                        PositionEvent posEvent = new PositionEvent(Device.TANGO, ObjType.CUBE, pos, id);
                        if (!RPCClient.client.Move(posEvent))
                            Debug.Log("Could not move cube");
                        cubeSelected = false;
                        if (!annotated)
                            go.gameObject.GetComponent<Renderer>().material.color = new Color32(0x00, 0x92, 0x0D, 0xFF);
                        else
                            go.gameObject.GetComponent<Renderer>().material.color = new Color32(0xB3, 0x00, 0x98, 0xFF);
                    }
                }
                else
                {
                    Debug.Log("Not clicked in valid area");
                }
            }
        }
        else
        {
            select();
        }
    }

    private void rotate()
    {
        if (!cubeSelected)
        {
            if (select())
            {
                //movingCube = Instantiate(movingCubeModel);
                //movingCube.transform.position = this.gameObject.transform.position;
                //movingCube.transform.rotation = this.gameObject.transform.rotation;
                //this.gameObject.SetActive(false);
            }
        }
        else
        {
            swipe();
            if (!tap)
            {
                switch (swipeDirection)
                {
                    case SwipeDirection.NONE:
                        return;
                    case SwipeDirection.LEFT:
                        go.transform.RotateAround(go.transform.position, Vector3.down, 10);
                        swipeDirection = SwipeDirection.NONE;
                        break;
                    case SwipeDirection.RIGHT:
                        go.transform.RotateAround(go.transform.position, Vector3.up, 10);
                        swipeDirection = SwipeDirection.NONE;
                        break;
                }
            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //GetTouch(0).position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider != null)
                    {
                        moveto = hit.point;
                        Position pos = new Position(moveto.x, moveto.y + 0.05, moveto.z);
                        Debug.Log("Move " + go + " to " + moveto);
                        PositionEvent posEvent = new PositionEvent(Device.TANGO, ObjType.CUBE, pos, id);
                        cubeSelected = false;
                        if (!annotated)
                            go.gameObject.GetComponent<Renderer>().material.color = new Color32(0x00, 0x92, 0x0D, 0xFF);
                        else
                            go.gameObject.GetComponent<Renderer>().material.color = new Color32(0xB3, 0x00, 0x98, 0xFF); Quaternion dir = go.transform.rotation;
                        DirectionEvent dirEvent = new DirectionEvent(Device.GEARVR, ObjType.CUBE, new Direction(dir.x, dir.y, dir.z, dir.w), id);
                        //Destroy(movingCube);
                        //movingCube = null;
                        //this.gameObject.SetActive(true);
                         if (!RPCClient.client.Move_And_Rotate(posEvent, dirEvent))
                             Debug.Log("Could not rotate cube");
                        tap = false;
                    }
                }
                else
                {
                    Debug.Log("No valid object or no metadata attached to cube");
                }
            }
        }
    }

    private bool select()
    {
        if (Input.GetMouseButtonDown(0))
        //if (Input.touchCount==1)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;
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
                        go.gameObject.GetComponent<Renderer>().material.color = new Color32(0x00, 0xFF, 0x16, 0xFF);
                        id = go.GetComponent<MetaData>().id;
                        if (RPCClient.client.Can_Interact(id))
                        {
                            RPCClient.client.LockGameObject(id);
                        Debug.Log("GameObject at " + go.transform.position + " selected");
                        cubeSelected = true;
                        return true;
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
        return false;
    }

    void swipe()
    {
        if (Input.touchCount == 0)
            return;

        if (Input.GetTouch(0).deltaPosition.sqrMagnitude != 0)
        {
            if (!swiping)
            {
                swiping = true;
                lastPosition = Input.GetTouch(0).position;
                return;
            }
            else
            {
                if (swipeDirection != SwipeDirection.NONE)
                {
                    Vector2 direction = Input.GetTouch(0).position - lastPosition;

                    if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                    {
                        if (direction.x > 0)
                        {
                            swipeDirection = SwipeDirection.RIGHT;
                            Debug.Log("swipe right");
                        }
                        else
                        {
                            swipeDirection = SwipeDirection.LEFT;
                            Debug.Log("swipe left");
                        }
                    }
                    else
                    {
                        if (direction.y > 0)
                            swipeDirection = SwipeDirection.UP;
                        else
                            swipeDirection = SwipeDirection.DOWN;
                    }
                }
            }
        }
        else
        {
                swiping = false;
                tap = true;
        }
    }

    /*private void inspect()
    {
        if (cubeSelected)
        {
            Information inf = RPCClient.client.GetInformationById(id);
            string msg = inf.Informtion;

            infoObj.GetComponent<TextMesh>().text = msg;
            infoObj.gameObject.SetActive(true);
        }
    }*/

    private void annotate()
    {
        if (select())
        {
            Annotation an = new Annotation(Device.PC, id);
            if (!RPCClient.client.Annotate(an))
            {
                Debug.Log("Annotation Failed");
            }
        }
    }

    void swipeTestPC()
    {
        if (Input.GetKeyDown("1"))
        {
            swipeDirection = SwipeDirection.RIGHT;
            Debug.Log("swipe right");
        }
        else
        {
            if (Input.GetKeyDown("2"))
            {
                swipeDirection = SwipeDirection.LEFT;
                Debug.Log("swipe left");
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            swiping = false;
            tap = true;
            Debug.Log("Place Cube");
        }
    }
}
