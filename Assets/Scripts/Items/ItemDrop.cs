using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GFC.Items
{
    public class ItemDrop : MonoBehaviour
    {
        static ItemDrop m_prefab;
        public static ItemDrop prefab { get { if (m_prefab == null) m_prefab = Resources.Load<GameObject>("Prefabs/Item Drop").GetComponent<ItemDrop>(); return m_prefab; } }

        private ItemSO m_item;
        public ItemSO item { get => m_item; set => SetItem(value); }

        private new SpriteRenderer renderer;

        private void Awake()
        {
            renderer = GetComponent<SpriteRenderer>();
        }

        public void SetItem(ItemSO newItem)
        {
            m_item = newItem;
            renderer.sprite = newItem.dropSprite;
        }


        public static implicit operator ItemSO(ItemDrop drop)
        {
            return drop.item;
        }

        public static ItemDrop Create(ItemSO item, Vector2 position)
        {
            ItemDrop drop = Instantiate(prefab, position, Quaternion.identity);
            drop.item = item;
            return drop;
        }
    }
}