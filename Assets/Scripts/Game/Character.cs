using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
	[SerializeField] private PlayerSO m_characterSO;

	private CharacterBaseSO m_characterController;


    private void Awake()
	{
		m_characterController = Instantiate(m_characterSO);
        m_characterController.SetCharacterObject(this.gameObject);

		m_characterController.Init();
	}

	private void Update()
	{
        m_characterController.UpdateMovement();
    }
}
