using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentScript : MonoBehaviour
{
    bool isIntermediaryGoal;
    public NavMeshAgent navMeshAgent;

    [SerializeField]
    FloatVariable maxHealth;
    public float health;

    [SerializeField]
    public ParticleSystem particles;


    void Start()
    {
        ManagerManagerScript.Instance.agents.Add(this);

        navMeshAgent = GetComponent<NavMeshAgent>();

        int randomIndex = Random.Range(0, ManagerManagerScript.Instance.intermediaryGoals.Count);
        navMeshAgent.destination = ManagerManagerScript.Instance.intermediaryGoals[randomIndex].transform.position;
        isIntermediaryGoal = true;

        health = maxHealth.Value;
    }

    private void Update()
    {
        float distanceRemaining = navMeshAgent.remainingDistance;
        if (distanceRemaining < 1 && isIntermediaryGoal)
        {
            navMeshAgent.destination = ManagerManagerScript.Instance.goal.transform.position;
            isIntermediaryGoal = false;
        }
    }

    public void TakeDamage(float value)
    {
        particles.Play();
        health -= value;
        if (health <= 0)
        {
            if(this != null)
            {
                gameObject.SetActive(false);
                Destroy(this.gameObject);

                ManagerManagerScript.Instance.goalScript.currentHealth += ManagerManagerScript.Instance.moneyPerKill.Value;
                if(ManagerManagerScript.Instance.goalScript.currentHealth > ManagerManagerScript.Instance.goalScript.maxHealth.Value)
                {
                    ManagerManagerScript.Instance.goalScript.currentHealth = ManagerManagerScript.Instance.goalScript.maxHealth.Value;
                }
            }
        }
    }

}
