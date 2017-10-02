using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//---------- CHANGE THIS NAME HERE -------
public class TEAM_BLUE_SCRIPT : MonoBehaviour
{
    //---------- CHANGE THIS NAME HERE -------
    public static TEAM_BLUE_SCRIPT AddYourselfTo(GameObject host) {
        //---------- CHANGE THIS NAME HERE -------
        return host.AddComponent<TEAM_BLUE_SCRIPT>();
    }

    /*vvvv DO NOT MODIFY vvvvv*/
    [SerializeField]
    public CharacterScript character1;
    [SerializeField]
    public CharacterScript character2;
    [SerializeField]
    public CharacterScript character3;

    void Start()
    {
        character1 = transform.Find("Character1").gameObject.GetComponent<CharacterScript>();
        character2 = transform.Find("Character2").gameObject.GetComponent<CharacterScript>();
        character3 = transform.Find("Character3").gameObject.GetComponent<CharacterScript>();
    }
    /*^^^^ DO NOT MODIFY ^^^^*/

    /* Your code below this line */
    // Update() is called every frame
    void Update()
	{
        // Debug.Log(character1.name + " " + character1.);
        //character1.FaceClosestWaypoint();
        //character1.SetFacing(new Vector3(-8f, 0, 8f));
        //character2.FaceClosestWaypoint();
        //character3.FaceClosestWaypoint();

        character1.rotateAngle(500);
        character2.rotateAngle(500);
        character3.rotateAngle(500);

        character1.MoveChar(new Vector3());
        character2.MoveChar(new Vector3(40.0f, 1.5f, 24.0f));
        character3.MoveChar(new Vector3(-40.0f, 1.5f, -24.0f));



    } 
}
