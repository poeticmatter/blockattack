﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocksManager : MonoBehaviour {

	public static BlocksManager instance;

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
		
	}
	

	void Update () {
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			for (int x = 0; x < grid.GetLength(0); x++)
			{
				MoveUp(x);
			}
		} else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			for (int x = 0; x < grid.GetLength(0); x++)
			{
				MoveDown(x);
			}
		} else if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			for (int y = 0; y < grid.GetLength(1); y++)
			{
				MoveRight(y);
			}
		}
		else if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
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
			grid[x, y].Move(new Vector2(x, y + yDirection));
			grid[x, y + yDirection] = grid[x, y];
			grid[x, y] = null;
			MoveYRecursive(yDirection, minY, maxY, x, y + yDirection);
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
			grid[x, y].Move(new Vector2(x + xDirection, y));
			grid[x + xDirection, y] = grid[x, y];
			grid[x, y] = null;
			MoveXRecursive(xDirection, minX, maxX, x + xDirection, y);
		}
	}


}
