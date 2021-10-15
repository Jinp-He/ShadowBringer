using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SnakeBody : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
       // Debug.Log("Hi I am a snakebody");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Dead()
    {
        Destroy(this.gameObject);
    }

}
