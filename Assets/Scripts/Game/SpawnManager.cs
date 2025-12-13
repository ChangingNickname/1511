using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [Header("Respawn")]
    [SerializeField] private float respawnDelaySeconds = 2f;

    private SpawnPoint[] _spawnPoints;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Find spawn points in the scene
        _spawnPoints = FindObjectsOfType<SpawnPoint>();

        if (_spawnPoints == null || _spawnPoints.Length == 0)
        {
            Debug.LogError("SpawnManager: No SpawnPoint objects found in the scene.");
        }
    }

    public Transform GetRandomSpawn()
    {
        if (_spawnPoints == null || _spawnPoints.Length == 0) return null;
        int idx = Random.Range(0, _spawnPoints.Length);
        return _spawnPoints[idx].transform;
    }

    public void Respawn(GameObject playerRoot)
    {
        StartCoroutine(RespawnRoutine(playerRoot));
    }

    private IEnumerator RespawnRoutine(GameObject playerRoot)
    {
        // Small delay so death feels real / lets effects play
        yield return new WaitForSeconds(respawnDelaySeconds);

        Transform sp = GetRandomSpawn();
        if (sp == null) yield break;

        // Move the player
        var cc = playerRoot.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false; // important: prevents teleport issues

        playerRoot.transform.SetPositionAndRotation(sp.position, sp.rotation);

        if (cc != null) cc.enabled = true;

        // Reset velocity if Rigidbody-based (optional)
        var rb = playerRoot.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}

