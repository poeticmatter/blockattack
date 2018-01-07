using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocksManager : MonoBehaviour {

	public static BlocksManager instance;
	public float inputDelay;
	private float timeSinceLastInput;
	public Block center;

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
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			timeSinceLastInput = inputDelay;
			for (int x = 0; x < grid.GetLength(0); x++)
			{
				MoveUp(x);
			}
		} else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			timeSinceLastInput = inputDelay;
			for (int x = 0; x < grid.GetLength(0); x++)
			{
				MoveDown(x);
			}
		} else if (Input.GetKeyDown(KeyCode.RightArrow))
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
	}

	private void MergeMoveIfPossible(int xFrom, int xTo, int yFrom, int yTo)
	{
		if (grid[xFrom, yFrom].value < 0 || grid[xFrom, yFrom].value == grid[xTo, yTo].value)
		{

			grid[xTo, yTo].value += grid[xFrom, yFrom].value;
			grid[xFrom, yFrom].Move(new Vector2(xTo, yTo));
			grid[xFrom, yFrom].destruct = true;
		}
	}
}
