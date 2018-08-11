using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Main
{
    public class Unit : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _navMeshAgent;

        [SerializeField] private float _range = 0.5f;

        public float Range { get { return _range; } }

        public NavMeshAgent GetNavMeshAgent()
        {
            return _navMeshAgent;
        }
    }
}
