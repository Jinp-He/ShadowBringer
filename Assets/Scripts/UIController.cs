using TMPro;
using UnityEngine;
namespace ShadowBringer
{
	public class UIController : MonoBehaviour
	{
		public Canvas canvas;
		private InputController inputController;
		private bool isPlan;
		// Start is called before the first frame update
		void Awake()
		{
			canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
			inputController = GetComponent<InputController>();
			inputController.OnPlan += ToggleStop;
			isPlan = false;
		}

		// Update is called once per frame
		void Update()
		{

		}

		private void ToggleStop()
		{
			TextMeshProUGUI _planText = canvas.GetComponentInChildren<TextMeshProUGUI>();
			if (isPlan)
			{
				_planText.alpha = 0;
				isPlan = false;
			}
			else
			{
				_planText.alpha = 1;
				isPlan = true;
			}
		}
	}
}
