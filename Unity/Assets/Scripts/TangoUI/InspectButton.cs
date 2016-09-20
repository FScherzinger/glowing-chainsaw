using UnityEngine;
using System.Collections;

public class InspectButton : MonoBehaviour
{

    [SerializeField] private ButtonHandler Handler;
    [SerializeField] private MenuButton Menu;


    public void inspect()
    {
        Handler.setCurrent(ButtonHandler.SelectedButton.inspect);
        Menu.CollapseMenu();
    }
}
