using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceManager : MonoBehaviour
{
    public static RaceManager instance;
    public Checkpoint[] allCheckPoints;

    public int totalLaps;

    public CarController playerCar;
    public List<CarController> allAICars = new List<CarController>();
    public int playerPosition;
    public float timeBetweenPosCheck = .2f;
    private float posCheckCounter;

    public float AIDefaultSpeed = 30f, playerDefaultSpeed = 30f, rubberBandSpeedModifier = 3.5f, rubberBandAcceleration = 0.5f;

    public bool isStarting;
    public float timeBetweenStartCount = 1f;
    private float startCounter;
    public int countDownCurrent = 3;

    public int playerStartPosisiton, AINumberToSpawn;
    public Transform[] startPoints;

    public List<CarController> carsToSpawn = new List<CarController>();

    public bool raceFisish;

    public string raceCompletedScene;

    private void Awake() {
        instance = this;
    }
    void Start()
    {
        for(int i = 0; i < allCheckPoints.Length; i++){
            allCheckPoints[i].checkPointNumber = i;
        }

        isStarting = true;
        startCounter = timeBetweenStartCount;
        UIManager.instance.countdownText.text = countDownCurrent + "!";

        playerStartPosisiton = Random.Range(0, AINumberToSpawn + 1);

        playerCar.transform.position = startPoints[playerStartPosisiton].position;
        playerCar.theRB.transform.position = startPoints[playerStartPosisiton].position;

        for(int i = 0; i < AINumberToSpawn + 1; i++)
        {
            if(i != playerStartPosisiton)
            {
                int SelectedCar = Random.Range(0, carsToSpawn.Count);

                allAICars.Add(Instantiate(carsToSpawn[SelectedCar], startPoints[i].position, startPoints[i].rotation));

                if(carsToSpawn.Count > AINumberToSpawn - i)
                {
                    carsToSpawn.RemoveAt(SelectedCar);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isStarting)
        {
            startCounter -= Time.deltaTime;
            if(startCounter <= 0){
                countDownCurrent--;
                startCounter = timeBetweenStartCount;

                UIManager.instance.countdownText.text = countDownCurrent + "!";

                if(countDownCurrent == 0){
                    isStarting = false;

                    UIManager.instance.countdownText.gameObject.SetActive(false);
                    UIManager.instance.goText.gameObject.SetActive(true);
                }
            }

        } else {
            posCheckCounter -= Time.deltaTime;
            if(posCheckCounter <= 0){
            playerPosition = 1;
            foreach(CarController AIcar in allAICars)
            {
                if(AIcar.currentLap > playerCar.currentLap)
                {
                    playerPosition++;
                } else if(AIcar.currentLap == playerCar.currentLap)
                {
                    if(AIcar.nextCheckpoint > playerCar.nextCheckpoint)
                    {
                        playerPosition++;
                    } else if(AIcar.nextCheckpoint == playerCar.nextCheckpoint)
                    {
                        if(Vector3.Distance(AIcar.transform.position, allCheckPoints[AIcar.nextCheckpoint].transform.position) < Vector3.Distance(playerCar.transform.position, allCheckPoints[AIcar.nextCheckpoint].transform.position))
                        {
                            playerPosition++;
                        }
                    }
                }
            }
            posCheckCounter = timeBetweenPosCheck;

            UIManager.instance.positionText.text = playerPosition + "/" + (allAICars.Count + 1);
            }

            // Improve AI(Rubber Banding)
            if(playerPosition == 1)
            {
                foreach(CarController AIcar in allAICars){
                    AIcar.maxSpeed = Mathf.MoveTowards(AIcar.maxSpeed, AIDefaultSpeed + rubberBandSpeedModifier, rubberBandAcceleration * Time.deltaTime);
                }

                playerCar.maxSpeed = Mathf.MoveTowards(playerCar.maxSpeed, playerDefaultSpeed - rubberBandSpeedModifier, rubberBandAcceleration * Time.deltaTime);
            }
            else {
                foreach(CarController AIcar in allAICars)
                {
                    AIcar.maxSpeed = Mathf.MoveTowards(AIcar.maxSpeed, AIDefaultSpeed - (rubberBandSpeedModifier * ((float) playerPosition / ((float)allAICars.Count + 1))), rubberBandAcceleration * Time.deltaTime);
                }
                // playerCar.maxSpeed = Mathf.MoveTowards(playerCar.maxSpeed, playerDefaultSpeed + (rubberBandSpeedModifier * (playerPosition / (allAICars.Count + 1))), rubberBandAcceleration * Time.deltaTime);
                playerCar.maxSpeed = Mathf.MoveTowards(playerCar.maxSpeed, playerDefaultSpeed + (rubberBandSpeedModifier * ((float) playerPosition / ((float)allAICars.Count + 1))), rubberBandAcceleration * Time.deltaTime);
            }
        }
    }

    public void FinishRace(){
        raceFisish = true;

        switch(playerPosition)
        {
            case 1:
            UIManager.instance.raceResultText.text = "You Finished 1st";
                break;
            case 2:
            UIManager.instance.raceResultText.text = "You Finished 2nd";
                break;
            case 3:
            UIManager.instance.raceResultText.text = "You Finished 3rd";
                break;
            default:
            UIManager.instance.raceResultText.text = "You Finished " + playerPosition + "th";
            break;
        }

        

        UIManager.instance.resultsScreen.SetActive(true);
    }
    public void ExitRace()
    {
        SceneManager.LoadScene(raceCompletedScene);
    }
}
