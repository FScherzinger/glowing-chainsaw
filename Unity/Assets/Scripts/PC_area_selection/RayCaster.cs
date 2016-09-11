using UnityEngine;
using System.Collections;

public class RayCaster : MonoBehaviour {

    public GameObject plane;
    private bool clicked = false;

    private Vector3 position_1;
    private Vector3 position_2;

	// Use this for initialization
	void Start()
    {

    }
	
	// Update is called once per frame
	void Update()
    {
        if( Input.GetMouseButtonDown(0) && !clicked )
        {
            clicked = true;
            position_1 = CastRay();
            plane.transform.position = position_1;
        }
        else if( Input.GetMouseButtonDown(0) )
        {
            clicked = false;
        }

        if( !clicked )
            return;

        position_2 = CastRay();

        Vector3 scale = position_2 - position_1;

        plane.transform.localScale = scale;
        
        

    }

    private Vector3 CastRay()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            return new Vector3(hit.point.x, 1f, hit.point.z);
        }
        // really bad. but works for the moment
        return new Vector3(1, 10000, 1);
    }
}
