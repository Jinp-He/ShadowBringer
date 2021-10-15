using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace ShadowBringer
{
    public class Walk : ActionBase
    {
        private Vector3 destination;

        [SerializeField]
        public float stopDistance = 1.0f;
        public Walk(WatsonController _player, NavMeshAgent _agent) : base(_player, _agent)
        {
            name = "Walk";
        }

        public NavMeshAgent Agent { get => agent; set => agent = value; }
		public Vector3 Destination { get => destination; set => destination = value; }

		override public void Enter()
        {
            Debug.Log("Enter Walk: " + destination);
            player.ChangeState(PlayerState.Walk);
            agent.SetDestination(Destination);
        }

        override public void Execute()
        {
            if (agent.remainingDistance <= stopDistance)
            {
                Debug.Log("Quit Walk: " + destination);
                Complete();
            }
        }

        override protected void OnPause()
        {
            agent.isStopped = true;
        }

        

        
    }
}
