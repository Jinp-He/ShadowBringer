using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


    /// <summary>
    /// Manages enemy detection of the player, will also spawn a VisionCone and change the state of Enemy.cs based on what is seen
    /// </summary>

    public class EnemyVision_Guard : EnemyVisionBase
    {

    

    void Update()
    {
        if (enemy == null || enemy.IsPaused())
            return;

        wait_timer -= Time.deltaTime;

        //While patroling, detect targets
        if (enemy.GetState() == EnemyState.Patrol)
        {
            if (DetectVisionTarget() != null) { enemy.state = EnemyState.Alert; }

        }

        //When just seen the VisionTarget, enemy alerted
        if (enemy.GetState() == EnemyState.Alert)
        {
            VisionTarget target_seen = CanSeeAnyVisionTarget();

            seen_timer += target_seen ? Time.deltaTime : -Time.deltaTime;

            if (target_seen != null && seen_timer < -0.5f)
            {
                Alert(target_seen);

                int distance = GetSeenDistance(target_seen.gameObject);
            }

            if (target_seen != null)
                enemy.SetAlertTarget(target_seen.transform.position);

            if (target_seen != null && seen_timer > detect_time)
            {
                Chase(target_seen);
            }

            if (target_seen != null && enemy.GetStateTimer() > 0.2f && CanSeeVisionTargetNear(target_seen))
            {
                Chase(target_seen);
                bool is_touch = CanTouchObject(target_seen.gameObject);
            }

            if (enemy.HasReachedTarget() || enemy.GetStateTimer() > alerted_time)
            {
                ResumeDefault();
            }

        }

        //If seen long enough (detect time), will go into a chase
        if (enemy.GetState() == EnemyState.Chase)
        {
            bool can_see_target = CanSeeVisionTarget(seen_character);

            seen_timer += can_see_target ? -Time.deltaTime : Time.deltaTime;
            seen_timer = Mathf.Max(seen_timer, 0f);

            if (enemy.GetStateTimer() > 0.5f)
            {
                enemy.SetFollowTarget(can_see_target ? seen_character.gameObject : null);
            }

            if (seen_timer > follow_time)
            {
                ResumeDefault();
            }

            if (enemy.HasReachedTarget() && !can_see_target)
                enemy.ChangeState(EnemyState.Confused);

            if (seen_character == null)
                enemy.ChangeState(EnemyState.Confused);

        }

        //After the chase, if VisionTarget is unseen, enemy will be confused
        if (enemy.GetState() == EnemyState.Confused)
        {
            VisionTarget target_seen = CanSeeAnyVisionTarget();
            if (target_seen != null && target_seen == seen_character)
            {
                Chase(target_seen);
                int distance = GetSeenDistance(target_seen.gameObject);
            }

            if (target_seen != null && target_seen != seen_character)
            {
                Alert(target_seen);
                int distance = GetSeenDistance(target_seen.gameObject);
            }

            if (!dont_return && enemy.GetStateTimer() > alerted_time)
            {
                ResumeDefault();
            }

        }

        if (enemy.GetState() == EnemyState.Wait)
        {
            if (enemy.GetStateTimer() > 0.5f && wait_timer < 0f)
            {
                ResumeDefault();
            }
        }
    }




    }


