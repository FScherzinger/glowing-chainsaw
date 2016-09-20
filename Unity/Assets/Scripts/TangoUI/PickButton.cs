using UnityEngine;
using System.Collections;

public class PickButton : MonoBehaviour {

    [SerializeField] private ButtonHandler Handler;
    [SerializeField] private MenuButton Menu;


    public void pick()
    {
        Handler.setCurrent(ButtonHandler.SelectedButton.pickAndPlace);
        Menu.CollapseMenu();
    }
}
