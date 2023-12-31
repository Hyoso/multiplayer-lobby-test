﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using NaughtyAttributes;
using Unity.Networking.Transport;
using System.Data;
using System;

public class Character : NetworkBehaviour
{
    public struct CustomNetworkData : INetworkSerializable
    {
        public int netInt;
        public string netStr;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref netInt);
            serializer.SerializeValue(ref netStr);
        }
    }

    public Vector3 position { get { return transform.position; } }

	[SerializeField] private PlayerSO m_characterSO;
    [SerializeField] private PlayerStatsController m_stats;
    [SerializeField] private CharacterAnimations m_animations;
    [SerializeField] private PlayerHandController m_hand;
    [SerializeField] private Transform m_pivot;
    [SerializeField] private Rigidbody2D m_rigidbody;

    private List<ActionID> m_actionIDsList = new List<ActionID>();

    private List<ActionID> m_actionRequests = new List<ActionID>();

    private PlayerSO m_characterController;
    private float m_attackCooldown;

	private NetworkVariable<CustomNetworkData> m_randomNumber =  new NetworkVariable<CustomNetworkData>(
        new CustomNetworkData
        {
            netInt = 0,
            netStr = "new name"
        }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void Awake()
    {
        SetupCharacter();

        for (int i = 0; i != 4; i++)
        {
            m_actionIDsList.Add(new ActionID { ID = i });
        }
    }

    private void Start()
    {
        PlayerObjectsManager.Instance.RegisterPlayerObject(this);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        SetupNetworkSettings();
        LoadLastState();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (IsOwner)
        {
            GameNetworkManager.Instance.SetLastPlayerState(new GameNetworkManager.LastPlayerState
            {
                position = transform.position,
                rotation = transform.eulerAngles
            });
        }
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        m_characterController.UpdateMovement();

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Shoot();
        //}

        if (m_hand.hasTarget)
        {
            m_attackCooldown -= Time.deltaTime;
            if (m_attackCooldown <= 0)
            {
                Shoot();
                m_attackCooldown = GetAttackSpeed();
            }
        }
    }

    private bool movingRight = false;
    private bool movingLeft = false;


    private void FixedUpdate()
    {
        //if (!IsOwner)
        //    return;

        //if (!IsOwnedByServer)
        //    return;

        for (int i = 0; i != m_actionRequests.Count; ++i)
        {
            ActionID action = m_actionRequests[i];
             
            // button down
            if (action.ID == 0) // up
            {

            }
            else if (action.ID == 1) // right
            {
                movingRight = true;
            }
            else if (action.ID == 2) // down
            {
            }
            else if (action.ID == 3) // left
            {
                movingLeft = true;
            }
            // button up
            else if (action.ID == 4) // up
            {

            }
            else if (action.ID == 5) // right
            {
                movingRight = false;
            }
            else if (action.ID == 6) // down
            {

            }
            else if (action.ID == 7) // left
            {
                movingLeft = false;
            }
        }

        if (movingRight)
        {
            Vector2 moveAmount = Vector3.right * 3.8f * Time.deltaTime;
            transform.Translate(moveAmount);
            m_rigidbody.MovePosition(m_rigidbody.position + moveAmount);
        }
        if (movingLeft)
        {
            Vector2 moveAmount = Vector3.left * 3.8f * Time.deltaTime;
            transform.Translate(moveAmount);
            m_rigidbody.MovePosition(m_rigidbody.position + moveAmount);
        }

        m_actionRequests.Clear();

    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        PlayerObjectsManager.Instance.UnRegisterPlayerObject(this);
    }

    [ServerRpc]
    public void DoMovementServerRpc(Vector2 moveAmount)
    {
        m_rigidbody.MovePosition(m_rigidbody.position + moveAmount);
    }

    [ServerRpc]
    public void DoActionServerRpc(ActionID action)
    {
        m_actionRequests.Add(action);
        Debug.Log("Recv action: " + action.ID);

        //m_rigidbody.MovePosition(m_rigidbody.position + moveAmount);
    }

    [ServerRpc]
    private void ShootServerRPC(Vector3 pos, Vector3 rot)
    {
        GameObject prefab = AssetsConfig.Instance.GetNetworkObjectWithName("FeatherBullet");
        GameObject spawnedGo = Instantiate(prefab);
        spawnedGo.GetComponent<NetworkObject>().Spawn(true);
        spawnedGo.transform.position = pos;
        spawnedGo.transform.eulerAngles = rot;
    }

    // for server respawn after starting host
    private void LoadLastState()
    {
        if (IsServer)
        {
            var lastState = GameNetworkManager.Instance.lastPlayerState;
            transform.position = lastState.position;
            transform.eulerAngles = lastState.rotation;
        }
    }

    private void SetupNetworkSettings()
    {
        m_randomNumber.OnValueChanged += (CustomNetworkData prevVal, CustomNetworkData newVal) =>
        {
            Debug.Log(OwnerClientId + " - " + newVal.netInt + " " + newVal.netStr);
        };

        if (IsOwner)
        {
            GameManager.Instance.SetupPlayerCam(this.gameObject);
            TargetsManager.Instance.RegisterPlayerTransform(this.transform);
        }
    }

    private void SetupCharacter()
    {
        m_characterController = Instantiate(m_characterSO);
        m_characterController.SetCharacterObject(this.gameObject);

        m_characterController.Init();
    }

    private void Shoot()
    {
        m_animations.Shoot();
        Vector3 shootAngle = m_hand.featherTransform.parent.eulerAngles;
        if (m_pivot.localScale.x < 0f)
        {
            shootAngle.z += 180f;
        }

        ShootServerRPC(m_hand.featherTransform.position, shootAngle);
    }

    private float GetAttackSpeed()
    {
        /// formula
        /// Cooldown = Base Attack Time ÷ (1 + (Increased Attack Speed ÷ 100)) = Attack Speed

        float attackSpeed = m_stats.GetSkillValue(PlayerStats.StatType.ATTACK_SPEED);
        float baseAttackSpeed = m_characterSO.baseStats.GetStat(PlayerStats.StatType.ATTACK_SPEED);

        float cooldown = baseAttackSpeed / (1 + attackSpeed * 0.01f);
        return cooldown;
    }
}
