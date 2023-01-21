using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public static RaceManager instance;
    public Checkpoint[] allCheckPoints;

    public int totalLaps;

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
        
    }
}
