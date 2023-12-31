﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "Data/Player")]
public class PlayerSO : CharacterBaseSO, IOffsetController
{
    public float speed = 1f;
    public float maxSpeed = 0.5f;
    public PlayerStats baseStats;

    private OffsetInput m_offsetInput;
    private Rigidbody2D m_rigidbody;
    private Transform m_transform;
    private Character m_character;


    protected override void Awake()
    {
    }

    public override void Init()
    {
        m_character = m_characterObject.GetComponent<Character>();
        m_offsetInput = m_characterObject.GetComponent<OffsetInput>();
        m_offsetInput.RegisterController(this);
        m_rigidbody = m_characterObject.GetComponent<Rigidbody2D>();
        m_transform = m_characterObject.GetComponent<Transform>();
    }

    public override void UpdateMovement()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("send action: " + 1);
            m_character.DoActionServerRpc(new ActionID { ID = 1 });
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            Debug.Log("send action: " + 5);
            m_character.DoActionServerRpc(new ActionID { ID = 5 });
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("send action: " + 3);
            m_character.DoActionServerRpc(new ActionID { ID = 3 });
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            Debug.Log("send action: " + 7);
            m_character.DoActionServerRpc(new ActionID { ID = 7 });
        }


        return;

        //m_characterObject.transform.position += Vector3.up * Time.deltaTime;
        Vector3 dir = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            dir.y = maxSpeed;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            dir.y = -maxSpeed;
        }

        if (Input.GetKey(KeyCode.A))
        {
            dir.x = -maxSpeed;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            dir.x = maxSpeed;
        }

        Vector2 moveAmount = dir.normalized * maxSpeed * Time.deltaTime;
        //m_character.DoMovementServerRpc(moveAmount);
    }

    public void OffsetControlChanged(Vector2 offset)
    {

        return;
        Vector3 dir = new Vector3(offset.x, offset.y, 0f);
        Vector3 moveAmount = dir * speed;
        moveAmount = Vector3.ClampMagnitude(moveAmount, maxSpeed);
        //m_rigidbody.velocity = moveAmount;

        //Vector3 moveAmount = dir * speed * Time.deltaTime;
        //m_transform.Translate(moveAmount, Space.World);
    }

    public void OffsetControlStopped()
    {
        return;
        //m_rigidbody.velocity = Vector3.zero;
    }

    public void OffsetControlStart()
    {
    }
}
