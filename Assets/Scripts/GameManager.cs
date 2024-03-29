using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public Room currentRoom { get; private set; }
    public bool playerInBattle { get; private set; }

    public event Action<bool> OnRoomEnter;
    public event Action OnBattleEnd;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            ClearCurrentRoom();
    }

    public void RoomChangeTrigger(Room room)
    {
        if (playerInBattle || room == currentRoom)
            return;
        Debug.Log($"Entering Room {room.gameObject.name}.");
        currentRoom = room;
        playerInBattle = room.Enter();
        OnRoomEnter?.Invoke(playerInBattle);
    }

    public void ClearCurrentRoom()
    {
        currentRoom.DisableEnemies();
        currentRoom.Clear();
    }

    public void EndBattle()
    {
        playerInBattle = false;
        OnBattleEnd?.Invoke();
    }
}
