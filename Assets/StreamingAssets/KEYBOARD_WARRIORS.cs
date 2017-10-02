using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
//---------- CHANGE THIS NAME HERE -------
public class KEYBOARD_WARRIORS : MonoBehaviour
{
    //---------- CHANGE THIS NAME HERE -------
    public static KEYBOARD_WARRIORS AddYourselfTo(GameObject host)
    {
        //---------- CHANGE THIS NAME HERE -------
        return host.AddComponent<KEYBOARD_WARRIORS>();
    }

    [SerializeField]
    public CharacterScript character1;
    [SerializeField]
    public CharacterScript character2;
    [SerializeField]
    public CharacterScript character3;
    public static ObjectiveScript middleObjective;
    public static ObjectiveScript leftObjective;
    public static ObjectiveScript rightObjective;
    public static team ourTeamColor;
    public static Agent agent1, agent2, agent3;

    void Start()
    {
        character1 = transform.Find("Character1").gameObject.GetComponent<CharacterScript>();
        character2 = transform.Find("Character2").gameObject.GetComponent<CharacterScript>();
        character3 = transform.Find("Character3").gameObject.GetComponent<CharacterScript>();
        middleObjective = GameObject.Find("MiddleObjective").GetComponent<ObjectiveScript>();
        leftObjective = GameObject.Find("LeftObjective").GetComponent<ObjectiveScript>();
        rightObjective = GameObject.Find("RightObjective").GetComponent<ObjectiveScript>();
        ourTeamColor = character1.getTeam();
        agent1 = new Agent(character1, leftObjective);
        agent2 = new Agent(character2, middleObjective);
        agent3 = new Agent(character3, rightObjective);
    }

    void Update()
    {
        agent1.Update();
		agent2.Update();
		agent3.Update();

    }


}

public class Agent
{
    public CharacterScript Self;
    public ObjectiveScript ActiveObjective;
    public int state = 0;
    bool switchedStates = false;
    float CombatTime = 1;
    Vector3 CurrentEnemy;
    float itemTime = 0;

    public Agent(CharacterScript character, ObjectiveScript firstObjective)
    {
        Self = character;
        ActiveObjective = firstObjective;
        Self.addDyingListener(onDeath);
        Self.addHitListener(combatRefresh1);
        Self.addShotListener(combatRefresh2);
    }

    public void onDeath()
    {
        state = 0;
    }

    public void combatRefresh1(team a, int b , int c)
    {
        CombatTime = .5f;
    }
    public void combatRefresh2(CharacterScript a, int b)
    {
        CombatTime = .5f;
    }

    ObjectiveScript ChooseNewObjective()
    {
        float a = 1000, b = 1000, c = 1000;
        if (KEYBOARD_WARRIORS.leftObjective.getControllingTeam() != KEYBOARD_WARRIORS.ourTeamColor)
            a = DistanceToObjective(KEYBOARD_WARRIORS.leftObjective, Self);
        if (KEYBOARD_WARRIORS.middleObjective.getControllingTeam() != KEYBOARD_WARRIORS.ourTeamColor)
            b = DistanceToObjective(KEYBOARD_WARRIORS.middleObjective, Self);
        if (KEYBOARD_WARRIORS.rightObjective.getControllingTeam() != KEYBOARD_WARRIORS.ourTeamColor)
            c = DistanceToObjective(KEYBOARD_WARRIORS.rightObjective, Self);
        if (a < b && a < c)
            return KEYBOARD_WARRIORS.leftObjective;
        if (b < a && b < c)
            return KEYBOARD_WARRIORS.middleObjective;
        else
            return KEYBOARD_WARRIORS.rightObjective;
    }

    float DistanceToObjective(ObjectiveScript s, CharacterScript c)
    {
        return Mathf.Abs((s.getObjectiveLocation() - c.transform.position).magnitude);
    }

    void Objective()
    {
       

        switchedStates = false;
        //Check if there is combat.
        if (Self.visibleEnemyLocations.Count != 0 || Self.attackedFromLocations.Count != 0)
        {
            CombatTime = .5f;
            state = 1;
        }
            

        //Check if target objective has been taken
        if (ActiveObjective.getControllingTeam() == KEYBOARD_WARRIORS.ourTeamColor)
            ActiveObjective = ChooseNewObjective();
        
        //Spin
        Self.rotateAngle(150);

        //If low hp then get items
        foreach (var item in Self.getItemList())
        {
            if (Self.getHP() < 50 && Self.getItemList().Count > 0 && Mathf.Abs((item.transform.position - Self.transform.position).magnitude) < 40)
            {
                itemTime = 5;
                switchedStates = true;
                state = 2;
            }
        }
        //If an item is near and not an objective get item
        foreach (var item in Self.getItemList())
        {
            if (Mathf.Abs((item.transform.position - Self.transform.position).magnitude) < 10)
            {
                switchedStates = true;
                itemTime = 2;
                state = 2;
            }
                
        }

        //Move to objective.
        Self.MoveChar(ActiveObjective.getObjectiveLocation());

    }
    Vector3 getBackwardsVector(Vector3 enemy, Vector3 player, float distance)
    {
        Vector3 eDirection = enemy - Self.transform.position;
        float f = Vector3.Angle(eDirection, Vector3.forward);
        float g = f - 180;
        float xgoal = Mathf.Sin(g) * distance;
        float zgoal = Mathf.Cos(g) * distance;
        return new Vector3(xgoal, 1.5f, zgoal);

    }


    void Combat()
    {
        ActiveObjective = ChooseNewObjective();
        //Find closest enemy to rotation
        foreach (var item in Self.visibleEnemyLocations)
        {
            CurrentEnemy = item;
            
        }
        foreach (var item in Self.attackedFromLocations)
        {
            CurrentEnemy = item;

        }

        Self.SetFacing(CurrentEnemy);
        Self.MoveChar(Self.FindClosestCover(CurrentEnemy));
        

        //if out of combat time return to objective state
        if (CombatTime < 0)
        {
            switchedStates = true;
            state = 0;
        }

        Self.attackedFromLocations.Clear();
    }

    void Fetch()
    {
        foreach (var item in Self.visibleEnemyLocations)
        {
            CurrentEnemy = item;

        }

        Self.MoveChar(Self.FindClosestItem().transform.position);
        if(itemTime<0)
            state = 0;
    }

    void Squad()
    {

    }

    void Global()
    {
        Debug.Log(state);
        if (Self.getZone() == zone.BlueBase || Self.getZone() == zone.RedBase)
        {
            Self.setLoadout(loadout.MEDIUM);
            Self.priority = firePriority.LOWHP;
        }
        CombatTime -= Time.deltaTime;
        itemTime -= Time.deltaTime;
    }

    public void Update()
    {
        switch (state)
        {
            case 0:
                Objective();
                break;
            case 1:
                Combat();
                break;
            case 2:
                Fetch();
                break;
            case 3:
                Squad();
                break;
        }
        Global();
    }

}