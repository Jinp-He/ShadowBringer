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
        bool isPlan;

		public int Count { get => actionQueue.Count; }

		private void Awake()
		{
            actionQueue = new Queue<ActionBase>();
            gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        }

        public void Enqueue(ActionBase _action)
        {
            actionQueue.Enqueue(_action);
        }

        public void Update()
        {
            isPlan = gameController.IsPlan;
            if (IsEmpty() || isPlan) { return; }
            if (currAction == null || currAction.IsComplete)
            {
                currAction = actionQueue.Peek();
                currAction.Enter();
                currAction.Execute();  
                actionQueue.Dequeue(); 
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
