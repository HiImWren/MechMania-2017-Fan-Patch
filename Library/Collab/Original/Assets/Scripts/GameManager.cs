using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DamageEvent : UnityEvent<int>
{

}
public enum team
{
    red,
    blue,
    none
}

public class GameManager : MonoBehaviour {

    public enum GameState
    {
        Ready,
        Finished
    }
    [SerializeField]
    private int TIMELIMIT = 300;
    private GameState currentGameState;

    [SerializeField]
    private CharacterScript BlueTeam1;
    [SerializeField]
    private CharacterScript BlueTeam2;
    [SerializeField]
    private CharacterScript BlueTeam3;
    [SerializeField]
    private CharacterScript RedTeam1;
    [SerializeField]
    private CharacterScript RedTeam2;
    [SerializeField]
    private CharacterScript RedTeam3;
    [SerializeField]
    private CSharpCompiler.RedLoader rl;
    [SerializeField]
    private CSharpCompiler.BlueLoader bl;
    [SerializeField]
    private ObjectiveScript objective0;
    [SerializeField]
    private ObjectiveScript objective1;
    [SerializeField]
    private ObjectiveScript objective2;
    private team[] securedObjectives = { team.none, team.none, team.none };

    //private static UnityEvent RedTeamScored;
    //private static UnityEvent BlueTeamScored;
    private static UnityEvent GameOver;
    private DamageEvent DamageR1 = new DamageEvent();
    private DamageEvent DamageR2 = new DamageEvent();
    private DamageEvent DamageR3 = new DamageEvent();
    private DamageEvent DamageB1 = new DamageEvent();
    private DamageEvent DamageB2 = new DamageEvent();
    private DamageEvent DamageB3 = new DamageEvent();
    private static int BlueTeamScore = 0;
    private static int RedTeamScore = 0;
    private static int gameTimer = 0;
    private static string RedName;
    private static string BlueName;
    [SerializeField]
    private static Text Timer;
    [SerializeField]
    private static Text Notify;
    [SerializeField]
    private static Text BlueTeam;
    [SerializeField]
    private static Text RedTeam;
    [SerializeField]
    private static Text BlueScore;
    [SerializeField]
    private static Text RedScore;
    [SerializeField]
    private static Slider RP1HP;
    [SerializeField]
    private static Slider RP2HP;
    [SerializeField]
    private static Slider RP3HP;
    [SerializeField]
    private static Slider BP1HP;
    [SerializeField]
    private static Slider BP2HP;
    [SerializeField]
    private static Slider BP3HP;
    [SerializeField]
    private static Slider BlueSlide;
    [SerializeField]
    private static Slider RedSlide;
    private int myNumber = 0;
    private static bool flagNotify;

    private class HUDScript
    {
        
        public static void NotifyPoints(team t, string captured, int points)
        {
            flagNotify = true;
            if (t == team.red)
            {
                Notify.color = Color.red;
                Notify.text = "Red Team ";
                if (captured.ToLower() == "item")
                    Notify.text += "picked up ";
                else if (captured.ToLower() == "objective")
                    Notify.text += "captured an objective for ";
                else if (captured.ToLower() == "kill")
                    Notify.text += "killed a player for ";
                Notify.text += points + " points!";
                RedScore.text = "" + RedTeamScore;
                RedSlide.value = RedTeamScore;
            }
            else if (t == team.blue)
            {
                Notify.color = Color.blue;
                Notify.text = "Blue Team ";
                if (captured.ToLower() == "item")
                    Notify.text += "picked up ";
                else if (captured.ToLower() == "objective")
                    Notify.text += "captured an objective for ";
                else if (captured.ToLower() == "kill")
                    Notify.text += "killed a player for ";
                Notify.text += points + " points!";
                BlueScore.text = "" + BlueTeamScore;
                BlueSlide.value = BlueTeamScore;
            }
        }
        public static void updatePoints()
        {
           // flagNotify = true;
                //Notify.color = Color.red;
                //Notify.text = "Red Team has captured an objective!";
                RedScore.text = "" + RedTeamScore;
                RedSlide.value = RedTeamScore;
                //Notify.color = Color.blue;
                //Notify.text = "Blue Team has captured an objective!";
                BlueScore.text = "" + BlueTeamScore;
                BlueSlide.value = BlueTeamScore;
            

        }
        public static void NotifyObjective(team t)
        {
            flagNotify = true;
            if (t == team.red)
            {
                Notify.color = Color.red;
                Notify.text = "Red Team has captured an objective!";
                //RedScore.text = "" + RedTeamScore;
                //RedSlide.value = RedTeamScore;
            }
            else if (t == team.blue)
            {
                Notify.color = Color.blue;
                Notify.text = "Blue Team has captured an objective!";
                //BlueScore.text = "" + BlueTeamScore;
                //BlueSlide.value = BlueTeamScore;
            }


        }
        public static void PlayerHit(team t, int player, int health)
        {
            if (t == team.red)
            {
                switch(player)
                {
                    case 1:
                        RP1HP.value = health;
                        break;
                    case 2:
                        RP2HP.value = health;
                        break;
                    case 3:
                        RP3HP.value = health;
                        break;
                }
            }
            else if (t == team.blue)
            {
                switch (player)
                {
                    case 1:
                        BP1HP.value = health;
                        break;
                    case 2:
                        BP2HP.value = health;
                        break;
                    case 3:
                        BP3HP.value = health;
                        break;
                }
            }
        }      

        public static void ChangeTeamName(team t, string name)
        {
            if(t == team.red)
            {
                RedTeam.text = name;
                RedName = name;
            }
                
            else if (t == team.blue)
            {
                BlueTeam.text = name;
                BlueName = name;
            }
                
        }
    }
    private IEnumerator displayNotif()
    {
        if (Notify.enabled != true)
        {
            Notify.enabled = true;
            yield return new WaitForSeconds(2);
            Notify.enabled = false;
        }
        else
            yield return new WaitForSeconds(0);
    }
    private void Winner(team t)
    {
        string gamelog = RedName + "vs" + BlueName + ".txt";
        if (t == team.red)
        {
            currentGameState = GameState.Finished;
            Notify.text = "RED TEAM WON!!";
            Notify.color = Color.red;
            Notify.enabled = true;
            if (System.IO.File.Exists(gamelog))
            {
                System.IO.File.Delete(gamelog);
            }
            using (StreamWriter writetext = File.AppendText(gamelog))
            {
                writetext.Write("RED");
                //writetext.WriteLine();
                writetext.Close();
            }
            Invoke("quit", 5.0f);
            //Application.Quit();
        }
        else if (t == team.blue)
        {
            currentGameState = GameState.Finished;
            Notify.text = "BLUE TEAM WON!!";
            Notify.color = Color.blue;
            Notify.enabled = true;
            if (System.IO.File.Exists(gamelog))
            {
                System.IO.File.Delete(gamelog);
            }
            using (StreamWriter writetext = File.AppendText(gamelog))
            {
                writetext.Write("BLUE");
                //writetext.WriteLine();
                writetext.Close();
            }
            Invoke("quit", 5.0f);
            //Application.Quit();
        }
        else if (t == team.none)
        {
            currentGameState = GameState.Finished;
            Notify.text = "TIE!!";
            Notify.color = Color.green;
            Notify.enabled = true;
            if (System.IO.File.Exists(gamelog))
            {
                System.IO.File.Delete(gamelog);
            }
            using (StreamWriter writetext = File.AppendText(gamelog))
            {
                writetext.WriteLine("TIE");
                //writetext.WriteLine();
                writetext.Close();
            }
            Invoke("quit",5.0f);
            //Application.Quit();
        }
            
    }
    // Use this for initialization
    void Start () {
        Application.runInBackground = true;
        Timer = GameObject.Find("Timer").GetComponent<Text>();
        Notify = GameObject.Find("Notify").GetComponent<Text>();
        RedTeam = GameObject.Find("R").GetComponent<Text>();
        BlueTeam = GameObject.Find("B").GetComponent<Text>();
        RedScore = GameObject.Find("RedScore").GetComponent<Text>();
        BlueScore = GameObject.Find("BlueScore").GetComponent<Text>();
        RP1HP = GameObject.Find("RP1HP").GetComponent<Slider>();
        RP2HP = GameObject.Find("RP2HP").GetComponent<Slider>();
        RP3HP = GameObject.Find("RP3HP").GetComponent<Slider>();
        BP1HP = GameObject.Find("BP1HP").GetComponent<Slider>();
        BP2HP = GameObject.Find("BP2HP").GetComponent<Slider>();
        BP3HP = GameObject.Find("BP3HP").GetComponent<Slider>();
        RedSlide = GameObject.Find("RedScoreSlider").GetComponent<Slider>();
        BlueSlide = GameObject.Find("BlueScoreSlider").GetComponent<Slider>();
        Notify.enabled = false;
        //HUDScript.NotifyPoints("red",);
        currentGameState = GameState.Ready;
        HUDScript.ChangeTeamName(team.red, rl.classname);
        HUDScript.ChangeTeamName(team.blue, bl.classname);



        for (int i = 0; i < rl.classname.Length; i++)
        {
            myNumber += (int)rl.classname[i];
        }
        for (int i = 0; i < bl.classname.Length; i++)
        {
            myNumber += (int)bl.classname[i];
        }



        /*if (RedTeamScored == null)
            RedTeamScored = new UnityEvent();
        if (BlueTeamScored == null)
            BlueTeamScored = new UnityEvent();
        RedTeamScored.AddListener(nullPing);
        BlueTeamScored.AddListener(nullPing);*/
        InvokeRepeating("gameTimerCountdown", 0.0f, 1.0f);

        if (GameOver == null)
            GameOver = new UnityEvent();
        GameOver.AddListener(nullPing);
        DamageB1.AddListener(nullping);
        DamageB2.AddListener(nullping);
        DamageB3.AddListener(nullping);
        DamageR1.AddListener(nullping);
        DamageR2.AddListener(nullping);
        DamageR3.AddListener(nullping);

        //Debug.Log(BlueTeam3);
        BlueTeam1.addDyingListener(B1Died);
        BlueTeam2.addDyingListener(B2Died);
        BlueTeam3.addDyingListener(B3Died);
        
        RedTeam1.addDyingListener(R1Died);
        RedTeam2.addDyingListener(R2Died);
        RedTeam3.addDyingListener(R3Died);

        BlueTeam1.addHitListener(HUDScript.PlayerHit);
        BlueTeam2.addHitListener(HUDScript.PlayerHit);
        BlueTeam3.addHitListener(HUDScript.PlayerHit);
        RedTeam1.addHitListener(HUDScript.PlayerHit);
        RedTeam2.addHitListener(HUDScript.PlayerHit);
        RedTeam3.addHitListener(HUDScript.PlayerHit);

        BlueTeam1.addShotListener(shoot);
        BlueTeam2.addShotListener(shoot);
        BlueTeam3.addShotListener(shoot);
        RedTeam1.addShotListener(shoot);
        RedTeam2.addShotListener(shoot);
        RedTeam3.addShotListener(shoot);

        BlueTeam1.addActionListener(someonePerformed);
        BlueTeam2.addActionListener(someonePerformed);
        BlueTeam3.addActionListener(someonePerformed);
        RedTeam1.addActionListener(someonePerformed);
        RedTeam2.addActionListener(someonePerformed);
        RedTeam3.addActionListener(someonePerformed);

        objective0.addObjectiveListener(ObjectiveSecured);
        objective1.addObjectiveListener(ObjectiveSecured);
        objective2.addObjectiveListener(ObjectiveSecured);
    }

    void nullping(int i)
    {
        //Do nothing
    }

    // Update is called once per frame
    void Update()
    {
        if (flagNotify)
        {
            flagNotify = false;
            StartCoroutine(displayNotif());
        }
        if (RedTeamScore == 500)
            Winner(team.red);
        else if (BlueTeamScore == 500)
            Winner(team.blue);
        else if (gameTimer == TIMELIMIT)
        {
            if (RedTeamScore > BlueTeamScore)
                Winner(team.red);
            else if (BlueTeamScore > RedTeamScore)
                Winner(team.blue);
            else
                Winner(team.none);
        }

    }

    private void gameTimerCountdown()
    {
        //update timer
        if (gameTimer < 300)
        {            
            gameTimer += 1;
            int sec = gameTimer % 60;
            Timer.text = "" + gameTimer / 60;
            if (sec < 10)
                Timer.text += ":0" + gameTimer % 60;
            else
                Timer.text += ":" + gameTimer % 60;
        }
        foreach(team t in securedObjectives)
        {
            if (t == team.blue)
                AddBluePoints(1, "objective");
            else if(t == team.red)
                AddRedPoints(1,"objective");
        }
        HUDScript.updatePoints();
    }

    private void shoot(CharacterScript charSc, int dmg)
    {
        if (charSc == BlueTeam1 && (charSc.getZone() != zone.BlueBase))
            DamageB1.Invoke(dmg);
        if (charSc == BlueTeam2 && (charSc.getZone() != zone.BlueBase))
            DamageB2.Invoke(dmg);
        if (charSc == BlueTeam3 && (charSc.getZone() != zone.BlueBase))
            DamageB3.Invoke(dmg);
        if (charSc == RedTeam1 && (charSc.getZone() != zone.RedBase))
            DamageR1.Invoke(dmg);
        if (charSc == RedTeam2 && (charSc.getZone() != zone.RedBase))
            DamageR2.Invoke(dmg);
        if (charSc == RedTeam3 && (charSc.getZone() != zone.RedBase))
            DamageR3.Invoke(dmg);

    }

    private void quit()
    {
        Application.Quit();
    }

    private void someonePerformed(actions a, team t, int player, int number)
    {
        if (t == team.blue)
        {
            if (a == actions.points)
                AddBluePoints(number, "item");
        }
        else if (t == team.red)
        {
            if (a == actions.points)
                AddRedPoints(number, "item");
        }
    }

    private void ObjectiveSecured(team t, ObjectiveScript objectiveScript)
    {
        HUDScript.NotifyObjective(t);
        if (objectiveScript == objective0)
            securedObjectives[0] = t;
        else if (objectiveScript == objective1)
            securedObjectives[1] = t;
        else if (objectiveScript == objective2)
            securedObjectives[2] = t;
    }


    void B1Died()
    {
        AddRedPoints(5, "kill");
    }
    void B2Died()
    {
        AddRedPoints(5, "kill");
    }
    void B3Died()
    {
        AddRedPoints(5, "kill");
    }
    void R1Died()
    {
        AddBluePoints(5, "kill");
    }
    void R2Died()
    {
        AddBluePoints(5, "kill");
    }
    void R3Died()
    {
        AddBluePoints(5, "kill");
    }

    void RedGotItem()
    {
        AddRedPoints(10, "item");
    }
    void BlueGotItem()
    {
        AddBluePoints(10, "item");
    }

    void AddRedPoints(int points, string reason)
    {
        RedTeamScore += points;
        if (reason == "kill")
            HUDScript.NotifyPoints(team.red, reason, points);
        else if (reason == "item")
            HUDScript.NotifyPoints(team.red, reason, points);
        //HUDD.UpdateHUDD.Invoke();
    }

    void AddBluePoints(int points, string reason)
    {
        BlueTeamScore += points;
        if (reason == "kill")
            HUDScript.NotifyPoints(team.blue, reason, points);
        else if (reason == "item")
            HUDScript.NotifyPoints(team.blue, reason, points);
        //HUDD.UpdateHUDD.Invoke();
    }

    public GameState getGameStates()
    {
        return currentGameState;
    }
    void nullPing()
    {
        //Do nothing
    }

    public int getBlueScore()
    {
       return BlueTeamScore;
    }

    public int getRedScore()
    {
        return RedTeamScore;
    }
    /*public void addScoringListener(string team, UnityAction a)
    {
        if (team.ToLower() == "red")
        {
            RedTeamScored.AddListener(a);
        }
        else if (team.ToLower() == "blue")
        {
            BlueTeamScored.AddListener(a);
        }
    }*/
    public void addGameOverListener(UnityAction a)
    {
        GameOver.AddListener(a);
    }
    public void addDamageListener(team t,int playernumber, UnityAction<int> a)
    {
        if (t == team.red)
            switch (playernumber)
            {
                case 1:
                    DamageR1.AddListener(a);
                    break;
                case 2:
                    DamageR2.AddListener(a);
                    break;
                case 3:
                    DamageR3.AddListener(a);
                    break;
            }
        if (t == team.blue)
            switch (playernumber)
            {
                case 1:
                    DamageB1.AddListener(a);
                    break;
                case 2:
                    DamageB2.AddListener(a);
                    break;
                case 3:
                    DamageB3.AddListener(a);
                    break;
            }
    }

    public int getNum()
    {
        return myNumber;
    }
}
