using Main;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Internal
{
    public class MoveCommand : Command 
    {
        public MoveCommand(Unit p_actor, Vector3 p_targetPosition, Action p_callbackFinish = null, Action p_callbackStopped = null)
        {
            _actor = p_actor;
            _targetPosition = p_targetPosition;
            onCommandFinish = p_callbackFinish;
            onCommandStopped = p_callbackStopped;
        }

        public override CommandType Execute()
        {
            _previousPosition = _actor.transform.position;

            _actor.GetNavMeshAgent().destination = _targetPosition;
            return CommandType.MOVE;
        }

        public void SetDestination(Vector3 p_destination)
        {
            _targetPosition = p_destination;
            _actor.GetNavMeshAgent().destination = _targetPosition;
        }

        public override void AUpdate()
        {
            if (_actor == null)
            {
                Stop();
                return;
            }

            bool _isInsideRange = false;
            if (_actor.GetNavMeshAgent().pathPending == true)
            {
        
                if ((Vector3.Distance(_previousPosition, _targetPosition)) < _actor.Range)
                {
                    _isInsideRange = true;
                }
            }
            else
            {
                if (((_actor.GetNavMeshAgent().remainingDistance < _actor.Range)))
                {
                    _isInsideRange = true;
                }
            }

            if (_isInsideRange == true)
            {
                onCommandFinish?.Invoke();
            }
        }

        public override void Stop()
        {
            if (_actor != null)
            {
                _actor.GetNavMeshAgent().isStopped = true;
                _actor.GetNavMeshAgent().ResetPath();
                _actor = null;  
            }
            onCommandFinish = null;
            base.Stop();
        }

        public override void Undo()
        {
            _actor.GetNavMeshAgent().destination = _previousPosition;
        }

        private Unit _actor;

        private Vector3 _previousPosition;
        private Vector3 _targetPosition;

    }
}
