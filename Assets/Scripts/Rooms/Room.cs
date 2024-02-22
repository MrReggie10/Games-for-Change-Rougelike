using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType { Normal, Start, Boss }

public class Room : MonoBehaviour
{
    public GameObject topBarrier;
    public GameObject rightBarrier;
    public GameObject bottomBarrier;
    public GameObject leftBarrier;

    [SerializeField] private List<GameObject> enemies;
    [SerializeField] private Collider2D exitHitbox;

    [HideInInspector] public Vector2Int mapPos;
    [field: SerializeField] public RoomType roomType { get; private set; }

    public List<SpriteRenderer> renderers;

    public void Init()
    {
        foreach (GameObject enemy in enemies)
            enemy.SetActive(false);
    }
    
    public void SetLocation(Vector2Int mapPos)
    {
        this.mapPos = mapPos;
    }

    public void SetDoors(bool[] doorOpenings)
    {
        Debug.LogError("SetDoors not implemented.");
    }

    public void ToggleDoor(int doorIndex)
    {
        Debug.LogError("ToggleDoor not implemented.");
    }

    public void Enter()
    {
        foreach (GameObject enemy in enemies)
            enemy.SetActive(true);
    }

    private void Update()
    {
        if (exitHitbox.IsTouching(PlayerSingleton.player.GetComponent<Collider2D>()))
        {
            Camera.main.transform.position = new Vector3(Vector2.Lerp(Camera.main.transform.position, transform.position, 0.125f).x, Vector2.Lerp(Camera.main.transform.position, transform.position, 0.125f).y, -10);
        }
    }
}
