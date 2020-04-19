using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class SpawnScript : MonoBehaviour
{
    [SerializeField]
    GameObject agentPrefab;

    [SerializeField]
    FloatVariable minTime;

    [SerializeField]
    FloatVariable maxTime;

    public float timeSinceLastSpawn;

    GameObject parent;

    void Start()
    {
        ManagerManagerScript.Instance.spawns.Add(this);
        timeSinceLastSpawn = Random.Range(0.5f, minTime.Value);

        parent = GameObject.Find("Agents");
    }

    void Update()
    {
        timeSinceLastSpawn -= Time.deltaTime;
        if (timeSinceLastSpawn <= 0)
        {
            SpawnAgent();
        }
        else if (timeSinceLastSpawn <= minTime.Value)
        {
            // Chance
            if (Random.value > 0.999)
            {
                SpawnAgent();
            }
        }
    }

    private void SpawnAgent()
    {
        GameObject go = Instantiate(agentPrefab, transform.position, transform.rotation);
        go.transform.SetParent(parent.transform);
        timeSinceLastSpawn = maxTime.Value;
    }
}
