using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform ragdollRoot; // usually hips/pelvis

    private Rigidbody[] ragdollBodies;
    private Collider[] ragdollColliders;

    private Vector3 lastVelocity;

    private void Awake()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (characterController == null) characterController = GetComponent<CharacterController>();

        ragdollBodies = GetComponentsInChildren<Rigidbody>(true);
        ragdollColliders = GetComponentsInChildren<Collider>(true);

        SetRagdollEnabled(false);
    }

    private void Update()
    {
        // CharacterController velocity is good enough for “carry momentum”
        if (characterController != null)
            lastVelocity = characterController.velocity;
        else
            lastVelocity = Vector3.zero;
    }

    public void DieRagdoll()
    {
        // Disable control
        if (characterController != null) characterController.enabled = false;
        if (animator != null) animator.enabled = false;

        // Enable ragdoll physics
        SetRagdollEnabled(true);

        // Push momentum into the ragdoll
        ApplyMomentum(lastVelocity);
    }

    public void ResetFromRagdoll(Vector3 position, Quaternion rotation)
    {
        // Turn off ragdoll physics first
        SetRagdollEnabled(false);

        // Move player root to spawn
        transform.SetPositionAndRotation(position, rotation);

        // Re-enable animation + controller
        if (animator != null) animator.enabled = true;
        if (characterController != null) characterController.enabled = true;
    }

    private void SetRagdollEnabled(bool enabled)
    {
        // IMPORTANT: don’t affect the CharacterController’s own collider
        foreach (var rb in ragdollBodies)
        {
            if (rb == null) continue;
            rb.isKinematic = !enabled;
            rb.detectCollisions = enabled;
        }

        foreach (var col in ragdollColliders)
        {
            if (col == null) continue;
            // Don’t disable CharacterController collider if it’s included
            if (characterController != null && col == characterController) continue;
            col.enabled = enabled;
        }
    }

    private void ApplyMomentum(Vector3 velocity)
    {
        // Give all bodies the same starting velocity (GMod-ish)
        foreach (var rb in ragdollBodies)
        {
            if (rb == null || rb.isKinematic) continue;
            rb.velocity = velocity;
        }

        // Optional: a little extra shove to hips for drama
        if (ragdollRoot != null)
        {
            var hips = ragdollRoot.GetComponent<Rigidbody>();
            if (hips != null && !hips.isKinematic)
                hips.AddForce(velocity * 15f, ForceMode.Impulse);
        }
    }
}
