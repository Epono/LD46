using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreateRuntimeSet
{

    [CreateAssetMenu(menuName = "ScriptableObjects/NavMeshAgents")]
    public class NavMeshAgentsRuntimeSet : RuntimeSet<NavMeshAgent>
    { }

    [CreateAssetMenu(menuName = "ScriptableObjects/Transforms")]
    public class TransformsRuntimeSet : RuntimeSet<Transform>
    { }
}
