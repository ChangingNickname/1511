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
    [SerializeField] private CinemachineVirtualCamera vcam;   // drag PlayerFollowCamera here
    [SerializeField] private CinemachineBrain brain;          // drag MainCamera's CinemachineBrain here (if present)
    [SerializeField] private CameraRespawnSnap cameraSnapper;



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
        StopAllCoroutines();
        StartCoroutine(RespawnRoutine(playerRoot, respawnDelaySeconds));
    }

    private IEnumerator RespawnRoutine(GameObject playerRoot, float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        Transform sp = GetRandomSpawn();
        if (sp == null) yield break;

        var cc = playerRoot.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        playerRoot.transform.SetPositionAndRotation(sp.position, sp.rotation);

        // after teleport
        if (vcam != null)
        {
            vcam.PreviousStateIsValid = false;
        }

        var brain = Camera.main != null ? Camera.main.GetComponent<Cinemachine.CinemachineBrain>() : null;
        if (brain != null)
        {
            // Force a cut this frame so it doesn't blend from old position
            var oldBlend = brain.m_DefaultBlend;
            brain.m_DefaultBlend = new Cinemachine.CinemachineBlendDefinition(Cinemachine.CinemachineBlendDefinition.Style.Cut, 0f);

            // restore next frame
            StartCoroutine(RestoreBlendNextFrame(brain, oldBlend));
        }


        if (cc != null) cc.enabled = true;

        // // IMPORTANT: do your camera snap/reset HERE (right after teleport)
        // cameraSnapper?.Snap(); // if you added the CameraRespawnSnap script
    }

    public void RespawnNow(GameObject playerRoot)
    {
        StopAllCoroutines();
        StartCoroutine(RespawnRoutine(playerRoot, 0f));
    }

    private IEnumerator RestoreBlendNextFrame(Cinemachine.CinemachineBrain brain, Cinemachine.CinemachineBlendDefinition oldBlend)
    {
        yield return null;
        if (brain != null) brain.m_DefaultBlend = oldBlend;
    }




    
}

