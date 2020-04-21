using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AgentScript : MonoBehaviour
{
    [SerializeField]
    FloatVariable maxHealthSimple;
    [SerializeField]
    FloatVariable moneyPerKillSimple;
    [SerializeField]
    FloatVariable damageToGoalSimple;

    [SerializeField]
    FloatVariable maxHealthDouble;
    [SerializeField]
    FloatVariable moneyPerKillDouble;
    [SerializeField]
    FloatVariable damageToGoalDouble;

    //
    [SerializeField]
    GameObject modelSimple;

    [SerializeField]
    GameObject modelDouble;

    //
    [SerializeField]
    public ParticleSystem particlesHit;

    [SerializeField]
    public ParticleSystem particlesDead;

    [SerializeField]
    public ParticleSystem particlesSacrifice;

    //
    [SerializeField]
    public Transform canvasTransform;

    [SerializeField]
    public Slider sliderHealth;

    //
    bool isIntermediaryGoal;
    public NavMeshAgent navMeshAgent;

    //
    public bool isSimple;
    public float health;
    public float moneyPerKill;
    public float damageToGoal;

    public GameObject model;

    public void Init(bool IsSimple)
    {
        this.isSimple = IsSimple;

        if (IsSimple)
        {
            modelSimple.SetActive(true);
            modelDouble.SetActive(false);

            model = modelSimple;

            health = maxHealthSimple.Value;
            moneyPerKill = moneyPerKillSimple.Value;
            damageToGoal = damageToGoalSimple.Value;

            sliderHealth.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
        else
        {
            modelSimple.SetActive(false);
            modelDouble.SetActive(true);

            model = modelDouble;

            health = maxHealthDouble.Value;
            moneyPerKill = moneyPerKillDouble.Value;
            damageToGoal = damageToGoalDouble.Value;

            sliderHealth.GetComponent<RectTransform>().localScale = new Vector3(1.5f, 1.5f, 1.0f);
        }
    }

    void Start()
    {
        ManagerManagerScript.Instance.agents.Add(this);

        navMeshAgent = GetComponent<NavMeshAgent>();

        int randomIndex = Random.Range(0, ManagerManagerScript.Instance.intermediaryGoals.Count);
        navMeshAgent.destination = ManagerManagerScript.Instance.intermediaryGoals[randomIndex].transform.position;
        isIntermediaryGoal = true;

        sliderHealth.maxValue = health;
        sliderHealth.value = health;
    }

    private void Update()
    {
        float distanceRemaining = navMeshAgent.remainingDistance;
        if (distanceRemaining < 1 && isIntermediaryGoal)
        {
            navMeshAgent.destination = ManagerManagerScript.Instance.goal.transform.position;
            isIntermediaryGoal = false;
        }

        Vector3 rot = canvasTransform.localRotation.eulerAngles;
        rot.y = -transform.rotation.eulerAngles.y;
        canvasTransform.localRotation = Quaternion.Euler(rot);
    }

    public bool iAmDead = false;

    public void TakeDamage(float value)
    {
        
        health -= value;
        sliderHealth.value = health;
        if (health <= 0)
        {
            if (this != null)
            {
                particlesHit.Play();
                particlesDead.Play();
                iAmDead = true;

                if (isSimple)
                {
                    SoundManagerScript.instance.PlayOneShotSound(SoundManagerScript.AudioClips.AgentSimpleDead);
                }
                else
                {
                    SoundManagerScript.instance.PlayOneShotSound(SoundManagerScript.AudioClips.AgentDoubleDead);
                }


                //model.SetActive(false);
                float duration = isSimple ? 2.0f : 3.0f;
                model.transform.DOMoveY(-5.0f, duration).SetEase(Ease.Linear);
                float randomValue = Random.value;
                if (randomValue > 0.75)
                {
                    model.transform.DOLocalRotate(new Vector3(0, 0, 90), duration, RotateMode.LocalAxisAdd);
                }
                else if (randomValue > 0.50)
                {
                    model.transform.DOLocalRotate(new Vector3(0, 0, -90), duration, RotateMode.LocalAxisAdd);
                }
                else if (randomValue > 0.25)
                {
                    model.transform.DOLocalRotate(new Vector3(90, 0, 0), duration, RotateMode.LocalAxisAdd);
                }
                else
                {
                    model.transform.DOLocalRotate(new Vector3(-90, 0, 0), duration, RotateMode.LocalAxisAdd);
                }
                //navMeshAgent.isStopped = true;
                canvasTransform.gameObject.SetActive(false);
                GetComponent<SphereCollider>().enabled = false;
                ManagerManagerScript.Instance.agents.Remove(this);


                ManagerManagerScript.Instance.goalScript.currentHealth += moneyPerKill;

                if (ManagerManagerScript.Instance.goalScript.currentHealth > ManagerManagerScript.Instance.goalScript.maxHealth.Value)
                {
                    ManagerManagerScript.Instance.goalScript.currentHealth = ManagerManagerScript.Instance.goalScript.maxHealth.Value;
                }

                Destroy(this.gameObject, 5.0f);
            }
        }
    }

}
