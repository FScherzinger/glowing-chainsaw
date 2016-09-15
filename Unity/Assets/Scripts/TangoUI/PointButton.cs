using UnityEngine;
using System.Collections;

public class PointButton : MonoBehaviour {

    public ButtonHandler Handler;

    public void point()
    {
        Handler.setCurrent(ButtonHandler.SelectedButton.point);
    }
}
