﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Coord
{
    public int x, y;

    public Coord(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public enum RoomMap { Nonexistant, NotSeen, NonVisited, Visited, Current }  //Nonexistant significa que no hay habitación
public enum ItemMap { None, Health, Item }  //Items en la habitación, no en uso

/// <summary>
/// Contiene el estado en el miimapa, los items que muestra el mismo y si es una habitación de boss.
/// </summary>
public struct RoomMapInfo
{
    public RoomMap vision;
    public ItemMap item;
    public bool boss;

    public RoomMapInfo(RoomMap vision, ItemMap item, bool boss)
    {
        this.vision = vision;
        this.item = item;
        this.boss = boss;
    }
}

public class Minimap : MonoBehaviour {

    public static Minimap instance;

    public int width, heigth;

    RoomMapInfo[,] map;

    /// <summary>
    /// Se crea el singleton
    /// Se hace un mapa de fil x col, y se incializa cada una
    /// </summary>
    private void Awake()
    {     
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        InitializeMap();
    }

    public void Start()
    {
        GameManager.instance.ui.UpdateMap(map);
    }

    /// <summary>
    /// Inicializa las propiedades de una habitación
    /// </summary>
    /// <param name="pos">La posición de la habitación</param>
    /// <param name="boss">Si es una habitación de boss</param>
    public void StoreRoom(Coord pos, bool boss)
    {
        map[pos.y, pos.x] = new RoomMapInfo(RoomMap.NotSeen, ItemMap.None, boss);
    }

    /// <summary>
    /// Inicializa la matriz del mapa
    /// </summary>
    public void InitializeMap()
    {
        map = new RoomMapInfo[heigth, width];

        for (int fila = 0; fila < heigth; fila++)
        {
            for (int col = 0; col < width; col++)
            {
                map[fila, col] = new RoomMapInfo(RoomMap.Nonexistant, ItemMap.None, false);
            }
        }
    }

    /// <summary>
    /// Actualiza el ítem que hay en una habitación
    /// </summary>
    /// <param name="pos">La posición de la habitación</param>
    /// <param name="item">El carácter que identifica al ítem</param>
    public void ItemExists(Coord pos, char item)
    {
        switch (item)
        {
            case 'i':
                map[pos.y, pos.x].item = ItemMap.Item;
                break;
            case 'h':
                map[pos.y, pos.x].item = ItemMap.Health;
                break;
            default:
                map[pos.y, pos.x].item = ItemMap.None;
                break;
        }
    }

    /// <summary>
    /// Actualiza el estado de la habitación
    /// </summary>
    /// <param name="pos">La posición de la habitación</param>
    void UpdateRoomState(Coord pos)
    {
        switch (map[pos.y, pos.x].vision)
        {
            case RoomMap.NotSeen:
                map[pos.y, pos.x].vision = RoomMap.NonVisited;
                break;
            case RoomMap.Current:
                map[pos.y, pos.x].vision = RoomMap.Visited;
                break;
        }
    }

    /// <summary>
    /// Comprueba las habitaciones adyacentes y les actualiza el estado.
    /// </summary>
    /// <param name="pos"></param>
    public void NewRoomExplored(Coord pos)
    {
        map[pos.y, pos.x].vision = RoomMap.Current;
        if (pos.x + 1 < map.GetLength(1)) UpdateRoomState(new Coord(pos.x + 1, pos.y));
        if (pos.y + 1 < map.GetLength(0)) UpdateRoomState(new Coord(pos.x, pos.y + 1));
        if (pos.x > 0) UpdateRoomState(new Coord(pos.x - 1, pos.y));
        if (pos.y > 0) UpdateRoomState(new Coord(pos.x, pos.y - 1));
        GameManager.instance.ui.UpdateMap(map);
    }
}