using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace ShadowBringer
{
    public class Attack : ActionBase
    {
        public float AttackRange = 1.0f;
        public bool isAttack = false;
        public float stopDistance = 1.0f;
        public Attack(WatsonController _player) : base(_player)
		{
            name = "Attack";
		}

        override public void Enter()
        {
            GameObject _enemy = CheckEnemy();
            if (_enemy != null)
            {
                _enemy.GetComponent<Enemy>().KnockDown();
                Complete();
            }
            else 
            {
                player.ChangeState(PlayerState.Idle);
                Complete();
            }
            
        }

        override public void Execute()
        {
        }

        override protected void OnPause()
        {

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
