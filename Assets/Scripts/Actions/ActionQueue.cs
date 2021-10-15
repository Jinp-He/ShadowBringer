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
        ActionBase currAction;

		public int Count { get => actionQueue.Count; }

		public ActionQueue() 
        { 
            actionQueue = new Queue<ActionBase>();
        }
        public void Enqueue(ActionBase _action)
        {
            actionQueue.Enqueue(_action);
        }

        public void KeepUpdate()
        {
            if (IsEmpty()) { return; }
            if (currAction == null || currAction.IsComplete)
            {
                actionQueue.Dequeue();
                currAction = actionQueue.Peek();
                currAction.Enter();
                currAction.Execute();
            }
            else
            {
                currAction.Execute();
            }
        }

        public bool IsEmpty()
        {
            return actionQueue.Count == 0;
        }

        
    }
}
