using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
	[SerializeField] private CharacterBaseSO m_characterSO;

	private CharacterBaseSO m_characterController;

	private void Awake()
	{
		m_characterController = Instantiate(m_characterSO);
	}

	private void Update()
	{
		m_characterSO.UpdateMovement(this.gameObject);
	}
}
