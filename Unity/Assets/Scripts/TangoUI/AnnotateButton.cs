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
        Debug.Log("annotating mode");
        Handler.setCurrent(ButtonHandler.SelectedButton.annotate);
        Menu.Button3.interactable = false;
        Menu.Button2.interactable = true;
        Menu.Button1.interactable = true;
        //Menu.CollapseMenu();
    }
}
