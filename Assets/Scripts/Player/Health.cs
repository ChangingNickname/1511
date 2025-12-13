using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class Health : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private int maxHp = 100;

    public int MaxHp => maxHp;
    public int CurrentHp { get; private set; }
    public bool IsDead { get; private set; }

    // UI can subscribe to these
    public event Action<int, int> OnHpChanged; // current, max
    public event Action OnDied;
    public event Action OnRespawned;

    private void Awake()
    {
        CurrentHp = maxHp;
        IsDead = false;
        OnHpChanged?.Invoke(CurrentHp, maxHp);
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        CurrentHp = Mathf.Max(0, CurrentHp - amount);
        OnHpChanged?.Invoke(CurrentHp, maxHp);

        if (CurrentHp <= 0)
            Die();
    }

    public void Kill()
    {
        TakeDamage(maxHp);
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;
        Debug.Log($"{name} died.");
        OnDied?.Invoke();
        var rag = GetComponent<RagdollController>();
        if (rag != null) rag.DieRagdoll();
    }

    public void RespawnFull()
    {
        IsDead = false;
        CurrentHp = maxHp;
        OnHpChanged?.Invoke(CurrentHp, maxHp);
        OnRespawned?.Invoke();
    }
}
