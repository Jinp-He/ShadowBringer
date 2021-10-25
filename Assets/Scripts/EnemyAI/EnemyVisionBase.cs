using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



/// <summary>
/// Manages enemy detection of the player, will also spawn a VisionCone and change the state of Enemy.cs based on what is seen
/// </summary>

public class EnemyVisionBase : MonoBehaviour
{
    [Header("Detection")]
    public float vision_angle = 30f;
    public float vision_range = 10f;
    public float vision_near_range = 5f;
    public float vision_height_above = 1f; //How far above can they detect
    public float vision_height_below = 10f; //How far below can they detect
    public float touch_range = 1f;
    public LayerMask vision_mask = ~(0);
    
    [Header("Alert")]
    public float detect_time = 1f;
    public float alerted_time = 10f;

    [Header("Chase")]
    public float follow_time = 10f;
    public bool dont_return = false; 

    [Header("Ref")]
    public Transform eye;
    public GameObject vision_prefab;
    public GameObject death_fx_prefab;

    protected EnemyAIBase enemy;

    
    protected VisionCone vision;

    protected float detect_timer = 0f;
    protected float wait_timer = 0f;

    protected bool isSeen;
    protected VisionTarget seen_character = null;
    protected float seen_timer = 0f;
    protected float unseen_timer = 0f;

    protected static List<EnemyVisionBase> enemy_list = new List<EnemyVisionBase>();

    protected void Awake()
    {
        enemy_list.Add(this);
        enemy = GetComponent<EnemyAIBase>();
        isSeen = false;
    }

    protected void OnDestroy()
    {
        enemy_list.Remove(this);
    }

    protected void Start()
    {

        //Initialize vision cone
        GenerateVisionCone();
        seen_character = null;
    }

    private void GenerateVisionCone()
    {
        GameObject vis = Instantiate(vision_prefab, GetEye(), Quaternion.identity);
        vis.transform.parent = transform;
        vision = vis.GetComponent<VisionCone>();
        vision.target = this;
        vision.vision_angle = vision_angle;
        vision.vision_range = vision_range;
        vision.vision_near_range = vision_near_range;
    }

    void Update()
    {
        updateVisionTarget();
    }

    private void updateVisionTarget()
    {
        VisionTarget _target = DetectVisionTarget();
       
        // TODO: change the logic so that it allow a temp lost of target.
        if (_target == null) //lost target or cannot find target
        {
            isSeen = false;
            seen_character = null;
            unseen_timer += Time.deltaTime;
            Debug.Log(": " + unseen_timer);
            seen_timer = 0f;
        }
        else 
        {
            //Debug.Log("Find target" + _target.transform.position);
            isSeen = true;
            seen_character = _target;
            unseen_timer = 0f;
            seen_timer += Time.deltaTime;
        }
    }




    /// <summary>
    /// Get the visionState of the enemy, decide the color of the vision cone
    /// </summary>
    /// <returns></returns>
    public VisionState GetVisionState()
    {
        return enemy.visionState;
    }

    public float GetSeenTimer()
    {
        return seen_timer; 
    }

    public float GetUnSeenTimer()
    {
        return unseen_timer;
    }

    public bool IsSeen() { return isSeen; }

    public VisionTarget GetTarget() { return seen_character; }

    public void ResetTimer()
    {
        seen_timer = 0f;
        unseen_timer = 0f;
    }


    /// <summary>
    /// Check if the vision see the visiontarget, if see the visiontarget
    /// </summary>
    /// <returns></returns>

    /// <summary>
    /// Look for specific target
    /// </summary>
    protected VisionTarget DetectVisionTarget()
    {
        if (wait_timer > 0f)
            return null;

        //Detect character
        foreach (VisionTarget character in VisionTarget.GetAll())
        {
            if (CanSeeVisionTarget(character))
            {
                return character;
            }
        }
        return null;
    }

    //Can see any vision target
    public VisionTarget CanSeeAnyVisionTarget()
    {
        foreach (VisionTarget character in VisionTarget.GetAll())
        {
            if (CanSeeVisionTarget(character))
            {
                return character;
            }
        }
        return null;
    }

    //Can the enemy see a vision target?
    public bool CanSeeVisionTarget(VisionTarget target)
    {
        return target != null && target.CanBeSeen()
            && (CanSeeObject(target.gameObject, vision_range, vision_angle) || CanTouchObject(target.gameObject));
    }

    public bool CanSeeVisionTargetNear(VisionTarget target)
    {
        return target != null && target.CanBeSeen()
            && (CanSeeObject(target.gameObject, vision_near_range, vision_angle) || CanTouchObject(target.gameObject));
    }

    //Can the enemy see an object ?
    public bool CanSeeObject(GameObject obj, float see_range, float see_angle)
    {
        Vector3 forward = transform.forward;
        Vector3 dir = obj.transform.position - GetEye();
        Vector3 dir_touch = dir; //Preserve Y for touch
        dir.y = 0f; //Remove Y for cone vision range

        float vis_range = see_range;
        float vis_angle = see_angle;
        float losangle = Vector3.Angle(forward, dir);
        float losheight = obj.transform.position.y - GetEye().y;
        bool can_see_cone = losangle < vis_angle / 2f && dir.magnitude < vis_range && losheight < vision_height_above && losheight > -vision_height_below;
        bool can_see_touch = dir_touch.magnitude < touch_range;
        if (obj.activeSelf && (can_see_cone || can_see_touch)) //In range and in angle
        {
            RaycastHit hit;
            bool raycast = Physics.Raycast(new Ray(GetEye(), dir.normalized), out hit, dir.magnitude, vision_mask.value);
            if (!raycast)
                return true; //No obstacles in the way (in case character not in layer)
            if (raycast && (hit.collider.gameObject == obj || hit.collider.transform.IsChildOf(obj.transform))) //See character
                return true; //The only obstacles is the character
        }
        return false;
    }

    //Is the enemy right next to the object ?
    public bool CanTouchObject(GameObject obj)
    {
        Vector3 dir = obj.transform.position - transform.position;
        if (dir.magnitude < touch_range) //In range and in angle
        {
            return true;
        }
        return false;
    }

    //Return seen distance of target: 0:touch,  1:near,  2:far,  3:other
    public int GetSeenDistance(GameObject target)
    {
        bool is_near = CanSeeObject(target, vision_near_range, vision_angle);
        bool is_touch = CanTouchObject(target);
        int distance = is_touch ? 0 : (is_near ? 1 : 2);
        return distance;
    }

    //Call this function from another script to manually alert of the target presense
    public void Alert(VisionTarget target)
    {
        if (target != null)
        {
            Alert(target.transform.position);
        }
    }

    //Alert with a position instead of object (such as noise)
    public void Alert(Vector3 target)
    {
        if (enemy != null)
            enemy.Alert(target);
        seen_timer = 0f;
    }

    //Call this function from another script to manually start chasing the target
    public void Chase(VisionTarget target)
    {
        if (target != null)
        {
            seen_character = target;
            seen_timer = 0f;
            if (enemy != null)
                enemy.Follow(seen_character.gameObject);
        }
    }

    public void Notify(VisionTarget target)
    {
        if (target != null)
        {
            enemy.Notify(target.transform.position);
        }
    }

    protected void ResumeDefault()
    {
        seen_character = null;
        wait_timer = 0f;
        if (enemy != null)
        {
            if (dont_return)
                enemy.ChangeState(EnemyState.Confused);
            else
                enemy.ChangeState(EnemyState.Patrol);
        }
    }

    //Stop detecting player for X seconds
    private void PauseVisionFor(float time)
    {
        wait_timer = time;
    }

    //Do nothing for X seconds
    private void WaitFor(float time)
    {
        wait_timer = time;
        if (enemy != null)
        {
            enemy.ChangeState(EnemyState.Wait);
            enemy.StopMove();
        }
    }

    private Vector3 GetEye()
    {
        return eye ? eye.position : transform.position;
    }

}


