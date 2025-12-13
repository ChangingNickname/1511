using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class HpUI : MonoBehaviour
{
    [SerializeField] private Health targetHealth;
    [SerializeField] private TMP_Text hpText;

    private void Reset()
    {
        hpText = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (targetHealth == null || hpText == null) return;
        hpText.text = $"HP: {targetHealth.CurrentHp}";
    }

    // Call this once when the local player is known (later for multiplayer)
    public void SetTarget(Health health)
    {
        targetHealth = health;
    }
}
