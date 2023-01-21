using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint_Checkker : MonoBehaviour
{

    public CarController TheCar;

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Checkpoint")
        {
            // Debug.Log("Hit Checkpoint " + other.GetComponent<Checkpoint>().checkPointNumber);

            TheCar.CheckpointHit(other.GetComponent<Checkpoint>().checkPointNumber);


        }
    }
}
