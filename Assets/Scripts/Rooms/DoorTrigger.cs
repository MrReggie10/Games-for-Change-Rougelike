using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] bool topOrRightSide;
    [SerializeField] Door door;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            door.RoomChangeTrigger(topOrRightSide ? 1 : 0);
    }
}
