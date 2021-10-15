using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
	public GameObject SnakeBody;
	public GameObject SnakeHead;
	private Vector3 prevVector;
	Vector3 movementVector;
	public KeyCode upKey, downKey, leftKey, rightKey, turnRightKey, turnLeftKey;
	private Queue<GameObject> snake;
	public float moveTimer = 0.5f;
	Camera camera;
	public float transform_time;
	bool isRotate;
	bool isEat;
	HashSet<float> set;
	enum State
	{
		South = 0,
		West = 1,
		North = 2,
		East = 3
	}
	State state;
	// Start is called before the first frame update
	void Awake()
	{
		set = new HashSet<float>();
		isEat = false;
		transform_time = 1f;
		   state = State.South;
		camera = Camera.main;
		isRotate = false;
		snake = new Queue<GameObject>();
		SetKey();
		GameObject _sb = Instantiate(SnakeBody, this.transform);
		SnakeHead = _sb;
		snake.Enqueue(_sb);
		prevVector = new Vector3(0, 1, 0);
		GetMovementVector();
		Move();
	}

	public void Initialize()
	{


	}

	public void SetKey()
	{
		upKey = KeyCode.W;
		downKey = KeyCode.S;
		leftKey = KeyCode.A;
		rightKey = KeyCode.D;
		turnRightKey = KeyCode.E;
		turnLeftKey = KeyCode.Q;
	}

	// Update is called once per frame
	void Update()
	{
		UpdateInput();
	}

	void UpdateInput()
	{
		GetMovementVector();
		//Move(movementVector);
	}

	Vector3 GetMovementVector()
	{
		if (Input.GetKey(downKey)) { movementVector = new Vector3(0, -1, 0); }
		else if (Input.GetKey(upKey)) { movementVector = new Vector3(0, 1, 0); }
		else if (Input.GetKey(leftKey)) 
		{
			switch (state)
			{
				case State.South:
					movementVector = new Vector3(-1, 0, 0);
					if (movementVector + prevVector == Vector3.zero) { movementVector = prevVector; }
					break;
				case State.North:
					movementVector = new Vector3(1, 0, 0);
					if (movementVector + prevVector == Vector3.zero) { movementVector = prevVector; }
					break;
				case State.West:
					movementVector = new Vector3(0, 0, 1);
					if (movementVector + prevVector == Vector3.zero) { movementVector = prevVector; }
					break;
				case State.East:
					movementVector = new Vector3(0, 0, -1);
					if (movementVector + prevVector == Vector3.zero) { movementVector = prevVector; }
					break;
			}
		}
		else if (Input.GetKey(rightKey)) 
		{
			switch (state)
			{
				case State.South:
					movementVector = new Vector3(1, 0, 0);
					if (movementVector + prevVector == Vector3.zero) { movementVector = prevVector; }
					break;
				case State.North:
					movementVector = new Vector3(-1, 0, 0);
					if (movementVector + prevVector == Vector3.zero) { movementVector = prevVector; }
					break;
				case State.West:
					movementVector = new Vector3(0, 0, -1);
					if (movementVector + prevVector == Vector3.zero) { movementVector = prevVector; }
					break;
				case State.East:
					movementVector = new Vector3(0, 0, 1);
					if (movementVector + prevVector == Vector3.zero) { movementVector = prevVector; }
					break;
			}
		}
		else { movementVector = prevVector; }
		if (!isRotate)
		{
			if (Input.GetKey(turnLeftKey))
			{
				state++;
				state = (State)((int)state % 4);
				camera.gameObject.transform.DORotate(new Vector3(camera.gameObject.transform.eulerAngles.x, (camera.gameObject.transform.eulerAngles.y + 90), camera.gameObject.transform.eulerAngles.z), transform_time).
					OnComplete(() =>
					{
						isRotate = false;
					}
					);
				isRotate = true;
				switch (state)
				{
					case State.South:
						camera.gameObject.transform.DOMove(transform.position + new Vector3(0,0,-10), transform_time);
						break;
					case State.North:
						camera.gameObject.transform.DOMove(transform.position + new Vector3(0, 0, 10), transform_time);
						break;
					case State.West:
						camera.gameObject.transform.DOMove(transform.position + new Vector3(-10, 0, 0), transform_time);
						break;
					case State.East:
						camera.gameObject.transform.DOMove(transform.position + new Vector3(10, 0, 0), transform_time);
						break;
				}
			}
			else if (Input.GetKey(turnRightKey))
			{
				state += 3;
				state = (State)((int)state % 4);
				camera.gameObject.transform.DORotate(new Vector3(camera.gameObject.transform.eulerAngles.x, (camera.gameObject.transform.eulerAngles.y - 90), camera.gameObject.transform.eulerAngles.z), transform_time).
					OnComplete(() =>
					{
				  isRotate = false; 
					 }); 
				isRotate = true;
				switch (state)
				{
					case State.South:
						camera.gameObject.transform.DOMove(transform.position + new Vector3(0, 0, -10), transform_time);
						break;
					case State.North:
						camera.gameObject.transform.DOMove(transform.position + new Vector3(0, 0, 10), transform_time);
						break;
					case State.West:
						camera.gameObject.transform.DOMove(transform.position + new Vector3(-10, 0, 0), transform_time);
						break;
					case State.East:
						camera.gameObject.transform.DOMove(transform.position + new Vector3(10, 0, 0), transform_time);
						break;
				}

			}
		}
		//else if (Input.GetKey(turnLeftKey)) { movementVector = new Vector3(0, 1, 0); }
		prevVector = movementVector;
		//Debug.Log("MovementVector is: " + movementVector);
		return movementVector;

	}

	private void Move()
	{
		Invoke("Move", moveTimer);
		GameObject _sb = Instantiate(SnakeBody, SnakeHead.transform.position + movementVector, this.transform.rotation);
		Vector3 ceiling = new Vector3(Mathf.RoundToInt(_sb.transform.position.x), Mathf.RoundToInt(_sb.transform.position.y), Mathf.RoundToInt(_sb.transform.position.z));
		float pos = GetComponent<GameController>().GetPos(ceiling);
		if (set.Contains(pos)) { Defeat(); }
		set.Add(pos);
		
		if (pos == GetComponent<BoardController>().GetFoodPos()) { isEat = true; GetComponent<BoardController>().isEaten(); }
		//Debug.Log("Snakehead position: " + _sb.transform.position + movementVector);
		SnakeHead = _sb;
		snake.Enqueue(_sb);
		if (isEat) { isEat = false; return; }
		GameObject _removed = snake.Dequeue();
		
		ceiling = new Vector3(Mathf.RoundToInt(_removed.transform.position.x), Mathf.RoundToInt(_removed.transform.position.y), Mathf.RoundToInt(_removed.transform.position.z));
		Debug.Log(GetComponent<GameController>().GetPos(ceiling));
		set.Remove(GetComponent<GameController>().GetPos(ceiling));
		_removed.GetComponent<SnakeBody>().Dead();
	}

	private void Defeat()
	{
		Debug.Log("You lose");
		CanvasGroup canvasGroup = GameObject.FindGameObjectWithTag("EndingImage").GetComponent<CanvasGroup>();
		canvasGroup.DOFade(1, 2f).OnComplete(()=> { SceneManager.LoadScene(0); });
		
	}
	




}
