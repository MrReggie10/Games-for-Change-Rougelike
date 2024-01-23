using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLerp : MonoBehaviour
{
    /*
     * created to have the camera look ahead of where the play is moving
    */

    [SerializeField] private GameObject cam;
    [SerializeField] private GameObject lerpGO;

    void Update()
    {
        lerpGO.transform.position = Vector2.Lerp(lerpGO.transform.position, transform.position, 0.05f);
        cam.transform.position = new Vector3(0, 0, -10) + transform.position + (transform.position - lerpGO.transform.position);
    }
}
