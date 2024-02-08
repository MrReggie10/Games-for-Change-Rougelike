using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GFC.Items;

public class PlayerInventory : MonoBehaviour
{
    public enum OverflowBehavior { Drop, Discard, IgnoreLimit }

    [SerializeField] List<ItemSO> m_items;
    public IReadOnlyList<ItemSO> items => m_items;
    [SerializeField] int m_capacity;
    public int capacity { get => m_capacity; set => SetCapacity(value); }

    public bool AddItem(ItemSO item, OverflowBehavior onOverflow = OverflowBehavior.Drop)
    {
        if(m_items.Count >= capacity)
        {
            switch(onOverflow)
            {
                case OverflowBehavior.Drop: DropItem(item); break;
                case OverflowBehavior.Discard: break;
                case OverflowBehavior.IgnoreLimit: m_items.Add(item); break;
            }
            return false;
        }
        m_items.Add(item);
        return true;
    }

    public bool RemoveItem(ItemSO item, bool drop = true)
    {
        int index = m_items.IndexOf(item);
        if (index < 0)
            return false;
        RemoveItem(m_items.IndexOf(item), drop);
        return true;
    }

    public ItemSO RemoveItem(int index, bool drop = true)
    {
        if (index >= m_items.Count || index < 0)
            return null;
        ItemSO item = m_items[index];
        m_items.RemoveAt(index);
        if (drop)
            DropItem(item);
        return item;
    }

    public int SetCapacity(int size, OverflowBehavior onOverflow = OverflowBehavior.Drop)
    {
        m_capacity = size;
        if (size >= m_items.Count)
            return 0;

        int overflowAmount = m_items.Count - size;
        switch(onOverflow)
        {
            case OverflowBehavior.Drop:
                for(int i = m_items.Count-1; i >= size; i--)
                {
                    DropItem(m_items[i]);
                    m_items.RemoveAt(i);
                }
                break;
            case OverflowBehavior.Discard:
                m_items.RemoveRange(size, overflowAmount);
                break;
            case OverflowBehavior.IgnoreLimit:
                break;
        }
        return overflowAmount;
    }

    private void DropItem(ItemSO item)
    {
        //placeholder
        Debug.Log($"Dropped item {item.name}.");
    }
}
