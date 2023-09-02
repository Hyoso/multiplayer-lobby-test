using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomData : MonoBehaviour
{
    public enum RoomTypes
    {
        START_ROOM,
        MONSTERS,
        TREASURE,
        BUFF,
        BOSS
    }

    public RoomTypes roomType;
    public Tilemap tilemap;
}
