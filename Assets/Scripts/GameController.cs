using UnityEngine;
using UnityEngine.Events;
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

		public UnityAction EnterPlan;
		public UnityAction ExitPlan;

		public bool IsPlan { get => isPlan; set => isPlan = value; }

		// Start is called before the first frame update
		private void Awake()
		{
			inputController = GetComponent<InputController>();
			UIController = GetComponent<UIController>();
			IsPlan = false;
		}

		// Update is called once per frame
		void Update()
		{

		}

		public void TogglePlan()
		{
			isPlan = !isPlan;
			if (isPlan) { EnterPlan(); }
			else { ExitPlan(); }
		}

	}
}
