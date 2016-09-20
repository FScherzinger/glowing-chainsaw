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
        Handler.setCurrent(ButtonHandler.SelectedButton.rotate);
        Menu.CollapseMenu();
    }
}
