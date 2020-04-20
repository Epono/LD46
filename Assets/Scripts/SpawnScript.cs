using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class SpawnScript : MonoBehaviour
{
    [SerializeField]
    GameObject agentPrefab;

    [SerializeField]
    FloatVariable minTimeSimple;
    [SerializeField]
    FloatVariable minTimeDouble;

    [SerializeField]
    FloatVariable maxTimeSimple;
    [SerializeField]
    FloatVariable maxTimeDouble;

    public float timeSinceLastSpawnSimple;
    public float timeSinceLastSpawnDouble;

    GameObject parent;

    void Start()
    {
        ManagerManagerScript.Instance.spawns.Add(this);
        timeSinceLastSpawnSimple = Random.Range(0.5f, minTimeSimple.Value);
        timeSinceLastSpawnDouble = 2 * maxTimeDouble.Value;

        parent = GameObject.Find("Agents");
    }

    void Update()
    {
        timeSinceLastSpawnSimple -= Time.deltaTime;
        timeSinceLastSpawnDouble -= Time.deltaTime;

        if (timeSinceLastSpawnSimple <= 0 || (timeSinceLastSpawnSimple <= minTimeSimple.Value && Random.value > 0.999))
        {
            SpawnAgent(true);
        }

        if (timeSinceLastSpawnDouble <= 0 || (timeSinceLastSpawnDouble <= minTimeDouble.Value && Random.value > 0.999))
        {
            SpawnAgent(false);
        }
    }

    private void SpawnAgent(bool simple)
    {
        GameObject go = Instantiate(agentPrefab, transform.position, transform.rotation);
        go.transform.SetParent(parent.transform);
        go.GetComponent<AgentScript>().Init(simple);
        if (simple)
        {
            timeSinceLastSpawnSimple = maxTimeSimple.Value;
        }
        else
        {
            timeSinceLastSpawnDouble = maxTimeDouble.Value;
        }
    }
}
