using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ShadowBringer
{
    public class ActionBase
    {
        private bool isComplete;
        //TODO change the controller to a more universal one
        protected WatsonController player;
        protected NavMeshAgent agent;

        public bool IsComplete { get => isComplete; set => isComplete = value; }

        public ActionBase(WatsonController _player, NavMeshAgent _agent)
        {
            player = _player;
            agent = _agent;
            IsComplete = false;
        }

        virtual public void Enter()
        {

        }
        virtual public void Execute()
        {

        }
        virtual protected void OnPause()
        {

        }

        protected void Complete()
        {
            IsComplete = true;
        }







    }
}
