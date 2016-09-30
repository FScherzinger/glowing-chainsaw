using UnityEngine;
using System.Collections;

public class PickButton : MonoBehaviour {

    [SerializeField] private ButtonHandler Handler;
    [SerializeField] private MenuButton Menu;


    public void pick()
    {
        Debug.Log("picking mode");
        Handler.setCurrent(ButtonHandler.SelectedButton.pickAndPlace);
        Menu.Button3.interactable = true;
        Menu.Button2.interactable = true;
        Menu.Button1.interactable = false;
        //Menu.CollapseMenu();
    }
}
