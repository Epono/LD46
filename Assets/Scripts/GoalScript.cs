using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalScript : MonoBehaviour
{
    [SerializeField]
    FloatVariable maxHealth;

    [SerializeField]
    FloatVariable damagePerAgent;

    public float currentHealth;

    void Start()
    {
        currentHealth = maxHealth.Value;
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        AgentScript agentScript = other.GetComponent<AgentScript>();
        if (agentScript != null)
        {
            agentScript.particles.Play();
            agentScript.gameObject.GetComponent<MeshRenderer>().enabled = false;
            Destroy(agentScript.gameObject, 0.5f);

            currentHealth -= damagePerAgent.Value;
            if(currentHealth <= 0)
            {
                Debug.Log("Game over");
            }
        }
    }
}
