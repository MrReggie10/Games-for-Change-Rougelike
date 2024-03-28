using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorGenerationDebug : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<FloorGeneration>().GenerateFloor(new FloorGeneration.FloorProperties(0, new FloorGeneration.MainRouteProperties(0, 0, 0, 0), 0));
    }
}
