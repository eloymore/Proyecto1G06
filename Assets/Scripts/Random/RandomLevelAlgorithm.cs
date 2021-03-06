﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomLevelAlgorithm : MonoBehaviour
{
    [Range(2, 200)]public int numberOfRooms = 15;
    [Range(0f, 1f)]public float baseChanceToClose = 0.6f;
    [Range(0f, 1f)]public float distanceModifierToClose = 0.03f;
    [Range(0, 6)]public int numberOfItems = 3;
    public bool bossDropItem = true;
    [Range(0, 8)]public int numberOfHealth = 3;
    [Range(0, 3)]public int droppedHealthByboss = 2;
    public enum RoomBossEntrance { Up, Down, Left, Right }
    public RoomBossEntrance roomBossEntrance = RoomBossEntrance.Up; 

    List<Room> rooms;
    List<Vector2Int> availablePositions;
    List<Room> unselectableRoomsByBoss;
    List<Vector2Int> items;
    List<Vector2Int> health;

    private void Start()
    {
        CreateMap();
        Vector2Int mapSize = GetMapSize();
        Minimap.instance.InitializeMap(mapSize);
        Vector2Int minimapOffset = MinimapOffset();
        foreach(Room room in rooms)
        {
            // Hay que hacer un offset para que las habitaciones quepan en el minimapa
            Minimap.instance.StoreRoom(room.position + minimapOffset, room.boss);
        }
        Minimap.instance.UpdateMapUI();
        GetComponent<RandomRoomPlacer>().PlaceRooms(rooms, minimapOffset, items, health);
    }

    /// <summary>
    /// Crea un mapa con las propiedades de la instancia de la clase
    /// </summary>
    public void CreateMap()
    {
        availablePositions = new List<Vector2Int>();
        InsertBaseRooms();
        int i = 2;
        while(i < numberOfRooms)
        {
            if (InsertRoom()) i++;
        }

        unselectableRoomsByBoss = new List<Room>();
        bool insertedRoomBoss = false;
        do { insertedRoomBoss = InsertBossRoom(); }
        while (!insertedRoomBoss);

        items = new List<Vector2Int>();
        health = new List<Vector2Int>();
        PlaceItems();
        PlaceHealth();
    }

    void PlaceItems()
    {
        int i = 0;
        while (i < numberOfItems)
        {
            Room randomRoom = rooms[Random.Range(1, rooms.Count - 1)];
            if (!items.Contains(randomRoom.position) && !health.Contains(randomRoom.position))
            {
                items.Add(randomRoom.position);
                i++;
            }
        }

        if (bossDropItem)
        {
            items.Add(rooms[rooms.Count - 1].position);
        }
    }

    void PlaceHealth()
    {
        int i = 0;
        while (i < numberOfHealth)
        {
            Room randomRoom = rooms[Random.Range(1, rooms.Count - 1)];
            if (!items.Contains(randomRoom.position) && !health.Contains(randomRoom.position))
            {
                health.Add(randomRoom.position);
                i++;
            }
        }

        for (i = 0; i < droppedHealthByboss; i++)
        {
            health.Add(rooms[rooms.Count - 1].position);
        }
    }

    /// <summary>
    /// Devuelve el tamaño del mapa creado
    /// </summary>
    public Vector2Int GetMapSize()
    {
        if (rooms.Count == 0) return new Vector2Int(-1, -1);
        int maxX = 0, maxY = 0, minX = 0, minY = 0;
        foreach (Room room in rooms)
        {
            if (room.position.x > maxX) maxX = room.position.x;
            else if (room.position.x < minX) minX = room.position.x;
            if (room.position.y > maxY) maxY = room.position.y;
            else if (room.position.y < minY) minY = room.position.y;
        }

        return new Vector2Int(Mathf.Abs(minX) + maxX + 1, Mathf.Abs(minY) + maxY + 1);
    }

    /// <summary>
    /// Offset para que en el minimapa no hayan posiciones negativas
    /// </summary>
    Vector2Int MinimapOffset()
    {
        if (rooms.Count == 0) return new Vector2Int(0, 0);
        int minX = 0, minY = 0;

        foreach (Room room in rooms)
        {
            if (room.position.x < minX) minX = room.position.x;
            if (room.position.y < minY) minY = room.position.y;
        }

        return new Vector2Int(Mathf.Abs(minX), Mathf.Abs(minY));
    }

    /// <summary>
    /// Coloca la sala inicial del nivel y la siguiente, a su derecha
    /// </summary>
    void InsertBaseRooms()
    {
        rooms = new List<Room>();
        Room startingRoom = new Room(0, 0, false);
        Room nextRoom = new Room(1, 0, false);
        ConnectRooms(ref startingRoom, ref nextRoom);
        rooms.Add(startingRoom);
        rooms.Add(nextRoom);
        AddAdjacentPositions(nextRoom.position);
    }

    /// <summary>
    /// Conecta dos habitaciones entre sí si son adyacentes
    /// </summary>
    /// <returns>true si ha podido, false si no</returns>
    bool ConnectRooms(ref Room room1, ref Room room2)
    {
        if (room1 == null || room2 == null) return false;

        if (room1.position.x + 1 == room2.position.x && room1.position.y == room2.position.y)   //room2 a la derecha de room1
        {
            room1.rightDoor = true;
            room2.leftDoor = true;
        } else if(room1.position.x - 1 == room2.position.x && room1.position.y == room2.position.y) //room2 a la izquierda de room1
        {
            room1.leftDoor = true;
            room2.rightDoor = true;
        } else if(room1.position.x == room2.position.x && room1.position.y + 1 == room2.position.y) //room2 encima de room1
        {
            room1.upDoor = true;
            room2.downDoor = true;
        } else if(room1.position.x == room2.position.x && room1.position.y - 1 == room2.position.y) //room2 debajo de room1
        {
            room1.downDoor = true;
            room2.upDoor = true;
        } else  // No adyacentes
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Conecta la habitación room con todas sus adyacentes de rooms
    /// </summary>
    void ConnectRoomToAdjacent(ref Room room)
    {
        Vector2Int roomPos = room.position;
        Room adjacent = rooms.Find(x => x.position.x - 1 == roomPos.x && x.position.y == roomPos.y);
        if (adjacent != null)
        {
            ConnectRooms(ref adjacent, ref room);
        }
        adjacent = rooms.Find(x => x.position.x + 1 == roomPos.x && x.position.y == roomPos.y);
        if (adjacent != null)
        {
            ConnectRooms(ref adjacent, ref room);
        }
        adjacent = rooms.Find(x => x.position.x == roomPos.x && x.position.y - 1 == roomPos.y);
        if (adjacent != null)
        {
            ConnectRooms(ref adjacent, ref room);
        }
        adjacent = rooms.Find(x => x.position.x == roomPos.x && x.position.y + 1 == roomPos.y);
        if (adjacent != null)
        {
            ConnectRooms(ref adjacent, ref room);
        }
    }

    /// <summary>
    /// Comprueba si la posición pos es válida (no tiene más de una habitación adyacente o es una habitación ya)
    /// </summary>
    bool CheckIfValidPosition(Vector2Int pos)
    {
        if(rooms.Contains(new Room(pos.x, pos.y, false))) return false;

        int adjacentRooms = 0;
        if(rooms.Contains(new Room(pos.x + 1, pos.y, false)))
        {
            adjacentRooms++;
        }
        if(rooms.Contains(new Room(pos.x - 1, pos.y, false)))
        {
            adjacentRooms++;
        }
        if(rooms.Contains(new Room(pos.x, pos.y + 1, false)))
        {
            adjacentRooms++;
        }
        if(rooms.Contains(new Room(pos.x, pos.y - 1, false)))
        {
            adjacentRooms++;
        }

        return adjacentRooms <= 1;
    }

    /// <summary>
    /// Añade las posiciones adyacentes a from si son válidas, a availablePositions
    /// </summary>
    void AddAdjacentPositions(Vector2Int from)
    {
        Vector2Int adjacent = new Vector2Int(from.x + 1, from.y);
        if (CheckIfValidPosition(adjacent)) availablePositions.Add(adjacent);
        adjacent = new Vector2Int(from.x - 1, from.y);
        if(CheckIfValidPosition(adjacent)) availablePositions.Add(adjacent);
        adjacent = new Vector2Int(from.x, from.y + 1);
        if(CheckIfValidPosition(adjacent)) availablePositions.Add(adjacent);
        adjacent = new Vector2Int(from.x, from.y - 1);
        if(CheckIfValidPosition(adjacent)) availablePositions.Add(adjacent);
    }

    /// <summary>
    /// Cierra el camino pos según la probabilidad de qeuse cierre
    /// </summary>
    /// <returns>true si se cierra, false si no</returns>
    bool ClosePath(Vector2Int pos)
    {
        float rng = Random.Range(0f, 1f);
        return rng < baseChanceToClose + (pos.x + pos.y) * distanceModifierToClose;
    }

    /// <summary>
    /// Abre los caminos adyacentes de una habitación de rooms
    /// </summary>
    /// <returns>true si ha abierto al menos 1, false si no</returns>
    bool OpenRandomPath()
    {
        Vector2Int pos = rooms[Random.Range(0, rooms.Count)].position;
        int openPaths = availablePositions.Count;
        AddAdjacentPositions(pos);
        return availablePositions.Count > openPaths;
    }

    /// <summary>
    /// Crea una habitación en la primera posición libre de availablePositions si puede
    /// </summary>
    /// <returns>true si la crea, false si no</returns>
    bool InsertRoom()
    {
        bool insertedRoom = false;
        Vector2Int pos;

        if (availablePositions.Count <= 0)
        {
            bool pathOpened = false;
            do
            {
                pathOpened = OpenRandomPath();
            } while (!pathOpened);
        }

        pos = availablePositions[0];
        // Solo cierra un camino si el número de posibilidades para avanzar es mayor de 1
        if (CheckIfValidPosition(pos) && (availablePositions.Count < 2 || !ClosePath(pos)))
        {
            PlaceRoom(pos, false);
            AddAdjacentPositions(pos);
            insertedRoom = true;
        }

        availablePositions.RemoveAt(0); // La posición se ha comprobado ya, se elimina
        return insertedRoom;
    }

    /// <summary>
    /// Coloca una habitación en la posición pos y la conecta a sus adyacentes
    /// </summary>
    private void PlaceRoom(Vector2Int pos, bool boss)
    {
        Room room = new Room(pos, boss);
        ConnectRoomToAdjacent(ref room);
        rooms.Add(room);
    }

    /// <summary>
    /// Coloca la habitación del boss en la posición más distante de la inicial posible con la dirección dada
    /// </summary>
    /// <returns>true si la crea, false si no</returns>
    bool InsertBossRoom()
    {
        bool insertedRoom = false;
        int maxDistanceToCenter = 0;
        Room mostDistantRoom = rooms[0];

        // Busca la habitación más alejada de la inicial
        foreach(Room room in rooms)
        {
            if (!unselectableRoomsByBoss.Contains(room) && Mathf.Abs(room.distanceFromStart) > Mathf.Abs(maxDistanceToCenter))
            {
                maxDistanceToCenter = room.distanceFromStart;
                mostDistantRoom = room;
            }
        }

        // Intenta colocar la habitación teniendo en cuentra su entrada
        switch (roomBossEntrance)
        {
            case RoomBossEntrance.Up:
                insertedRoom = PlaceRoomBoss(mostDistantRoom.position + Vector2Int.down);
                break;
            case RoomBossEntrance.Down:
                insertedRoom = PlaceRoomBoss(mostDistantRoom.position + Vector2Int.up);
                break;
            case RoomBossEntrance.Left:
                insertedRoom = PlaceRoomBoss(mostDistantRoom.position + Vector2Int.right);
                break;
            case RoomBossEntrance.Right:
                insertedRoom = PlaceRoomBoss(mostDistantRoom.position + Vector2Int.left);
                break;
        }

        if (!insertedRoom) unselectableRoomsByBoss.Add(mostDistantRoom);
        return insertedRoom;
    }

    /// <summary>
    /// Coloca una habitación de boss si pos es una posición válida
    /// </summary>
    /// <returns>true si ha podido, false si no</returns>
    private bool PlaceRoomBoss(Vector2Int pos)
    {
        if (CheckIfValidPosition(pos))
        {
            PlaceRoom(pos, true);
            return true;
        }
        else return false;
    }
}
