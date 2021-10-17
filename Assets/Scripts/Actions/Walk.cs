using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
namespace ShadowBringer
{
    public class Walk : ActionBase
    {

        [SerializeField]
        public float stopDistance = 1.0f;
        public Walk(WatsonController _player, Vector3 _destination) : base(_player, _destination)
        {
            name = "Walk";
        }


		override public void Enter()
        {
			//Debug.Log("Enter Walk: " + base.Destination);
            player.ChangeState(PlayerState.Walk);
            player.Agent.SetDestination(Destination);
        }

        override public void Execute()
        {
            if (player.Agent.remainingDistance <= stopDistance)
            {
				//Debug.Log("Quit Walk: " + base.Destination);
                Complete();
            }
        }

        override protected void OnPause()
        {
            //Debug.Log("Is stopped");
            player.Agent.isStopped = true;
        }

        

        
    }
}
