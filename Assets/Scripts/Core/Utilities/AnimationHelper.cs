using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHelper : MonoBehaviour
{
	[System.Serializable]
	public class AnimState
	{
		public string animationName;
		public bool isPositionRelative;
		public bool overrideFirstPosition;

		[System.NonSerialized]
		public int animationNameHash = -1;

		public int GetAnimNameHash()
		{
			if (animationNameHash == -1)
			{
				animationNameHash = Animator.StringToHash(animationName);
			}

			return animationNameHash;
		}
	}

	public List<AnimState> AnimStatesList => m_animStates;
	public Animator Animator => m_animator;
	public bool ShouldUpdateAnimState => m_animator != null;

	[SerializeField] private List<AnimState> m_animStates = new List<AnimState>();
	[SerializeField] private Animator m_animator;
	[SerializeField] private bool m_useFirstFrameHack = false;
	[SerializeField] private bool m_continueAnimationOnEnable = true;
	[SerializeField] private bool m_turnOnAnimatorOnEnable = true;

	private Vector3 m_startPosition = Vector3.zero;
	private bool m_animStarted;
	private Action m_animFinishedCallback;
	private AnimState m_activeAnimState;
	private AnimState m_incomingAnimState;
	private bool m_activeAnimFinished = true;
	private float m_currentClipTime;
	private bool m_isOverrideFirstPosition;
	private bool m_isRelativePosition;
	private int m_layer;

	private void Awake()
	{
		if (m_animator == null)
		{
			m_animator = GetComponent<Animator>();
		}
	}

	private void Start()
	{
		if (m_useFirstFrameHack)
		{
			m_animator.Update(0.02f);
		}
	}

	private void OnEnable()
	{
		if (m_animator != null)
		{
			m_animator.enabled = m_turnOnAnimatorOnEnable;

			if (m_activeAnimState != null && m_continueAnimationOnEnable)
			{
				m_animator.Play(m_activeAnimState.animationName, m_layer, m_currentClipTime);
			}
		}
	}

	private void OnDisable()
	{
		if (m_animator != null)
		{
			m_animator.enabled = false;
		}
	}

	public string GetCurrentAnim()
	{
		if (m_activeAnimState != null)
		{
			return m_activeAnimState.animationName;
		}

		return "";
	}

	public void AddState(string stateName)
	{
		foreach (var item in m_animStates)
		{
			if (item.animationName == stateName)
			{
				return;
			}
		}

		AnimState newState = new AnimState();
		newState.animationName = stateName;

		m_animStates.Add(newState);
	}

	public void SetAnimator(Animator animator)
	{
		m_animator = animator;
	}

	public void ChangeAnimState(int idx, string animName)
	{
		AnimState state = m_animStates[idx];
		state.animationName = animName;
		state.animationNameHash = -1;
	}

	private void PlayAnimationInternal(AnimState state, Action onFinishedCallback = null, float startTime = 0f, float crossfade = 0f, int layer = 0)
	{
		if (state == null)
		{
			onFinishedCallback?.Invoke();
			return;
		}

		m_incomingAnimState = state;
		m_animFinishedCallback = onFinishedCallback;
		m_startPosition = transform.localPosition;
		m_isOverrideFirstPosition = state.overrideFirstPosition;
		m_isRelativePosition = state.isPositionRelative;
		m_layer = layer;

		if (crossfade > 0f)
		{
			m_animator.CrossFade(state.animationName, crossfade);
		}
		else
		{
			m_animator.Play(state.animationName, layer, startTime);
		}

		m_currentClipTime = 0;
		m_animStarted = false;
	}

	private void SetupNewActiveAnimState(AnimState state)
	{
		m_activeAnimState = state;
		m_isOverrideFirstPosition = state.overrideFirstPosition;
		m_isRelativePosition = state.isPositionRelative;
		m_activeAnimFinished = false;
		m_animStarted = true;
	}

	private void Update()
	{
		if (ShouldUpdateAnimState)
		{
			UpdateAnimState();
		}
	}

	private void LateUpdate()
	{
		if (m_isRelativePosition)
		{
			transform.localPosition += m_startPosition;
		}
		else if (m_isOverrideFirstPosition)
		{
			float currentClipTime = m_animStarted ? m_animator.GetCurrentAnimatorStateInfo(m_layer).normalizedTime : 0f;
			Vector3 diff = m_startPosition - transform.localPosition;
			transform.localPosition += Vector3.Lerp(diff, Vector3.zero, currentClipTime);
		}
	}

	public void PlayAnimation(string animationName, Action onFinishedCallback = null, float startTime = 0f, bool checkIsPlayingFirst = false, float crossfade = 0f, int layer = 0)
	{
		if (m_animator.enabled)
		{
			if (checkIsPlayingFirst)
			{
				AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo(layer);
				bool isPlaying = stateInfo.IsName(animationName);

				if (isPlaying)
				{
					Debug.LogError("Animation is already playing");
					return;
				}
			}

			if (gameObject.activeInHierarchy && gameObject.activeSelf)
			{
				AnimState state = GetAnimStateFromAnimationName(animationName);
				if (state != null)
				{
					PlayAnimationInternal(state, onFinishedCallback, startTime, crossfade, layer);
				}
			}
		}
		else
		{
			Debug.LogError("Animator is disabled but you're trying to play an animation");
		}
	}

	private void UpdateAnimState()
	{
		AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo(m_layer);
		m_currentClipTime = stateInfo.normalizedTime;
		
		if (m_incomingAnimState != null)
		{
			if (stateInfo.shortNameHash == m_incomingAnimState.GetAnimNameHash())
			{
				SetupNewActiveAnimState(m_incomingAnimState);
				m_incomingAnimState = null;
			}
		}
		else
		{
			UpdateAnimFinished(stateInfo);
			UpdateAnimStarted(stateInfo);
		}
	}

	private void UpdateAnimFinished(AnimatorStateInfo stateInfo)
	{
		int animStateShortNameHash = stateInfo.shortNameHash;
		int currentAnimStateShortNameHash = m_activeAnimState != null ? m_activeAnimState.GetAnimNameHash() : -1;

		if (animStateShortNameHash != currentAnimStateShortNameHash)
		{
			AnimState state = GetAnimStateFromAnimationNameHash(animStateShortNameHash);
			if (state != null)
			{
				SetupNewActiveAnimState(state);
			}
		}
	}

	private void UpdateAnimStarted(AnimatorStateInfo stateInfo)
	{
		int animStateShortNameHash = stateInfo.shortNameHash;
		int currentAnimStateShortNameHash = m_activeAnimState != null ? m_activeAnimState.GetAnimNameHash() : -1;

		if (!m_activeAnimFinished)
		{
			AnimatorClipInfo[] clips = m_animator.GetCurrentAnimatorClipInfo(m_layer);
			bool clipHasFrames = clips.Length > 0 && clips[0].clip.length < Mathf.Epsilon;
			if (stateInfo.normalizedTime >= 1f || animStateShortNameHash != currentAnimStateShortNameHash || clipHasFrames)
			{
				m_activeAnimFinished = true;
				AnimationFinished();
			}
		}
	}

	private void AnimationFinished()
	{
		if (m_animFinishedCallback != null)
		{
			Action action = m_animFinishedCallback;
			m_animFinishedCallback = null;


			action.Invoke();
		}
	}

	private bool HasAnimationStateWithName(string animName)
	{
		return GetAnimStateFromAnimationName(animName) != null;
	}

	private AnimState GetAnimStateFromAnimationName(string animName)
	{
		foreach (var item in m_animStates)
		{
			if (item.animationName == animName)
			{
				return item;
			}
		}

		return null;
	}

	private AnimState GetAnimStateFromAnimationNameHash(int animNameHash)
	{
		foreach (var item in m_animStates)
		{
			if (item.GetAnimNameHash() == animNameHash)
			{
				return item;
			}
		}

		return null;
	}

	public void SetAnimatorSpeed(float speed)
	{
		m_animator.speed = speed;
	}

	public void StopAnimator()
	{
		m_animator.enabled = false;
	}


#if UNITY_EDITOR
	private void OnValidate()
	{
		if (m_animator == null)
		{
			m_animator = GetComponent<Animator>();
		}
	}
#endif
}
