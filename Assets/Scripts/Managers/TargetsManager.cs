using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TargetsManager : Singleton<TargetsManager>
{
    /// <summary>
    /// todo
    /// - add retarget if out of range
    /// - add retarget if target is dead
    /// 
    /// </summary>

    public TargetBase currentTarget { get { return m_currentTarget; } }

    [SerializeField] private List<TargetBase> m_targets = new List<TargetBase>();
    [SerializeField] private Transform m_playerTransform;

    private float m_retargetCooldown;
    private TargetBase m_currentTarget;

    protected override void Init()
    {
    }

    private void Update()
    {
        UpdateSearchForNewTarget();
    }

    public void RegisterTarget(TargetBase target)
    {
        m_targets.Add(target);
    }

    public void UnRegisterTarget(TargetBase target)
    {
        if (m_targets.Contains(target))
        {
            m_targets.Remove(target);
        }
    }

    public void RegisterPlayerTransform(Transform player)
    {
        m_playerTransform = player;
    }

    private void UpdateSearchForNewTarget()
    {
        if (m_playerTransform != null)
        {
            m_retargetCooldown -= Time.deltaTime;
            if (m_retargetCooldown <= 0)
            {
                TargetBase newTarget = GetNearestTarget(m_playerTransform.position);
                SetCurrentTarget(newTarget);

                if (newTarget != null)
                {
                    m_retargetCooldown += GameplayConfig.Instance.retargetCooldown;
                }
            }
        }
    }

    private TargetBase GetNearestTarget(Vector3 pos)
    {
        if (m_targets.Count == 0)
        {
            return null;
        }

        TargetBase closestObject = m_targets[0];
        float closestDistance = Vector3.Distance(pos, closestObject.position);

        for (int i = 1; i < m_targets.Count; i++)
        {
            float distance = Vector3.Distance(pos, m_targets[i].position);
            if (distance < closestDistance)
            {
                closestObject = m_targets[i];
                closestDistance = distance;
            }
        }

        return closestObject;
    }

    private void SetCurrentTarget(TargetBase target)
    {
        if (m_currentTarget == target)
        {
            return;
        }

        if (m_currentTarget != null)
        {
            m_currentTarget.ResetTargetView();
        }

        m_currentTarget = target;
        if (m_currentTarget != null)
        {
            m_currentTarget.SetAsTarget();
        }
    }
}
