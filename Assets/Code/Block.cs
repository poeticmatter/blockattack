using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

	public Vector2 destination;
	private Vector2 velocity = Vector2.zero;
	public float maxVelocity;
	public float smoothTime;
	public int minY;
	public int maxY;
	public int minX;
	public int maxX;

	void Start () {
		BlocksManager.instance.grid[(int)destination.x, (int)destination.y] = this;
	}
	
	
	void Update () {
		transform.position = Vector2.SmoothDamp(transform.position, destination, ref velocity, smoothTime, maxVelocity, Time.deltaTime);
	}

	public void Move(Vector2 destination)
	{
		this.destination = destination;
	}
}
