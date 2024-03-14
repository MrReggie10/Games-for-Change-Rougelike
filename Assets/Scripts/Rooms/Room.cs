using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType { Normal, Start, Boss }

public class Room : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemies;
    public List<Door> connectedDoors;

    public Vector2Int mapPos;
    [SerializeField] Vector2Int m_size;
    public Vector2Int size => m_size;
    public RoomType roomType;
    public bool isCleared { get; private set; }

    private void Start()
    {
        Init();
        if (roomType == RoomType.Start)
            GameManager.instance.RoomChangeTrigger(this);
    }

    public void Init()
    {
        for(int i = 0; i < enemies.Count; i++)
        {
            GameObject enemy = enemies[i];
            enemy.GetComponent<CombatTarget>().OnDeath += delegate { enemies.Remove(enemy); if (enemies.Count == 0) Clear(); };
            enemy.SetActive(false);
        }
        if (enemies.Count == 0)
            isCleared = true;
    }

    public void SetLocation(Vector2Int mapPos)
    {
        this.mapPos = mapPos;
    }

    public bool Enter()
    {
        if (isCleared)
            return false;
        foreach (GameObject enemy in enemies)
            enemy.SetActive(true);
        LockDoors();
        return true;
    }

    public void DisableEnemies()
    {
        foreach (GameObject enemy in enemies)
            enemy.SetActive(false);
    }

    public void LockDoors()
    {
        foreach(Door door in connectedDoors)
        {
            door.Lock();
        }
    }

    public void UnlockDoors()
    {
        foreach (Door door in connectedDoors)
        {
            door.Unlock();
        }
    }

    public void Clear()
    {
        isCleared = true;
        GameManager.instance.EndBattle();
        UnlockDoors();
    }
}
