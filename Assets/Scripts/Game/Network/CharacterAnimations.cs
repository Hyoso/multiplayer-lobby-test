using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterAnimations : NetworkBehaviour
{
    private enum MoveDirections
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    // movement anims
    private const string ANIM_IDLE_LEFT_BACK = "IdleLeftBack";
    private const string ANIM_IDLE_LEFT_FRONT = "IdleLeftFront";
    private const string ANIM_IDLE_RIGHT_BACK = "IdleRightBack";
    private const string ANIM_IDLE_RIGHT_FRONT = "IdleRightFront";
    private const string ANIM_RUN_LEFT_BACK = "RunLeftBack";
    private const string ANIM_RUN_LEFT_FRONT = "RunLeftFront";
    private const string ANIM_RUN_RIGHT_BACK = "RunRightBack";
    private const string ANIM_RUN_RIGHT_FRONT = "RunRightFront";
    private const string DEFAULT_ANIM = ANIM_IDLE_RIGHT_FRONT;

    // attack anims
    private const string ANIM_LOAD = "Load";
    private const string ANIM_IDLE = "Idle";
    private const string ANIM_SHOOT = "Shoot";



    [SerializeField] private AnimationHelper m_movementAnimator;
    [SerializeField] private AnimationHelper m_attackAnimator;

    private Rigidbody2D m_rigidbody;
    private MoveDirections m_verticalDir = MoveDirections.DOWN;
    private MoveDirections m_horizontalDir = MoveDirections.RIGHT;
    private MoveDirections m_lastVerticalDir = MoveDirections.DOWN;
    private MoveDirections m_lastHorizontalDir = MoveDirections.RIGHT;
    
    private Camera m_camera;

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_movementAnimator.PlayAnimation(DEFAULT_ANIM);
        m_camera = GameManager.Instance.playerCam.GetComponent<Camera>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        //if (m_rigidbody.velocity.magnitude > 0)
            UpdateMoveDirection();

        UpdateMovementAnimations();
    }

    public void Shoot(System.Action onFinishedAnim = null)
    {
        m_attackAnimator.PlayAnimation(ANIM_SHOOT, () =>  onFinishedAnim?.Invoke());
    }

    private void UpdateMovementAnimations()
    {
        if (m_rigidbody.velocity.magnitude > 0)
        {
            if (m_verticalDir == MoveDirections.DOWN && m_horizontalDir == MoveDirections.RIGHT)
            {
                m_movementAnimator.PlayAnimation(ANIM_RUN_RIGHT_FRONT, checkIsPlayingFirst: true);
            }
            else if (m_verticalDir == MoveDirections.DOWN && m_horizontalDir == MoveDirections.LEFT)
            {
                m_movementAnimator.PlayAnimation(ANIM_RUN_LEFT_FRONT, checkIsPlayingFirst: true);
            }
            else if (m_verticalDir == MoveDirections.UP && m_horizontalDir == MoveDirections.RIGHT)
            {
                m_movementAnimator.PlayAnimation(ANIM_RUN_RIGHT_BACK, checkIsPlayingFirst: true);
            }
            else if (m_verticalDir == MoveDirections.UP && m_horizontalDir == MoveDirections.LEFT)
            {
                m_movementAnimator.PlayAnimation(ANIM_RUN_LEFT_BACK, checkIsPlayingFirst: true);
            }
        }
        else
        {
            // idle
            if (m_verticalDir == MoveDirections.DOWN && m_horizontalDir == MoveDirections.RIGHT)
            {
                m_movementAnimator.PlayAnimation(ANIM_IDLE_RIGHT_FRONT, checkIsPlayingFirst: true);
            }
            else if (m_verticalDir == MoveDirections.DOWN && m_horizontalDir == MoveDirections.LEFT)
            {
                m_movementAnimator.PlayAnimation(ANIM_IDLE_LEFT_FRONT, checkIsPlayingFirst: true);
            }
            else if (m_verticalDir == MoveDirections.UP && m_horizontalDir == MoveDirections.RIGHT)
            {
                m_movementAnimator.PlayAnimation(ANIM_IDLE_RIGHT_BACK, checkIsPlayingFirst: true);
            }
            else if (m_verticalDir == MoveDirections.UP && m_horizontalDir == MoveDirections.LEFT)
            {
                m_movementAnimator.PlayAnimation(ANIM_IDLE_LEFT_BACK, checkIsPlayingFirst: true);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = transform.position.z - m_camera.transform.position.z;
        Vector3 worldMousePos = m_camera.ScreenToWorldPoint(mousePos);
        Vector3 dirToMouse = transform.position - worldMousePos;
        dirToMouse = -dirToMouse.normalized;

        Debug.DrawLine(transform.position, transform.position + dirToMouse);
    }

    private void UpdateMoveDirection()
    {
        m_verticalDir = m_lastVerticalDir;
        m_horizontalDir = m_lastHorizontalDir;

        // todo: change this to target enemy
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = transform.position.z - m_camera.transform.position.z;
        Vector3 worldMousePos = m_camera.ScreenToWorldPoint(mousePos);
        Vector3 dirToMouse = transform.position - worldMousePos;
        dirToMouse = -dirToMouse.normalized;

        if (dirToMouse.y < 0)
        {
            m_verticalDir = MoveDirections.DOWN;
        }
        else
        {
            m_verticalDir = MoveDirections.UP;
        }

        if (dirToMouse.x < 0)
        {
            m_horizontalDir = MoveDirections.LEFT;
        }
        else
        {
            m_horizontalDir = MoveDirections.RIGHT;
        }

        //if (m_rigidbody.velocity.y < 0)
        //{
        //    m_verticalDir = MoveDirections.DOWN;
        //}
        //else if (m_rigidbody.velocity.y > 0)
        //{
        //    m_verticalDir = MoveDirections.UP;
        //}

        //if (m_rigidbody.velocity.x < 0)
        //{
        //    m_horizontalDir = MoveDirections.LEFT;
        //}
        //else if (m_rigidbody.velocity.x > 0)
        //{
        //    m_horizontalDir = MoveDirections.RIGHT;
        //}

        m_lastVerticalDir = m_verticalDir;
        m_lastHorizontalDir = m_horizontalDir;
    }
}
