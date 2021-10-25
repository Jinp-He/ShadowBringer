using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision_Camera : EnemyVisionBase
{
    public float notify_time = 6.0f;
    void Update()
    {
        if (enemy == null || enemy.IsPaused())
            return;

        wait_timer -= Time.deltaTime;

        if (enemy.GetState() == EnemyState.Patrol)
        {
            if (DetectVisionTarget() != null) 
            {
                //Debug.Log("Into Alert Mode");
                Alert(DetectVisionTarget()); 
            }


        }

        if (enemy.GetState() == EnemyState.Alert)
        {
            VisionTarget target_seen = CanSeeAnyVisionTarget();

            seen_timer += target_seen ? Time.deltaTime : -Time.deltaTime;

            if (target_seen != null && seen_timer < -0.5f)
            {
                Alert(target_seen);
            }

            if (target_seen != null)
                enemy.SetAlertTarget(target_seen.transform.position);

            if (target_seen != null && seen_timer > detect_time)
            {
                //Debug.Log("Into Notify Mode");
                Notify(target_seen);
            }


        }

        if (enemy.GetState() == EnemyState.Notify)
        {
            if (enemy.GetStateTimer() > notify_time)
            {
                //Debug.Log("Into Patrol Mode");
                ResumeDefault();
            }
        }


    }
}
