using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ShadowBringer
{
    /// <summary>
    /// Action分为移动和攻击类型的技能
    /// </summary>
    public class ActionBase
    {
        private bool isComplete;
        private bool isMove;
        //TODO change the controller to a more universal one
        protected WatsonController player;
        public string name;
		private Vector3 destination;
        private GameController gameController;


		public bool IsComplete { get => isComplete; set => isComplete = value; }
		public bool IsMove { get => isMove; set => isMove = value; }
		public Vector3 Destination { get => destination; set => destination = value; }

		public ActionBase(WatsonController _player, Vector3 _destination)
        {
            player = _player;
            Destination = _destination;
            IsMove = true;
            IsComplete = false;
            gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        }

        public ActionBase(WatsonController _player)
        {
            player = _player;
            Destination = Vector3.zero;
            IsMove = false;
            IsComplete = false;
        }

        virtual public void Enter()
        {

        }
        virtual public void Execute()
        {
            if (gameController.IsPaused) { return; }
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
