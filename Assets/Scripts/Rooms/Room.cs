using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType { Normal, Start, Boss, Treasure, Connector }
public enum RouteType { Normal, Hot, Cold }

public class Room : MonoBehaviour
{
    [Header("Local Properties")]
    [SerializeField] private List<GameObject> enemies;

    [Header("Template Properties")]
    [SerializeField] Vector2Int m_size;
    public Vector2Int size => m_size;
    [SerializeField] RoomType m_roomType;
    public RoomType roomType => m_roomType;
    [SerializeField] RouteType m_routeType;
    public RouteType routeType => m_routeType;
    //[SerializeField] List<bool> m_availableDoors;
    //public IReadOnlyList<bool> availableDoors => m_availableDoors;

    [Header("Map Properties (Set by script)")]
    public Vector2Int mapPos;
    public List<Door> connectedDoors;
    public bool isCleared { get; private set; }

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
