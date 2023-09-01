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
        NONE,
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

    private void Start()
    {
        OpenDoor();
    }

    public void OpenDoor()
    {
        if (direction == Direction.NORTH)
        {
            m_animator.PlayAnimation(ANIM_NORTH_OPEN);
        }
        else if (direction == Direction.EAST)
        {
            m_animator.PlayAnimation(ANIM_EAST_OPEN);
        }
        else if (direction == Direction.SOUTH)
        {
            m_animator.PlayAnimation(ANIM_SOUTH_OPEN);
        }
        else if (direction == Direction.WEST)
        {
            m_animator.PlayAnimation(ANIM_WEST_OPEN);
        }
        else
        {
            Debug.LogError("Door direction must be set");
        }
    }

    public void CloseDoor()
    {
        if (direction == Direction.NORTH)
        {
            m_animator.PlayAnimation(ANIM_NORTH_CLOSE);
        }
        else if (direction == Direction.EAST)
        {
            m_animator.PlayAnimation(ANIM_EAST_CLOSE);
        }
        else if (direction == Direction.SOUTH)
        {
            m_animator.PlayAnimation(ANIM_SOUTH_CLOSE);
        }
        else if (direction == Direction.WEST)
        {
            m_animator.PlayAnimation(ANIM_WEST_CLOSE);
        }
        else
        {
            Debug.LogError("Door direction must be set");
        }
    }
}
