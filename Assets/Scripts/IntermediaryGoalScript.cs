using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntermediaryGoalScript : MonoBehaviour
{
    void Start()
    {
        ManagerManagerScript.Instance.intermediaryGoals.Add(this);
    }
}
