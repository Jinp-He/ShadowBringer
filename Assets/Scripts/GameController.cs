using UnityEngine;
/// <summary>
/// Scene Management, Reload Level, Save.
/// </summary>

namespace ShadowBringer
{
	public class GameController : MonoBehaviour
	{
		private bool isPlan;

		private InputController inputController;
		private UIController UIController;

		public bool IsPlan { get => isPlan; set => isPlan = value; }

		// Start is called before the first frame update
		private void Awake()
		{
			inputController = GetComponent<InputController>();
			UIController = GetComponent<UIController>();
			IsPlan = false;


			inputController.OnPlan += SetIsPlan;
		}

		// Update is called once per frame
		void Update()
		{

		}

		private void SetIsPlan()
		{
			isPlan = !isPlan;
		}

	}
}
