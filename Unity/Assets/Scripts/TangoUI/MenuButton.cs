using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{

    public Button Button1;
    public Button Button2;
    public Button Button3;
    public Button Button4;

    private bool MenuState = false; // collapsed = true, extended = false

    // instead of a boolean we use an unsigned integer as lock.
    // with that we have more inside on how many coroutines are still running
    private uint locked = 0;

    public void MenuButtonClick()
    {
        // first of all check if we are allowed to click.
        if( locked > 0 )
        {
            Debug.Log( "locked" );
            return;
        }
        // check menu state and perform the proper operation.
        if( MenuState )
            CollapseMenu();
        else
            ExtendMenu();
        MenuState = !MenuState;
    }

    private void CollapseMenu()
    {
        Debug.Log( "Collapsing Menu" );
        StartCoroutine( MoveButton( Button1, -5 ) );
        StartCoroutine( MoveButton( Button2, -10 ) );
        StartCoroutine( MoveButton( Button3, -15 ) );
        StartCoroutine( MoveButton( Button4, -20 ) );
    }

    private void ExtendMenu()
    {
        Debug.Log( "Extending Menu" );
        StartCoroutine( MoveButton( Button1, 5 ) );
        StartCoroutine( MoveButton( Button2, 10 ) );
        StartCoroutine( MoveButton( Button3, 15 ) );
        StartCoroutine( MoveButton( Button4, 20 ) );
    }

    /// <summary>
    /// MoveButton
    ///     Moves the button on the y-axis.
    ///     Performs a transition, after every movement of 1 unit we will
    ///     pass back to Unity to prevent from freezing.
    /// </summary>
    /// <param name="button">The button to move</param>
    /// <param name="distance">the distance the button will be translated in unity units</param>
    private IEnumerator MoveButton( Button button, int distance )
    {
        ++locked;
        // activate the button if we extend the menu
        if( distance > 0 )
            button.gameObject.SetActive( true );

        int covered_track = 0;

        // distinguish whether we will collapse or extend the menu
        float step = (distance < 0) ? 40f : -40f;
        distance = Mathf.Abs( distance );

        while( covered_track < distance )
        {
            button.transform.position += new Vector3( 0f, step, 0f );
            covered_track++;

            yield return null;
        }
        --locked;
        // only hide the button if we collapse the menu
        if( step > 0 )
            button.gameObject.SetActive( false );
    }
}
