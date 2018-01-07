using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OnDestroy : MonoBehaviour {

	public UnityEvent _event;

	public void Destroying()
	{
		_event.Invoke();
		GetComponent<Block>().destruct = false;
	}

	public void RemoveObject()
	{
		Destroy(gameObject);
	}
}
