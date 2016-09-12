using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    public GameObject menu_panel;

    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;
    public Button button5;

    private bool menu_state = false;
    private bool locked = false;

    void Update()
    {
        if( Input.GetKeyDown( KeyCode.Escape ) )
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        if( locked )
            return;
        if( !menu_state )
            StartCoroutine( ExtendMenu() );
        else
            StartCoroutine( CollapseMenu() );
    }

    private IEnumerator ExtendMenu()
    {
        menu_state = true;
        locked = true;

        for( int i = 0; i < 100; ++i )
        {
            menu_panel.transform.position += new Vector3( -2, 0, 0 );
            if( i % 5 == 0 )
                yield return null;
        }
        locked = false;

        
    }

    private IEnumerator CollapseMenu()
    {
        menu_state = false;
        locked = true;

        for( int i = 0; i < 100; ++i )
        {
            menu_panel.transform.position += new Vector3( 2, 0, 0 );
            if( i % 5 == 0 )
                yield return null;
        }
        locked = false;
    }

    public void StartSplitscreen()
    {
        SceneManager.LoadScene( "Splitscreen" );
    }
}
