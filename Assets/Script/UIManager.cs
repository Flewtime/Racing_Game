using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public TMP_Text lapCounterText, bestLapTimeText, CurrLapTimeText, positionText, countdownText, goText, raceResultText;

    public GameObject pauseScreen;
    public GameObject resultsScreen;

    public bool isPaused;
    private void Awake() {
        instance = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            pauseUnPause();
        }   
    }



    public void pauseUnPause(){
        isPaused = !isPaused;
        pauseScreen.SetActive(isPaused);

        if(isPaused){
            Time.timeScale = 0f;
        } else {
            Time.timeScale = 1f;
        }
    }


    public void ExitRace()
    {
        Time.timeScale = 1f;
        RaceManager.instance.ExitRace();
    }

    public void QuitGame(){
        Application.Quit();
        Debug.Log("Game Quit");
    }
}
