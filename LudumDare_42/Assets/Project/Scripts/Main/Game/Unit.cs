using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Main.Game
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Unit : MonoBehaviour
    {
        [SerializeField] private float _range = 0.5f;

        private NavMeshAgent _navMeshAgent;

        public float Range { get { return _range; } }

        private void Start()
        {
            _navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        }

        public NavMeshAgent GetNavMeshAgent()
        {
            return _navMeshAgent;
        }
    }
}
