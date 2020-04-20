using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEditor.U2D.Sprites;
using UnityEngine;

public class GoalScript : MonoBehaviour
{
    [SerializeField]
    public FloatVariable maxHealth;

    [SerializeField]
    FloatVariable initialHealth;

    [SerializeField]
    FloatVariable burnRate;

    [SerializeField]
    GameObject mesh;

    [SerializeField]
    GameObject meshDead;

    [SerializeField]
    GameObject collider;

    [SerializeField]
    GameObject particles;

    [SerializeField]
    public ParticleSystem particlesDead;

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
        mesh.transform.localScale = new Vector3(2 + ratio / 4, Math.Max(1.0f, ratio * 1.5f), 2 + ratio / 4);
        collider.transform.localScale = new Vector3(1.5f /*+ Math.Max(1, ratio / 2)*/ + 1, 1, 1);
        particles.transform.localScale = new Vector3(Math.Max(0.5f, ratio / 1.5f), Math.Max(0.5f, ratio), Math.Max(0.5f, ratio / 1.5f));
        particles.transform.position = new Vector3(particles.transform.position.x, ratio / 2, particles.transform.position.z);

        if (!ManagerManagerScript.Instance.gameOver)
        {
            TakeDamage(burnRate.Value * Time.deltaTime);
        }
    }

    public void TakeDamage(AgentScript agentScript)
    {
        if (agentScript != null)
        {
            agentScript.particlesHit.Play();
            agentScript.gameObject.GetComponent<MeshRenderer>().enabled = false;
            agentScript.model.SetActive(false);
            agentScript.particlesSacrifice.Play();
            agentScript.navMeshAgent.isStopped = true;
            agentScript.canvasTransform.gameObject.SetActive(false);
            Destroy(agentScript.gameObject, 0.5f);

            TakeDamage(agentScript.damageToGoal);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            ManagerManagerScript.Instance.GameOver(false);
            particlesDead.gameObject.SetActive(true);
            particlesDead.Play();
            particles.GetComponent<ParticleSystem>().Stop();
            mesh.SetActive(false);
            meshDead.SetActive(true);
            meshDead.transform.localScale = mesh.transform.localScale;
        }
    }
}
