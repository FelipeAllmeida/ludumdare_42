#if NETFX_CORE
using System;

namespace Vox
{
    [Serializable]
    public class TaskNode
    {
        private Action _delegatedCallback;
        private Action _onAnticipate;
        private Action _onFinish;

        private System.Threading.Tasks.Task _currentTask;
        private System.Threading.CancellationTokenSource _cancellationToken;

        private int _id;
        private string _tag;
        private NodeState _currentState;
        private bool _shouldNodeBeCleaned = false;

        #region Constructor
        /// <summary>
        /// Constructor of TaskNode. Used internally by the Task class to create the thread, do not use it.
        /// </summary>
        /// <remarks>
        /// Constructor of TaskNode. Used internally by the Task class to create the thread, do not use it.
        /// </remarks>
        public TaskNode(int p_id, string p_tag, Action p_callbackMethod, Action p_callbackFinish)
        {
            this._cancellationToken = new System.Threading.CancellationTokenSource();
            this._id = p_id;
            this._tag = p_tag;
            this._onFinish = p_callbackFinish;
            this._onAnticipate = () =>
            {
                _onFinish?.Invoke();
                Cancel();
            };

            _currentTask = System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                p_callbackMethod?.Invoke();
                ChangeState(NodeState.FINISHED);
            }, _cancellationToken.Token);

            _currentTask.Start();

            ChangeState(NodeState.PLAYING);
        }
        #endregion

        private void ChangeState(NodeState p_state)
        {
            if (p_state == NodeState.PAUSED)
                throw new ArgumentOutOfRangeException("PAUSED NodeState not suported");

            _currentState = p_state;
        }

        #region Public Data
        /// <summary>
        /// Tag which can be used later to identificate and relate this threads to others. Allowing them to be cleaned separedly if required.
        /// </summary>
        /// <remarks>
        /// Tags are useful to be set when you are creating tweens that should stay even if their a scene is unloaded, or if you are using several tweens related to a state of a state machine and want to clear them easily when changing a state.
        /// You can use the tags manually in your own or use the functions Task.ClearNodesExceptForTag() or Task.ClearNodesWithTag().
        /// 
        /// Example of a tag being set is show below:
        /// 
        /// <code>
        /// void override StateOnUnloadedScene()
        /// {
        /// 	Vox.Thread.ClearNodesWithTag("GamePlayState");
        /// }
        /// </code>
        /// 
        /// </remarks>
        public string tag { get { return _tag; } }
        #endregion

        #region UpdateNode
        /// <summary>
        /// Used internally by the TaskModule class to update the tween, do not use it.
        /// </summary>
        /// <remarks>
        /// Used internally by the TaskModule class to update the tween, do not use it.
        /// </remarks>
        public void UpdateNode()
        {
            if (currentState == NodeState.FINISHED)
            {
                if (_onFinish != null) _onFinish();
                Cancel();
            }
        }
        #endregion

        #region Private Get-Only Data
        /// <summary>
        /// Current state of task node. Used for debug and get only.
        /// </summary>
        /// <remarks>
        /// Current state of task node. Used for debug and get only.
        /// </remarks>
        public NodeState currentState { get { return _currentState; } }
        #endregion

        #region Node Control Functions
        /// <summary>
        /// Cancel the tween not calling the onFinish callback if it has one.
        /// </summary>
        /// <remarks>
        /// Cancel the tween not calling the onFinish callback if it has one.
        /// 
        /// <code>
        /// Vox.TweenNode _moveSphereInCircle = null;
        /// 
        /// void StartSphereAnimation()
        /// {
        /// 	_moveSphereInCircle = Tween.FloatTo(0f, 1f, 1f, EaseType.Linear, delegate(float p_value)
        /// 	{
        /// 		_sphere.transform.position = new Vector3(Mathf.Sin(p_value), Mathf.Cos(p_value), 0);
        /// 	});
        /// 
        /// 
        /// 	_moveSphereInCircle.onFinish += delegate()
        /// 	{
        /// 		Debug.Log("Will not be called if cancelled");
        /// 	};
        /// 
        /// 	_moveSphereInCircle.Cancel();
        /// }
        /// </code>	 	
        /// </remarks>
        public void Cancel()
        {
            _onAnticipate = null;
            _onFinish = null;
            _cancellationToken.Cancel();
            _shouldNodeBeCleaned = true;
        }

        /// <summary>
        /// Force skip the delegated method callback and call its delegated finish callback.
        /// </summary>
        public void Antecipate()
        {
            if (_onAnticipate != null) _onAnticipate();
        }

        public bool ShouldNodeBeCleaned()
        {
            return _shouldNodeBeCleaned;
        }
        #endregion
    }
}
#endif