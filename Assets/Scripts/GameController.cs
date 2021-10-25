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
		private bool isPaused;
		private bool isEmilyMode;

		private InputController inputController;
		private UIController UIController;

		public UnityAction EnterPlan;
		public UnityAction ExitPlan;
		public UnityAction EnterEmilyMode;
		public UnityAction EnterWatsonMode;

		private GameObject Emily;
		private GameObject Watson;
		private EmilyController EmilyController;
		private WatsonController WatsonController;

		public bool IsPlan { get => isPlan; set => isPlan = value; }
		public bool IsPaused { get => isPaused; set => isPaused = value; }
		public bool IsEmilyMode { get => isEmilyMode; set => isEmilyMode = value; }

		// Start is called before the first frame update
		private void Awake()
		{
			

			inputController = GetComponent<InputController>();
			UIController = GetComponent<UIController>();
			IsPlan = false;
			isPaused = true;

			Emily = GameObject.Find("Emily");
			Watson = GameObject.Find("Watson");
			EmilyController = Emily.GetComponent<EmilyController>();
			WatsonController = Watson.GetComponent<WatsonController>();
			ToggleMode();

			EnterEmilyMode += DebugEmilyMode;
			EnterWatsonMode += DebugWatsonMode;
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

		public void ToggleMode()
		{
			isEmilyMode = !isEmilyMode;
			if (isEmilyMode) { EnterEmilyMode(); }
			else { EnterWatsonMode(); }
		}

		public void ToggleStop()
		{
			isPaused = !isPaused;
		}

		




		/// Helper Functio


		
		public GameObject GetRecentPlayer()
		{
			if (IsEmilyMode)
			{
				return Emily;
			}
			else
			{
				return Watson;
			}
		}



		/// Debug Function

		public void DebugEmilyMode()
		{
			Debug.Log("Enter Emily Mode");
		}

		public void DebugWatsonMode()
		{
			Debug.Log("Enter Watson Mode");
		}
	}
}
