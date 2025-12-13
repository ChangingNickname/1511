using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTester : MonoBehaviour
{
    private Health _hp;

    private void Awake() => _hp = GetComponent<Health>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) _hp?.Kill();
        if (Input.GetKeyDown(KeyCode.J)) _hp?.TakeDamage(25);
    }
}
