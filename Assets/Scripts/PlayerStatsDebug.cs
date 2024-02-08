using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GFC.Items;

public class PlayerStatsDebug : MonoBehaviour
{
    enum ClothingAction { Add, Remove, Get, Set }
    PlayerStats player;

    [SerializeField] HatSO hat;
    [SerializeField] List<ShirtSO> shirts;
    [SerializeField] List<PantsSO> pants;
    [SerializeField] ShoesSO shoes;
    [Space]
    [SerializeField] ClothingAction action;

    void Start()
    {
        player = GetComponent<PlayerStats>();
        player.OnEquipClothing += (_, clothing) => Debug.Log($"Equipped clothing of type {clothing.type} named {clothing.name}.");
        player.OnUnequipClothing += (_, clothing) => Debug.Log($"Unequipped clothing of type {clothing.type} named {clothing.name}.");
        player.OnAttackChange += player => Debug.Log($"Player's attack changed to {player.attack}");
        player.OnDefenseChange += player => Debug.Log($"Player's defense changed to {player.defense}");
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            //hat
            switch (action)
            {
                case ClothingAction.Add:
                    player.EquipHat(hat);
                    break;
                case ClothingAction.Remove:
                    player.UnequipHat();
                    break;
                case ClothingAction.Get:
                    hat = player.equippedHat;
                    break;
                case ClothingAction.Set:
                    goto case ClothingAction.Add;
            }
        }
        if(Input.GetKeyDown(KeyCode.W))
        {
            //shirts
            switch(action)
            {
                case ClothingAction.Add:
                    foreach (ShirtSO shirt in shirts)
                        player.EquipShirt(shirt);
                    break;
                case ClothingAction.Remove:
                    foreach (ShirtSO shirt in shirts)
                        if (!player.UnequipShirt(shirt)) Debug.Log($"Shirt {shirt.name} not in list.");
                    break;
                case ClothingAction.Get:
                    shirts = new List<ShirtSO>(player.equippedShirts);
                    break;
                case ClothingAction.Set:
                    bool[] used = new bool[shirts.Count];
                    int reps = 0;
                    for(int i = 0; i < player.equippedShirts.Count && reps < 100; i++, reps++)
                    {
                        int index = shirts.FindIndex(shirt => shirt == player.equippedShirts[i]);
                        bool wasInList = false;
                        int a = 0;
                        while(index >= 0)
                        {
                            a++;
                            if(a >= 100)
                            {
                                Debug.LogError("Infinite loop detected while setting shirts (searching phase). Operation cancelled");
                                return;
                            }
                            if(used[index])
                            {
                                index = shirts.FindIndex(index+1, shirt => shirt == player.equippedShirts[i]);
                                continue;
                            }
                            else
                            {
                                used[index] = true;
                                wasInList = true;
                                break;
                            }
                        }
                        if (!wasInList)
                        {
                            player.UnequipShirt(player.equippedShirts[i]);
                            i--;
                        }
                    }
                    if(reps == 100)
                    {
                        Debug.LogError("Infinite loop detected while setting shirts. Operation cancelled");
                        return;
                    }
                    for(int i = 0; i < shirts.Count; i++)
                    {
                        if (used[i])
                            continue;
                        player.EquipShirt(shirts[i]);
                    }
                    break;
            }
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            //pants
            switch (action)
            {
                case ClothingAction.Add:
                    foreach (PantsSO pant in pants)
                        player.EquipPants(pant);
                    break;
                case ClothingAction.Remove:
                    foreach (PantsSO pant in pants)
                        if (!player.UnequipPants(pant)) Debug.Log($"Pants {pant.name} not in list.");
                    break;
                case ClothingAction.Get:
                    pants = new List<PantsSO>(player.equippedPants);
                    break;
                case ClothingAction.Set:
                    bool[] used = new bool[pants.Count];
                    int reps = 0;
                    for (int i = 0; i < player.equippedPants.Count && reps < 100; i++, reps++)
                    {
                        int index = pants.FindIndex(pants => pants == player.equippedPants[i]);
                        bool wasInList = false;
                        int a = 0;
                        while (index >= 0)
                        {
                            a++;
                            if (a >= 100)
                            {
                                Debug.LogError("Infinite loop detected while setting pants (searching phase). Operation cancelled");
                                return;
                            }
                            if (used[index])
                            {
                                index = pants.FindIndex(index+1, pants => pants == player.equippedPants[i]);
                                continue;
                            }
                            else
                            {
                                used[index] = true;
                                wasInList = true;
                                break;
                            }
                        }
                        if (!wasInList)
                        {
                            player.UnequipPants(player.equippedPants[i]);
                            i--;
                        }
                    }
                    if (reps == 100)
                    {
                        Debug.LogError("Infinite loop detected while setting pants. Operation cancelled");
                        return;
                    }
                    for (int i = 0; i < pants.Count; i++)
                    {
                        if (used[i])
                            continue;
                        player.EquipPants(pants[i]);
                    }
                    break;
            }
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            //shoes
            switch(action)
            {
                case ClothingAction.Add:
                    player.EquipShoes(shoes);
                    break;
                case ClothingAction.Remove:
                    player.UnequipShoes();
                    break;
                case ClothingAction.Get:
                    shoes = player.equippedShoes;
                    break;
                case ClothingAction.Set:
                    goto case ClothingAction.Add;
            }
        }
    }
}
