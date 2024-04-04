using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBossHealth : MonoBehaviour
{
    [SerializeField] private CombatTarget bossTarget;
    [SerializeField] private Slider healthSlider;

    private void Awake()
    {
        bossTarget.OnHit += _ => ChangeHealthVisual();
    }

    private void Start()
    {
        ChangeHealthVisual();
    }

    private void ChangeHealthVisual()
    {
        healthSlider.value = ((float)bossTarget.health) / ((float)bossTarget.GetStats().maxHealth);
    }
}
