using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public TMP_Text lapCounterText, bestLapTimeText, CurrLapTimeText, positionText, countdownText, goText, raceResultText;

    public GameObject resultsScreen;
    private void Awake() {
        instance = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void ExitRace()
    {
        RaceManager.instance.ExitRace();
    }
}
