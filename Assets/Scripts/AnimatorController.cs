using UnityEngine;
namespace ShadowBringer
{
	public class AnimatorController : MonoBehaviour
	{
		private WatsonController watsonController;
		private Animator animator;
		private void Awake()
		{
			watsonController = GetComponent<WatsonController>();
			animator = GetComponent<Animator>();
		}
		//listen on the speed on Player speed and change the behavior.
		void Update()
		{
			switch (watsonController.GetState())
			{
				case PlayerState.Walk:
					animator.SetFloat("Speed", 3);
					break;
				case PlayerState.Run:
					animator.SetFloat("Speed", 5);
					break;
				case PlayerState.Attack:
					break;
				case PlayerState.Idle:
					animator.SetFloat("Speed", 0);
					break;
			}

		}



	}
}
