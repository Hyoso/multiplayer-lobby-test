using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Coroutines
{
	public static IEnumerator Delay(float seconds, Action callback)
	{
		yield return new WaitForSeconds(seconds);
		callback?.Invoke();
	}

	public static IEnumerator WaitForFrames(int frames, Action callback)
	{
		int frameCounter= 0;
		while (frames < frameCounter)
		{
			frameCounter++;
			yield return null;
		}

		callback?.Invoke();
	}

	public static IEnumerator WaitForEndOfFrame(Action callback)
	{
		yield return new WaitForEndOfFrame();
		callback?.Invoke();
	}

	#region TRANSFORM_COROUTINES

	public static IEnumerator MoveTo(Transform transform, Vector3 start, Vector3 end, float duration, Ease.Type type = Ease.Type.LINEAR, Action<Vector3> onValueChanged = null, Action onFinished = null, bool clamp = true, bool unscaledTime = false)
	{
		Ease.Function function = Ease.GetEase(type);
		yield return MoveToInternal(false, transform, transform.position, start, end, duration, function, onValueChanged, onFinished, clamp, unscaledTime);
	}

	private static IEnumerator MoveToInternal(bool isLocal, Transform transform, Vector3 position, Vector3 start, Vector3 end, float duration, Ease.Function function, Action<Vector3> onValueChanged, Action onFinished, bool clamp, bool unscaledTime)
	{
		float timer = 0f;

		while (timer <= duration)
		{
			timer += unscaledTime ? Time.unscaledTime : Time.deltaTime;
			float percent = clamp ? Mathf.Clamp01(timer / duration) : (timer / duration);
			float evaluation = function(percent);
			Vector3 targetPos = clamp ? Vector3.Lerp(start, end, evaluation) : Vector3.LerpUnclamped(start, end, evaluation);
			if (isLocal)
			{
				transform.localPosition = targetPos;
			}
			else
			{
				transform.position = targetPos;
			}

			onValueChanged?.Invoke(targetPos);

			yield return null;
		}

		if (isLocal)
		{
			transform.localPosition = end;
		}
		else
		{
			transform.position = end;
		}

		onFinished?.Invoke();
	}

	#endregion
}
