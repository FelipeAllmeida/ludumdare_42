using Main;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Internal.Commands
{
    public class MoveCommand : Command
    {
        public MoveCommand(Unit p_actor, Vector3 p_targetPosition, EventHandler<CommandCallbackEventArgs> p_callbackFinish = null, EventHandler<CommandCallbackEventArgs> p_callbackStopped = null)
            : base(Commands.MOVE)
        {
            _actor = p_actor;
            _targetPosition = p_targetPosition;
            CallbackFinish += p_callbackFinish;
            CallbackStopped += p_callbackStopped;
        }

        private Unit _actor;

        private Vector3 _previousPosition;
        private Vector3 _targetPosition;

        public override Commands Execute()
        {
            _previousPosition = _actor.transform.position;

            _actor.GetNavMeshAgent().destination = _targetPosition;
            return Commands.MOVE;
        }

        public void SetDestination(Vector3 p_destination)
        {
            _targetPosition = p_destination;
            _actor.GetNavMeshAgent().destination = _targetPosition;
        }

        public override void AUpdate()
        {
            if (_actor == null) return;

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
                Finish();
            }
        }

        public override void Stop()
        {
            if (_actor != null)
            {
                _actor.GetNavMeshAgent().isStopped = true;
                _actor.GetNavMeshAgent().ResetPath();
            }

            base.Stop();
        }

        public override void Undo()
        {
            _actor.GetNavMeshAgent().destination = _previousPosition;
        }
    }
}
