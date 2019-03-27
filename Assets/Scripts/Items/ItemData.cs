﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : MonoBehaviour {

    public char tier;
    public char Tier { get { return char.ToUpper(tier); } }
    public Sprite sprite { get { return GetComponent<SpriteRenderer>().sprite; } }
    public IItem[] effects { get { return GetComponents<IItem>(); } }

    /// <summary>
    /// Al chocarse con el jugador, recoger el item
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null)
        {
            for (int i = 0; i < effects.Length; i++)
                effects[i].PickEffect();
            transform.parent = ItemManager.instance.transform;
            transform.GetComponent<BoxCollider2D>().enabled = false;    //Desactiva los componentes innecesarios.
            transform.GetComponent<SpriteRenderer>().enabled = false;
            ItemManager.instance.AddItem(this);
        }
    }
}