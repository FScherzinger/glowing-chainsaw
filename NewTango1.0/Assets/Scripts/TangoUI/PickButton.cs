using UnityEngine;
using System.Collections;

public class PickButton : MonoBehaviour {

    public ButtonHandler Handler;

    public void pick()
    {
        Handler.setCurrent(ButtonHandler.SelectedButton.pickAndPlace);
    }
}
