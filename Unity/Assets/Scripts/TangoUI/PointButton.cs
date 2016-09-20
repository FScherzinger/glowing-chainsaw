using UnityEngine;
using System.Collections;

public class PointButton : MonoBehaviour
{

    [SerializeField]
    private ButtonHandler Handler;
    [SerializeField]
    private MenuButton Menu;


    public void point()
    {
        Handler.setCurrent(ButtonHandler.SelectedButton.point);
        Menu.CollapseMenu();
    }
}
