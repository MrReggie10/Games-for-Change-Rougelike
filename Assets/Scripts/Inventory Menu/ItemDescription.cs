using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GFC.Items;
using TMPro;

public class ItemDescription : MonoBehaviour
{
    private ItemSO m_displayedItem;
    public ItemSO displayedItem { get => m_displayedItem; set => DisplayItem(value); }

    [SerializeField] TMP_Text itemName;
    [SerializeField] Image itemThumbnail;
    [SerializeField] TMP_Text mainStats;
    [SerializeField] TMP_Text description;

    private void Awake()
    {
        ClearDisplay();
    }

    public void ClearDisplay()
    {
        m_displayedItem = null;
        itemName.text = "";
        itemThumbnail.enabled = false;
        mainStats.text = "";
        description.text = "";
    }

    public void DisplayItem(ItemSO item)
    {
        m_displayedItem = item;
        if(item == null)
        {
            ClearDisplay();
            return;
        }

        itemName.text = item.name;
        itemThumbnail.sprite = item.inventorySprite;
        itemThumbnail.enabled = true;
        mainStats.text = item.info;
        description.text = item.description;

    }
}
