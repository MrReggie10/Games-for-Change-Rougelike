using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GFC.Items;

public class ItemButton : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] Image image;
    public ItemSO item { get; private set; }
    public new RectTransform transform => (RectTransform)base.transform;

    public void SetItem(ItemSO item)
    {
        if(item == null)
        {
            Debug.LogError("Null item invalid for SetItem(). Please use SetNull() instead.");
            return;
        }
        this.item = item;
        image.sprite = item.inventorySprite;
        button.interactable = true;
    }

    public void SetNull(Sprite nullSprite)
    {
        item = null;
        image.sprite = nullSprite;
        button.interactable = false;
    }

    public void SetClickCallback(UnityEngine.Events.UnityAction callback)
    {
        button.onClick.AddListener(callback);
    }
}
