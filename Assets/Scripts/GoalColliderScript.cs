using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalColliderScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        AgentScript agentScript = other.GetComponent<AgentScript>();
        if (agentScript != null)
        {
            gameObject.GetComponentInParent<GoalScript>().TakeDamage(agentScript);
        }
    }
}
