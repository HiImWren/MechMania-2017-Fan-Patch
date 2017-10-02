using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum typeOfItem
{
    health,
    points,
    speed,
    power
}

public class ItemScript : MonoBehaviour {

    private float rotateSpeed = 30.0f;
   // private ItemEvent giveItem = new ItemEvent();
    private typeOfItem myItem;

   // private GameManager gm;

	// Use this for initialization
	void Start () {
		if(this.name == "Points")
        {
            myItem = typeOfItem.points;
        }
        else if (this.name == "SpeedUp")
        {
            myItem = typeOfItem.speed;
        }
        else if (this.name == "HealthPack")
        {
            myItem = typeOfItem.health;
        }
        else if (this.name == "Power")
        {
            myItem = typeOfItem.power;
        }

        //gm = GameObject.Find("Main Camera").GetComponent<GameManager>();
        //giveItem.AddListener(nullping);
        //gm.addItemListener(giveItem);
    }
	private void nullping(typeOfItem t, CharacterScript c)
    {
        //Do nothing
    }
	// Update is called once per frame
	void Update ()
    {
        transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
    }

    public typeOfItem getTypeOfItem()
    {
        return myItem;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Character")
        {
            //giveMyItem(other.GetComponent<CharacterScript>());
           // Destroy(this.gameObject);
        }
    }
   /* private void giveMyItem(CharacterScript characterScript)
    {
        giveItem.Invoke(myItem, characterScript);
    }*/

}
