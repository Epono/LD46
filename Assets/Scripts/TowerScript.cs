using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Security;
using System.Threading;

public class TowerScript : MonoBehaviour
{
    public List<AgentScript> agentScripts = new List<AgentScript>();

    private float timeBeforeNextShot = 0.5f;

    [SerializeField]
    GameObject particles;

    //
    [SerializeField]
    GameObject modelSimple;

    [SerializeField]
    GameObject modelDouble;

    //
    [SerializeField]
    FloatVariable timeBetweenShotsSimple;
    [SerializeField]
    FloatVariable damageSimple;
    [SerializeField]
    FloatVariable lifeSpanSimple;

    //
    [SerializeField]
    FloatVariable timeBetweenShotsDouble;
    [SerializeField]
    FloatVariable damageDouble;
    [SerializeField]
    FloatVariable lifeSpanDouble;

    //
    [SerializeField]
    GameObject bulletPrefab;

    GameObject bulletsParent;
    GameObject model;

    public float timeSinceCreated;
    public bool isSimple;

    public float lifeSpan;
    public float damage;
    public float timeBetweenShots;

    public void Init(bool isSimple)
    {
        this.isSimple = isSimple;
        if (isSimple)
        {
            lifeSpan = lifeSpanSimple.Value;
            damage = damageSimple.Value;
            timeBetweenShots = timeBetweenShotsSimple.Value;
            
            modelSimple.SetActive(true);
            modelDouble.SetActive(false);

            model = modelSimple;
        }
        else
        {
            lifeSpan = lifeSpanDouble.Value;
            damage = damageDouble.Value;
            timeBetweenShots = timeBetweenShotsDouble.Value;
            
            modelSimple.SetActive(false);
            modelDouble.SetActive(true);

            model = modelDouble;
        }
    }

    void Start()
    {
        bulletsParent = GameObject.Find("Bullets");
        timeSinceCreated = 0;
        model.transform.DOScale(0.25f, lifeSpan).SetEase(Ease.Linear);
        particles.transform.DOScale(0.25f, lifeSpan).SetEase(Ease.Linear);
    }

    void Update()
    {
        if (agentScripts.Count != 0)
        {
            timeBeforeNextShot -= Time.deltaTime;

            if (timeBeforeNextShot <= 0)
            {
                while (agentScripts.Count > 0)
                {
                    if (agentScripts[0] == null || !agentScripts[0].gameObject.active)
                    {
                        agentScripts.RemoveAt(0);
                    }
                    else
                    {
                        Shoot(agentScripts[0]);
                        break;
                    }
                }
            }
        }
        else
        {
            timeBeforeNextShot = timeBetweenShots;
        }

        timeSinceCreated += Time.deltaTime;
        if (timeSinceCreated >= lifeSpan)
        {
            ManagerManagerScript.Instance.towerScripts.Remove(this);
            gameObject.SetActive(false);
            Destroy(this.gameObject);
        }
    }

    private void Shoot(AgentScript agentScript)
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.transform.SetParent(bulletsParent.transform);

        float duration = 0.3f;
        Vector3 target = agentScript.transform.position;
        target.x += agentScript.navMeshAgent.velocity.x * duration;
        target.z += agentScript.navMeshAgent.velocity.z * duration;
        bullet.transform.DOMove(target, duration).OnComplete(() => OnCompleteCallback(agentScript, bullet)).SetEase(Ease.Linear);

        timeBeforeNextShot = timeBetweenShots;
    }

    private void OnCompleteCallback(AgentScript agentScript, GameObject bullet)
    {
        bullet.SetActive(false);
        Destroy(bullet);

        if (agentScript != null)
        {
            agentScript.TakeDamage(damage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        AgentScript agentScript = other.gameObject.GetComponent<AgentScript>();
        if (agentScript != null && agentScript.gameObject.active)
        {
            agentScripts.Add(agentScript);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        AgentScript agentScript = other.gameObject.GetComponent<AgentScript>();
        if (agentScript != null)
        {
            agentScripts.Remove(agentScript);
        }
    }
}
