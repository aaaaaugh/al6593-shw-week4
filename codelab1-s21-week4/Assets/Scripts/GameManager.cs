using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float gameTime = 10; //sets timer to 10 secs before the game ends

    public Text timerText;

    float timer = 0; //var set to 0

    //static variable means the value is the same for all the objects of this class type and the class itself
    public static GameManager instance; //this static var will hold the Singleton. basically you're using the same thing as you did with the PlayerControl just now

    private int score = 0; //remember: private means that it'll stay in the script, so this can only be seen in the game manager 

    const string DIR_LOGS = "/Logs"; //you're making a directory
    const string FILE_SCORES = DIR_LOGS + "/highScore.txt"; //making a new constant so you have a txt file of all your saved scores in a directory
    const string FILE_ALL_SCORES = DIR_LOGS + "/allScores.csv"; //new constant so you have a txt file of ALL THE SCORES
    string FILE_PATH_ALL_SCORES; //string for all scores
    string FILE_PATH_HIGH_SCORES; //string for high scores

    public int Score //HANDLES THE PROPERTY
    {
        get { return score;  } //what you've done is that you can't see score anywhere OUTSIDE of this script, but you can see Score 
        set
        {
            score = value;

            //Debug.Log("Someone set the Score!"); //now you have this message to tell you everytime it changes!
            if (score > HighScore)//now you can just check it instead of having to write a buncha code on Update 
            {
                HighScore = score; //USING THE PROPERTY NOT THE VARIABLE
            }

            string fileContents = "";
            if (File.Exists(FILE_PATH_ALL_SCORES))
            {
                fileContents = File.ReadAllText(FILE_PATH_ALL_SCORES);
            }

            fileContents += score + ","; //CSV = comma separated values
            File.WriteAllText(FILE_PATH_ALL_SCORES, fileContents);

        }  
    }
    
    List<int> highScores; //list of high scores that will be able to keep all the high scores someone gets (top 3)
    List<int> allHighScores; //All high scores
    
    bool isGame = true; //use a boolean to make a Property for the timer as well
    
    const string FILE_HIGH_SCORES = "/Logs/highScore.txt"; //you have to make a constant for the save file 
    string FILE_PATH_HIGH_SCORE;

    const string PREF_KEY_HIGH_SCORE = "HighScoreKey"; //creating a variable so you can track your strings w/ ease
    int highScore = 0; //IF THE HIGH SCORE IS -1

    public int HighScore //^we are creating a new property for the variable highScore
    {
        get
        {
            if (highScore < 1) //checking if the score is less than -1. only happens 1st time you're pulling out
            {
                //highScore = PlayerPrefs.GetInt(PREF_KEY_HIGH_SCORE, defaultValue: 3); //grab it out of PlayerPrefs. either 3 or default
                if (File.Exists(FILE_PATH_HIGH_SCORES))
                {
                    string fileContents = File.ReadAllText(FILE_PATH_HIGH_SCORES);

                    bool successful;
                    
                    successful = Int32.TryParse(fileContents, out highScore);

                    if (!successful)
                    {
                        highScore = 0;
                    }
                }
                else
                {
                    highScore = 3;
                }
            }
            
            return highScore; 
        }
        set
        {
            highScore = value;
            Debug.Log("new high score!!!");
            Debug.Log("File Path: " + FILE_PATH_HIGH_SCORES);
            PlayerPrefs.SetInt(PREF_KEY_HIGH_SCORE, highScore); 

            if (!File.Exists(FILE_PATH_HIGH_SCORES)) //putting an ! means it does not exist
            {
                Directory.CreateDirectory(Application.dataPath + DIR_LOGS);
                //File.Create(FILE_PATH_HIGH_SCORES);
            }
            
            File.WriteAllText(FILE_PATH_HIGH_SCORES, highScore + ""); //make an empty string 
        }
    } 

    int targetScore = 3;

    int currentLevel = 0;

    public TextMesh text;  //TextMesh Component to tell you the time and the score

    void Awake()
    {
        if (instance == null) //instance hasn't been set yet
        {
            DontDestroyOnLoad(gameObject);  //Dont Destroy this object when you load a new scene
            instance = this;  //set instance to this object
        }
        else  //if the instance is alsready set to an object
        {
            Destroy(gameObject); //destroy this new object, so there is only ever one
        }
    }

    // Start is called before the first frame update
    void Start()
    {
      
        timer = 0; //if you have to reset
        FILE_PATH_HIGH_SCORE = Application.dataPath + FILE_HIGH_SCORES;
        FILE_PATH_HIGH_SCORES = Application.dataPath + FILE_SCORES;
        FILE_PATH_ALL_SCORES =  Application.dataPath + FILE_ALL_SCORES;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime; //fragment of the time passed since last frame

        if (!isGame) //if we're not in the GAME display the scores
        {
            string highScoreString = "high scores\n\n";

            for (var i = 0; i < highScores.Count; i++)
            {
                highScoreString += highScores[i] + "\n";
            }

            //Debug.Break(); //pauses unity when it gets to this line of code 

            timerText.text = highScoreString;
        }
        else //but unless we're in the game, show this instead
        {
            timerText.text = "time: " + (int) (gameTime - timer); //will show how much time goes by
            if (gameTime < timer && isGame) //time is up but only do this if we're in the game scene
            {   isGame = false;
                SceneManager.LoadScene("GameOverScene");
                GameObject.Find("Player"); //destroy the player obj
                GameObject.Destroy(GameObject.Find("Player")); 
                UpdateHighScores();
                gameTime = 10; //RESETS back to 10 secs
            }
        
            //update the text with the score and level
            text.text = "level:" + currentLevel + 
                        "\nboops: " + score + 
                        "\nbooped: " + targetScore +
                        "\nhigh score: " + HighScore; //change the variable highScore to the Property HighScore
        
            if (score == targetScore)  //if the current score == the targetScore
            {
                currentLevel++; //increase the level number
                SceneManager.LoadScene(currentLevel); //go to the next level when u hit the prize
                targetScore += targetScore + targetScore/2; //update target score
            }
        }
        
       // Debug.Log("Time: " + timer);

       
    }

    void UpdateHighScores()
    {
       // if (highScores == null) //if there isn't a high score yet
       // {
            highScores = new List<int>(); //make a new list
            allHighScores = new List<int>(); //make a new list

            string fileContents = File.ReadAllText(FILE_PATH_ALL_SCORES); //the player prefs - what saves all the time
            
            string[] fileScores = fileContents.Split(','); //turns it into individual values. string manipulation, you have to make an array of strings
            
            
            
            for (var i = 0; i < fileScores.Length; i++)
            { 
                
                int outvalue; //placeholder
               if (int.TryParse(fileScores[i], out outvalue)) //basically a boolean - argument is if you can't parse it then you skip it
               {
                   allHighScores.Add(Convert.ToInt32(fileScores[i])); //the syntax? 
               }
                
            }

            
            highScores.Add(GetMax(allHighScores, 1)); //the three highest scores will be displayed here
            highScores.Add(GetMax(allHighScores, 2));
            highScores.Add(GetMax(allHighScores, 3));
            
       // }

        for (var i = 0; i < highScores.Count; i++) //if you're doing a list it's Count. YOU HAVE TO LOOP THROUGH THE HIGH SCORE
        {
            if (score > highScores[i]) //look at the highest score
            {
                highScores.Insert(i, score);
                break; //after we Insert something we can get out of this loop
            }
        }

        string saveHighScoreString = "";

        for (var i = 0; i < highScores.Count; i++)
        {
            saveHighScoreString += highScores[i] + ",";
        }

        File.WriteAllText(FILE_PATH_HIGH_SCORE, saveHighScoreString);
        }


    int GetMax(List<int> newList, int num) //this function returns a value (that's why we don't use void) 
    {
        newList.Sort(); //sorts from highest to lowest
        int max = newList[newList.Count - num]; //allows for the highest number to be shown
        return max;
    }
    
    }
    