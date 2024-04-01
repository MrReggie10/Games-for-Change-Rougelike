using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GFC.Items;

public class ItemDropDebug : MonoBehaviour
{
    [SerializeField] ItemSO item;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
            ItemDrop.Create(item, Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
}
