using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class GoalScript : MonoBehaviour
{
    [SerializeField]
    public FloatVariable maxHealth;

    [SerializeField]
    FloatVariable initialHealth;

    [SerializeField]
    FloatVariable damagePerAgent;

    [SerializeField]
    FloatVariable burnRate;

    [SerializeField]
    GameObject mesh;

    [SerializeField]
    GameObject collider;

    [SerializeField]
    GameObject particles;

    public float currentHealth;

    public float maxScale = 4;
    public float ratio;

    void Start()
    {
        currentHealth = initialHealth.Value;
    }

    void Update()
    {
        ratio = (currentHealth / maxHealth.Value) * maxScale;
        mesh.transform.localScale = new Vector3(2 + ratio / 4, ratio * 1.5f, 2 + ratio / 4);
        collider.transform.localScale = new Vector3(1.5f /*+ Math.Max(1, ratio / 2)*/ + 1, 1, 1);
        particles.transform.localScale = new Vector3(ratio / 1.5f, ratio, ratio / 1.5f);
        particles.transform.position = new Vector3(particles.transform.position.x, ratio / 2, particles.transform.position.z);

        currentHealth -= burnRate.Value * Time.deltaTime;
    }

    public void TakeDamage(AgentScript agentScript)
    {
        if (agentScript != null)
        {
            agentScript.particles.Play();
            agentScript.gameObject.GetComponent<MeshRenderer>().enabled = false;
            Destroy(agentScript.gameObject, 0.5f);

            currentHealth -= damagePerAgent.Value;
            if (currentHealth <= 0)
            {
                Debug.Log("Game over");
            }
        }
    }
}
