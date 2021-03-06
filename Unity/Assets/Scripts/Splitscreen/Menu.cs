﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    public GameObject menu_panel;
	public GameObject splitscreen;
	public GameObject maincam;

    private bool menu_state = false;
    private bool locked = false;
	private bool switchScreen = false;

    void Update()
    {
        if( Input.GetKeyDown( KeyCode.Escape ) )
        {
            /*ToggleMenu();*/
			if(maincam!=null)
				maincam.SetActive (switchScreen);
            if (splitscreen != null) {
                splitscreen.SetActive(!switchScreen);
                switchScreen = !switchScreen;
                Camera PCSplitScreen = GameObject.Find("PC View").GetComponent<Camera>();
                PCSplitScreen.transform.position = maincam.transform.position;
                PCSplitScreen.transform.rotation = maincam.transform.rotation;
                    }
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
		if(maincam!=null)
			maincam.SetActive (false);
		if(splitscreen!=null)
			splitscreen.SetActive (true);
    }
}
