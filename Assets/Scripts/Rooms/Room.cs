using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public GameObject topBarrier;
    public GameObject rightBarrier;
    public GameObject bottomBarrier;
    public GameObject leftBarrier;

    [SerializeField] private Collider2D exitHitbox;

    [HideInInspector] public int mapXPos;
    [HideInInspector] public int mapYPos;

    public List<SpriteRenderer> renderers = new List<SpriteRenderer>();
    
    public void SetLocation(int x, int y)
    {
        mapXPos = x;
        mapYPos = y;
    }

    private void Update()
    {
        if (exitHitbox.IsTouching(PlayerSingleton.player.GetComponent<Collider2D>()))
        {
            Camera.main.transform.position = new Vector3(Vector2.Lerp(Camera.main.transform.position, transform.position, 0.125f).x, Vector2.Lerp(Camera.main.transform.position, transform.position, 0.125f).y, -10);
        }
    }
}
