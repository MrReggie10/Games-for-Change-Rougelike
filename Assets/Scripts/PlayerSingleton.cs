using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSingleton : MonoBehaviour
{
    public static GameObject player;

    private void Awake()
    {
        player = gameObject;
    }
}
