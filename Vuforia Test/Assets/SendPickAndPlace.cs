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
        Vector3 pickrot = armAtFixedMarkerrotation;
        pickrot = pickrot * Mathf.PI / 180; pick_n_place = new PickAndPlace
        {
            serverAddr = "192.168.1.101",
            serverPort = 9000,
            pap_event = new PickAndPlaceEvent
            {
                Limb = Limb.RIGHT,

                Initial_pos = new Position
                {
                    X_right = "0.5",
                    Y_right = "0.5",
                    Z_right = "0"
                },

                Final_pos = new Position
                {
                    X_right = "0.5",
                    Y_right = "0.5",
                    Z_right = "0"
                },

                Initial_ori = new Orientation
                {
                    Yaw_right = pickrot.x.ToString(),
                    Pitch_right = pickrot.y.ToString(),
                    Roll_right = pickrot.z.ToString()
                },

                Final_ori = new Orientation
                {
                    Yaw_right = pickrot.x.ToString(),
                    Pitch_right = pickrot.y.ToString(),
                    Roll_right = pickrot.z.ToString()
                },

                Speed = new Speed
                {
                    Speed_right = "0.2"
                },

                Angls = new Angles(),
                Mode = Reference_sys.ABSOLUTE,
                Kin = Kinematics.INVERSE
            }
        };

        thread = new Thread( pick_n_place.Send );
        thread.Start();
        state = 0;
        pickandPlaceButtonClicked = false;
    }

    private void SendPAP(Vector3 from, Vector3 to)
    {
        if (!thread.IsAlive)
        {
            Vector3 pickpos = from - fixedMarker.transform.position + armAtFixedMarkerposition;
            Vector3 placepos = to - fixedMarker.transform.position + armAtFixedMarkerposition;
            //Rotation currently only Armposition of Baxter when picking one Object. Is this even always the same for same Objectrotation???
            Vector3 pickrot = armAtFixedMarkerrotation;
            pickrot = pickrot * Mathf.PI / 180;
            Vector3 placerot = armAtFixedMarkerrotation;
            placerot = placerot * Mathf.PI / 180;
            Debug.Log(placerot);
            pick_n_place = new PickAndPlace
            {
                serverAddr = "192.168.1.101",
                serverPort = 9000,
                pap_event = new PickAndPlaceEvent
                {
                    Limb = Limb.RIGHT,

                    Initial_pos = new Position
                    {
                        X_right = pickpos.x.ToString(),
                        Y_right = pickpos.y.ToString(),
                        Z_right = pickpos.z.ToString()
                    },

                    Final_pos = new Position
                    {
                        X_right = placepos.x.ToString(),
                        Y_right = placepos.y.ToString(),
                        Z_right = placepos.z.ToString()
                    },

                    Initial_ori = new Orientation
                    {
                        Yaw_right = pickrot.x.ToString(),
                        Pitch_right = pickrot.y.ToString(),
                        Roll_right = pickrot.z.ToString()
                    },

                    Final_ori = new Orientation
                    {
                        Yaw_right = placerot.x.ToString(),
                        Pitch_right = placerot.y.ToString(),
                        Roll_right = placerot.z.ToString()
                    },

                    Speed = new Speed
                    {
                        Speed_right = "0.2"
                    },

                    Angls = new Angles(),
                    Mode = Reference_sys.ABSOLUTE,
                    Kin = Kinematics.INVERSE
                }
            };
            thread = new Thread(pick_n_place.Send);
            thread.Start();
        }
        else
        {
            Debug.Log("Pick and Place not Send, old thread still running");
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
