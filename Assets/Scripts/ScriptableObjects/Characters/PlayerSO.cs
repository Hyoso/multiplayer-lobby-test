﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "Data/Player")]
public class PlayerSO : CharacterBaseSO, IOffsetController
{
    public float speed = 1f;

    private OffsetInput m_offsetInput;
    private Rigidbody2D m_rigidbody;

    protected override void Awake()
    {
    }

    public override void Init()
    {
        m_offsetInput = m_characterObject.GetComponent<OffsetInput>();
        m_offsetInput.RegisterController(this);
        m_rigidbody = m_characterObject.GetComponent<Rigidbody2D>();
    }

    public override void UpdateMovement()
	{
		//m_characterObject.transform.position += Vector3.up * Time.deltaTime;
	}

    public void OffsetControlChanged(Vector2 offset)
    {
        Vector3 dir = new Vector3(offset.x, offset.y, 0f);
        m_rigidbody.velocity = dir * speed;
    }

    public void OffsetControlStopped()
    {
        m_rigidbody.velocity = Vector3.zero;
    }

    public void OffsetControlStart()
    {
    }
}
