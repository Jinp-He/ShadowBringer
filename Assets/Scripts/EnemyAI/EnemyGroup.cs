using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manage the enemy state in the group.
/// Position of target will be notify together in the group
/// </summary>
public class EnemyGroup : MonoBehaviour
{
	// Start is called before the first frame update
	List<EnemyAIBase> enemyList;
	VisionTarget target;
	Vector3 targetPos;

	void Awake()
	{
		enemyList.Add(GetComponentInChildren<EnemyAIBase>());
	}

	// Update is called once per frame
	void Update()
	{
		if (target != null || targetPos != null) { Debug.Log("Notify"); NotifyAll(); }
	}

	void ChangeTarget(VisionTarget _target)
	{
		target = _target;

	}

	void NotifyAll()
	{
		foreach (EnemyAIBase enemy in enemyList)
		{
			if (enemy.isMovable)
			{
				if (target != null) { enemy.Alert(target.transform.position); }
				else if (targetPos != null) { enemy.Alert(targetPos); }

			}
		}
	}

	public void SetVisionTarget(VisionTarget _target)
	{
		target = _target;
		NotifyAll();
	}

	public void SetVisionTarget(Vector3 pos)
	{
		targetPos = pos;
		NotifyAll();
	}



}
