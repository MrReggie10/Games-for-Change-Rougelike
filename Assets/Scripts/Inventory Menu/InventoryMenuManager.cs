using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GFC.Items;

public class InventoryMenuManager : MonoBehaviour
{
    [SerializeField] PlayerStats stats;
    [SerializeField] PlayerInventory inventory;
    [Space]
    [SerializeField] ItemButton hatButton;
    [SerializeField] List<ItemButton> shirtButtons;
    [SerializeField] List<ItemButton> pantsButtons;
    [SerializeField] ItemButton shoesButton;
    [Space]
    [SerializeField] RectTransform inventoryDisplay;
    [SerializeField] List<ItemButton> inventoryItemButtons;
    [SerializeField] TMP_Text inventoryText;
    [Space]
    [SerializeField] ItemDescription description;
    [Space]
    [SerializeField] Sprite noItemSprite;
    [SerializeField] float itemButtonSpacing;
    [Space]
    [SerializeField] private bool updateItemsAtEOF;

    void Awake()
    {
        stats.OnEquipClothing += delegate { updateItemsAtEOF = true; };
        stats.OnUnequipClothing += delegate { updateItemsAtEOF = true; };
        InitEquipmentButtons();
        InitInventoryButtons(6);
    }

    private void Start()
    {
        Open();
    }

    void InitEquipmentButtons()
    {
        hatButton.SetClickCallback(UnequipHat);
        for(int i = 0; i < shirtButtons.Count; i++)
        {
            int index = i; //this is needed because if the delegate accesses i directly, it would get what i is at the time of calling the delegate, which would = the list size
            shirtButtons[i].SetClickCallback(delegate { UnequipShirt(index); });
        }
        for (int i = 0; i < pantsButtons.Count; i++)
        {
            int index = i; //this is needed because if the delegate accesses i directly, it would get what i is at the time of calling the delegate, which would = the list size
            pantsButtons[i].SetClickCallback(delegate { UnequipPants(index); });
        }
        shoesButton.SetClickCallback(UnequipShoes);
    }

    void InitInventoryButtons(int maxButtons)
    {
        Transform baseButton = inventoryItemButtons[0].transform;
        while (inventoryItemButtons.Count < maxButtons)
        {
            inventoryItemButtons.Add(Instantiate(baseButton.gameObject, inventoryDisplay).GetComponent<ItemButton>());
        }
        while(inventoryItemButtons.Count > maxButtons)
        {
            int end = inventoryItemButtons.Count - 1;
            Destroy(inventoryItemButtons[end].gameObject);
            inventoryItemButtons.RemoveAt(end);
        }

        for(int i = 0; i < inventoryItemButtons.Count; i++)
        {
            int index = i; //this is needed because if the delegate accesses i directly, it would get what i is at the time of calling the delegate, which would = the list size
            inventoryItemButtons[i].SetClickCallback(delegate { UseInventoryItem(index); });
        }
    }

    public void Open()
    {
        UpdateEquipment();
        UpdateInventory();
        description.ClearDisplay();
        updateItemsAtEOF = false;
        enabled = true;
    }

    public void Close()
    {
        enabled = false;
    }

    public void UpdateEquipment()
    {
        if(stats.equippedHat)
        {
            hatButton.SetItem(stats.equippedHat);
        }
        else
        {
            hatButton.SetNull(noItemSprite);
        }

        float firstX = -(stats.equippedShirts.Count - 1) * itemButtonSpacing / 2;
        for (int i = 0; i < stats.equippedShirts.Count; i++)
        {
            shirtButtons[i].gameObject.SetActive(true);
            shirtButtons[i].transform.anchoredPosition = new Vector2(firstX + itemButtonSpacing * i, 0);
            shirtButtons[i].SetItem(stats.equippedShirts[i]);
        }
        for (int i = stats.equippedShirts.Count; i < shirtButtons.Count; i++)
        {
            shirtButtons[i].gameObject.SetActive(false);
        }
        if (stats.equippedShirts.Count == 0)
        {
            shirtButtons[0].gameObject.SetActive(true);
            shirtButtons[0].transform.anchoredPosition = Vector2.zero;
            shirtButtons[0].SetNull(noItemSprite);
        }

        firstX = -(stats.equippedPants.Count - 1) * itemButtonSpacing / 2;
        for (int i = 0; i < stats.equippedPants.Count; i++)
        {
            pantsButtons[i].gameObject.SetActive(true);
            pantsButtons[i].transform.anchoredPosition = new Vector2(firstX + itemButtonSpacing * i, 0);
            pantsButtons[i].SetItem(stats.equippedPants[i]);
        }
        for (int i = stats.equippedPants.Count; i < pantsButtons.Count; i++)
        {
            pantsButtons[i].gameObject.SetActive(false);
        }
        if (stats.equippedPants.Count == 0)
        {
            pantsButtons[0].gameObject.SetActive(true);
            pantsButtons[0].transform.anchoredPosition = Vector2.zero;
            pantsButtons[0].SetNull(noItemSprite);
        }

        if (stats.equippedShoes)
        {
            shoesButton.SetItem(stats.equippedShoes);
        }
        else
        {
            shoesButton.SetNull(noItemSprite);
        }
    }

    public void UpdateInventory()
    {
        if (inventory.items.Count > inventoryItemButtons.Count)
            InitInventoryButtons(inventory.items.Count + 4);
        inventoryText.text = $"Inventory {inventory.items.Count}/{inventory.capacity}";
        float firstX = -(inventory.items.Count - 1) * itemButtonSpacing / 2;
        for (int i = 0; i < inventory.items.Count; i++)
        {
            inventoryItemButtons[i].gameObject.SetActive(true);
            inventoryItemButtons[i].transform.anchoredPosition = new Vector2(firstX + itemButtonSpacing * i, 0);
            inventoryItemButtons[i].SetItem(inventory.items[i]);
        }
        for (int i = inventory.items.Count; i < inventoryItemButtons.Count; i++)
        {
            inventoryItemButtons[i].gameObject.SetActive(false);
        }
        inventoryDisplay.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, itemButtonSpacing * inventory.items.Count);
    }
    
    void LateUpdate()
    {
        if(updateItemsAtEOF)
        {
            updateItemsAtEOF = false;
            UpdateEquipment();
            UpdateInventory();
        }
    }

    #region Button Callbacks
    public void UseInventoryItem(int buttonIndex)
    {
        if(buttonIndex >= inventory.items.Count || buttonIndex < 0)
        {
            Debug.LogError($"Attempted to use item at invalid index {buttonIndex}. Inventory size: {inventory.items.Count}");
            return;
        }
        bool validUse = inventory.items[buttonIndex].Use(stats, inventory);
        if(validUse) inventory.RemoveItem(buttonIndex, drop: false);
        updateItemsAtEOF = true;
    }

    public void UnequipHat()
    {
        HatSO hat = stats.UnequipHat();
        if(hat != null)
        {
            inventory.AddItem(hat);
            updateItemsAtEOF = true;
        }
    }

    public void UnequipShirt(int index)
    {
        if(index < 0 || index >= stats.equippedShirts.Count)
        {
            Debug.LogError($"Attempted to unequip shirt at invalid index {index}. Shirt count: {stats.equippedShirts.Count}");
            return;
        }
        ShirtSO shirt = stats.equippedShirts[index];
        inventory.AddItem(shirt);
        stats.UnequipShirt(shirt);
        updateItemsAtEOF = true;
    }

    public void UnequipPants(int index)
    {
        if (index < 0 || index >= stats.equippedPants.Count)
        {
            Debug.LogError($"Attempted to unequip pants at invalid index {index}. Pants count: {stats.equippedPants.Count}");
            return;
        }
        PantsSO pants = stats.equippedPants[index];
        inventory.AddItem(pants);
        stats.UnequipPants(pants);
        updateItemsAtEOF = true;
    }

    public void UnequipShoes()
    {
        ShoesSO shoes = stats.UnequipShoes();
        if (shoes != null)
        {
            inventory.AddItem(shoes);
            updateItemsAtEOF = true;
        }
    }
    #endregion
}
