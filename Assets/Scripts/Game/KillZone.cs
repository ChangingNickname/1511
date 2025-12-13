using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // If the collider is a child, GetComponentInParent finds Health on the root
        var hp = other.GetComponentInParent<Health>();
        if (hp != null)
        {
            hp.Kill();
        }
    }
}
