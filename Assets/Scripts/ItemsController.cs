using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsController : MonoBehaviour {


    //p////rivate string red;
    //private string blue;
    [SerializeField]
    private GameObject healthpack;
    [SerializeField]
    private GameObject points;
    [SerializeField]
    private GameObject speed;
    [SerializeField]
    private GameObject power;

    private int number = 0;
    private Transform spawn1, spawn2;

    //[SerializeField]
    //private GameObject itemParent;

    [SerializeField]
    private Transform itemLoc1;
    private bool loc1 = false;
    [SerializeField]
    private Transform itemLoc12;
    private bool loc12 = false;
    [SerializeField]
    private Transform itemLoc2;
    private bool loc2 = false;
    [SerializeField]
    private Transform itemLoc22;
    private bool loc22 = false;
    [SerializeField]
    private Transform itemLoc3;
    private bool loc3 = false;
    [SerializeField]
    private Transform itemLoc32;
    private bool loc32 = false;
    [SerializeField]
    private Transform itemLoc4;
    private bool loc4 = false;
    [SerializeField]
    private Transform itemLoc42;
    private bool loc42 = false;

    [SerializeField]
    private TriggerEnter charr1;
    [SerializeField]
    private TriggerEnter charr2;
    [SerializeField]
    private TriggerEnter charr3;
    [SerializeField]
    private TriggerEnter charb1;
    [SerializeField]
    private TriggerEnter charb2;
    [SerializeField]
    private TriggerEnter charb3;

    public List<GameObject> SpawnedItems;

    private int count = 0;
    // Use this for initialization
    void Start () {
        InvokeRepeating("myFun", 0.0f,3.0f);
        number = GetComponent<GameManager>().getNum();
        Random.InitState(number);
        SpawnedItems = new List<GameObject>();

        charb1.addItemListener(itemPickedUp);
        charb2.addItemListener(itemPickedUp);
        charb3.addItemListener(itemPickedUp);
        charr1.addItemListener(itemPickedUp);
        charr2.addItemListener(itemPickedUp);
        charr3.addItemListener(itemPickedUp);

    }

    void itemPickedUp(typeOfItem typeOfItem, Vector3 pos)
    {
        //Debug.Log("item: " + typeOfItem + " pos " + pos);
        if (pos == itemLoc1.position)
            loc1 = false;
        if (pos == itemLoc12.position)
            loc12 = false;
        if (pos == itemLoc2.position)
            loc2 = false;
        if (pos == itemLoc22.position)
            loc22 = false;
        if (pos == itemLoc3.position)
            loc3 = false;
        if (pos == itemLoc32.position)
            loc32 = false;
        if (pos == itemLoc4.position)
            loc4 = false;
        if (pos == itemLoc42.position)
            loc42 = false;

    }
	
	// Update is called once per frame
	void Update () {
      //  Debug.Log(loc1 + " " + loc12 + " " + loc2 + " " + loc22 + " "+loc3 + " "+loc32 + " "+loc4 + " "+loc42 + " ");
	}

    void myFun()
    {
        int rannum = (int)(Random.value * 100);
        int loc = (int)(Random.value * 4);
       // Debug.Log(loc);
        //Debug.Log();
        if(rannum % (4 + 2*count) == 0)
        {
            StartCoroutine(spawnItem());
            if(count < 3)
                count++;
        }
    }

    IEnumerator spawnItem()
    {
        int item = (int)(Random.value * 4);
       // Debug.Log("Starting..");
        yield return new WaitForSeconds(0.5f);
       // Debug.Log("Stopping..");
        int loc = (int)(Random.value * 4);

        spawn1 = null;
        spawn2 = null;

        if (loc == 1)
        {
            if (!loc3)
                spawn1 = itemLoc3;
            if (!loc32)
                spawn2 = itemLoc32;
            loc3 = true;
            loc32 = true;
        }
        else if (loc == 2)
        {
            if (!loc2)
                spawn1 = itemLoc2;
            if (!loc22)
                spawn2 = itemLoc22;
            loc2 = true;
            loc22 = true;
        }
        else if (loc == 3)
        {
            if (!loc1)
                spawn1 = itemLoc1;
            if (!loc12)
                spawn2 = itemLoc12;
            loc1 = true;
            loc12 = true;
        }
        else if (loc == 0)
        {
            if (!loc4)
                spawn1 = itemLoc4;
            if (!loc42)
                spawn2 = itemLoc42;
            loc4 = true;
            loc42 = true;
        }

        if (item == 1)
        {
                spawnHP();
        }
        else if (item == 2)
        {
            spawnSpeed();
        }
        else if (item == 3)
        {
            spawnPower();
        }
        else if (item == 0)
        {
            spawnPoints();
        }

    }
    void spawnHP()
    {
        if(spawn1 != null)
        {
            GameObject g = Instantiate(healthpack);
            g.transform.position = spawn1.position;
            SpawnedItems.Add(g);
        }
        if(spawn2 != null)
        {
            GameObject g2 = Instantiate(healthpack);
            g2.transform.position = spawn2.position;
            SpawnedItems.Add(g2);
        }
        
    }
    void spawnSpeed()
    {
        if (spawn1 != null)
        {
            GameObject g = Instantiate(speed);
            g.transform.position = spawn1.position;
            SpawnedItems.Add(g);
        }
        if (spawn2 != null)
        {
            GameObject g2 = Instantiate(speed);
            g2.transform.position = spawn2.position;
            SpawnedItems.Add(g2);
        }

    }
    void spawnPower()
    {
        if (spawn1 != null)
        {
            GameObject g = Instantiate(power);
            g.transform.position = spawn1.position;
            SpawnedItems.Add(g);
        }
        if (spawn2 != null)
        {
            GameObject g2 = Instantiate(power);
            g2.transform.position = spawn2.position;
            SpawnedItems.Add(g2);
        }
    }
    void spawnPoints()
    {
        if (spawn1 != null)
        {
            GameObject g = Instantiate(points);
            g.transform.position = spawn1.position;
            SpawnedItems.Add(g);
        }
        if (spawn2 != null)
        {
            GameObject g2 = Instantiate(points);
            g2.transform.position = spawn2.position;
            SpawnedItems.Add(g2);
        }
    }
}
