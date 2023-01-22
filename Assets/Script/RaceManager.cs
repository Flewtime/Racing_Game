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

    private void Awake() {
        instance = this;
    }
    void Start()
    {
        for(int i = 0; i < allCheckPoints.Length; i++){
            allCheckPoints[i].checkPointNumber = i;
        }
    }

    // Update is called once per frame
    void Update()
    {
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
    }
}
