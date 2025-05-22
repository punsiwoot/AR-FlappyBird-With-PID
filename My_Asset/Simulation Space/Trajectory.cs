using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Trajectory : MonoBehaviour
{
    // Visualizer Parameters
    public Visualization visualizer;
    public GameObject pointPrefab_track;
    public Transform spawnParent_track;
    public float Store_delta = 0.5f;

    private List<GameObject> spawnedPoints_track = new List<GameObject>();
    private List<Vector3> trackPosition = new List<Vector3>();
    private float minSpawPoint = 0.02f; 
    private Vector3 lastPosition = new Vector3(99,99,99);

    private float minTrackPIDValue = 0.5f;
    private Vector3 lastTrackP = new Vector3(99,99,99);
    private Vector3 lastTrackI = new Vector3(99,99,99);
    private Vector3 lastTrackD = new Vector3(99,99,99);
    private float lastValueP = 0.0f;
    private float lastValueI = 0.0f;
    private float lastValueD = 0.0f;
    private float OneTimeStepPosition = 0.0f;

    // Motor parameters
    private float J = 0.01f;  // kg.m^2 (inertia)
    private float b = 0.1f;   // N.m.s (friction)
    private float K = 0.01f;  // Motor constant
    private float R = 1.0f;   // Ohms (resistance)
    private float L = 0.5f;   // H (inductance)

    // Time step
    private float timeStep = 0.01f;
    private float maxTime = 15.0f;
    private float CurrentTime = 0.0f;

    // parameters
    public float Kp = 35f;
    public float Ki = 5f;
    public float Kd = 20f;
    public int Level = 0;
    public bool play = true;
    public int state = 0;   // state 0 is on going state 1 is game finish state 2 is game over



    // Target positions
    private float targetPosition = 0.5f; // Initial target position in radians
    private float position = 0.5f;       // Current position in radians
    private float omega = 0.0f;          // Angular speed
    private float integral = 0.0f;       // Integral error
    private float prevError = 0.0f;      // Previous error
    private float current = 0.0f;        // Current
    // Start is called before the first frame update


    void Start()
    {
        
    }
    public void ResetTrajectory()
    {
        play = false;
        CurrentTime = 0.0f;
        targetPosition = 0.5f;
        position = 0.5f;
        omega = 0.0f;
        integral = 0.0f;
        prevError = 0.0f;
        current = 0.0f;
        lastTrackP = new Vector3(99,99,99);
        lastTrackI = new Vector3(99,99,99);
        lastTrackD = new Vector3(99,99,99);

        lastValueP = 0.0f;
        lastValueI = 0.0f;
        lastValueD = 0.0f;
    }

    public void UpdateTrajectory()
    {
        if (play)
        {
            // Debug.Log("Update Trajectory");
            // Debug.Log(CurrentTime);

            // Update target position based on simulation time
            if (Level == 0)
            {
                if (CurrentTime > 10f)
                {
                    targetPosition = 0.2f;
                }
                else if (CurrentTime > 6f)
                {
                    targetPosition = 0.6f;
                }
                else if (CurrentTime > 2f)
                {
                    targetPosition = 0.3f;
                }
            }
        

            // Calculate error and PID terms
            float error = targetPosition - position;
            integral += error * timeStep;
            float derivative = (error - prevError) / timeStep;


            float P = Kp * error;
            float I = Ki * integral;
            float D = Kd * derivative;
            // PID control for voltage
            float voltage = P + I + D;

            // Motor dynamics
            float currentDot = (voltage - (R * current - K * omega)) / L;
            float omegaDot = (K * current - b * omega) / J;

            // Update current, omega, and position with Euler integration
            current += currentDot * timeStep;
            omega += omegaDot * timeStep;
            position += omega * timeStep;

            // Map position to Unity's range (0-4) and set static position
            if (Level == 0 )
            {
                Vector3 staticPosition = new Vector3((CurrentTime/maxTime)*3f-1.5f, position, 0);
                transform.localPosition = staticPosition;
                OneTimeStepPosition = (0.01f/maxTime)*3f;

                // Visualization
                Vector3 P_valuePosition = new Vector3(staticPosition.x,P,0.0f);
                if ( Vector3.Distance(lastTrackP,P_valuePosition) > minTrackPIDValue)
                {
                    visualizer.Store_P(P,staticPosition.x);
                    visualizer.Store_P(lastValueP,staticPosition.x-OneTimeStepPosition);
                    lastTrackP = P_valuePosition;
                }
                Vector3 I_valuePosition = new Vector3(staticPosition.x,I,0.0f);
                if ( Vector3.Distance(lastTrackI,I_valuePosition) > minTrackPIDValue)
                {
                    visualizer.Store_I(I,staticPosition.x);
                    visualizer.Store_I(lastValueI,staticPosition.x-OneTimeStepPosition);
                    lastTrackI = I_valuePosition;
                }
                Vector3 D_valuePosition = new Vector3(staticPosition.x,D,0.0f);
                if ( Vector3.Distance(lastTrackD,D_valuePosition) > minTrackPIDValue)
                {
                    visualizer.Store_D(D,staticPosition.x);
                    visualizer.Store_D(lastValueD,staticPosition.x-OneTimeStepPosition);
                    lastTrackD = D_valuePosition;
                }

                if ( Vector3.Distance(lastPosition,staticPosition) > minSpawPoint)
                {
                    trackPosition.Add(new Vector3(staticPosition.x+spawnParent_track.position.x,staticPosition.y+spawnParent_track.position.y,staticPosition.z+spawnParent_track.position.z));
                    lastPosition = staticPosition;
                }
                // trackPosition.Add(new Vector3(staticPosition.x+spawnParent_track.position.x,staticPosition.y+spawnParent_track.position.y,staticPosition.z+spawnParent_track.position.z));
                // if (CurrentTime%Store_delta==0)
                // {
                //     visualizer.Store(P,I,D,staticPosition.x);
                // }
            }
            else 
            {
                Vector3 staticPosition = new Vector3((CurrentTime/maxTime)*1f, position, 0);
                transform.localPosition = staticPosition;
                // if (CurrentTime%Store_delta==0)
                // {
                //     visualizer.Store(P,I,D,staticPosition.x);
                // }
            }
            // Debug.Log("Updated Position");

            if ((CurrentTime > maxTime) && (state==0)) //victory
            {
                play = false;
                state = 1;
                end_point(P,I,D,transform.localPosition.x);
            }
            else if ((state==2)) //game over
            {
                play = false;
                end_point(P,I,D,transform.localPosition.x);
            }
            // Update Before Loop
            prevError = error;
            CurrentTime += timeStep;
            lastValueP = P;
            lastValueI = I;
            lastValueD = D;
            
        }
    }

    private void end_point(float P,float I, float D,float X)
    {
        // last one before clipping point
        visualizer.Store_P(P,X);
        visualizer.Store_P(lastValueP,X-OneTimeStepPosition);
        visualizer.Store_I(I,X);
        visualizer.Store_I(lastValueI,X-OneTimeStepPosition);
        visualizer.Store_D(D,X);
        visualizer.Store_D(lastValueD,X-OneTimeStepPosition);
        visualizer.SpawLine();
        CreatePath();
    }

    private void CreatePath()
    {
        // Vector3 lastPosition = trackPosition[0];
        for (int i = 0; i < trackPosition.Count; i++)
        {
            GameObject point_track = Instantiate(pointPrefab_track, trackPosition[i], pointPrefab_track.transform.rotation, spawnParent_track);
            spawnedPoints_track.Add(point_track);
            // float Distance_check = Vector3.Distance(lastPosition,trackPosition[i]);
            // if ( Vector3.Distance(lastPosition,trackPosition[i]) > minSpawPoint)
            // {
            //     GameObject point_track = Instantiate(pointPrefab_track, trackPosition[i], pointPrefab_track.transform.rotation, spawnParent_track);
            //     spawnedPoints_track.Add(point_track);
            //     lastPosition = trackPosition[i];
            // }
        }
    }

    private void clearPath()
    {
        trackPosition.Clear();
        foreach (GameObject point in spawnedPoints_track)
        {
            if (point != null)
            {
                Destroy(point);
            }
        }
        spawnedPoints_track.Clear();
    }

    public void calPOn(){visualizer.CalP = true;visualizer.CalculateLine();visualizer.SpawLine();}
    public void calIOn(){visualizer.CalI = true;visualizer.CalculateLine();visualizer.SpawLine();}
    public void calDOn(){visualizer.CalD = true;visualizer.CalculateLine();visualizer.SpawLine();}
    public void calPOff(){visualizer.CalP = false;visualizer.CalculateLine();visualizer.SpawLine();}
    public void calIOff(){visualizer.CalI = false;visualizer.CalculateLine();visualizer.SpawLine();}
    public void calDOff(){visualizer.CalD = false;visualizer.CalculateLine();visualizer.SpawLine();}



}
