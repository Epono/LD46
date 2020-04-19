using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerGhostScript : MonoBehaviour
{
    public List<GameObject> collisions = new List<GameObject>();

    void Start()
    {
        
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        collisions.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        collisions.Remove(other.gameObject);
    }
}
