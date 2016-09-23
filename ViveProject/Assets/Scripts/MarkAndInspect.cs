using UnityEngine;
using System.Collections;
using VRTK;
using de.dfki.events;

public class MarkAndInspect : VRTK_InteractableObject
{
    
    public bool mark=false;
    public bool ismarked = false;
    public bool inspect = false;
    private float touchAngle;
    private bool turnable;
    private bool turned;

    private GameObject grabcontroller;
    private GameObject infoObj;
    public GameObject emptyCube;
    private GameObject eCube;

    public override void Grabbed(GameObject currentGrabbingObject)
    {
        int id = this.gameObject.GetComponent<MetaData>().id;
        base.Grabbed(currentGrabbingObject);
        grabcontroller=base.GetGrabbingObject();
        grabcontroller.transform.Find("RadialMenu").gameObject.SetActive(false);
        turnable = true;
        if (RPCClient.client.Can_Interact(id))
        {
            RPCClient.client.LockGameObject(id);
            eCube = Instantiate(emptyCube);
            eCube.transform.position = transform.position;
            gameObject.GetComponent<ReceivedObject>().noUpdate = true;
            Debug.Log(base.GetGrabbingObject());
        }


    }

    public override void Ungrabbed(GameObject previousGrabbingObject)
    {
        int id = this.gameObject.GetComponent<MetaData>().id;
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;
        grabcontroller.transform.Find("RadialMenu").gameObject.SetActive(true);
        turnable = false;
        PositionEvent posEvent = new PositionEvent(Device.VIVE, ObjType.CUBE, new Position(pos.x, pos.y, pos.z), id);
        if (turned)
        {
            DirectionEvent dirEvent = new DirectionEvent(Device.VIVE, ObjType.CUBE, new Direction(rot.x, rot.y, rot.z, rot.w), id);
            if (!RPCClient.client.Move_And_Rotate(posEvent, dirEvent))
                Debug.Log("Could not move cube");
            turned = false;
        }else
        {
            if (!RPCClient.client.Move(posEvent))
                Debug.Log("Could not move cube");
        }

        Destroy(eCube);
        gameObject.GetComponent<ReceivedObject>().noUpdate = false;
        base.Ungrabbed(previousGrabbingObject);
    }


    public override void StartUsing(GameObject usingObject)
    {
        int id = this.gameObject.GetComponent<MetaData>().id;
        base.StartUsing(usingObject);
        if (mark)
        {
            string msg = "";
            System.Random rnd = new System.Random();
            int anno_id = rnd.Next();
            Renderer rend = gameObject.GetComponent<Renderer>();
            rend.material.color = Color.blue;
            rend.material.SetColor("_SpecColor", Color.blue);
            /*Annotation annote = new Annotation(Device.VIVE, id, anno_id, msg);
            if (!RPCClient.client.Annotate(annote))
                Debug.Log("Could not annotate cube");*/
        }
        else if (inspect)
        {

            /*infoObj.GetComponent<TextMesh>().text = msg;
            infoObj.gameObject.SetActive(true);
            */
        }
    }

    public override void StopUsing(GameObject usingObject)
    {
        /*base.StopUsing(usingObject);
        infoObj.SetActive(false);*/
    }
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        //infoObj = transform.Find("Information").gameObject;

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
