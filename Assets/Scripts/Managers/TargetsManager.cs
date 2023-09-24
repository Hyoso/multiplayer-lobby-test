using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TargetsManager : Singleton<TargetsManager>
{
    [SerializeField] private List<TargetBase> m_targets = new List<TargetBase>();

    protected override void Init()
    {
    }


    public void RegisterTarget(TargetBase target)
    {
        m_targets.Add(target);
    }

    public TargetBase GetNearestTarget(Vector3 pos)
    {
        if (m_targets.Count == 0)
        {
            Debug.LogWarning("The objectPositions list is empty.");
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
}
