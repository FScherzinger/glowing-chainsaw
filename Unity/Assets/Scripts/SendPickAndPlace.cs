using UnityEngine;
using System.Collections;
using System.Threading;

using de.dfki.tecs.robot.baxter;

public class SendPickAndPlace : MonoBehaviour
{

    private PickAndPlace pick_n_place;
    private Thread thread;

    public bool pickandPlaceButtonClicked;
    private int state;
    private GameObject movefrom;
    private Vector3 moveto;

    public Vector3 armAtFixedMarkerposition;
    public Vector3 armAtFixedMarkerrotation;
    public GameObject fixedMarker;
    // Use this for initialization
    void Start()
    {
        state = 0;
        pickandPlaceButtonClicked = false;
    }

    public void SendPAP(Vector3 from, Vector3 to)
    {
        SendPAP(from,to, Vector3.zero, Vector3.zero);
    }


    public void SendPAP(Vector3 from, Vector3 to, Vector3 fromrot, Vector3 torot)
    {
        bool waiting = true;
        while (waiting)
        {
            if (thread == null || !thread.IsAlive)
            {
                waiting = false;
                Vector3 markertoarm = new Vector3(-fixedMarker.transform.position.z, fixedMarker.transform.position.x, armAtFixedMarkerposition.z);
                from = new Vector3(-from.z, from.x, armAtFixedMarkerposition.z);
                Vector3 pickpos = from - markertoarm + armAtFixedMarkerposition;
                to = new Vector3(-to.z, to.x, armAtFixedMarkerposition.z);
                Vector3 placepos = to - markertoarm + armAtFixedMarkerposition;
                //Has to be changed if more rotations than around y axis possible;
                Vector3 pickrot = armAtFixedMarkerrotation;
                pickrot.x = pickrot.x + fromrot.y;
                pickrot = pickrot * Mathf.PI / 180;
                Vector3 placerot = armAtFixedMarkerrotation;
                placerot.x = placerot.x + torot.y;
                placerot = placerot * Mathf.PI / 180;
                Debug.Log("pick " + pickpos);
                Debug.Log("place " + placepos);
                pick_n_place = new PickAndPlace
                {
                    serverAddress = "192.168.1.101",
                    serverPort = 9000,
                    pap_event = new PickAndPlaceEvent
                    {
                        Limb = Limb.RIGHT,

                        Initial_pos = new Position
                        {
                            X_right = pickpos.x.ToString().Replace(',', '.'),
                            Y_right = pickpos.y.ToString().Replace(',', '.'),
                            Z_right = pickpos.z.ToString().Replace(',', '.')
                        },

                        Final_pos = new Position
                        {
                            X_right = placepos.x.ToString().Replace(',', '.'),
                            Y_right = placepos.y.ToString().Replace(',', '.'),
                            Z_right = placepos.z.ToString().Replace(',', '.')
                        },

                        Initial_ori = new Orientation
                        {
                            Yaw_right = pickrot.x.ToString().Replace(',', '.'),
                            Pitch_right = pickrot.y.ToString().Replace(',', '.'),
                            Roll_right = pickrot.z.ToString().Replace(',', '.')
                        },

                        Final_ori = new Orientation
                        {
                            Yaw_right = placerot.x.ToString().Replace(',', '.'),
                            Pitch_right = placerot.y.ToString().Replace(',', '.'),
                            Roll_right = placerot.z.ToString().Replace(',', '.')
                        },

                        Speed = new Speed
                        {
                            Speed_right = "0.2"
                        },

                        Angls = new Angles(),
                        Mode = Reference_sys.ABSOLUTE,
                        Kin = Kinematics.INVERSE
                    },
                    init = false
                };
                thread = new Thread(pick_n_place.Send);
                thread.Start();
            }
            else
            {
                Debug.Log("Pick and Place not Send, old thread still running");
            }
        }
        state = 0;
        pickandPlaceButtonClicked = false;
    }

    public void ClickPAPButton()
    {
        pickandPlaceButtonClicked = !pickandPlaceButtonClicked;
        state = 0;
    }

    void Update()
    {
        if (pickandPlaceButtonClicked)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Test");
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider != null)
                    {
                        if (state == 0)
                        {
                            if (hit.collider.GetType() == typeof(MeshCollider))
                            {
                                Debug.Log("No valid object");
                            }
                            else
                            {
                                movefrom = hit.collider.gameObject;
                                Debug.Log("GameObject at " + movefrom.transform.position + " selected");
                                state++;
                            }
                        }
                        else
                        {
                            moveto = hit.point;
                            Debug.Log("Move" + movefrom.transform.position + "to" + moveto);
                            SendPAP(movefrom.transform.position, moveto);
                            pickandPlaceButtonClicked = false;
                            state = 0;
                        }
                    }
                }
                else
                {
                    Debug.Log("Not clicked in valid area");
                }
            }
        }
    }

    void OnApplicationClose()
    {
        thread.Abort();
    }
}
