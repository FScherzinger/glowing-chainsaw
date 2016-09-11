using UnityEngine;
using System.Collections;
using VRTK;

public class MarkAndInspect : VRTK_InteractableObject
{
    
    public bool mark=false;
    public bool ismarked = false;
    public bool inspect = false;
    private GameObject infoObj;

    public override void StartUsing(GameObject usingObject)
    {
        base.StartUsing(usingObject);
        if (mark)
        {
            ismarked = !ismarked;
        }else if (inspect)
        {
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
