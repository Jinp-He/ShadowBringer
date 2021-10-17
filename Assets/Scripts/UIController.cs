using TMPro;
using UnityEngine;
namespace ShadowBringer
{
	public class UIController : MonoBehaviour
	{
		public Canvas canvas;
		private InputController inputController;
		private GameController gameController;
		private bool isPlan;
		public TextMeshProUGUI planText;
		// Start is called before the first frame update
		void Awake()
		{
			canvas = GameObject.Find("StoppingCanvas").GetComponent<Canvas>();
			inputController = GetComponent<InputController>();
			gameController = GetComponent<GameController>();
			planText = canvas.GetComponentInChildren<TextMeshProUGUI>();
		}

		// Update is called once per frame
		void Update()
		{
			if (!gameController.IsPlan)
			{
				planText.alpha = 0;
				isPlan = false;
			}
			else {
				planText.alpha = 1;
				isPlan = true;
			}
		}


	}
}
