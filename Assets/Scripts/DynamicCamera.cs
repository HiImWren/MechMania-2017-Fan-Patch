using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCamera : MonoBehaviour {

    public CharacterScript blue1;
    public CharacterScript blue2;
    public CharacterScript blue3;
    public CharacterScript red1;
    public CharacterScript red2;
    public CharacterScript red3;

    float viewTimer = 5;
    System.Random rand = new System.Random();
    CharacterScript target;

    // Use this for initialization
    void Start () {
        target = blue2;
        blue1.addShotListener(OnShot);
        blue2.addShotListener(OnShot);
        blue3.addShotListener(OnShot);
        red1.addShotListener(OnShot);
        red2.addShotListener(OnShot);
        red3.addShotListener(OnShot);
    }

  

    private void OnShot(CharacterScript c, int d)
    {
        if (viewTimer < 0)
        {
            target = c;
            viewTimer = 5;
        }
    }

    // Update is called once per frame
    void Update () {

        if(viewTimer < -4)
        {
            viewTimer = 5;
            switch (rand.Next(1, 6))
            {
                case 1:
                    target = blue1;
                    break;
                case 2:
                    target = blue2;
                    break;
                case 3:
                    target = blue3;
                    break;
                case 4:
                    target = red1;
                    break;
                case 5:
                    target = red2;
                    break;
                case 6:
                    target = red3;
                    break;
            }
        }

        if (target.state == characterState.DEAD || target.state == characterState.RESPAWNING)
        {
            viewTimer = 5;
            switch (rand.Next(1, 6))
            {
                case 1:
                    target = blue1;
                    break;
                case 2:
                    target = blue2;
                    break;
                case 3:
                    target = blue3;
                    break;
                case 4:
                    target = red1;
                    break;
                case 5:
                    target = red2;
                    break;
                case 6:
                    target = red3;
                    break;
            }
        }


        transform.position = target.getPrefabObject().transform.position;
        viewTimer -= Time.deltaTime;
	}

}
