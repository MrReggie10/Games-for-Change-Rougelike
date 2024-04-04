using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIPlayerHealth : MonoBehaviour
{
    [SerializeField] private CombatTarget playerTarget;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Slider healthSlider;

    private void Awake()
    {
        playerTarget.OnHit += _ => ChangeHealthVisual();
    }

    private void Start()
    {
        ChangeHealthVisual();
    }

    private void ChangeHealthVisual()
    {
        healthText.SetText(playerTarget.health.ToString() + "/" + playerTarget.GetStats().maxHealth.ToString());
        healthSlider.value = ((float) playerTarget.health) / ((float) playerTarget.GetStats().maxHealth);
    }
}
