using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;


public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [Header("Respawn")]
    [SerializeField] private float respawnDelaySeconds = 2f;

    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private CinemachineBrain brain;

    private SpawnPoint[] _spawnPoints;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _spawnPoints = FindObjectsOfType<SpawnPoint>();
        if (_spawnPoints.Length == 0)
            Debug.LogError("SpawnManager: No SpawnPoints found.");
    }

    private Transform GetRandomSpawn()
    {
        return _spawnPoints[Random.Range(0, _spawnPoints.Length)].transform;
    }

    // === PUBLIC API ===

    public void Respawn(GameObject playerRoot)
    {
        StopAllCoroutines();
        StartCoroutine(RespawnRoutine(playerRoot, respawnDelaySeconds));
    }

    public void RespawnNow(GameObject playerRoot)
    {
        StopAllCoroutines();
        StartCoroutine(RespawnRoutine(playerRoot, 0f));
    }

    // === CORE LOGIC ===

    private IEnumerator RespawnRoutine(GameObject playerRoot, float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        Transform sp = GetRandomSpawn();
        if (sp == null) yield break;

        var rag = playerRoot.GetComponent<RagdollController>();
        var cc  = playerRoot.GetComponent<CharacterController>();

        // Disable controller before teleport
        if (cc != null) cc.enabled = false;

        // Handle ragdoll reset OR normal teleport
        if (rag != null)
            rag.ResetFromRagdoll(sp.position, sp.rotation);
        else
            playerRoot.transform.SetPositionAndRotation(sp.position, sp.rotation);

        // Re-enable controller
        if (cc != null) cc.enabled = true;

        // === CINEMACHINE HARD RESET ===
        if (vcam != null)
            vcam.PreviousStateIsValid = false;

        if (brain != null)
        {
            var oldBlend = brain.m_DefaultBlend;
            brain.m_DefaultBlend =
                new CinemachineBlendDefinition(
                    CinemachineBlendDefinition.Style.Cut, 0f);

            yield return null; // one frame

            brain.m_DefaultBlend = oldBlend;
        }
    }
}
