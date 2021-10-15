using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace ShadowBringer
{
    public class Attack : ActionBase
    {
        public float AttackRange = 5.0f;
		public Attack(WatsonController _player, NavMeshAgent _agent) : base(_player, _agent)
		{
		}

        override public void Enter()
        {
            Debug.Log("Enter Attack");
            if (CheckEnemy() != null)
            {
                CheckEnemy().GetComponent<Enemy>().KnockDown();
            }
            player.ChangeState(PlayerState.Idle);
            Complete();
        }

        override public void Execute()
        {
            
        }

        override protected void OnPause()
        {
            agent.isStopped = true;
        }

        private GameObject CheckEnemy()
        {
            GameObject[] list = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in list)
            {
                if (Vector3.Distance(player.transform.position, enemy.transform.position) <= AttackRange)
                {
                    return enemy;
                }
            }
            return null;
        }
    }
}
