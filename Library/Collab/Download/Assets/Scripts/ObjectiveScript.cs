using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class ObjectEvent : UnityEvent<team, ObjectiveScript>
{
}

public class ObjectiveScript : MonoBehaviour
{
    [SerializeField]
    private Light myLight;
    private team controlled = team.none;
    private float timer = 0;
    private bool startTime = false;
    private int redOnMe = 0;
    private int blueOnMe = 0;
    private List<CharacterScript> charactersOnMe = new List<CharacterScript>();
    private ObjectEvent ObjectiveSecured = new ObjectEvent();

    [SerializeField]
    private Image circleFill;
    [SerializeField]
    private float timeToCapture = 3.0f;

    // Use this for initialization
    void Start()
    {
        ObjectiveSecured.AddListener(nullping);
        Application.runInBackground = true;
    }

    void nullping(team t, ObjectiveScript o)
    {

    }

    // Update is called once per frame
    void Update()
    {
        int length = charactersOnMe.Count;
        //Debug.Log(length);
        for (int i = 0; i < length; i++)
        {
            //Debug.Log("I SEE " + charactersOnMe[i].name + " and he has " + charactersOnMe[i].getHP() + " " + i);
            if (charactersOnMe[i].getHP() == 0)
            {
                if (charactersOnMe[i].getTeam() == team.red)
                    redOnMe--;
                else if (charactersOnMe[i].getTeam() == team.blue)
                    blueOnMe--;
                charactersOnMe.Remove(charactersOnMe[i]);
                length--;
                i--;
                //Debug.Log("I SEE " + charactersOnMe[i].name + " and he has " + charactersOnMe[i].getHP());
            }
        }
        if (redOnMe > 0 && blueOnMe == 0 && controlled != team.red)
        {
            StartTimer(team.red);
        }
        else if (redOnMe == 0 && blueOnMe > 0 && controlled != team.blue)
        {
            //Debug.Log("starting blue timer");
            StartTimer(team.blue);
        }
        else
        {
            StopTimer();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Character")
        {
            charactersOnMe.Add(other.transform.parent.GetComponent<CharacterScript>());
            if (other.transform.parent.GetComponent<CharacterScript>().getTeam() == team.red)
                redOnMe++;
            else if (other.transform.parent.GetComponent<CharacterScript>().getTeam() == team.blue)
                blueOnMe++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Character")
        {
            charactersOnMe.Remove(other.transform.parent.GetComponent<CharacterScript>());
            if (other.transform.parent.GetComponent<CharacterScript>().getTeam() == team.red)
                redOnMe--;
            else if (other.transform.parent.GetComponent<CharacterScript>().getTeam() == team.blue)
                blueOnMe--;
        }
    }
    void StartTimer(team t)
    {

        //ERROR HERE CHECK ON THIS
        if (startTime && Time.time > timer + timeToCapture)
        {
            //Debug.Log("Captured");
            if (redOnMe > 0 && blueOnMe == 0)
                Captured(team.red);
            else if (blueOnMe > 0 && redOnMe == 0)
                Captured(team.blue);
        }
        else if (startTime)
        {
            //Display time to capture
            if (redOnMe > 0 && blueOnMe == 0)
                circleFill.color = new Color(1.0f, 0.0f, 0.0f, 0.6f);
            else if (blueOnMe > 0 && redOnMe == 0)
                circleFill.color = new Color(0.0f, 0.55f, 1.0f, 0.6f);
            circleFill.fillAmount = (Time.time - timer) / timeToCapture;
        }
        else
        {
            // Debug.Log("Started at " + Time.time);
            startTime = true;
            timer = Time.time;
        }
    }
    void StopTimer()
    {
        startTime = false;
        timer = 0.0f;
        circleFill.fillAmount = 0;
    }

    void Captured(team t)
    {
        startTime = false;
        timer = 0.0f;
        if (t == team.red)
        {
            controlled = team.red;
            myLight.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
            GetComponent<MeshRenderer>().material.color = new Color(1.0f, 0.0f, 0.0f, 0.588f);
            ObjectiveSecured.Invoke(team.red, this);
        }
        else if (t == team.blue)
        {
            controlled = team.blue;
            myLight.color = new Color(0.0f, 0.682f, 1.0f, 1.0f);
            GetComponent<MeshRenderer>().material.color = new Color(0.0f, 0.685f, 1.0f, 0.588f);
            ObjectiveSecured.Invoke(team.blue, this);
        }
    }

    public void addObjectiveListener(UnityAction<team, ObjectiveScript> a)
    {
        ObjectiveSecured.AddListener(a);
    }
    public team getCapturedTeam()
    {
        return controlled;
    }

    public Vector3 getObjectiveLocation()
    {
        return this.transform.position;
    }
}