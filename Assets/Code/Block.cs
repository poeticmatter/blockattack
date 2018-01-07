using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour {

	public Vector2 destination;
	private Vector2 velocity = Vector2.zero;
	public float maxVelocity;
	public float smoothTime;
	public int minY;
	public int maxY;
	public int minX;
	public int maxX;
	public Text text;

	public int _value;
	public int value
	{
		get	{ return _value; }
		set {
			_value = value;
			text.text = ""+value;
			text.fontSize = value > 99 ? 16 : 32;
		}
	}
	public bool destruct = false;

	void Start () {
		BlocksManager.instance.grid[(int)destination.x, (int)destination.y] = this;
		value = value;
	}
	
	
	void Update () {
		transform.position = Vector2.SmoothDamp(transform.position, destination, ref velocity, smoothTime, maxVelocity, Time.deltaTime);
		if (destruct && (((Vector2)transform.position) - destination).magnitude < 0.1)
		{
			Destroy(gameObject);
		}
	}

	public void Move(Vector2 destination)
	{
		this.destination = destination;
	}
}
