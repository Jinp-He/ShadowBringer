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
		public KeyCode ModeKey = KeyCode.Tab;
		public KeyCode PauseKey = KeyCode.Escape;

		public UnityAction OnPlan;
		private GameController gameController;
		private CameraController cameraController;

		private bool isChangeable;

		private void Awake()
		{
			gameController = gameObject.GetComponent<GameController>();
			cameraController = gameObject.GetComponent<CameraController>();
			isChangeable = true;
		}

		// Update is called once per frame
		void Update()
		{
			if(gameController.IsPaused)
			{ return; }
			if (Input.GetKeyDown(PlanKey))
			{
				gameController.TogglePlan();
			}

			if (Input.GetKeyDown(ModeKey) && isChangeable)
			{
				gameController.ToggleMode();
			}

			if (Input.GetAxis("Mouse ScrollWheel") != 0)
			{
				cameraController.scale -= Input.GetAxis("Mouse ScrollWheel") * 10f+ Time.deltaTime;
				if (cameraController.scale < -2f) { cameraController.scale = -2f; }
				if (cameraController.scale >= 2f){ cameraController.scale = 2f; }
			}
		}

		public void DisableChangeMode()
		{
			isChangeable = false;
		}
	}
}
