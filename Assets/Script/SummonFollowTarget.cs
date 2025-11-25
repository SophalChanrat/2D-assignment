using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonFollowTarget : MonoBehaviour
{
    private Transform target;
    private Vector3 offset;
    
    public void SetTarget(Transform enemyTransform)
    {
        if (enemyTransform == null) return;
        
        target = enemyTransform;
        offset = transform.position - target.position;
    }
    
    void LateUpdate()
    {
        // Follow the target if it exists and is not destroyed
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}
