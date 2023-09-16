using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ShopBase : NetworkBehaviour
{
    public NetworkVariable<int> shopLevel = new NetworkVariable<int>();
}
