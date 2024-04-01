using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GFC.Items;

[RequireComponent(typeof(MeleeAttack), typeof(Movement), typeof(ItemUser))]
[RequireComponent(typeof(PlayerInventory), typeof(PlayerStats))]
public class PlayerController : MonoBehaviour
{
    private MeleeAttack attack;
    private Movement movement;
    private ItemUser itemUser;
    private PlayerInventory inventory;
    private PlayerStats stats;

    [SerializeField] InventoryMenuManager inventoryMenu;
    [SerializeField] GameObject pickupIndicator;

    private bool attackOnCooldown;
    private bool dashOnCooldown;

    private int itemSelectIndex;

    void Awake()
    {
        attack = GetComponent<MeleeAttack>();
        movement = GetComponent<Movement>();
        itemUser = GetComponent<ItemUser>();
        itemUser.OnItemApproach += RegisterItemNearby;
        itemUser.OnItemLeave += UnregisterItemNearby;

        inventory = GetComponent<PlayerInventory>();
        stats = GetComponent<PlayerStats>();
        attackOnCooldown = false;
        dashOnCooldown = false;
        if (inventoryMenu)
            inventoryMenu.Close();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (inventoryMenu.enabled)
            {
                inventoryMenu.Close();
                Time.timeScale = 1;
            }
            else
            {
                inventoryMenu.Open();
                Time.timeScale = 0;
            }
        }
        if(Input.GetKeyDown(KeyCode.F) && itemUser.dropsInRange.Count != 0)
        {
            PickupItem();
        }
        if (inventoryMenu != null && inventoryMenu.enabled)
            return;
        if (!attack.attacking)
        {
            attack.AimAt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
        if(Input.GetMouseButtonDown(0) && !attackOnCooldown)
        {
            attack.Attack();
            StartCoroutine(AttackCooldown());
        }
        movement.SetInput(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
        if (Input.GetKeyDown(KeyCode.Space) && !dashOnCooldown)
        {
            movement.Dash();
            StartCoroutine(DashCooldown());
        }
        if(Input.GetKey(KeyCode.LeftShift))
        {
            movement.sprinting = true;
        }
        else
        {
            movement.sprinting = false;
        }
    }

    private void RegisterItemNearby(ItemDrop item)
    {
        pickupIndicator.SetActive(true);
    }

    private void UnregisterItemNearby(ItemDrop item)
    {
        if(itemUser.dropsInRange.Count == 0)
        {
            pickupIndicator.SetActive(false);
        }
        else if(itemUser.dropsInRange.Count >= itemSelectIndex)
        {
            itemSelectIndex = itemUser.dropsInRange.Count - 1;
        }
    }

    private void PickupItem()
    {
        if (inventory.items.Count >= inventory.capacity)
        {
            //TODO: Make it so picking up an item with a full inventory opens the inventory and prompts you to either wear it or make space for it. Or at least tell the player "inventory full"
            Debug.Log("Inventory Full");
            return;
        }
        Debug.Log($"Picking up item {itemSelectIndex}");
        ItemSO item = itemUser.Pickup(itemSelectIndex);
        inventory.AddItem(item);
    }

    private IEnumerator AttackCooldown()
    {
        attackOnCooldown = true;
        yield return new WaitForSeconds(stats.attackCooldown);
        attackOnCooldown = false;
    }

    private IEnumerator DashCooldown()
    {
        dashOnCooldown = true;
        yield return new WaitForSeconds(stats.dashCooldown);
        dashOnCooldown = false;
    }
}
