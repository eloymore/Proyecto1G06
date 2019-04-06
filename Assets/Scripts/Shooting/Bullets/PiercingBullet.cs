﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiercingBullet : PlayerBullet {

    /// <summary>
    /// Hace daño al enemigo. Se destruye
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(Damage);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Environment"))
            Destroy(gameObject);

        Damage *= 0.8f;
    }
}
