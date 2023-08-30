using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTile : MonoBehaviour
{
    public enum DoorState
    {
        NONE,
        OPEN,
        CLOSED,
        BOMBED
    }

    public enum Direction
    {
        NORTH,
        EAST,
        SOUTH,
        WEST
    }

    private const string ANIM_WEST_OPEN = "WestOpen";
    private const string ANIM_WEST_CLOSE = "WestClose";
    private const string ANIM_NORTH_OPEN = "NorthOpen";
    private const string ANIM_NORTH_CLOSE = "NorthClose";
    private const string ANIM_EAST_OPEN = "EastOpen";
    private const string ANIM_EAST_CLOSE = "EastClose";
    private const string ANIM_SOUTH_OPEN = "SouthOpen";
    private const string ANIM_SOUTH_CLOSE = "SouthClose";


    public AnimationHelper m_animator;
    public DoorState entered;
    public Direction direction;

    public void OpenDoor()
    {

    }

    public void CloseDoor()
    {

    }
}
