using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using DG.Tweening;

public enum EnemyState
{
    None = 0,
    Patrol = 2,
    Alert = 5,
    Chase = 10,
    Confused = 15, //After lost track of target
    Wait = 20,
    Notify = 25,
}

public enum EnemyPatrolType
{
    Rewind = 0,
    Loop = 2,
}

public enum VisionState
{
    Safe = 0,
    Alert = 2,
    Danger = 5,
}

/// <summary>
/// Handles enemy movement and the different enemy behaviors
/// </summary>

[RequireComponent(typeof(Rigidbody))]
public class EnemyAIBase : MonoBehaviour
{
    public float move_speed = 2f;
    public float run_speed = 4f;
    public float rotate_speed = 120f;
    public float fall_speed = 5f;
    public LayerMask obstacle_mask = ~(0);
    public bool use_pathfind = false;
    public bool isMovable = true;

    [Header("State")]
    public EnemyState state = EnemyState.Patrol;

    [Header("Patrol")]
    public EnemyPatrolType type;
    public float wait_time = 1f;
    public GameObject[] patrol_path;

    [Header("Alert")]
    public float alert_notify_time = 3f;
    public float alert_walk_time = 10f;

    [Header("Follow")]
    public GameObject follow_target;
    public float memory_duration = 4f;

    [Header("Vision")]
    public VisionState visionState;

    public UnityAction onDeath;

    protected Rigidbody rigid;
    protected NavMeshAgent nav_agent;

    protected Vector3 start_pos;
    protected Vector3 move_vect;
    protected Vector3 face_vect;
    protected float rotate_val;
    protected float state_timer = 0f;
    protected bool paused = false;
    protected EnemyVisionBase vision;
    public Dictionary<EnemyState, VisionState> stateChangeDict = new Dictionary<EnemyState, VisionState>
    {
        { EnemyState.Patrol, VisionState.Safe},
         { EnemyState.Chase, VisionState.Danger},
         { EnemyState.Confused, VisionState.Alert},
         { EnemyState.Notify, VisionState.Danger},
        { EnemyState.Alert, VisionState.Alert},
        { EnemyState.Wait, VisionState.Safe},
        { EnemyState.None, VisionState.Safe}
    };

    protected Vector3 move_target;
    protected Vector3 alert_target;
    protected float current_speed = 1f;
    protected Vector3 current_move;
    protected Vector3 current_rot_target;
    protected float current_rot_mult = 1f;
    protected bool waiting = false;
    protected float wait_timer = 0f;

    protected int current_path = 0;
    protected bool path_rewind = false;
    protected bool using_navmesh = false;

    protected Vector3 last_seen_pos;
    protected GameObject last_target;
    protected VisionTarget last_seen_target;
    protected float memory_timer = 0f;

    protected List<Vector3> path_list = new List<Vector3>();

    protected static List<EnemyAIBase> enemy_list = new List<EnemyAIBase>();
    public EnemyGroup boss;

    protected void Awake()
    {
        vision = this.GetComponent<EnemyVisionBase>();
        enemy_list.Add(this);
        rigid = GetComponent<Rigidbody>();
        nav_agent = GetComponent<NavMeshAgent>();
        move_vect = Vector3.zero;
        start_pos = transform.position;
        move_target = transform.position;
        current_rot_target = transform.position + transform.forward;
        alert_target = follow_target ? follow_target.transform.position : transform.position;
        last_seen_pos = transform.position;
        current_speed = move_speed;
        rotate_val = 0f;
        state = EnemyState.Patrol;

        boss = GetComponentInParent<EnemyGroup>();

        path_list.Add(transform.position);

        foreach (GameObject patrol in patrol_path)
        {
            if (patrol)
                path_list.Add(patrol.transform.position);
        }

        current_path = 0;
        if (path_list.Count >= 2)
            current_path = 1; //Dont start at start pos
    }

    protected void OnDestroy()
    {
        enemy_list.Remove(this);
    }

    protected void FixedUpdate()
    {
        if (paused)
            return;
        FixedUpdateMovement();
    }

    protected void Update()
    {
        if (paused)
            return;

        state_timer += Time.deltaTime;
        wait_timer += Time.deltaTime;
        UpdateStatus();
        ManualRotation();
    }

    /// <summary>
    /// Check fronted and groundeed, if navmeshAgent, set destination, other set movement speed.
    /// </summary>
    protected void FixedUpdateMovement()
    {
        bool fronted = CheckFronted(transform.forward);
        bool grounded = CheckGrounded(Vector3.down);
        Vector3 dist_vect = (move_target - transform.position);
        move_vect = dist_vect.normalized * current_speed * Mathf.Min(dist_vect.magnitude, 1f);
        if (use_pathfind && nav_agent && using_navmesh && dist_vect.magnitude > 1f)
        {
            nav_agent.enabled = true;
            nav_agent.speed = current_speed;
            nav_agent.SetDestination(move_target);
            rigid.velocity = Vector3.zero;
        }
        else
        {
            if (fronted)
                move_vect = Vector3.zero;
            if (!grounded)
                move_vect += Vector3.down * fall_speed;

            if (nav_agent && nav_agent.enabled)
                nav_agent.enabled = false;

            current_move = Vector3.MoveTowards(current_move, move_vect, move_speed * 10f * Time.fixedDeltaTime);
            rigid.velocity = current_move;
        }
    }


    virtual protected void UpdateStatus()
    {
        
    }

    /// <summary>
    /// If not controlled by agent and has a target, automatically rotate to target.
    /// </summary>
    virtual protected void ManualRotation()
    {
        bool controlled_by_agent = use_pathfind && nav_agent && nav_agent.enabled && using_navmesh && nav_agent.hasPath;
        rotate_val = 0f;

        if (!controlled_by_agent && state != EnemyState.None && state != EnemyState.Wait)
        {
            Vector3 dir = current_rot_target - transform.position;
            dir.y = 0f;
            if (dir.magnitude > 0.1f)
            {
                Quaternion target = Quaternion.LookRotation(dir.normalized, Vector3.up);
                Quaternion reachedRotation = Quaternion.RotateTowards(transform.rotation, target, rotate_speed * current_rot_mult * Time.deltaTime);
                rotate_val = Quaternion.Angle(transform.rotation, target);
                face_vect = dir.normalized;
                transform.rotation = reachedRotation;
            }
        }
    }

    virtual protected void UpdateAlert()
    {
        
    }

    virtual protected void UpdateConfused()
    {
        
    }

    virtual protected void UpdatePatrol()
    {
        
    }


    virtual protected void UpdateFollow()
    {
        
        
    }

    virtual protected void UpdateNotify()
    {}

    //---- Patrol -----

    protected void RewindPath()
    {
        path_rewind = !path_rewind;
        current_path += path_rewind ? -1 : 1;
        current_path = Mathf.Clamp(current_path, 0, path_list.Count - 1);
    }

    protected void GoToNextPath()
    {

        if (type == EnemyPatrolType.Loop)
        {
            current_path = (current_path + 1) % path_list.Count;
            current_path = Mathf.Clamp(current_path, 0, path_list.Count - 1);
        }
        else
        {
            if (current_path <= 0 || current_path >= path_list.Count - 1)
                path_rewind = !path_rewind;
            current_path += path_rewind ? -1 : 1;
            current_path = Mathf.Clamp(current_path, 0, path_list.Count - 1);
        }
    }

    //---- Chase -----

    public void SetAlertTarget(Vector3 pos)
    {
        alert_target = pos;
    }

    public void SetFollowTarget(GameObject atarget)
    {
        follow_target = atarget;
        if (follow_target != null)
        {
            last_seen_pos = follow_target.transform.position;
            memory_timer = 0f;
        }
    }

    //---- Actions -----

    public void Alert(Vector3 pos)
    {
        if (state != EnemyState.Chase)
        {
            ChangeState(EnemyState.Alert);
            SetAlertTarget(pos);
            StopMove();
        }
    }

    public void Notify(Vector3 pos)
    {
        ChangeState(EnemyState.Notify);
        SetAlertTarget(pos);
    }

    public void Follow(GameObject target)
    {
        ChangeState(EnemyState.Chase);
        SetFollowTarget(target);
        using_navmesh = true;
    }

    public void MoveTo(Vector3 pos, float speed = 1f)
    {
        move_target = pos;
        current_speed = speed;
        using_navmesh = true;
    }
    
    public void FaceToward(Vector3 pos, float speed_mult = 1f)
    {
        current_rot_target = pos;
        current_rot_mult = speed_mult;
    }

    public void FaceTo(Vector3 pos)
    {
        //TODO do not hard code the transform time!
        transform.DOLookAt(new Vector3(pos.x,transform.position.y,pos.z), 1);
    }

    public void StopMove()
    {
        using_navmesh = false;
        move_target = rigid.position;
        current_move = Vector3.zero;
        rigid.velocity = Vector3.zero;
        if (nav_agent && nav_agent.enabled)
            nav_agent.ResetPath(); //Cancel previous path
    }

    public void Kill()
    {
        if (onDeath != null)
            onDeath.Invoke();

        Destroy(gameObject);
    }

    public void ChangeState(EnemyState state)
    {

        this.state = state;
        Debug.Log("Change state to: " + state);
        vision.ResetTimer();
        if (stateChangeDict.Count != 0)
            this.visionState = stateChangeDict[GetState()];
        else { this.visionState = VisionState.Safe; }
        state_timer = 0f;
        wait_timer = 0f;
        waiting = false;
    }

    public void Pause()
    {
        paused = true;
    }

    public void UnPause()
    {
        paused = false;
    }

    //---- Check state -----

    public bool CheckFronted(Vector3 dir)
    {
        Vector3 origin = transform.position + Vector3.up * 1f;
        RaycastHit hit;
        bool success = Physics.Raycast(new Ray(origin, dir.normalized), out hit, dir.magnitude, obstacle_mask.value);
        return success && (follow_target == null || !hit.collider.transform.IsChildOf(follow_target.transform));
    }

    public bool CheckGrounded(Vector3 dir)
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        RaycastHit hit;
        return Physics.Raycast(new Ray(origin, dir.normalized), out hit, dir.magnitude, obstacle_mask.value);
    }

    protected void CheckIfFacingReachedTarget(Vector3 targ)
    {
        //Check if reached target
        Vector3 dist_vect = (targ - transform.position);
        dist_vect.y = 0f;
        float dot = Vector3.Dot(transform.forward, dist_vect.normalized);
        if (dot > 0.99f)
        {
            waiting = true;
            wait_timer = 0f;
        }
    }

    //---- Getters ------

    public bool HasReachedTarget()
    {
        Vector3 targ = follow_target ? follow_target.transform.position : last_seen_pos;
        if (state == EnemyState.Alert)
            targ = alert_target;
        return (targ - transform.position).magnitude < 0.5f;
    }

    protected Vector3 GetRandomLookTarget()
    {
        Vector3 center = transform.position;
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
        return center + offset;
    }

    public EnemyState GetState()
    {
        return state;
    }

    public float GetStateTimer()
    {
        return state_timer;
    }

    public Vector3 GetMove()
    {
        return move_vect;
    }

    public Vector3 GetFacing()
    {
        return face_vect;
    }

    public float GetRotationVelocity()
    {
        return rotate_val;
    }

    public bool IsRunning()
    {
        return state == EnemyState.Chase;
    }

    public Vector3 GetNextTarget()
    {
        if (use_pathfind && nav_agent && nav_agent.enabled && using_navmesh && nav_agent.hasPath)
            return nav_agent.nextPosition;
        return move_target;
    }

    public bool IsPaused()
    {
        return paused;
    }

    public static EnemyAIBase GetNearest(Vector3 pos, float range = 999f)
    {
        float min_dist = range;
        EnemyAIBase nearest = null;
        foreach (EnemyAIBase enemy in enemy_list)
        {
            float dist = (enemy.transform.position - pos).magnitude;
            if (dist < min_dist)
            {
                min_dist = dist;
                nearest = enemy;
            }
        }
        return nearest;
    }

    public static List<EnemyAIBase> GetAllInRange(Vector3 pos, float range)
    {
        List<EnemyAIBase> range_list = new List<EnemyAIBase>();
        foreach (EnemyAIBase enemy in enemy_list)
        {
            float dist = (enemy.transform.position - pos).magnitude;
            if (dist < range)
            {
                range_list.Add(enemy);
            }
        }
        return range_list;
    }

    public static List<EnemyAIBase> GetAll()
    {
        return enemy_list;
    }

    public void changeVisionState(VisionState vs)
    {
        visionState = vs;
    }

    //----- Debug Gizmos -------

    [ExecuteInEditMode]
    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 prev_pos = transform.position;


            foreach (GameObject patrol in patrol_path)
            {
                if (patrol)
                {
                    Gizmos.DrawLine(prev_pos, patrol.transform.position);
                    prev_pos = patrol.transform.position;
                }
            }

            if (type == EnemyPatrolType.Loop)
                Gizmos.DrawLine(prev_pos, transform.position);



    }
}