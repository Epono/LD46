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

    public float timeUntilNextSpawnSimple;
    public float timeUntilNextSpawnDouble;

    GameObject parent;

    void Start()
    {
        ManagerManagerScript.Instance.spawns.Add(this);
        timeUntilNextSpawnSimple = Random.Range(0.5f, minTimeSimple.Value);
        timeUntilNextSpawnDouble = 15.0f;

        parent = GameObject.Find("Agents");
    }

    void Update()
    {
        timeUntilNextSpawnSimple -= Time.deltaTime;
        timeUntilNextSpawnDouble -= Time.deltaTime;

        if (timeUntilNextSpawnSimple <= 0 || (timeUntilNextSpawnSimple <= minTimeSimple.Value && Random.value > 0.999))
        {
            SpawnAgent(true);
        }

        if (timeUntilNextSpawnDouble <= 0 || (timeUntilNextSpawnDouble <= minTimeDouble.Value && Random.value > 0.999))
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
            timeUntilNextSpawnSimple = maxTimeSimple.Value;
        }
        else
        {
            timeUntilNextSpawnDouble = maxTimeDouble.Value;
        }
    }
}
