using UnityEngine;
using System.Collections;

public class InspectButton : MonoBehaviour {

    public ButtonHandler Handler;

    public void inspect()
    {
        Handler.setCurrent(ButtonHandler.SelectedButton.inspect);
    }
}
