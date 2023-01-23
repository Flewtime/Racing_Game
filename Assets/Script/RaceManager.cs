using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
