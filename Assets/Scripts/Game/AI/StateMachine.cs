using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StateMachine : NetworkBehaviour
{
    protected Dictionary<int, StateMachineBehaviour> m_stateMachineBehaviours = new Dictionary<int, StateMachineBehaviour>();
    protected StateMachineBehaviour m_currentState;
    protected AnimationHelper m_animator;

    public virtual void Setup(AnimationHelper animator)
    {
        m_animator = animator;
    }

    public virtual void UpdateMachine()
    {
        m_currentState?.OnStateUpdate();
    }

    public virtual void AddState(int stateId, StateMachineBehaviour state)
    {
        if (m_stateMachineBehaviours.ContainsKey(stateId))
        {
            Debug.LogError("State already exists");
            return;
        }

        m_stateMachineBehaviours.Add(stateId, state);
    }

    public virtual void SetState(int stateId)
    {
        if (m_stateMachineBehaviours.ContainsKey(stateId))
        {
            Debug.LogError("State does not exist");
            return;
        }

        m_currentState?.OnStateExit();

        m_currentState = m_stateMachineBehaviours[stateId];

        m_currentState?.OnStateEnter();
    }

    public StateMachineBehaviour GetCurrentState()
    {
        return m_currentState;
    }
}
