using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public PlayerController playerController;
    public BoardController boardController;
    // Start is called before the first frame update
    public int boardLen;
    void Awake()
    {
        boardLen = 9;
        playerController = GetComponent<PlayerController>();
        boardController = GetComponent<BoardController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetPos(Vector3 vec) 
    {
        if (vec == null) { return -1; }
        return  vec.x + vec.y * boardLen + vec.z * boardLen * boardLen;
    }
}
