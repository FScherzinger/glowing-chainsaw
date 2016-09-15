using UnityEngine;
using System.Collections;

public class ButtonHandler : MonoBehaviour {

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
                Debug.Log("picking mode");
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
    }
}
