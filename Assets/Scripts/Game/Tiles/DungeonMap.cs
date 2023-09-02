using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static LevelGenerator;

public class DungeonMap : MonoBehaviour
{
    public GameObject tilemapColliderPrefab { get { return m_tilemapColliderPrefab; } private set { } }
    public GameObject doorPrefab { get { return m_doorPrefab; } private set { } }
    public GameObject roomEntrancePrefab { get { return m_roomEntrancePrefab; } private set { } }

    public Transform doorsParent { get { return m_doorsParent; } private set { } }
    public Transform collidersParent { get { return m_collidersParent; } private set { } }
    public Transform roomEntranceTilesParent { get { return m_roomEntranceTilesParent; } private set { } }

    public TileBase floorTile { get { return m_floorTile; } private set { } }
    public TileBase roomLinkTile { get { return m_roomLinkTile; } private set { } }
    public TileBase doorTile { get { return m_doorTile; } private set { } }
    public TileBase roomEntranceTile { get { return m_roomEntranceTile; } private set { } }


    public RoomData startRoom;
    public List<RoomData> roomTilemaps = new List<RoomData>();
    public List<HallwayReplacementTiles> hallwayReplacementTiles = new List<HallwayReplacementTiles>();

    [HideInInspector] public List<Vector3Int> roomLinks = new List<Vector3Int>();
    [HideInInspector] public List<Tilemap> colliderTilemaps = new List<Tilemap>();
    [HideInInspector] public List<Vector3Int> roomEntranceCells = new List<Vector3Int>();
    [HideInInspector] public Tilemap tilemap;

    [SerializeField] private GameObject m_tilemapColliderPrefab;
    [SerializeField] private GameObject m_doorPrefab;
    [SerializeField] private GameObject m_roomEntrancePrefab;

    [SerializeField] private Transform m_doorsParent;
    [SerializeField] private Transform m_collidersParent;
    [SerializeField] private Transform m_roomEntranceTilesParent;

    [SerializeField] private TileBase m_floorTile;
    [SerializeField] private TileBase m_roomLinkTile;
    [SerializeField] private TileBase m_doorTile;
    [SerializeField] private TileBase m_roomEntranceTile;

    [ReadOnly] public List<Room> generatedRooms = new List<Room>();
}
