using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 1.0
/// Adding functionality of walk,run,attack and corresponding animation
/// </summary>
/// 

namespace ShadowBringer
{
	public enum PlayerState
	{
		Idle = 0,
		Walk = 1,
		Run = 2,
		Attack = 4,
	}

	public class WatsonController : MonoBehaviour
	{

		Camera MapCamera;
		public NavMeshAgent agent;
		GameController gameController;
		PhantomController phantomController;

		

		public float WalkSpeed = 3.5f;
		public float RunSpeed = 7.0f;
		private PlayerState state;
		public bool isPaused;
		public float AttackRange = 5f;

		ActionQueue actionQueue;

		public NavMeshAgent Agent { get => agent; set => agent = value; }

		private void Awake()
		{
			gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
			MapCamera = Camera.main;
			Agent = GetComponent<NavMeshAgent>();
			actionQueue = GetComponent<ActionQueue>();
			isPaused = false;
			gameController.EnterPlan += CleanQueue;
			
			phantomController = GetComponent<PhantomController>();
			
		}

		/// <summary>
		/// 如果在规划模式，则进入规划，否则按照actionQueue行动
		/// </summary>
		private void Update()
		{
			bool _isPlan = gameController.IsPlan;
			actionQueue.IsStop = gameController.IsPlan;
			if (isPaused) { return; }
			if (_isPlan)
			{
				UpdatePlan();
			}
			else
			{
				UpdateActions();
			}
		}

		protected void CleanQueue()
		{
			actionQueue.Clear();
		}

		private void UpdatePlan()
		{
			if (Input.GetMouseButtonDown(1))
			{
				actionQueue.Dequeue();
				phantomController.DrawPhantom(actionQueue);
			}
			if (actionQueue.Count < 4)
			{
				if (Input.GetMouseButtonDown(0))
				{
					Ray ray = MapCamera.ScreenPointToRay(Input.mousePosition);
					RaycastHit hit;
					if (Physics.Raycast(ray, out hit, Mathf.Infinity) && hit.transform.gameObject.tag == "Terrain")
					{
						Walk _action = new Walk(this, hit.point);
						_action.Destination = hit.point;
						actionQueue.Enqueue(_action);

						phantomController.DrawPhantom(actionQueue);
					}
				}
				
				if (Input.GetKeyDown(KeyCode.Q))
				{
					ActionBase _action = new Attack(this);
					actionQueue.Enqueue(_action);
					phantomController.DrawPhantom(actionQueue);
				}
			}

		}

		private void UpdateActions()
		{
			phantomController.DestroyPhantom();
			actionQueue.IsStop = gameController.IsPlan;
			if (actionQueue.Count == 0)
			{
				ChangeState(PlayerState.Idle);
				return;
			}
		}


		public void ChangeState(PlayerState _state)
		{
			state = _state;
		}

		public PlayerState GetState()
		{
			return state;
		}

		
	}
}
