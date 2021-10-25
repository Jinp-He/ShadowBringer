using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI_Guard : EnemyAIBase
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    override protected void UpdateStatus()
    {

        if (state == EnemyState.Alert)
        {
            UpdateAlert();
        }

        if (state == EnemyState.Patrol)
        {
            UpdatePatrol();
        }

        if (state == EnemyState.Chase)
        {
            UpdateFollow();
        }

        if (state == EnemyState.Confused)
        {
            UpdateConfused();
        }

    }
    // Update is called once per frame
    override protected void UpdateAlert()
    {
        if (state_timer < alert_notify_time)
        {
            FaceToward(alert_target);
        }
        else if (state_timer < alert_notify_time + alert_walk_time)
        {
            MoveTo(alert_target, move_speed);
        }
    }

    override protected void UpdateConfused()
    {
        if (wait_timer > alert_notify_time)
        {
            wait_timer = 0f;
            waiting = false;
            alert_target = GetRandomLookTarget();
            MoveTo(alert_target);
        }
    }

    override protected void UpdatePatrol()
    {
        float dist = (transform.position - start_pos).magnitude;
        bool is_far = dist > 0.5f;

        //Regular patrol
        if (!waiting)
        {
            //Move following path
            Vector3 targ = path_list[current_path];
            MoveTo(targ, move_speed);
            FaceToward(GetNextTarget());

            //Check if reached target
            Vector3 dist_vect = (targ - transform.position);
            dist_vect.y = 0f;
            if (dist_vect.magnitude < 0.1f)
            {
                waiting = true;
                wait_timer = 0f;
            }

            //Check if obstacle ahead
            bool fronted = CheckFronted(dist_vect.normalized);
            if (fronted && wait_timer > 2f)
            {
                RewindPath();
                wait_timer = 0f;
            }
        }

        //Waiting
        if (waiting)
        {
            //Wait a bit
            if (wait_timer > wait_time)
            {
                GoToNextPath();
                waiting = false;
                wait_timer = 0f;
            }
        }
    }


    override protected void UpdateFollow()
    {
        Vector3 targ = follow_target ? follow_target.transform.position : last_seen_pos;

        //Use memory if no more target
        if (follow_target == null && last_target != null && memory_duration > 0.1f)
        {
            memory_timer += Time.deltaTime;
            if (memory_timer < memory_duration)
            {
                last_seen_pos = last_target.transform.position;
                targ = last_seen_pos;
            }
        }

        //Move to target
        MoveTo(targ, run_speed);
        FaceToward(GetNextTarget(), 2f);

        if (follow_target != null)
        {
            last_target = follow_target;
            last_seen_pos = follow_target.transform.position;
            memory_timer = 0f;
        }
    }
}
