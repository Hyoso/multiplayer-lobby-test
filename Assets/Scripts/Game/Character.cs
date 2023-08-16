﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using NaughtyAttributes;
using Unity.Networking.Transport;
using System.Data;

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

	[SerializeField] private PlayerSO m_characterSO;

	private CharacterBaseSO m_characterController;

	private NetworkVariable<CustomNetworkData> m_randomNumber =  new NetworkVariable<CustomNetworkData>(
        new CustomNetworkData
        {
            netInt = 0,
            netStr = "new name"
        }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void Awake()
    {
        SetupCharacter();
    }

    private void Start()
    {
        //m_connectionHandler.SetClientPlayerPrefab(0);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        SetupNetworkSettings();
    }

    private void Update()
	{
        if (!IsOwner)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_randomNumber.Value = new CustomNetworkData
            {
                netInt = Random.Range(10, 100),
                netStr = "string 2"
            };
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            ShootBulletServerRPC();
        }

        m_characterController.UpdateMovement();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    [ServerRpc]
    private void ShootBulletServerRPC()
    {
        GameObject prefab = AssetsConfig.Instance.GetNetworkObjectWithName("Bullet");
        GameObject spawnedGo = Instantiate(prefab);
        spawnedGo.GetComponent<NetworkObject>().Spawn(true);
    }

    private void SetupNetworkSettings()
    {
        m_randomNumber.OnValueChanged += (CustomNetworkData prevVal, CustomNetworkData newVal) =>
        {
            Debug.Log(OwnerClientId + " - " + newVal.netInt + " " + newVal.netStr);
        };
    }

    private void SetupCharacter()
    {
        m_characterController = Instantiate(m_characterSO);
        m_characterController.SetCharacterObject(this.gameObject);

        m_characterController.Init();
    }
}
