﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCanvas : MonoBehaviour {


    public void CargaNivel(string nombreNivel)
    {
        SceneManager.LoadScene(nombreNivel);
    }

    public void CerrarJuego()
    {
     
        Application.Quit();
    }
}