﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : Shooting {

    public float baseDamage = 2;
    public float damageMultiplier = 1;
    public BulletMovement[] possibleBullets;
    //Weapons weapon; //Arma, de momento sin usar

    public void Start()
    {
        GameManager.instance.ui.UpdateDamage(baseDamage);
        //weapon = Weapons.Default;
    }

    /// <summary>
    /// Apunta hacia el ratón y dispara al hacer Fire1
    /// </summary>
    public override void Update() 
    {
        Vector2 lookDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;   //Vector entre el ratón y el jugador
        float angle = Mathf.Atan(lookDirection.y / lookDirection.x) * (180 / Mathf.PI);

        transform.eulerAngles = new Vector3(0, 0, angle + (lookDirection.x < 0f ? 180f : 0f));              //Arctan se queda con el ángulo más pequeño y utilizamos un offset para corregirlo

        if (Input.GetButton("Fire1") && shootCooldown == 0f)        //Dispara al hacer clic
            Shoot();

        // Se comprueba el shootCooldown a la hora de disparar
        base.Update();
    }

    /// <summary>
    /// Dispara una bala
    /// </summary>
    public override void Shoot()
    {
        ResetCooldown();
        BulletMovement newBullet = Instantiate<BulletMovement>(bulletPrefab, shootingPoint.position, Quaternion.identity, bulletPool);
        newBullet.GetComponent<PlayerBullet>().Damage = baseDamage * damageMultiplier;
        newBullet.Rotate(transform.right);
    }

    /// <summary>
    /// Añade amount de daño base
    /// </summary>
    public void AddDamage(float amount)
    {
        baseDamage += amount;
        GameManager.instance.ui.UpdateDamage(baseDamage * damageMultiplier);
    }

    /// <summary>
    /// Cambia la bala según el nuevo efecto.
    /// </summary>
    public void ChangeEffect(BulletEffects effect, ItemData data)
    {
        bulletPrefab = possibleBullets[(int)effect];
        GameManager.instance.ui.UpdateEffect(data.sprite);
    }

    public void ChangeWeapon(Weapons weaponNew, ItemData data)
    {
        UIManager ui = GameManager.instance.ui;
        switch (weaponNew)
        {
            case (Weapons.Default):
                rateOfFire = 0.5f;
                damageMultiplier = 1f;
                break;
            case (Weapons.LenteConvergente):
                rateOfFire = 0.75f;
                damageMultiplier = 2.5f;
                break;
            case (Weapons.ReactorCinético):
                rateOfFire = 5f;
                damageMultiplier = 0.75f;
                break;
        }

        ui.UpdateDamage(baseDamage * damageMultiplier);
        ui.UpdateWeapon(data.sprite);

        //weapon = weaponNew;
    }
}
