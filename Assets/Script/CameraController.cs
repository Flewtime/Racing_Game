using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public CarController target;
    private Vector3 offsetDirection;

    public float minDistance, maxDistance;
    private float activeDistance;

    void Start()
    {
        offsetDirection = transform.position - target.transform.position;       

        activeDistance = minDistance;

        offsetDirection.Normalize();
    }

    void Update()
    {
        activeDistance = minDistance + ((maxDistance - minDistance) * (target.theRB.velocity.magnitude / target.maxSpeed));

        transform.position = target.transform.position + (offsetDirection * activeDistance);        
    }
}
