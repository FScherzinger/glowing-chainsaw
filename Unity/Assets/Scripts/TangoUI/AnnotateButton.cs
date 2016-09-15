using UnityEngine;
using System.Collections;

public class AnnotateButton : MonoBehaviour {

    public ButtonHandler Handler;

    public void annotate()
    {
        Handler.setCurrent(ButtonHandler.SelectedButton.annotate);
    }
}
