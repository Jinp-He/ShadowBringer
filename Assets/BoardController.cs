using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{

    public int boardLen;
    public GameObject food_prefab;
    public GameObject food;
    // Start is called before the first frame update
    void Awake()
    {
        food = null;
        boardLen = 9;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.FindGameObjectWithTag("Food") == null) { GenerateFood();}
        
    }

    void GenerateFood()
    {
        int x = Random.Range(0, boardLen-2);
        int y = Random.Range(0, boardLen-2);
        int z = Random.Range(0, boardLen-2);
        food = Instantiate(food_prefab, new Vector3(x - boardLen/2, y - boardLen / 2, z - boardLen / 2), this.transform.rotation);
    }

    public float GetFoodPos()
    {
        if (food == null) { return -1; }
        return GetComponent<GameController>().GetPos(food.transform.position);
    }

    public void isEaten()
    {
        Destroy(food);
        food = null;
    }

}
