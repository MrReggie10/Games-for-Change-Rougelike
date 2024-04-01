using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GFC.Items;

public class ItemUser : MonoBehaviour
{
    private List<ItemDrop> m_drops;
    public IReadOnlyList<ItemDrop> dropsInRange => m_drops;

    public event Action<ItemDrop> OnItemApproach;
    public event Action<ItemDrop> OnItemLeave;

    private void Awake()
    {
        m_drops = new List<ItemDrop>();
    }

    public ItemSO Pickup(int index)
    {
        if (m_drops.Count <= index)
            return null;
        ItemDrop item = m_drops[index];
        m_drops.RemoveAt(index);
        Destroy(item.gameObject);
        OnItemLeave?.Invoke(item);
        return item;
    }

    public bool Pickup(ItemDrop item)
    {
        if (!m_drops.Contains(item))
            return false;
        m_drops.Remove(item);
        Destroy(item.gameObject);
        OnItemLeave?.Invoke(item);
        return true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ItemDrop item = collision.GetComponent<ItemDrop>();
        if (item == null)
            return;
        if (m_drops.Contains(item))
            return;
        Debug.Log($"Adding item {item.gameObject.name} with internal item {item.item}.");
        m_drops.Add(item);
        OnItemApproach?.Invoke(item);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ItemDrop item = collision.GetComponent<ItemDrop>();
        if (item == null)
            return;
        if (!m_drops.Contains(item))
            return;
        m_drops.Remove(item);
        OnItemLeave?.Invoke(item);
    }
}
