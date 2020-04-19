using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.UI;

public class ManagerManagerScript : MonoBehaviour
{
    private static ManagerManagerScript _instance;
    public static ManagerManagerScript Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

    }

    public List<AgentScript> agents = new List<AgentScript>();
    public List<SpawnScript> spawns = new List<SpawnScript>();
    public List<IntermediaryGoalScript> intermediaryGoals = new List<IntermediaryGoalScript>();
    public List<TowerScript> towerScripts = new List<TowerScript>();

    [SerializeField]
    public Transform goal;
    public GoalScript goalScript;

    [SerializeField]
    public GameObject towerGhostPrefab;
    [SerializeField]
    public GameObject towerPrefab;

    [SerializeField]
    FloatVariable towerCost;

    [SerializeField]
    public FloatVariable moneyPerKill;

    private TowerGhostScript towerGhostScript;
    private GameObject towerGhost;

    [SerializeField]
    LayerMask layerMask;

    [SerializeField]
    Slider slider;


    GameObject towersParent;

    void Start()
    {
        towerGhost = Instantiate(towerGhostPrefab, Vector3.zero, Quaternion.identity);
        towerGhostScript = towerGhost.GetComponent<TowerGhostScript>();

        goalScript = goal.GetComponent<GoalScript>();
        slider.maxValue = goalScript.maxHealth.Value;

        towersParent = GameObject.Find("Towers");
    }

    void Update()
    {
        slider.value = goalScript.currentHealth;

        bool clicked = Input.GetMouseButtonDown(0);

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100, layerMask))
        {
            Vector3 towerGhostPosition = hit.point;
            towerGhostPosition.x = (float)Math.Round(towerGhostPosition.x + 0.5);
            towerGhostPosition.y = 0.0f;
            towerGhostPosition.z = (float)Math.Round(towerGhostPosition.z + 0.5);
            towerGhostScript.transform.position = towerGhostPosition;

            if (clicked)
            {
                if (IsValidPosition(towerGhostPosition))
                {
                    if (goalScript.currentHealth > towerCost.Value)
                    {
                        towerGhostPosition.y = 0.0f;
                        GameObject newTower = Instantiate(towerPrefab, towerGhostPosition, Quaternion.identity);
                        newTower.transform.SetParent(towersParent.transform);
                        towerScripts.Add(newTower.GetComponent<TowerScript>());
                        goalScript.currentHealth -= towerCost.Value;
                    }
                    else
                    {
                        Debug.Log("Not enough money/hp");
                    }
                }
                else
                {
                    Debug.Log("Location invalid");
                }
            }
        }
    }

    private bool IsValidPosition(Vector3 position)
    {
        if (towerGhostScript.collisions.Count != 0)
            return false;

        foreach (TowerScript towerScript in towerScripts)
        {
            if (Vector3.Distance(position, towerScript.transform.position) < 1.9)
            {
                return false;
            }
        }

        return true;
    }
}
