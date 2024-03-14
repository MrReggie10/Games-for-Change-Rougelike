using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Room room1;
    public Room room2;

    [SerializeField] GameObject barrier;
    [SerializeField] GameObject door;

    private void Awake()
    {
        Unlock();
    }

    public void Unlock()
    {
        barrier.SetActive(false);
    }

    public void Lock()
    {
        barrier.SetActive(true);
    }

    public void RoomChangeTrigger(int side)
    {
        if(side == 0)
        {
            GameManager.instance.RoomChangeTrigger(room1);
        }
        else if(side == 1)
        {
            GameManager.instance.RoomChangeTrigger(room2);
        }
        else
        {
            Debug.LogError($"RoomChangeTrigger called with invalid side id. Was {side}, needs to be 0 or 1");
        }
    }
}
