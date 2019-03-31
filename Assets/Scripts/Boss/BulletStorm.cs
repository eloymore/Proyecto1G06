﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletStorm : Shooting
{
    public float deviation = 0.3f;
    public Transform[] shootingPoints;

    float[] shootCooldowns;

    private void OnEnable()
    {
        shootCooldowns = new float[shootingPoints.Length];
        for(int i = 0; i < shootingPoints.Length; i++)
        {
            shootCooldowns[i] = Random.Range(0f, 1 / rateOfFire);
        }
    }

    public void Shoot(int i)
    {
        ResetCooldown(i);
        BulletMovement newBullet = Instantiate<BulletMovement>(bulletPrefab, shootingPoints[i].position, Quaternion.identity, bulletPool);
        newBullet.Rotate(shootingPoints[i].right + new Vector3(Random.Range(-deviation / 2, deviation / 2), Random.Range(-deviation / 2, deviation / 2), 0));
    }

    /// <summary>
    /// Reduce shootCooldown cada segundo
    /// </summary>
    public override void Cooldown()
    {
        for (int i = 0; i < shootCooldowns.Length; i++)
        {
            if (shootCooldowns[i] > 0f)
                shootCooldowns[i] -= Time.deltaTime;        //Se reduce si no es 0
            else
            {
                shootCooldowns[i] = 0f;
                Shoot(i);
            }
        }
    }

    /// <summary>
    /// Pone el cooldown en su valor máximo
    /// </summary>
    public void ResetCooldown(int i)
    {
        shootCooldowns[i] = 1 / rateOfFire;
    }
}