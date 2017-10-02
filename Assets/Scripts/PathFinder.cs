using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
	UnityEngine.AI.NavMeshAgent player;
	public Vector3 destination;

	public void MoveChar(Vector3 position)
	{
		player.destination = position;
	}

	// Use this for initialization
	void Start()
	{
		player = GetComponent<UnityEngine.AI.NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		player.updateRotation = false;
		if( destination != player.destination )
			MoveChar(destination);
	}
}
