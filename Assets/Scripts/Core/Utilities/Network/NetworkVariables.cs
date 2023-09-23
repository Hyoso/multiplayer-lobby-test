using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct NetworkString : INetworkSerializable
{
    public string data;

    public NetworkString(string _str)
    {
        data = _str;
    }
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref data);
    }
}