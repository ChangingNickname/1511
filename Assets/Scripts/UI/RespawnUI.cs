using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;

public class RespawnUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health targetHealth;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private Button respawnButton;
    [SerializeField] private GameObject playerRoot;


    [Header("Disable on death (optional)")]
    [SerializeField] private MonoBehaviour[] disableWhenDead; // drag ThirdPersonController etc.

    private void Awake()
    {
        // Button click hookup
        if (respawnButton != null)
            respawnButton.onClick.AddListener(OnRespawnClicked);
    }

    private void OnEnable()
    {
        if (respawnButton != null) respawnButton.gameObject.SetActive(false); // force hide on start
        SetCursorForUI(false); // start in gameplay mode
        HookHealth(true);
        RefreshUI();
    }

    private void OnDisable()
    {
        HookHealth(false);
    }

    private void HookHealth(bool hook)
    {
        if (targetHealth == null) return;

        if (hook)
        {
            targetHealth.OnHpChanged += HandleHpChanged;
            targetHealth.OnDied += HandleDied;
            targetHealth.OnRespawned += HandleRespawned;
        }
        else
        {
            targetHealth.OnHpChanged -= HandleHpChanged;
            targetHealth.OnDied -= HandleDied;
            targetHealth.OnRespawned -= HandleRespawned;
        }
    }

    public void SetTarget(Health h)
    {
        HookHealth(false);
        targetHealth = h;
        HookHealth(true);
        RefreshUI();
    }

    private void HandleHpChanged(int current, int max)
    {
        if (hpText == null) return;

        if (targetHealth != null && targetHealth.IsDead)
            hpText.text = "DEAD";
        else
            hpText.text = $"HP: {current}";
    }

    private void HandleDied()
    {
        if (hpText != null) hpText.text = "DEAD";
        if (respawnButton != null) respawnButton.gameObject.SetActive(true);

        SetControlEnabled(false);
        SetCursorForUI(true);

    }

    private void HandleRespawned()
    {
        if (respawnButton != null) respawnButton.gameObject.SetActive(false);
        SetControlEnabled(true);
        SetCursorForUI(false);
        RefreshUI();
    }


    private void RefreshUI()
    {
        if (targetHealth == null || hpText == null) return;

        if (targetHealth.IsDead)
        {
            hpText.text = "DEAD";
            if (respawnButton != null) respawnButton.gameObject.SetActive(true);
            SetControlEnabled(false);
        }
        else
        {
            hpText.text = $"HP: {targetHealth.CurrentHp}";
            if (respawnButton != null) respawnButton.gameObject.SetActive(false);
            SetControlEnabled(true);
        }
    }

    private void SetControlEnabled(bool enabled)
    {
        if (disableWhenDead == null) return;
        foreach (var b in disableWhenDead)
            if (b != null) b.enabled = enabled;
    }

    private void OnRespawnClicked()
    {
        if (targetHealth == null || !targetHealth.IsDead) return;

        // Hide UI first
        if (respawnButton != null) respawnButton.gameObject.SetActive(false);

        // Re-enable control scripts
        SetControlEnabled(true);

        // IMPORTANT: lock cursor immediately so you return to gameplay mode
        SetCursorForUI(false);

        // Move player + restore HP
        if (SpawnManager.Instance != null)
            SpawnManager.Instance.Respawn(playerRoot); // make sure this is the player object

        targetHealth.RespawnFull();
    }


    private void SetCursorForUI(bool uiMode)
    {
        Cursor.visible = uiMode;
        Cursor.lockState = uiMode ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
