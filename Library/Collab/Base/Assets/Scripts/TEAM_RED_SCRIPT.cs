using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEAM_RED_SCRIPT : MonoBehaviour
{
	private Vector3 position = new Vector3(20.0f, 0.0f, 20.0f);

    /// <summary>
    /// DO NOT MODIFY THIS! 
    /// vvvvvvvvv
    /// </summary>
    [SerializeField]
    private CharacterScript character1;
    [SerializeField]
    private CharacterScript character2;
    [SerializeField]
    private CharacterScript character3;
    /// <summary>
    /// ^^^^^^^^
    /// </summary>
    void Start()
    {
       // character1.FaceClosestWaypoint();

    }



    void Update()
	{
        ////character1.FaceClosestWaypoint();
        ////character2.FaceClosestWaypoint();
        ////character3.FaceClosestWaypoint();
        //character1.MoveChar(new Vector3(21.5f, 2f, -19.3f));
        //character2.MoveChar(new Vector3(29f, 2f, 30f));
        //character3.MoveChar(new Vector3(0.0f, 0f, 0f));
        ////		GetComponent<PathFinder>().location = position;

    }

    
}
