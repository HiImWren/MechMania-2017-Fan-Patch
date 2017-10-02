using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class ItemEvent : UnityEvent<typeOfItem, Vector3>
{
}
public class TriggerEnter : MonoBehaviour {

    private ItemEvent gotItem = new ItemEvent();
    private GameManager gm;

	// Use this for initialization
	void Start ()
    {
        gm = GameObject.Find("Main Camera").GetComponent<GameManager>();
        gotItem.AddListener(nullping);
	}

	void nullping(typeOfItem t, Vector3 v)
    {
        //Do Nothing
    }
	
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            gotItem.Invoke(other.GetComponent<ItemScript>().getTypeOfItem(), other.transform.position);
            foreach (GameObject item in gm.IC.SpawnedItems)
            {
                if (item == other.transform.parent.gameObject)
                {
                    gm.IC.SpawnedItems.Remove(item);
                    break;
                }
            }
            Destroy(other.transform.parent.gameObject);
        }
    }

    public void addItemListener(UnityAction<typeOfItem,Vector3> a)
    {
        gotItem.AddListener(a);
    }
    
}
