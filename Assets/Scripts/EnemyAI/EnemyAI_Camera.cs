using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using DG.Tweening;




/// <summary>
/// Handles Camera's behavior
/// </summary>

[RequireComponent(typeof(Rigidbody))]
public class EnemyAI_Camera : EnemyAIBase
{
    private bool isClockWise = true;
    float visionTimer = 0f;
    private bool isRotate = false;
    public float rotateAngle = 90;
    public float alert_patrol_time = 4f;
    public float notify_patrol_time = 4f;
    private Vector3 fromTransform;
    private Vector3 toTransform;
    bool isClockwise;




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

        if (state == EnemyState.Notify)
        {
            UpdateNotify();
        }

    }

    override protected void ManualRotation()
    {
        if (state == EnemyState.Patrol) { rotate_speed = 10f; }
        else{ rotate_speed = 120f;  base.ManualRotation();}
    }
    override protected void UpdateAlert()
    {
        //Debug.Log("Into Alert Mode");
        bool _isSeen = vision.IsSeen();
        float _seen_timer = vision.GetSeenTimer();
        float _unseen_timer = vision.GetUnSeenTimer();
        VisionTarget _target = vision.GetTarget();
        if (_isSeen)
        {
            last_seen_pos = _target.transform.position;
            last_seen_target = _target;
            if (_seen_timer > alert_notify_time)
            {
                ChangeState(EnemyState.Notify);
            }
            visionTimer += Time.deltaTime;
        }
        else
        {
            if (_unseen_timer > alert_patrol_time)
            {
                ChangeState(EnemyState.Patrol);
            }
        }
        FaceToward(last_seen_pos);
        
        
    }

    //Rotate manually
    override protected void UpdatePatrol()
    {
        
        if (vision.IsSeen())
        {
            VisionTarget _target = vision.GetTarget();
            ChangeState(EnemyState.Alert);
            last_seen_pos = _target.transform.position;
            last_seen_target = _target;
        }
        if (transform.eulerAngles == toTransform)
        {
            isClockwise = !isClockwise;
            Rotate(isClockwise, rotateAngle);
        }
        
    }

    void Rotate(bool isClockwise, float rotateAngle)
    {
        isRotate = true;
        toTransform = transform.eulerAngles + new Vector3(0, isClockWise ? rotateAngle : -rotateAngle, 0);
        FaceToward(transform.position + toTransform);
    }

    override protected void UpdateNotify()
    {
        bool _isSeen = vision.IsSeen();
        float _seen_timer = vision.GetSeenTimer();
        float _unseen_timer = vision.GetUnSeenTimer();
        VisionTarget _target = vision.GetTarget();
        //TODO upload the position
        if (!_isSeen)
        {
            if (_unseen_timer > notify_patrol_time)
            {
                ChangeState(EnemyState.Patrol);
            }
        }
        else 
        {
            last_seen_pos = _target.transform.position;
            last_seen_target = _target;
        }
        FaceToward(last_seen_pos);
    }

}