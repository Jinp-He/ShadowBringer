using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadowBringer
{
    /// <summary>
    /// ActionQueue keeps a queue of action, and will automatically update once curraction is down.
    /// </summary>
    public class ActionQueue : MonoBehaviour
    {
        public Queue<ActionBase> actionQueue;
        GameController gameController;
        ActionBase currAction;
        public string myName;

        private bool isStop;

        public bool isDebug;


		public int Count { get => actionQueue.Count; }
		public bool IsStop { get => isStop; set => isStop = value; }

		private void Awake()
		{
            gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
            actionQueue = new Queue<ActionBase>();
            isDebug = false;
        }

        public void Enqueue(ActionBase _action)
        {
            Debug.Log("ENqueue with " + _action.name);
            actionQueue.Enqueue(_action);

        }

        public void Update()
        {
            if (gameController.IsPaused) { return; }
            if (isStop) { return; }
            if (IsEmpty())
            { return; }
            if (currAction == null)
            {
                currAction = actionQueue.Peek();
                currAction.Enter();
            }
            else if (currAction.IsComplete)
            {
                Debug.Log("Dequeue with " + currAction.name);
                actionQueue.Dequeue();
                if (actionQueue.Count == 0) { currAction = null; return; }
                currAction = actionQueue.Peek();
                currAction.Enter();
            }
            else
            {
                
                currAction.Execute();
            }

        }

        public ActionBase Dequeue()
        {
            if (!IsEmpty()) { return actionQueue.Dequeue(); }
            else return null;
        }

        public bool IsEmpty()
        {
            return actionQueue.Count == 0;
        }

        public void Clear() { actionQueue.Clear(); }

        public Queue<Vector3> GetMovementQueue()
        {
            Queue <Vector3> _queue = new Queue<Vector3>();
            foreach (ActionBase action in actionQueue)
            {
                if (action.IsMove)
                {
                    _queue.Enqueue(action.Destination);
                }
            }
            return _queue;
        }
    }
}
