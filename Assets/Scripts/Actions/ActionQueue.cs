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
        Queue<ActionBase> actionQueue;
        GameController gameController;
        ActionBase currAction;

        private bool isStop;


		public int Count { get => actionQueue.Count; }
		public bool IsStop { get => isStop; set => isStop = value; }

		private void Awake()
		{
            actionQueue = new Queue<ActionBase>();

        }

        public void Enqueue(ActionBase _action)
        {
            actionQueue.Enqueue(_action);
           
        }

        public void Update()
        {
            if (IsEmpty() || isStop) { return; }
            if (currAction == null || currAction.IsComplete)
            {
                currAction = actionQueue.Peek();
                currAction.Enter();
                actionQueue.Dequeue(); 
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
            Debug.Log("Start movement queue");
            Queue <Vector3> _queue = new Queue<Vector3>();
            foreach (ActionBase action in actionQueue)
            {
                if (action.IsMove)
                {
                    _queue.Enqueue(action.Destination);
                    Debug.Log(action.Destination);
                }
            }
            return _queue;
        }
    }
}
