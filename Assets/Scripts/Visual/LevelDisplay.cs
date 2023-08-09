using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelDisplay : MonoBehaviour
{
	public Slider bar;
	public TextMeshProUGUI levelUpText;
	public Button button;

	private float targetSliderValue;

	Coroutine lerpSliderCoroutine;

	private void Start()
	{
	}

	public void InitBar(float expForLevel, float curExp)
	{
		bar.maxValue = expForLevel;
		bar.value = curExp;
		targetSliderValue = curExp;

		lerpSliderCoroutine = StartCoroutine(LerpLevelSlider());
	}

	public void SetSliderTargetVal(float target)
	{
		targetSliderValue = target;
	}

	private IEnumerator LerpLevelSlider()
	{
		while (true)
		{
			bar.value = Mathf.Lerp(bar.value, targetSliderValue, Time.deltaTime);
			yield return new WaitForEndOfFrame();
		}
	}
}
