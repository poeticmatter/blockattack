using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BlocksManager : MonoBehaviour {

	public static BlocksManager instance;
	public float inputDelay;
	private float timeSinceLastInput;
	public Block center;
	public Block attackerPrefab;
	public Block defenderPrefab;
	private int enemySpawn = 1;
	private int enemySpawnStrength = -1;
	private const int INPUT_MODE = 0;
	private const int SPAWN_MODE = 1;
	private const int GAME_OVER = 2;
	private int mode = 0;
	private int turnCounter = 100;
	public Text turnText;
	public Text gameOverText;

	void Awake ()
	{
		if (instance != null)
		{
			Debug.LogError("More than one BlockManager");
		}
		instance = this;
	}

	public Block[,] grid = new Block [8,8];


	void Start () {
		RegisterCenter();
		SpawnDefenders();
	}

	private void RegisterCenter ()
	{

		grid[3, 3] = center;
		grid[3, 4] = center;
		grid[4, 3] = center;
		grid[4, 4] = center;
	}
	

	void Update () {
		if (timeSinceLastInput > 0)
		{
			timeSinceLastInput -= Time.deltaTime;
			return;
		}
		Debug.Log(mode);
		if (mode == INPUT_MODE)
		{
			InputMode();

		} else if (mode == SPAWN_MODE)
		{	
			mode = INPUT_MODE;
			SpawnDefenders();
			SpawnAttackers();
			turnCounter++;
			turnText.text = "Turn: " + turnCounter;
			enemySpawn = (int)Mathf.Log(turnCounter);

		} else
		{
			gameOverText.enabled = true;
			if (Input.anyKeyDown)
			{
				SceneManager.LoadScene(0);
			}
		}
		

	}

	private void InputMode()
	{
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			timeSinceLastInput = inputDelay;
			for (int x = 0; x < grid.GetLength(0); x++)
			{
				MoveUp(x);
			}
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			timeSinceLastInput = inputDelay;
			for (int x = 0; x < grid.GetLength(0); x++)
			{
				MoveDown(x);
			}
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			timeSinceLastInput = inputDelay;
			for (int y = 0; y < grid.GetLength(1); y++)
			{
				MoveRight(y);
			}
		}
		else if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			timeSinceLastInput = inputDelay;
			for (int y = 0; y < grid.GetLength(1); y++)
			{
				MoveLeft(y);
			}
		}
	}


	private void MoveUp(int x)
	{
		for (int y = grid.GetLength(1)-1; y >= 0; y--)
		{
			MoveYRecursive(1, 0, grid.GetLength(1)-1, x, y);
		}
	}

	private void MoveDown(int x)
	{
		for (int y = 0; y < grid.GetLength(1); y++)
		{
			MoveYRecursive(-1, 0, grid.GetLength(1)-1, x, y);
		}
	}

	private void MoveYRecursive(int yDirection, int minY, int maxY, int x, int y)
	{
		if (y < minY || y > maxY)
		{
			return;
		}
		if(grid[x, y] == null)
		{
			return;
		}
		maxY = grid[x, y].maxY;
		minY = grid[x, y].minY;
		if (y + yDirection < minY || y + yDirection > maxY)
		{
			return;
		}
		if (grid[x, y + yDirection] == null)
		{
			ActuateMove(x, x, y, y + yDirection);
			MoveYRecursive(yDirection, minY, maxY, x, y + yDirection);
		}
		else
		{
			MergeMoveIfPossible(x, x, y, y + yDirection);
		}
	}

	private void MoveRight(int y)
	{
		for (int x = grid.GetLength(0) - 1; x >= 0; x--)
		{
			MoveXRecursive(1, 0, grid.GetLength(0) - 1, x, y);
		}
	}

	private void MoveLeft(int y)
	{
		for (int x = 0; x < grid.GetLength(0); x++)
		{
			MoveXRecursive(-1, 0, grid.GetLength(0) - 1, x, y);
		}
	}

	private void MoveXRecursive(int xDirection, int minX, int maxX, int x, int y)
	{
		if (x < minX || x > maxX)
		{
			return;
		}
		if (grid[x, y] == null)
		{
			return;
		}
		maxX = grid[x, y].maxX;
		minX = grid[x, y].minX;
		if (x + xDirection < minX || x + xDirection > maxX)
		{
			return;
		}
		if (grid[x + xDirection, y] == null)
		{
			
			ActuateMove(x, x + xDirection, y, y);
			MoveXRecursive(xDirection, minX, maxX, x + xDirection, y);
		} else
		{
			MergeMoveIfPossible(x, x + xDirection, y, y);
		}
	}

	private void ActuateMove(int xFrom, int xTo, int yFrom, int yTo)
	{
		grid[xFrom, yFrom].Move(new Vector2(xTo, yTo));
		grid[xTo, yTo] = grid[xFrom, yFrom];
		grid[xFrom, yFrom] = null;
		mode = SPAWN_MODE;
	}

	private void MergeMoveIfPossible(int xFrom, int xTo, int yFrom, int yTo)
	{
		bool attacker = grid[xFrom, yFrom].value < 0;
		if (attacker && grid[xTo, yTo].value < 0)
		{
			return;
		}
		if ( attacker || grid[xFrom, yFrom].value == grid[xTo, yTo].value)
		{
			grid[xTo, yTo].value += grid[xFrom, yFrom].value;
			if (grid[xTo, yTo].value <= 0)
			{
				grid[xTo, yTo].destruct = true;
			}
			grid[xFrom, yFrom].Move(new Vector2(xTo, yTo));
			grid[xFrom, yFrom].destruct = true;
			mode = SPAWN_MODE;
		}
	}

	private void SpawnAttackers()
	{
		int[] xPositions = new int[]
		{
			0, 1, 2, 3, 4, 5, 6, 7
		};
		int[] yPositions = new int[]
		{
			0, 1, 2, 3, 4, 5, 6, 7
		};
		int enemySpawned = 0;
		bool spawned;
		while (enemySpawned < enemySpawn)
		{
			RandomizeArray(xPositions);
			RandomizeArray(yPositions);
			spawned = false;
			for (int x = 0; x < xPositions.Length; x++)
			{
				for (int y = 0; y < yPositions.Length; y++)
				{
					if (xPositions[x] == 0 || yPositions[y] == 0 || xPositions[x] == 7 || yPositions[y] == 7)
					{
						if (grid[xPositions[x], yPositions[y]] == null)
						{
							Spawn(attackerPrefab, xPositions[x], yPositions[y], enemySpawnStrength);
							spawned = true;
							break;
						}
					}
				}
				if (spawned) break;
			}
			if (spawned)
				enemySpawned++;
			else
				break;
		}
	}
	private void SpawnDefenders()
	{
		int[] xPositions = new int[]
		{
			1, 2, 3, 4, 5, 6
		};
		int[] yPositions = new int[]
		{
			1, 2, 3, 4, 5, 6
		};
		int defendersSpawned = 0;
		bool spawned;
		while (defendersSpawned < 2)
		{
			RandomizeArray(xPositions);
			RandomizeArray(yPositions);
			spawned = false;
			for (int x = 0; x < xPositions.Length; x++)
			{
				for (int y = 0; y < yPositions.Length; y++)
				{
					if (grid[xPositions[x],yPositions[y]] == null)
					{
						Spawn(defenderPrefab, xPositions[x], yPositions[y], 1);
						spawned = true;
						break;
					}
				}
				if (spawned) break;
			}
			if (spawned)
				defendersSpawned++;
			else
				break;
		}
	}

	private void Spawn(Block prefab, int x, int y, int value)
	{
		Block instance = (Block)Instantiate(prefab);
		grid[x, y] = instance;
		instance.transform.position = new Vector2(x, y);
		instance.Move(new Vector2(x, y));
		instance.value = value;
		int min = value > 0 ? 1 : 0;
		int max = value > 0 ? 6 : 7;
		instance.minX = x <= 3 ? min : 4;
		instance.maxX = x <= 3 ? 3 : max;
		instance.minY = y <= 3 ? min : 4;
		instance.maxY = y <= 3 ? 3 : max;
		
	}

	private void RandomizeArray(int [] array)
	{
		for (var i = array.Length - 1; i > 0; i--)
		{
			var r = Random.Range(0, i);
			var tmp = array[i];
			array[i] = array[r];
			array[r] = tmp;
		}
	}

	public void GameOver()
	{
		Debug.Log("Game Over");
		mode = GAME_OVER;
	}

}
