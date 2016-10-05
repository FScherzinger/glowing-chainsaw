using UnityEngine;
using System.Collections;
using VRTK;
using de.dfki.events;
using System.Threading;

public class MarkAndInspect : VRTK_InteractableObject
{
    
    public bool mark=false;
    public bool ismarked = false;
    public bool inspect = false;
    private float touchAngle;
    private bool turnable;
    private bool turned;
    private volatile bool ready;
    private volatile bool locked;
    private volatile bool otherlock;

    private GameObject grabcontroller;
    private GameObject infoObj;
    public GameObject emptyCube;
    private GameObject eCube;

    public override void Grabbed(GameObject currentGrabbingObject)
    {
        int id = this.gameObject.GetComponent<MetaData>().id;
        base.Grabbed(currentGrabbingObject);
        grabcontroller = base.GetGrabbingObject();
        grabcontroller.transform.Find("RadialMenu").gameObject.SetActive(false);
        turnable = true;
        eCube = Instantiate(emptyCube);
        eCube.transform.position = transform.position;
        gameObject.GetComponent<ReceivedObject>().noUpdate = true;
        StartCoroutine(blockObject(id));
        Debug.Log("works");
    }


    private void InteractLog(int id)
    {
        if (RPCClient.client.Can_Interact(id))
        {
            RPCClient.client.LockGameObject(id);
            ready = true;
            locked = true;
        }
        else
        {
            ready = true;
            otherlock = true;
        }
    }

    IEnumerator blockObject(int id)
    {
        Debug.Log("blocking...");
        ready = false;
        Thread thread = new Thread(() => InteractLog(id));
        thread.Start(id);
        while (!ready)
        {
            yield return null;
        }
        Debug.Log("Blocked");
        Debug.Log(base.GetGrabbingObject());

        yield return null;
    }

    private void MoveRotThread(PositionEvent posEvent, DirectionEvent dirEvent)
    {
        RPCClient.client.Move_And_Rotate(posEvent, dirEvent);
    }

    IEnumerator moveRotObject(PositionEvent posEvent, DirectionEvent dirEvent,GameObject previousGrabbingObject)
    {
        Thread thread = new Thread(() => MoveRotThread(posEvent,dirEvent));
        thread.Start();
        yield return null;
    }

    private void MoveThread(PositionEvent posEvent)
    {
        RPCClient.client.Move(posEvent) ;
    }

    IEnumerator moveObject(PositionEvent posEvent,GameObject previousGrabbingObject)
    {
        Thread thread = new Thread(() => MoveThread(posEvent));
        thread.Start();
        yield return null;
    }

    IEnumerator SendMove(Vector3 pos, Quaternion rot,int id, GameObject previousGrabbingObject)
    {
        PositionEvent posEvent = new PositionEvent(Device.VIVE, ObjType.CUBE, new Position(pos.x, pos.y, pos.z), id);
        Debug.Log("moving...");
        while(!locked && !otherlock)
        {
            yield return null;
        }
        Debug.Log("ready...");
        if (locked)
        {
            if (turned)
            {
                DirectionEvent dirEvent = new DirectionEvent(Device.VIVE, ObjType.CUBE, new Direction(rot.x, rot.y, rot.z, rot.w), id);
                StartCoroutine(moveRotObject(posEvent, dirEvent, previousGrabbingObject));
                turned = false;
            }
            else
            {
                StartCoroutine(moveObject(posEvent, previousGrabbingObject));
            }
            locked = false;
            Debug.Log("success");
        }
        else
        {
            if (otherlock)
            {
                otherlock = false;
                Debug.Log("GameObject locked otherwise");
            }
            Debug.Log("fail");
        }
        Destroy(eCube);
        gameObject.GetComponent<ReceivedObject>().noUpdate = false;
        base.Ungrabbed(previousGrabbingObject);
        yield return null;
    }

    public override void Ungrabbed(GameObject previousGrabbingObject)
    {

        int id = this.gameObject.GetComponent<MetaData>().id;
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;
        grabcontroller.transform.Find("RadialMenu").gameObject.SetActive(true);
        turnable = false;
        StartCoroutine(SendMove(pos,rot,id, previousGrabbingObject));
    }


    public override void StartUsing(GameObject usingObject)
    {
        int id = this.gameObject.GetComponent<MetaData>().id;
        base.StartUsing(usingObject);
        if (mark)
        {
            Renderer rend = gameObject.GetComponent<Renderer>();
            rend.material.color = Color.blue;
            rend.material.SetColor("_SpecColor", Color.blue);
            Annotation an = new Annotation(Device.VIVE, id);
            if (!RPCClient.client.Annotate(an))
            {
                Debug.Log("Annotation Failed");
            }
        }
        else if (inspect)
        {
            Device tmp_an_dev = gameObject.GetComponent<ReceivedObject>().dev;
            string msg = gameObject.name + "\n" + "at " + gameObject.transform.position + "\n";
            if (tmp_an_dev != Device.VUFORIA)
            {
                msg +="Annotaded by "+ tmp_an_dev;
            }
            else
            {
                msg += "not Annotated";
            }
            infoObj.GetComponent<TextMesh>().text = msg;
            infoObj.gameObject.SetActive(true);
            
        }
    }

    public override void StopUsing(GameObject usingObject)
    {
        base.StopUsing(usingObject);
        infoObj.SetActive(false);
    }
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        infoObj = transform.Find("Information").gameObject;

    }

    // Update is called once per frame
    protected override void Update() {
        if (turnable&& turned)
        {
            float angle = touchAngle - 90;
            transform.eulerAngles= new Vector3(0f, angle, 0f);
        }
    }

    public void DoTouchpadAxisChanged(float angle)
    {
        if (turnable)
        {
            touchAngle = angle;
            turned = true;
        }
    }

public void Inspect(bool ins)
    {
        inspect = ins;
    }

    public void Mark(bool m)
    {
        mark = m;
    }
}
