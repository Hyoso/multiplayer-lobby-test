using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSequence : MonoBehaviour
{
    [SerializeField, MinMaxSlider(0, 20)] private Vector2Int m_animationsRange;
    [SerializeField] private List<AnimationBase> m_animations = new List<AnimationBase>();

    private int m_currentAnimationIdx = 0;

    private void Start()
    {
        foreach (var item in m_animations)
        {
            item.Init();
        }
    }

    private void Update()
    {
        bool animationComplete = m_animations[m_currentAnimationIdx].UpdateAnimation();
        if (animationComplete)
        {
            m_animations[m_currentAnimationIdx].ResetSettings();

            m_currentAnimationIdx++;
            if (m_currentAnimationIdx > m_animationsRange.y)
            {
                m_currentAnimationIdx = m_animationsRange.x;
            }

            m_animations[m_currentAnimationIdx].Init();
        }
    }
}
