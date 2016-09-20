using UnityEngine;
using System.Collections;

public class AnnotateButton : MonoBehaviour
{

    [SerializeField]
    private ButtonHandler Handler;
    [SerializeField]
    private MenuButton Menu;


    public void annotate()
    {
        Handler.setCurrent(ButtonHandler.SelectedButton.annotate);
        Menu.CollapseMenu();
    }
}
