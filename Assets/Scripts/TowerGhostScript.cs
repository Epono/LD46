using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerGhostScript : MonoBehaviour
{
    public List<GameObject> collisions = new List<GameObject>();

    //
    [SerializeField]
    GameObject modelSimple;

    [SerializeField]
    GameObject modelDouble;

    bool isSimple = true;

    void Start()
    {
        ChangeSimple(isSimple);
    }

    public void ChangeSimple(bool newSimple)
    {
        isSimple = newSimple;

        if (isSimple)
        {
            modelSimple.SetActive(true);
            modelDouble.SetActive(false);
        }
        else
        {
            modelSimple.SetActive(false);
            modelDouble.SetActive(true);
        }
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
