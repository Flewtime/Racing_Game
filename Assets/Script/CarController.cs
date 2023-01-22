using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Rigidbody theRB;
    public float maxSpeed;

    public float forwardAccel = 8f, reverseAccel = 4f;

    private float speedInput;

    public float turnStrength = 180f;
    private float turnInput;

    private bool grounded;

    public Transform groundRayPoint, groundRayPoint2;

    public LayerMask whatIsGround;

    public float groundRayLength = .75f;

    private float dragOnGround;

    public float gravityMod = 10f;

    public Transform rodaDepanKiri, rodaDepanKanan;

    public float maxWheelTurn = 25f;

    public ParticleSystem[] dustTrail;

    public float maxEmission = 25f, emissionFadeSpeed = 20f;
    private float emissionRate;

    public AudioSource engineSFX, ngepotSFX;
    public float ngepotFadeSpeed;

    public int nextCheckpoint;
    public int currentLap;

    public float lapTime, bestLapTime;

    public bool isAI;

    // AI Fundamentals
    public int currentTarget;
    private Vector3 targetPoint;
    public float aiAccelerateSpeed = 1f, aiTurnSpeed = 0.8f, aiReachPointRange = 5f, aiPointVariance = 3f, aiMaxTurn = 15f;
    private float aiSpeedInput, aiSpeedModifier;
    void Start()
    {
        theRB.transform.parent = null;

        UIManager.instance.lapCounterText.text = currentLap + "/" + RaceManager.instance.totalLaps;
        if(isAI){
            targetPoint = RaceManager.instance.allCheckPoints[currentTarget].transform.position;
            RandomiseAITarget();

            aiSpeedModifier = Random.Range(.8f, 1.1f);
        }

        dragOnGround = theRB.drag;
    }

    // Update is called once per frame
    void Update()
    {
        lapTime += Time.deltaTime;

        if(!isAI)
        {

        var ts = System.TimeSpan.FromSeconds(lapTime);
        UIManager.instance.CurrLapTimeText.text = string.Format("{0:00}m{1:00}.{2:000}s", ts.Minutes, ts.Seconds, ts.Milliseconds);


        speedInput = 0f;
        if(Input.GetAxis("Vertical") > 0){
            speedInput = Input.GetAxis("Vertical") * forwardAccel;
        } else if(Input.GetAxis("Vertical") < 0){
            speedInput = Input.GetAxis("Vertical") * reverseAccel;
        }


        turnInput = Input.GetAxis("Horizontal");

        /* if(grounded && Input.GetAxis("Vertical") != 0){
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f,turnInput * turnStrength * Time.deltaTime * Mathf.Sign(speedInput) * (theRB.velocity.magnitude / maxSpeed), 0f));
        } */

        } else 
        {
            targetPoint.y = transform.position.y;

            if(Vector3.Distance(transform.position, targetPoint) < aiReachPointRange)
            {
                setNextAITarget();
            }

            Vector3 targetDire = targetPoint - transform.position;
            float angle = Vector3.Angle(targetDire, transform.forward);

            Vector3 localpos = transform.InverseTransformPoint(targetPoint);

            if(localpos.x < 0f)
            {
                angle = -angle;
            }

            turnInput = Mathf.Clamp(angle / aiMaxTurn, -1f, 1f);

            if(Mathf.Abs(angle) < aiMaxTurn)
            {
                aiSpeedInput = Mathf.MoveTowards(aiSpeedInput, 1f, aiAccelerateSpeed);
            } else {
                aiSpeedInput = Mathf.MoveTowards(aiSpeedInput, aiTurnSpeed, aiAccelerateSpeed);
            }

            speedInput = aiSpeedInput * forwardAccel * aiSpeedModifier;
        }

        // Turning the Wheel
        rodaDepanKiri.localRotation = Quaternion.Euler(rodaDepanKiri.localRotation.eulerAngles.x, (turnInput * maxWheelTurn) - 180, rodaDepanKiri.localRotation.eulerAngles.z);
        rodaDepanKanan.localRotation = Quaternion.Euler(rodaDepanKanan.localRotation.eulerAngles.x, (turnInput * maxWheelTurn), rodaDepanKanan.localRotation.eulerAngles.z);

        // transform.position = theRB.position;

        // Control Particles emission
        emissionRate = Mathf.MoveTowards(emissionRate, 0f, emissionFadeSpeed * Time.deltaTime);


        if(grounded && (Mathf.Abs(turnInput) > .5f || (theRB.velocity.magnitude < maxSpeed * .5f && theRB.velocity.magnitude != 0)))
        {
            emissionRate = maxEmission;
        }

        if(theRB.velocity.magnitude == 0.5f)
        {
            emissionRate = 0;
        }


        for(int i = 0; i < dustTrail.Length; i++) 
        {
            var emissionModule = dustTrail[i].emission;

            emissionModule.rateOverTime = emissionRate;
        }

        // SFX
        if(engineSFX != null)
        {
            engineSFX.pitch = 1f + (theRB.velocity.magnitude / maxSpeed) * 2f;
        }

        if(ngepotSFX != null)
        {
            if(Mathf.Abs(turnInput) > 0.5f)
            {
                ngepotSFX.volume = 1f;
            } else 
            {
                ngepotSFX.volume = Mathf.MoveTowards(ngepotSFX.volume, 0f, ngepotFadeSpeed * Time.deltaTime);
            }
        }

    }


    private void FixedUpdate() {

        grounded = false;

        RaycastHit hit;
        Vector3 normalTarget = Vector3.zero;

        if(Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            grounded = true;

            normalTarget = hit.normal;
        }

        if(Physics.Raycast(groundRayPoint2.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            grounded = true;

            normalTarget = (normalTarget + hit.normal) / 2f;
        }

        // when on ground rotate to match the normal
        if(grounded)
        {
            transform.rotation = Quaternion.FromToRotation(transform.up, normalTarget) * transform.rotation;
        }

        // accelerates the car
        if(grounded)
        {
            theRB.drag = dragOnGround;
            theRB.AddForce (transform.forward * speedInput * 1000f);
        } else {
            theRB.drag = .1f;

            theRB.AddForce(-Vector3.up * gravityMod * 100f);
        }
        

        if(theRB.velocity.magnitude > maxSpeed)
        {
            theRB.velocity = theRB.velocity.normalized * maxSpeed;
        }

        transform.position = theRB.position;

        if(grounded && speedInput != 0){
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f,turnInput * turnStrength * Time.deltaTime * Mathf.Sign(speedInput) * (theRB.velocity.magnitude / maxSpeed), 0f));
        }

    }

    public void CheckpointHit(int checkPointNumber)
    {
        if(checkPointNumber == nextCheckpoint)
        {
            nextCheckpoint++;

            if(nextCheckpoint == RaceManager.instance.allCheckPoints.Length)
            {
                nextCheckpoint = 0;
                LapCompleted();
            }
        }
        if(isAI){
            if(checkPointNumber == currentTarget)
            {
                setNextAITarget();
            }
        }
    }

    public void setNextAITarget()
    {
                currentTarget++;
                if(currentTarget >= RaceManager.instance.allCheckPoints.Length)
                {
                    currentTarget = 0;
                }
                targetPoint = RaceManager.instance.allCheckPoints[currentTarget].transform.position;
                RandomiseAITarget();
    }

    public void LapCompleted()
    {
        currentLap++;

        if(lapTime < bestLapTime || bestLapTime == 0){
            bestLapTime = lapTime;
        }

        lapTime = 0f;

        if(!isAI)
        {
        var ts = System.TimeSpan.FromSeconds(bestLapTime);
        UIManager.instance.bestLapTimeText.text = string.Format("{0:00}m{1:00}.{2:000}s", ts.Minutes, ts.Seconds, ts.Milliseconds);

        UIManager.instance.lapCounterText.text = currentLap + "/" + RaceManager.instance.totalLaps;
        }
    }
    public void RandomiseAITarget()
    {
        targetPoint += new Vector3(Random.Range(-aiPointVariance, aiPointVariance), 0f, Random.Range(-aiPointVariance, aiPointVariance));
    }
}
