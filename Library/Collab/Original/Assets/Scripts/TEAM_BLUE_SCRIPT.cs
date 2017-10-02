using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEAM_BLUE_SCRIPT : MonoBehaviour
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
    int i = 0;
    /// <summary>
    /// ^^^^^^^^
    /// </summary>
    void Start()
    {
        // character1.FaceClosestWaypoint();


    }



    void Update()
	{
        character2.MoveChar(new Vector3());
        character3.MoveChar(new Vector3());
        //character1.FaceClosestWaypoint();
        //character2.FaceClosestWaypoint();
        //character3.FaceClosestWaypoint();
        //character1.MoveChar(new Vector3(-21.5f, 2f, 19.3f));
        //if (i == 0) {
        //character2.MoveChar(character2.FindClosestCover(character1.transform.position));
        //  i++;
        //}
        //character3.MoveChar(new Vector3(29.0f, 0f, 10f));

        //		GetComponent<PathFinder>().location = position;

    }

    
}
