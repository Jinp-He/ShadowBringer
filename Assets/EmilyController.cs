using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadowBringer 
{
    public class EmilyController : PlayerControllerBase
    {
        // Start is called before the first frame update
        new void Awake()
        {
            base.Awake();
            gameController.EnterEmilyMode += EnterEmilyMode;
            gameController.EnterWatsonMode += EnterWatsonMode;
        }

        // Update is called once per frame
        void Update()
        {
            if (gameController.IsPaused) { return; }
            if (!gameController.IsEmilyMode) { return; }
            UpdateMove();
        }

        void UpdateMove()
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

        void EnterEmilyMode()
        {
            playerIndicator.SetActive(true);
        }

        public void EnterWatsonMode()
        {
            playerIndicator.SetActive(false);
        }
    }
}
