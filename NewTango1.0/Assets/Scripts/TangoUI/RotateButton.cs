using UnityEngine;
using System.Collections;

public class RotateButton : MonoBehaviour {

    public ButtonHandler Handler;

    public void rotate()
    {
        Handler.setCurrent(ButtonHandler.SelectedButton.rotate);
    }
}
