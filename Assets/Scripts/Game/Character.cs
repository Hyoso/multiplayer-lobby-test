using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

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

	private NetworkVariable<CustomNetworkData> m_randomNumber =  new NetworkVariable<CustomNetworkData>(
        new CustomNetworkData
        {
            netInt = 0,
            netStr = "new name"
        }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
	private CharacterBaseSO m_characterController;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        m_characterController = Instantiate(m_characterSO);
        m_characterController.SetCharacterObject(this.gameObject);

        m_characterController.Init();

        m_randomNumber.OnValueChanged += (CustomNetworkData prevVal, CustomNetworkData newVal) =>
        {
            Debug.Log(OwnerClientId +  " - "  + newVal.netInt + " " + newVal.netStr);
        };
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

        m_characterController.UpdateMovement();
    }
}
