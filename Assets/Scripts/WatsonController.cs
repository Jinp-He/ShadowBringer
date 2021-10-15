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

	public struct Action
	{
		public PlayerState state;
		public Vector3 pos;
		public Action(PlayerState _state, Vector3 _pos) 
		{
			pos = _pos;
			state = _state;
		}
		public Action(PlayerState _state) { state = _state; pos = Vector3.zero; }

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
			inputController = gameController.GetComponent<InputController>();
			actionQueue = new ActionQueue();
			isActionComplete = true;
			isPaused = false;
			//inputController.OnPlan += CleanPath;
		}

		private void Update()
		{
			UpdatePlan();
			UpdateActions();
			//UpdateMovement();
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
							Debug.Log("Enqueue Walk");
							Walk _action = new Walk(this, agent);
							_action.Destination = hit.point;
							actionQueue.Enqueue(_action);
						}
					}
					if (Input.GetKeyDown(KeyCode.Q))
					{
						Debug.Log("Enqueue Attack");
						ActionBase _action = new Attack(this, agent);
						actionQueue.Enqueue(_action);
					}
				}
			}
			else { return; }

		}

		private void UpdateActions()
		{
			if (isPaused || gameController.IsPlan) { return; }
			if (actionQueue.Count == 0) 
			{
				ChangeState(PlayerState.Idle);
				return; 
			}
			actionQueue.KeepUpdate();
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
