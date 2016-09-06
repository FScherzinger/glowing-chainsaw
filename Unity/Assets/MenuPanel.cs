using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuPanel : MonoBehaviour {

    bool menu = true;
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void ExpandMenu()
    {
        bool toggle = false;
        button1.gameObject.SetActive( true );
        button2.gameObject.SetActive( true );
        button3.gameObject.SetActive( true );
        button4.gameObject.SetActive( true );
        RectTransform rect = button1.GetComponent<RectTransform>();
        if( rect.localPosition.x == 300 )
            toggle = true;
        
        StartCoroutine( YieldingWork( toggle ) );
    }

    IEnumerator YieldingWork( bool toggle )
    {
        while( true )
        {
            // expand
            if( toggle )
            {
                for( int i = 0; i < 25; ++i )
                {
                    button1.transform.position = new Vector3( button1.transform.position.x - 2.5f,
                                                              button1.transform.position.y,
                                                              button1.transform.position.z );
                    button2.transform.position = new Vector3( button2.transform.position.x - 2.5f,
                                                              button2.transform.position.y + 2.5f,
                                                              button2.transform.position.z );
                    button3.transform.position = new Vector3( button3.transform.position.x,
                                                              button3.transform.position.y + 2.5f,
                                                              button3.transform.position.z );
                    button4.transform.position = new Vector3( button4.transform.position.x + 2.5f,
                                                              button4.transform.position.y + 2.5f,
                                                              button4.transform.position.z);
                    yield return null;
                }
                break;
                
            }
            // collapse
            else
            {
                for( int i = 0; i < 25; ++i )
                {
                    button1.transform.position = new Vector3( button1.transform.position.x + 2.5f,
                                                              button1.transform.position.y,
                                                              button1.transform.position.z );
                    button2.transform.position = new Vector3( button2.transform.position.x + 2.5f,
                                                              button2.transform.position.y - 2.5f,
                                                              button2.transform.position.z );
                    button3.transform.position = new Vector3( button3.transform.position.x,
                                                              button3.transform.position.y - 2.5f,
                                                              button3.transform.position.z );
                    button4.transform.position = new Vector3( button4.transform.position.x - 2.5f,
                                                              button4.transform.position.y - 2.5f,
                                                              button4.transform.position.z );
                    yield return null;
                }
                button1.gameObject.SetActive( false );
                button2.gameObject.SetActive( false );
                button3.gameObject.SetActive( false );
                button4.gameObject.SetActive( false );
                break;
            }
        }
    }
}
