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

		LineRenderer lineRenderer;

		private Vector3 prevPosition;

		public Material phantomMaterial;
		public Material originalMaterial;



		public float WalkSpeed = 3.5f;
		public float RunSpeed = 7.0f;
		private PlayerState state;
		public bool isPaused;
		public float AttackRange = 5f;
		public Renderer watsonRenderer;

		ActionQueue actionQueue;
		ActionQueue phantomQueue;

		public NavMeshAgent Agent { get => agent; set => agent = value; }

		private void Awake()
		{
			gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
			MapCamera = Camera.main;
			lineRenderer = GetComponent<LineRenderer>();
			lineRenderer.positionCount = 1;
			lineRenderer.SetPosition(0, transform.position);
			lineRenderer.enabled = false;
			
			Agent = GetComponent<NavMeshAgent>();
			actionQueue = gameObject.AddComponent<ActionQueue>();
			actionQueue.myName = "actionQueue";
			phantomQueue = gameObject.AddComponent<ActionQueue>();
			phantomQueue.myName = "phantomQueue";
			isPaused = false;
			watsonRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
			gameController.EnterPlan += EnterPlan;
			gameController.ExitPlan += ExitPlan;

			actionQueue.isDebug = true;
		}

		private void EnterPlan()
		{
			lineRenderer.enabled = true;
			actionQueue.Clear();
			watsonRenderer.material = phantomMaterial;
			ChangeState(PlayerState.Idle);
			prevPosition = transform.position;
			phantomQueue.IsStop = false;
			actionQueue.IsStop = true;
			agent.ResetPath();
			Debug.Log("Entering Plan mode");
		}

		private void ExitPlan()
		{
			lineRenderer.enabled = false;
			phantomQueue.Clear();
			watsonRenderer.material = originalMaterial;
			ChangeState(PlayerState.Idle);
			phantomQueue.IsStop = true;
			actionQueue.IsStop = false;
			agent.ResetPath();
			transform.position = prevPosition;
			Debug.Log("Exiting Plan mode");
		}

		/// <summary>
		/// 如果在规划模式，则进入规划，否则按照actionQueue行动
		/// </summary>
		private void Update()
		{
			if (isPaused) { return; }
			if (gameController.IsPlan)
			{
				UpdatePlan();
			}
			else
			{
				UpdateActions();
			}
		}

		private void UpdatePlan()
		{
			if (phantomQueue.IsEmpty()) { ChangeState(PlayerState.Idle); }
			if (Input.GetMouseButtonDown(1))
			{
				actionQueue.Dequeue();
				phantomQueue.Dequeue();
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
						actionQueue.Enqueue(_action);
						_action = new Walk(this, hit.point);
						phantomQueue.Enqueue(_action);
						DrawDottedPath();
					}
					
				}
				if (Input.GetKeyDown(KeyCode.Q))
				{
					ActionBase _action = new Attack(this);
					actionQueue.Enqueue(_action);
					_action = new Attack(this);
					phantomQueue.Enqueue(_action);
				}
			}

		}

		private void UpdateActions()
		{
			
			if (actionQueue.Count == 0)
			{
				ChangeState(PlayerState.Idle);
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

		private void DrawDottedPath()
		{
			int _count = lineRenderer.positionCount;
			lineRenderer.positionCount += agent.path.corners.Length + 1;
			for (int i = _count; i < lineRenderer.positionCount-1; i++)
			{
				Vector3 vec = agent.path.corners[i - _count];
				lineRenderer.SetPosition(i, vec);
			}
			lineRenderer.SetPosition(lineRenderer.positionCount - 1, agent.destination);
		}
	}
}
