using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace ShadowBringer
{
	/// <summary>
	/// Phantom
	/// </summary>
	public class PhantomController : MonoBehaviour
	{
		WatsonController watsonController;
		public GameObject watsonPhantomPrefab;
		GameObject phantom;
		NavMeshAgent phantomAgent;
		Queue<Vector3> movementQueue;
		private bool isPhantom;
		private bool isGenerated;

		private LineRenderer lineRenderer;
		private List<Vector3> lineList;

		public float stoppedDistance;
		// Start is called before the first frame update
		void Awake()
		{
			watsonController = GetComponent<WatsonController>();
			isPhantom = false;
			stoppedDistance = 0.1f;

			lineRenderer = GetComponent<LineRenderer>();
			lineRenderer.startWidth = 0.15f;
			lineRenderer.endWidth = 0.15f;
			lineRenderer.positionCount = 0;
		}

		// Update is called once per frame
		void Update()
		{
			if (!isPhantom) { return; }
			UpdateMovement();

		}

		private void UpdateMovement()
		{
			if (movementQueue.Count == 0)
			{
				return;
			}
			if (phantomAgent.remainingDistance <= stoppedDistance || !phantomAgent.hasPath)
			{
				Vector3 _destination = movementQueue.Dequeue();
				phantomAgent.destination = _destination;
			}
		}

		public void DestroyPhantom()
		{
			if (!isPhantom) { return; }
			phantom.gameObject.SetActive(false);
		}

		public void DrawPhantom(ActionQueue _actionQueue)
		{
			GeneratePhantom();
			movementQueue = _actionQueue.GetMovementQueue();
		}

		//public void DrawPhantom(ActionBase _action)
		//{
		//    GeneratePhantom();

		//}

		private void GeneratePhantom()
		{
			if (isPhantom) 
			{ 
				phantom.transform.position = phantom.transform.parent.position;
				movementQueue.Clear();
				phantomAgent.ResetPath();
				return; 
			}
			if (isGenerated) 
			{ 
				phantom.transform.position = phantom.transform.parent.position;
				movementQueue.Clear();
				phantom.gameObject.SetActive(true);
				phantomAgent.ResetPath();
				return;
			}
			isPhantom = true;
			isGenerated = true;
			phantom = Instantiate(watsonPhantomPrefab, transform);
			phantom.transform.parent = watsonController.transform;
			phantomAgent = phantom.GetComponent<NavMeshAgent>();
		}

		private void DrawPath()
		{
			lineRenderer.positionCount = phantomAgent.path.corners.Length;
			lineRenderer.SetPosition(0, transform.position);

			if (phantomAgent.path.corners.Length < 2)
			{ return; }

			for (int i = 1; i < phantomAgent.path.corners.Length; i++)
			{
				Vector3 vec = phantomAgent.path.corners[i];
				lineRenderer.SetPosition(i, vec);
			}
		}
	}
}
