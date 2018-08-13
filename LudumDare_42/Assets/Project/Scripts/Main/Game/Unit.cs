using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Vox;

namespace Main.Game
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Unit : MonoBehaviour
    {
        [SerializeField] private float _range = 0.5f;

        [SerializeField] private TriggerDetector _triggerDetector;
        public TriggerDetector TriggerDetector { get { return _triggerDetector; } }

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
