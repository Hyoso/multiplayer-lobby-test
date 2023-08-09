using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "Data/Player")]
public class PlayerSO : CharacterBaseSO
{
	public override void UpdateMovement(GameObject go)
	{
		go.transform.position += Vector3.up * Time.deltaTime;
	}
}
