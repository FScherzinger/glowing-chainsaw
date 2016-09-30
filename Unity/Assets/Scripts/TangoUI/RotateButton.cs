using UnityEngine;
using System.Collections;

public class RotateButton : MonoBehaviour
{

    [SerializeField]
    private ButtonHandler Handler;
    [SerializeField]
    private MenuButton Menu;


    public void rotate()
    {
        Debug.Log("rotating mode");
        Handler.setCurrent(ButtonHandler.SelectedButton.rotate);
        Menu.Button3.interactable = true;
        Menu.Button2.interactable = false;
        Menu.Button1.interactable = true;
        //Menu.CollapseMenu();
    }
}
