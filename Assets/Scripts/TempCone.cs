using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempCone : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }

    public void coneOfSight()
    {
        RaycastHit hit;
        Vector3 destination = new Vector3(transform.forward.x, 0, transform.forward.z);
        destination = new Vector3(destination.x * Mathf.Cos(325 * Mathf.PI / 180.0f) - destination.z * Mathf.Sin(325 * Mathf.PI / 180.0f), 0, destination.z * Mathf.Cos(325 * Mathf.PI / 180.0f) + destination.x * Mathf.Sin(325 * Mathf.PI / 180.0f));
        for (int i = 0; i < 15; i++)
        {
            if (Physics.Raycast(transform.position, destination, out hit, 15))
            {
                // Debug.Log("Raycasting");
                if (hit.collider.tag == "Character")
                {
                    Debug.Log("I see a character!");
                }
            }

            destination = new Vector3(destination.x * Mathf.Cos(5 * Mathf.PI / 180.0f) - destination.z * Mathf.Sin(5 * Mathf.PI / 180.0f), 0, destination.z * Mathf.Cos(5 * Mathf.PI / 180.0f) + destination.x * Mathf.Sin(5 * Mathf.PI / 180.0f));
            Debug.DrawRay(transform.position, destination * 15, Color.magenta);

        }

    }
}
