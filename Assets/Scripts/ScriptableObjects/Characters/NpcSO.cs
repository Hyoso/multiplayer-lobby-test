using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPC", menuName = "Data/NPC")]
public class NpcSO : CharacterBaseSO
{
	public override void UpdateMovement(GameObject go)
	{
		go.transform.position += Vector3.down * Time.deltaTime;
	}
}
