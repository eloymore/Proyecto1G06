﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPointDronB : MonoBehaviour {

    public Transform Amigo;
    public float distance;

    Transform player;

    private void Start()
    {
        player = GameManager.instance.player.transform;
    }

    void Update ()
    {
        if(Amigo != null)
        {
            Vector3 playerAmigo = player.position - Amigo.position;
            transform.position = Amigo.position + (playerAmigo / playerAmigo.magnitude) * distance;
        }
        else
        {
            this.enabled = false;
        }
    }
}
