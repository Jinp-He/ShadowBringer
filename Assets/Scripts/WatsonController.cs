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
		NavMeshAgent agent;
		GameController gameController;
		InputController inputController;
		Animator animator;


		const float DoubleClickTimer = 0.3f;
		const float DoubleClickMagnitude = 1.0f;
		const float StoppingDistance = 1.0f;

		const float AttackTimer = 1.0f;


		public float WalkSpeed = 3.5f;
		public float RunSpeed = 7.0f;
		private PlayerState state;
		private bool isActionComplete;
		public bool isPaused;

		public float AttackRange = 5f;


		Queue<Vector3> navMeshPath; 
		ActionQueue actionQueue;


		private void Awake()
		{
			animator = GetComponent<Animator>();
			navMeshPath = new Queue<Vector3>();
			gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
			MapCamera = Camera.main;
			agent = GetComponent<NavMeshAgent>();
			actionQueue = GetComponent<ActionQueue>();
			isActionComplete = true;
			isPaused = false;
			gameController.EnterPlan += CleanQueue;
		}

		private void Update()
		{
			UpdatePlan();
			UpdateActions();
		}

		protected void CleanQueue()
		{
			actionQueue.Clear();
		}

		private void UpdatePlan()
		{
			
			if (gameController.IsPlan)
			{
				if (actionQueue.Count < 4)
				{
					if (Input.GetMouseButtonDown(0))
					{
						Ray ray = MapCamera.ScreenPointToRay(Input.mousePosition);
						RaycastHit hit;
						if (Physics.Raycast(ray, out hit, Mathf.Infinity) && hit.transform.gameObject.tag == "Terrain")
						{

							Walk _action = new Walk(this, agent);
							_action.Destination = hit.point;
							actionQueue.Enqueue(_action);
						}
					}
					if (Input.GetMouseButtonDown(1))
					{

						actionQueue.Dequeue();
					}
					if (Input.GetKeyDown(KeyCode.Q))
					{

						ActionBase _action = new Attack(this, agent);
						actionQueue.Enqueue(_action);
					}
				}
			}
			else { return; }

		}

		private void UpdateActions()
		{
			actionQueue.IsStop = gameController.IsPlan;
			if (isPaused) { return; }
			if (actionQueue.Count == 0) 
			{
				ChangeState(PlayerState.Idle);
				return; 
			}
		}

		private void CompleteAction()
		{
			Debug.Log("Complete action by completeaction()");
			isActionComplete = true;
		}
		private GameObject CheckEnemy()
		{
			GameObject[] list  = GameObject.FindGameObjectsWithTag("Enemy");
			foreach (GameObject enemy in list)
			{
				if (Vector3.Distance(this.transform.position, enemy.transform.position) <= AttackRange)
				{
					return enemy;
				}
			}
			return null;
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
