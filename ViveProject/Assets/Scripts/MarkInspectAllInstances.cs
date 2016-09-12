using UnityEngine;
using System.Collections;

public class MarkInspectAllInstances : MonoBehaviour {
    private GameObject[] InteractableCubes;

    public void Marking(bool val)
    {
        InteractableCubes = GameObject.FindGameObjectsWithTag("InteractableCube");
        foreach (GameObject cube in InteractableCubes)
        {
            cube.GetComponent<MarkAndInspect>().mark = val;
        }
    }

    public void Inspecting(bool val)
    {
        InteractableCubes = GameObject.FindGameObjectsWithTag("InteractableCube");
        foreach (GameObject cube in InteractableCubes)
        {
            cube.GetComponent<MarkAndInspect>().inspect = val;
        }
    }
}
