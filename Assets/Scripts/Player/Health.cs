using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private int maxHp = 100;
    [SerializeField] private bool respawnOnDeath = true;

    public int CurrentHp { get; private set; }
    public bool IsDead { get; private set; }

    private void Awake()
    {
        CurrentHp = maxHp;
        IsDead = false;
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        CurrentHp = Mathf.Max(0, CurrentHp - amount);

        if (CurrentHp <= 0)
            Die();
    }

    public void Kill()
    {
        if (IsDead) return;
        CurrentHp = 0;
        Die();
    }

    private void Die()
    {
        IsDead = true;

        // You can later replace this with animation / ragdoll / UI.
        Debug.Log($"{name} died.");

        if (respawnOnDeath)
        {
            // Reset HP now (or after respawn delay — your choice)
            CurrentHp = maxHp;
            IsDead = false;

            if (SpawnManager.Instance != null)
                SpawnManager.Instance.Respawn(gameObject);
            else
                Debug.LogError("No SpawnManager in scene.");
        }
    }
}
