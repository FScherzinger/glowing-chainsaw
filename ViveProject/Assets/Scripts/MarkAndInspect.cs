using UnityEngine;
using System.Collections;
using VRTK;
using de.dfki.events;

public class MarkAndInspect : VRTK_InteractableObject
{
    
    public bool mark=false;
    public bool ismarked = false;
    public bool inspect = false;
    private GameObject infoObj;
    public GameObject emptyCube;
    private GameObject eCube;

    public override void Grabbed(GameObject currentGrabbingObject)
    {
        int id = this.gameObject.GetComponent<MetaData>().id;
        if (RPCClient.client.Can_Interact(id))
        {
            RPCClient.client.LockGameObject(id);
            eCube = Instantiate(emptyCube);
            eCube.transform.position = transform.position;
            gameObject.GetComponent<ReceivedObject>().noUpdate = true;
        }
        base.Grabbed(currentGrabbingObject);
    }

    public override void Ungrabbed(GameObject previousGrabbingObject)
    {
        int id = this.gameObject.GetComponent<MetaData>().id;
        Vector3 pos = transform.position;
        PositionEvent posEvent = new PositionEvent(Device.VIVE, ObjType.CUBE, new Position(pos.x, pos.y, pos.z), id);
        if (!RPCClient.client.Move(posEvent))
            Debug.Log("Could not move cube");
        Destroy(eCube);
        gameObject.GetComponent<ReceivedObject>().noUpdate = false;
        base.Ungrabbed(previousGrabbingObject);
    }


    public override void StartUsing(GameObject usingObject)
    {
        base.StartUsing(usingObject);
        if (mark)
        {
            ismarked = !ismarked;
        }else if (inspect)
        {
            infoObj.GetComponent<TextMesh>().text = ismarked.ToString();
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
