using UnityEngine;
using UnityEngine.Events;
namespace ShadowBringer
{
	/// <summary>
	/// Get the input from player
	/// </summary>
	public class InputController : MonoBehaviour
	{


		public KeyCode PlanKey = KeyCode.Space;
		public KeyCode PauseKey = KeyCode.Escape;

		public UnityAction OnPlan;
		private GameController gameController;

		private void Awake()
		{
			gameController = gameObject.GetComponent<GameController>();

		}

		// Update is called once per frame
		void Update()
		{
			if (Input.GetKeyDown(PlanKey))
			{
				//Debug.Log("Plankey pressed");
				OnPlan();
			}

			if (Input.GetMouseButtonDown(0))
			{

			}
		}
	}
}
