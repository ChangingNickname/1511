using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;

public class CameraRespawnSnap : MonoBehaviour
{
    [SerializeField] private CinemachineBrain brain;
    [SerializeField] private CinemachineVirtualCamera vcam;

    private CinemachineBlendDefinition _oldBlend;

    private void Awake()
    {
        if (brain == null) brain = Camera.main?.GetComponent<CinemachineBrain>();
        if (brain != null) _oldBlend = brain.m_DefaultBlend;
    }

    public void Snap()
    {
        if (brain == null || vcam == null) return;
        StopAllCoroutines();
        StartCoroutine(SnapRoutine());
    }

    private IEnumerator SnapRoutine()
    {
        // force CUT so it doesn't blend from the old far position
        _oldBlend = brain.m_DefaultBlend;
        brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0f);

        vcam.PreviousStateIsValid = false;

        yield return null; // one frame

        brain.m_DefaultBlend = _oldBlend;
    }
}
