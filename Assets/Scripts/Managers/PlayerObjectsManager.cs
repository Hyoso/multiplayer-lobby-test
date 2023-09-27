using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObjectsManager : Singleton<PlayerObjectsManager>
{
    [SerializeField] private List<Character> m_playerObjects = new List<Character>();


    protected override void Init() { }

    public void RegisterPlayerObject(Character playerObject)
    {
        m_playerObjects.Add(playerObject);
    }
    
    public void UnRegisterPlayerObject(Character playerObject)
    {
        if (m_playerObjects.Contains(playerObject)) 
        { 
            m_playerObjects.Remove(playerObject);
        }
    }

    public Transform GetClosestPlayerToPoint(Vector3 pos, float maxDistance = 500f)
    {
        if (m_playerObjects.Count == 0)
        {
            Debug.LogWarning("The objectPositions list is empty.");
            return null;
        }

        Character closestObject = null;
        float closestDistance = maxDistance;

        for (int i = 0; i < m_playerObjects.Count; i++)
        {
            float distance = Vector3.Distance(pos, m_playerObjects[i].position);

            if (distance <= maxDistance && (closestObject == null || distance < closestDistance))
            {
                closestObject = m_playerObjects[i];
                closestDistance = distance;
            }
        }

        if (closestObject != null)
        {
            return closestObject.transform;
        }

        return null;
    }
}
