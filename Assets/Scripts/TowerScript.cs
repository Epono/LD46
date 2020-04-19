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

    [SerializeField]
    FloatVariable timeBetweenShots;
    private float timeBeforeNextShot = 0.5f;

    [SerializeField]
    FloatVariable damage;

    [SerializeField]
    FloatVariable towerLifeSpan;

    [SerializeField]
    GameObject bulletPrefab;

    GameObject bulletsParent;

    public float timeSinceCreated;

    void Start()
    {
        bulletsParent = GameObject.Find("Bullets");
        timeSinceCreated = 0;
        transform.DOScaleY(0, towerLifeSpan.Value).SetEase(Ease.Linear);
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
                    if (agentScripts[0] == null)
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
            timeBeforeNextShot = timeBetweenShots.Value;
        }

        timeSinceCreated += Time.deltaTime;
        if(timeSinceCreated >= towerLifeSpan.Value)
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

        timeBeforeNextShot = timeBetweenShots.Value;
    }

    private void OnCompleteCallback(AgentScript agentScript, GameObject bullet)
    {
        bullet.SetActive(false);
        Destroy(bullet);

        if (agentScript != null)
        {
            agentScript.TakeDamage(damage.Value);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        AgentScript agentScript = other.gameObject.GetComponent<AgentScript>();
        if (agentScript != null)
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
