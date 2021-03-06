﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class NextLevelTrigger : MonoBehaviour {

    public string sceneName;

    Interactable interactable;

    private void Awake()
    {
        interactable = GetComponent<Interactable>();
    }

    /// <summary>
    /// Recarga la escena al entrar el jugador
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == GameManager.instance.player && interactable.interactable)
        {
            GameManager.instance.LoadScene(sceneName);
        }
    }
}
