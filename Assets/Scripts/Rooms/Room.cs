using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public GameObject topBarrier;
    public GameObject rightBarrier;
    public GameObject bottomBarrier;
    public GameObject leftBarrier;

    public int mapXPos;
    public int mapYPos;

    public List<SpriteRenderer> renderers = new List<SpriteRenderer>();
    
    public void SetLocation(int x, int y)
    {
        mapXPos = x;
        mapYPos = y;
    }
}
