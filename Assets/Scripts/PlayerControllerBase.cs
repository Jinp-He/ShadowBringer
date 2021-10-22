using UnityEngine;
using UnityEngine.AI;

namespace ShadowBringer
{
	public class PlayerControllerBase : MonoBehaviour
	{
		public NavMeshAgent agent;

		public Material phantomMaterial;
		public Material originalMaterial;

		public GameObject playerIndicator;

		public float WalkSpeed = 3.5f;
		public float RunSpeed = 7.0f;
		public bool isPaused;
		public float AttackRange = 5f;
		public Renderer watsonRenderer;

		protected Camera MapCamera;
		protected GameController gameController;
		protected PlayerState state;

		protected ActionQueue actionQueue;

		virtual protected void Awake()
		{
			Debug.Log("Dude, I am awake");
			gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
			MapCamera = Camera.main;
			playerIndicator = transform.Find("PlayerIndicator").gameObject;
			agent = GetComponent<NavMeshAgent>();
			playerIndicator.SetActive(false);
			Debug.Assert(playerIndicator != null);
		}


		public NavMeshAgent Agent { get => agent; set => agent = value; }


		public void ChangeState(PlayerState _state)
		{
			state = _state;
		}

		public PlayerState GetState()
		{
			return state;
		}

		public GameObject GetPlayerIndicator()
		{
			return playerIndicator;
		}
	}
}