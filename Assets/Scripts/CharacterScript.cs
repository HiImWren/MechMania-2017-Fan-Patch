using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class MyEvent : UnityEvent<team, int, int>
{
}
[System.Serializable]
public class ActionEvent : UnityEvent<actions, team, int, int>
{
}
[System.Serializable]
public class ShotEvent : UnityEvent<CharacterScript, int>
{
}

// ENUMS --------------------------------------------------------------------------------------- |
public enum zone
{
    Normal,
    BlueBase,
    RedBase,
    Objective,
    None
}
public enum actions
{
    health,
    points,
    speed,
    powerUp,
    None
}

public enum loadout
{
    LONG,
    MEDIUM,
    SHORT
}

public enum characterState
{
    ALIVE,
    RESPAWNING,
    DEAD
}

public enum firePriority
{
    CLOSEST,
    FURTHEST,
    LOWHP,
    HIGHHP
}

public class CharacterScript : MonoBehaviour
{

    // EVENTS -------------------------------------------------------------------------------------- |
    private UnityEvent IDied = new UnityEvent();
    private MyEvent IGotHit = new MyEvent();
    private ShotEvent IShot = new ShotEvent();
    private ActionEvent IPerformed = new ActionEvent();
    private Animator anim;
    private ParticleSystem part;
    private Light light;

    // VARIABLES ----------------------------------------------------------------------------------- |
    
    // vision and loadout
    private loadout myLoadout = loadout.SHORT;
    private int range = 15;
    private float rayAngle = 5;
    private int numberOfRays = 19;
    private int coneAngle = 315;
    private bool loadoutFlag = true;
    

    // Health, damage, and timers
    private int health = 100;
    private int damping = 2;
    private int damage = 10;
    private int reloadTimer = 0;
    private int respawnTImer = 0;
    private int enemyLocationTimer = 0;
    public characterState state
    {
        get
        {
            return currentState;
        }
    }
    private characterState currentState = characterState.ALIVE;     // Character state for respawning mechanics
    public firePriority priority = firePriority.CLOSEST;            // Which firing priority algorithm
    private zone myZone;                                            // Zone checks whether you're in your base or not
    private GameObject speedDisplay, powerDisplay;

    // Turning
    private Vector3 lookDirection;
    private Quaternion lookRotation;
    private bool turnLock = false;

    // List of objects used in functions
    private List<GameObject> visible;
    private List<GameObject> waypoints;
    private List<GameObject> walls;
    private List<GameObject> objectives;
    public List<Vector3> visibleEnemyLocations;
    public List<Vector3> attackedFromLocations;

    private bool moveIsDone = false;
    private Vector3 moveDestination;
    private GameManager gm;
    private TriggerEnter enter;
    // Serialized Fields
    [SerializeField]
    private int playerNumber;           // The number of the character
    [SerializeField]
    private team MyTeam;                // Team color
    [SerializeField]   
    private NavMeshAgent agent;         // The navmesh agent used for moving the characters
    [SerializeField]
    private GameObject prefabObject;    // The actual location of the prefab that is moving


    // MONO FUNCTIONS ----------------------------------------------------------------------------- |

    /*
     * This function is called when the game is booted and is used for initializations.
     * 
     */
    void Start()
    {
        enter = transform.GetComponentInChildren<TriggerEnter>();
        // Event listeners
        IDied.AddListener(nullPing);
        IGotHit.AddListener(nullPing2);
        IShot.AddListener(nullPing2);
        IPerformed.AddListener(nullPing3);
        enter.addItemListener(triggered);

        speedDisplay = prefabObject.transform.Find("SpeedUp").gameObject;
        powerDisplay = prefabObject.transform.Find("Power").gameObject;
        speedDisplay.SetActive(false);
        powerDisplay.SetActive(false);
        
        gm = GameObject.Find("Main Camera").GetComponent<GameManager>();
        gm.addDamageListener(MyTeam, playerNumber, RecordHit);

        anim = prefabObject.transform.Find("Space_Soldier_A_LOD1").GetComponent<Animator>();
        part = prefabObject.GetComponent<ParticleSystem>();
        light = prefabObject.transform.Find("Spotlight").GetComponent<Light>();

        visible = new List<GameObject>();
        visibleEnemyLocations = new List<Vector3>();
        attackedFromLocations = new List<Vector3>();

        moveDestination = prefabObject.transform.position;

        // These functions are called every second. useful for timers and other time based events
        
        InvokeRepeating("enemyLocationTimerCountdown", 0.0f, 1.0f);
    }
    /*
     * Similar to Start but happens after all items are initialized in game space, is used for populating
     * since you're guaranteed to know the objects exist.
     *
     * 
     */
    private void Awake()
    {
        waypoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("Waypoint"));
        walls = new List<GameObject>(GameObject.FindGameObjectsWithTag("Wall"));
        objectives = new List<GameObject>();
        objectives.Add(GameObject.Find("MiddleObjective"));
        objectives.Add(GameObject.Find("LeftObjective"));
        objectives.Add(GameObject.Find("RightObjective"));
    }

    /*
     * Main game loop function. This function is called for every monobehaviour every frame
     * We use it to run the functions that happen constantly like cone of vision
     * and checking if we're in base
     * 
     */
    void Update()
    {
        checkZone();
        if (MyTeam == team.blue)
        {
            if (myZone == zone.BlueBase)
                loadoutFlag = true;
            else
                loadoutFlag = false;
        }
        if (MyTeam == team.red)
        {
            if (myZone == zone.RedBase)
                loadoutFlag = true;
            else
                loadoutFlag = false;
        }
        if (health > 0)
        {
            if (!turnLock)
            {
                TurnChar();
            }
            visible.Clear();
            visibleEnemyLocations.Clear();
            GetVisibleObjects(visible);
            Fire(priority);
            AnimDoneMoving(1.0f);
        }
        else if (currentState != characterState.RESPAWNING)
        {
            currentState = characterState.RESPAWNING;
            refreshRespawnTimer();
            IDied.Invoke();
            prefabObject.SetActive(false);
        }
        else if (respawnTImer == 0)
        {
            currentState = characterState.ALIVE;
            moveDestination = transform.position;   
            health = 100;
            prefabObject.transform.position = transform.position;
            IGotHit.Invoke(MyTeam, playerNumber, health);
            prefabObject.SetActive(true);
        }
    }

    private void triggered(typeOfItem type, Vector3 v)
    {
        gainItem(type);
    }
    // Events ping, used in the events system, not important
    private void nullPing()
    {
        //Do nothing
    }
    private void nullPing2(team s, int o, int i)
    {
        //Do nothing
    }
    private void nullPing2(CharacterScript s, int i)
    {
        //Do nothing
    }
    private void nullPing3(actions s, team t, int o, int i)
    {
        //Do nothing
    }
    // --------------------------------------------------------------------------------------------- |


    //getters
    public List<GameObject> getItemList()
    {
        return gm.IC.SpawnedItems;
    }

    public GameObject getPrefabObject()
    {
        return prefabObject;
    }
       
    public int getRedScore()
    {
        return gm.getRedScore();
    }
    public int getBlueScore()
    {
        return gm.getBlueScore();
    }


    // TURNING RELATED FUNCTIONS ------------------------------------------------------------------- |

    /*
     * This function is called every frame in Update.
     * Once a facing direction is set (using SetFacing), this function slowly rotates the object
     * Towards that direction a little each frame using slerp
     * 
     */
    private void TurnChar()
    {
        Vector3 _direction = (lookDirection - prefabObject.transform.position).normalized;
        lookRotation = Quaternion.LookRotation(_direction);
        prefabObject.transform.rotation = Quaternion.Slerp(prefabObject.transform.rotation, lookRotation, Time.deltaTime * damping);
    }

    /*
     * This helper function returns whether or not the rotation towards the facing is (mostly) completed
     * 
     */
    public bool isDoneTurning()
    {
        if (Quaternion.Angle(lookRotation, prefabObject.transform.rotation) < 20f)
            return true;
        else
            return false;
    }
    /*
     * A simple function to lock your character's facing in a specific direction
     */
    public void setLock()
    {
        turnLock = !turnLock;
    }

    /*
     * Thie function sets your character's facing. The character will then rotate towards this vector
     * 
     * @parameter newPos the new position you want to face towards
     */
    public void SetFacing(Vector3 newPos)
    {
        lookDirection = newPos;
    }

    /*
     * Wrapper for find closest waypoint that also sets your facing towards it
     */
    public void FaceClosestWaypoint()
    {
        SetFacing(FindClosestWaypoint().transform.position);
    }

    /*
     * This function goes through all waypoints (which are arbitrary points we set up at the edges of walls so you can face the corners of walls)
     * 
     * @return closestPoint the closest waypoint to the character
     */
    public GameObject FindClosestWaypoint()
    {
        GameObject closestPoint = null;
        float closestDistance = Mathf.Infinity;
        foreach (GameObject point in waypoints)
        {
            float dist = Vector3.Distance(point.transform.position, prefabObject.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestPoint = point;
            }
        }
        return closestPoint;
    }


    /*
     * Rotate the character by a certain angle in degrees
     * 
     * @parameter angle The angle you want to rotate
     */
    public void rotateAngle(float angle)
    {
        angle = angle * Mathf.PI / 180.0f;
        Vector3 newfacing = new Vector3(prefabObject.transform.position.x + (prefabObject.transform.forward.x * Mathf.Cos(angle) - prefabObject.transform.forward.z * Mathf.Sin(angle)), 0, prefabObject.transform.position.z + (prefabObject.transform.forward.z * Mathf.Cos(angle) + prefabObject.transform.forward.x * Mathf.Sin(angle)));
        SetFacing(newfacing);
    }
    // --------------------------------------------------------------------------------------------- |

    // MOVEMENT RELATED FUNCTIONS ----------------------------------------------------------------- |


    /*
    * A simple call to the navmesh agent to move to a certain position on the nevmesh grid
    * 
    * @parameter goal A vector representing the location you want to move to
    */
    public void MoveChar(Vector3 goal)
    {
        if (agent.isActiveAndEnabled && moveDestination != goal)
        {
            agent.destination = goal;
            anim.SetFloat("Speed", agent.speed);
            moveDestination = goal;
        }
    }

    /*
    * This helper function is useful to know when your character has reached it's destination
    * 
    * @returns bool representing whether the character has reached it's destination
    */
    public bool isDoneMoving()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }
    /*
    * This helper function is useful to know when your character has reached it's destination
    * within a certain passed distance.
    * @returns bool representing whether the character has reached within distance of destination
    */
    public bool isDoneMoving(float distance)
    {
        if (!agent.pathPending && getHP() > 0)
        {
            if (agent.remainingDistance <= distance)
            {
                return true;
            }
        }
        return false;
    }

    //Tells the animator that it is within the distance to stop the animation
    private bool AnimDoneMoving(float distance)
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= distance)
            {
                anim.SetFloat("Speed", 0.0f);
                return true;
            }
        }
        return false;
    }
    /*
     * This function finds the closest cover given an enemy's position
     * It simply checks the front and back of every wall and finds the closest wall which hides the character
     * from an enemy's given position.
     * 
     * @parameter enemyPosition A vector representing an enemy location
     * @return ClosestCover A vector represeting the closest cover
     */
    public Vector3 FindClosestCover(Vector3 enemyPosition)
    {
        Vector3 closestCover = new Vector3(0, 0, 0);
        NavMeshPath path = new NavMeshPath();
        float closestDistance = Mathf.Infinity;
        foreach (GameObject wall in walls)
        {
            Vector3 frontOfWall = wall.transform.position + wall.transform.forward * 1.5f;
            if (isCover(frontOfWall, enemyPosition))
            {
                calculateDistance(ref closestCover, path, ref closestDistance, frontOfWall);
            }
            Vector3 backofWall = wall.transform.position - wall.transform.forward * 1.5f;

            if (isCover(backofWall, enemyPosition))
            {
                calculateDistance(ref closestCover, path, ref closestDistance, backofWall);
            }
        }
        return closestCover;
    }

    /*
     * The function finds the closest objective to the character
     * 
     * @return closestObjective A vector represeting the closest objective's location
     */
    public Vector3 FindClosestObjective()
    {
        Vector3 closestObjective = new Vector3(0, 0, 0);
        NavMeshPath path = new NavMeshPath();
        float closestDistance = Mathf.Infinity;
        foreach (GameObject obj in objectives)
        {
            calculateDistance(ref closestObjective, path, ref closestDistance, obj.transform.position);
        }
        return closestObjective;
    }

    /*
     * The function finds the closest item pickup to the character
     * 
     * @return closestItem The gameobject instance of the closest item
     */
    public GameObject FindClosestItem()
    {
        GameObject closestItem = new GameObject();
        NavMeshPath path = new NavMeshPath();
        float closestDistance = Mathf.Infinity;
        foreach (GameObject item in gm.IC.SpawnedItems)
        {
            float dist = Vector3.Distance(item.transform.position, prefabObject.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestItem = item;
            }
        }
        return closestItem;
    }

    /*
     * This is a refactor function that calculates the actual movement distance
     * on the nevmesh towards a certain point. Can't really be called outside of those functions
     */
    private void calculateDistance(ref Vector3 closestCover, NavMeshPath path, ref float closestDistance, Vector3 point)
    {
        NavMesh.CalculatePath(prefabObject.transform.position, point, NavMesh.AllAreas, path);
        float dist = PathLength(path);
        if (dist < closestDistance)
        {
            closestDistance = dist;
            closestCover = point;
        }
    }

   /*
    * This function takes two positions, a point and the enemy position, and checks if that point provides cover
    * from the enemy position by raycasting between the two points
    * 
    * @parameter point The point you're checking if it's cover
    * @parameter enemyPosition The enemy position you're checking cover from
    * @return bool whether or not the point provides cover
    */
    private bool isCover(Vector3 point, Vector3 enemyPosition)
    {
        RaycastHit hit;
        if (Physics.Raycast(enemyPosition, point - enemyPosition, out hit, Vector3.Distance(enemyPosition, point)))
        {
            if (hit.collider.tag == "Wall")
            {
                return true;
            }
        }
        return false;
    }

    /*
     * This is a nevmesh helper function that takes a path through the nevmesh and returns how long the path is.
     * 
     * @parameter path A certain path through the nevmesh
     *
     * @return float The actual walking distance of the path
     */
    private float PathLength(NavMeshPath path)
    {
        if (path.corners.Length < 2)
            return 0;

        Vector3 previousCorner = path.corners[0];
        float lengthSoFar = 0.0F;
        int i = 1;
        while (i < path.corners.Length)
        {
            Vector3 currentCorner = path.corners[i];
            lengthSoFar += Vector3.Distance(previousCorner, currentCorner);
            previousCorner = currentCorner;
            i++;
        }
        return lengthSoFar;
    }
    // --------------------------------------------------------------------------------------------- |

    // VISION RELATED FUNCTIONS -------------------------------------------------------------------- |

    /*
    * This is the function that controls the cone of vision. 
    * Depending on the character's loadout, the cone is either wider(short) or narrower(long)
    * It then simply shoots raycasts every small amount of degrees and checks for collisions with characters
    * and then it stores it in the output list
    *
    * @parameter visibleObjects An output list that will be populated with visible characters (if any)
    */
    private void GetVisibleObjects(List<GameObject> visibleObjects)
    {
        RaycastHit hit;
        Vector3 destination = new Vector3(prefabObject.transform.forward.x, 0, prefabObject.transform.forward.z);
        destination = new Vector3(destination.x * Mathf.Cos(coneAngle * Mathf.PI / 180.0f) - destination.z * Mathf.Sin(coneAngle * Mathf.PI / 180.0f), 0, destination.z * Mathf.Cos(coneAngle * Mathf.PI / 180.0f) + destination.x * Mathf.Sin(coneAngle * Mathf.PI / 180.0f));
        for (int i = 0; i < numberOfRays; i++)
        {
            if (Physics.Raycast(prefabObject.transform.position, destination, out hit, range))
            {
                if (hit.collider.tag == "Character")
                {
                    CharacterScript cs = hit.collider.GetComponentInParent<CharacterScript>();
                    if (cs.getTeam() != this.getTeam())
                    {
                        visibleObjects.Add(hit.collider.transform.parent.gameObject);
                        visibleEnemyLocations.Add(hit.collider.transform.position);
                    }
                }
            }
            destination = new Vector3(destination.x * Mathf.Cos(rayAngle * Mathf.PI / 180.0f) - destination.z * Mathf.Sin(rayAngle * Mathf.PI / 180.0f), 0, destination.z * Mathf.Cos(rayAngle * Mathf.PI / 180.0f) + destination.x * Mathf.Sin(rayAngle * Mathf.PI / 180.0f));
            Debug.DrawRay(prefabObject.transform.position, destination * range, Color.magenta);
        }
    }
    // --------------------------------------------------------------------------------------------- |

    // FIRE RELATED FUNCTIONS ---------------------------------------------------------------------- |

    /*
    * This is the main Firing function that gets called every update loop
    * it simply reads the character's firing priority and fires the appropriate function
    *
    * @parameter priority This character's firing priority
    */
    private void Fire(firePriority priority)
    {
        if (priority == firePriority.CLOSEST)
        {
            fireClosest();
        }
        else if (priority == firePriority.FURTHEST)
        {
            fireFurthest();
        }
        else if (priority == firePriority.LOWHP)
        {
            fireLowestHP();
        }
        else if (priority == firePriority.HIGHHP)
        {
            fireHighestHP();
        }

    }

    //new reloading timer 
    private IEnumerator reload()
    {
        yield return new WaitForSeconds(1);
        reloadTimer = 0;
    }

    //new respawning timer 
    private IEnumerator respawn()
    {
        yield return new WaitForSeconds(5);
        respawnTImer = 0;
    }


    /*
    * Fire at closest enemy to this character, assuming it sees more than one
    */
    private void fireClosest()
    {
        GameObject target = null;
        float cDist = Mathf.Infinity;
        foreach (GameObject g in visible)
        {
            float dist = Vector3.Distance(prefabObject.transform.position, g.transform.position);
            if (dist < cDist)
            {
                cDist = dist;
                target = g;
            }
        }
        if (target != null && reloadTimer == 0)
        {
            CharacterScript targetScript = target.GetComponent<CharacterScript>();
            IShot.Invoke(targetScript, damage);
            anim.SetTrigger("Shoot");
            part.Play();
            //StartCoroutine(shootLine(target.transform.position));
            targetScript.attackedFromLocations.Add(prefabObject.transform.position);
            targetScript.refreshEnemyLocationTimer();
            
            this.refreshReloadTimer();
        }
    }


    /*
    * Fire at furthest enemy to this character, assuming it sees more than one
    */
    private void fireFurthest()
    {
        GameObject target = null;
        float cDist = 0;
        foreach (GameObject g in visible)
        {
            float dist = Vector3.Distance(prefabObject.transform.position, g.transform.position);
            if (dist > cDist)
            {
                cDist = dist;
                target = g;
            }
        }
        if (target != null && reloadTimer == 0)
        {
            
            CharacterScript targetScript = target.GetComponent<CharacterScript>();
            IShot.Invoke(targetScript, damage);
            anim.SetTrigger("Shoot");
            part.Play();
            targetScript.attackedFromLocations.Add(prefabObject.transform.position);
            targetScript.refreshEnemyLocationTimer();
            this.refreshReloadTimer();
        }
    }

    /*
    * Fire at enemy with lowest HP visible
    */
    private void fireLowestHP()
    {
        GameObject target = null;
        float lowestHP = Mathf.Infinity;
        foreach (GameObject g in visible)
        {
            CharacterScript potentialTarget = g.GetComponent<CharacterScript>();
            int hp = potentialTarget.getHP();
            if (hp < lowestHP)
            {
                lowestHP = hp;
                target = g;
            }
        }
        if (target != null && reloadTimer == 0)
        {
            
            CharacterScript targetScript = target.GetComponent<CharacterScript>();
            IShot.Invoke(targetScript, damage);
            anim.SetTrigger("Shoot");
            part.Play();
            targetScript.attackedFromLocations.Add(prefabObject.transform.GetChild(0).transform.position);
            targetScript.refreshEnemyLocationTimer();
            this.refreshReloadTimer();
        }
    }

    /*
    * Fire at enemy with highest HP visible
    */
    private void fireHighestHP()
    {
        GameObject target = null;
        float highestHP = 0;
        foreach (GameObject g in visible)
        {
            CharacterScript potentialTarget = g.GetComponent<CharacterScript>();
            int hp = potentialTarget.getHP();
            if (hp > highestHP)
            {
                highestHP = hp;
                target = g;
            }
        }
        if (target != null && reloadTimer == 0)
        {
            CharacterScript targetScript = target.GetComponent<CharacterScript>();
            IShot.Invoke(targetScript, damage);
            anim.SetTrigger("Shoot");
            part.Play();
            targetScript.attackedFromLocations.Add(prefabObject.transform.position);
            targetScript.refreshEnemyLocationTimer();
            this.refreshReloadTimer();
        }
    }

    //Render the shot Dont use this
    /*private IEnumerator shootLine(Vector3 pos)
    {
        lineRenderer.SetPosition(0, prefabObject.transform.position);
        lineRenderer.SetPosition(1, pos);
        lineRenderer.enabled = true;
        yield return new WaitForSeconds(0.05f);
        lineRenderer.enabled = false;
    }*/

    /*
    * This function is called by the damage handling event system
    * It's responsible for reducing the character's health and invoke
    * the event for getting hit
    * 
    * @parameter damage how much damage the character took
    */
    private void RecordHit(int damage)
    {
        if (health > 0)
        {
            health -= damage;
            IGotHit.Invoke(MyTeam, playerNumber, health);
        }
    }


    /*
    * This function is used to set the loadout of the character, which is one of 3 options
    * Long range loadout has the longest range of all loadouts, but does the least amount of damage
    * Medium range loadout has a good balance of damage and range
    * Short range loadout has the highest damage and widest cone of vision, but the shortest range
    * Loadouts can only be set while the character is in it's base
    * 
    * @parameter newLoadout the choice of new loadout
    */
    public void setLoadout(loadout newLoadout)
    {
        if (loadoutFlag)
        {
            if (newLoadout == loadout.LONG)
            {
                range = 35;
                rayAngle = 3.5f;
                numberOfRays = 20;
                coneAngle = 330;
                damage = 20;
                light.range = 35;
                light.spotAngle = 60;
            }
            else if (newLoadout == loadout.MEDIUM)
            {
                range = 25;
                rayAngle = 4;
                numberOfRays = 20;
                coneAngle = 320;
                damage = 25;
                light.range = 25;
                light.spotAngle = 80;
            }
            else
            {
                range = 15;
                rayAngle = 5;
                numberOfRays = 19;
                coneAngle = 315;
                damage = 35;
                light.range = 15;
                light.spotAngle = 90;
            }
        }
    }

    /*
    * This function runs every update and updates the character's zone, whether it's in the base or on an objective
    */
    private void checkZone()
    {
        NavMeshHit navMeshHit;
        if (NavMesh.SamplePosition(agent.transform.position, out navMeshHit, 1f, NavMesh.AllAreas))
        {
            int MyMask = navMeshHit.mask;
            if (MyMask == 1)
                myZone = zone.Normal;
            else if (MyMask == 8)
                myZone = zone.RedBase;
            else if (MyMask == 16)
                myZone = zone.BlueBase;
            else if (MyMask == 32)
                myZone = zone.Objective;
            else
                myZone = zone.None;
        }
    }

    /*
    * Timer function for enemylocation, called every second
    * Enemy location list contains the locations of enemys that shot you.
    * This list is cleared after 4 seconds of not updating, meaning the enemies have probably moved
    */
    private void enemyLocationTimerCountdown()
    {
        if (enemyLocationTimer > 0)
        {
            enemyLocationTimer -= 1;
        }
        else
        {
            visibleEnemyLocations.Clear();
        }
    }

    /*
    * refresh the respawn timer after dying
    */
    private void refreshRespawnTimer()
    {
        respawnTImer = 5;
        StartCoroutine(respawn());
    }

    /*
    * refresh the reload timer after shooting
    */
    private void refreshReloadTimer()
    {
        reloadTimer = 1;
        StartCoroutine(reload());
    }

    /*
    * refresh the enemy location timer
    */
    private void refreshEnemyLocationTimer()
    {
        enemyLocationTimer = 4;
    }

    /*
    * Get character's health
    */
    public int getHP()
    {
        return this.health;
    }


    /*
    * Get character's team
    */
    public team getTeam()
    {
        return this.MyTeam;
    }

    /*
    * Get character's current zone
    */
    public zone getZone()
    {
        return this.myZone;
    }

    /*
    * Get character's reload timer left
    */
    public int getReloadTime()
    {
        return this.reloadTimer;
    }

    // EVENT LISTENERS -----------------------------------------------------------------------------------------------|
    public void addDyingListener(UnityAction a)
    {
        IDied.AddListener(a);
    }

    public void addHitListener(UnityAction<team, int, int> a)
    {
        IGotHit.AddListener(a);
    }

    public void addShotListener(UnityAction<CharacterScript, int> a)
    {
        IShot.AddListener(a);
    }

    public void addActionListener(UnityAction<actions, team, int, int> a)
    {
        IPerformed.AddListener(a);
    }


    // ITEM FUNCTIONALITY --------------------------------------------------------------------------------------------- |

    private void gainItem(typeOfItem item)
    {
        switch (item)
        {
            case typeOfItem.health:
                health += 50;
                if (health > 100)
                    health = 100;
                IGotHit.Invoke(MyTeam, playerNumber, health);
                break;
            case typeOfItem.speed:
                addSpeed();
                //IPerformed.Invoke(actions.speed, MyTeam, playerNumber, 0);
                break;
            case typeOfItem.power:
                addPower();
                //IPerformed.Invoke(actions.powerUp, MyTeam, playerNumber, 0);
                break;
            case typeOfItem.points:
                IPerformed.Invoke(actions.points, MyTeam, playerNumber, 10);
                break;
        }
    }
    private void addSpeed()
    {
        StartCoroutine(speedUp());
    }
    private void addPower()
    {
        StartCoroutine(powerUp());
    }

    private IEnumerator speedUp()
    {
        agent.speed += 3;
        speedDisplay.SetActive(true);
        yield return new WaitForSeconds(30);
        agent.speed -= 3;
        speedDisplay.SetActive(false);
       // IPerformed.Invoke(actions.speed, MyTeam, playerNumber, -1);
    }
    private IEnumerator powerUp()
    {
        damage += 10;
        powerDisplay.SetActive(true);
        yield return new WaitForSeconds(30);
        powerDisplay.SetActive(false);
        damage -= 10;
       // IPerformed.Invoke(actions.powerUp, MyTeam, playerNumber, -1);
    }
}
