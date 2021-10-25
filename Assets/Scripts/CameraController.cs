using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadowBringer
{
    public class CameraController : MonoBehaviour
    {
        GameController gameController;
        Camera mapCamera;

        public Vector3 Offset;
        public float scale;

        public const float orthorSize = 5.5f;
        // Start is called before the first frame update
        void Awake()
        {
            gameController = GetComponent<GameController>();
            mapCamera = Camera.main;

            Offset = new Vector3(7, 9, -6);
            scale = 0f;
        }

        void Update()
        {
            GameObject _player = gameController.GetRecentPlayer();
            mapCamera.transform.position = _player.transform.position + Offset;
            mapCamera.orthographicSize = orthorSize + scale;
           
        }
    }
}
