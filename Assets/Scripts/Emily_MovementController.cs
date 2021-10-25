using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Emily_MovementController : MonoBehaviour
{
    Camera MapCamera;
    NavMeshAgent agent;
        // Start is called before the first frame update
    void Start()
    {
        MapCamera = Camera.main;
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = MapCamera.ScreenPointToRay(Input.mousePosition);
            
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity) && hit.transform.gameObject.tag == "Terrain")
            {
                Debug.Log(hit.point);
                agent.SetDestination(hit.point);
            }
        }
    }
}
