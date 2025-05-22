using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Trajectory_Rungkatta : MonoBehaviour
{
    // Visualizer Parameters
    public Visualization visualizer;
    public GameObject pointPrefab_track;
    public Transform spawnParent_track;
    public float Store_delta = 0.5f;
    public float offset_x_local = 0f;

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
    public Material lineMaterial_Y;

    // Motor parameters
    private float J = 0.01f;  // kg.m^2 (inertia)
    private float b = 0.85f;   // N.m.s (friction)
    private float K = 2.7f;  // Motor constant
    private float R = 10f;   // Ohms (resistance)
    private float L = 0.5f;   // H (inductance)

    //Limit Constrain
    private float V_max = 15f;
    private float Current_Max = 10f;
    private float Omega_max = 0.9f;

    private float derivertive_alpha = 0.15f;
    private float Previous_D = 0.0f;
    
    public float Load_motor = 0.12f;

    // Time step
    private float timeStep = 0.01f;
    private float maxTime = 0.0f; //Dynamic
    private float CurrentTime = 0.0f;
    private float podiumSize = 0.0f; //Dyanamic
    private float MaxEror_1 = 0.05f;
    private float MaxEror_2 = 0.04f;
    private float MaxEror_3 = 0.03f;
    private float MaxEror_4 = 0.02f;

    // parameters
    public float Kp = 0f;
    public float Ki = 0f;
    public float Kd = 0f;
    public int Level = 1;
    public bool play = false;
    public bool checking_level = false;
    public int state = 0;   // state 0 is on going state 1 is game finish state 2 is game over

    // //for validation ///////////////////////////
    // private List<(float, float)> trackSomeTime = new List<(float, float)>();

    // Target positions
    public float targetPosition = 0.0f; // Initial target position in radians
    public float position = 0.0f;       // Current position in radians
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
        state = 0;
        // targetPosition = 0.5f;
        // position = 0.5f;
        omega = 0.0f;
        integral = 0.0f;
        prevError = 0.0f;
        current = 0.0f;
        clearPath();
        visualizer.ResetData();
        
        lastTrackP = new Vector3(99,99,99);
        lastTrackI = new Vector3(99,99,99);
        lastTrackD = new Vector3(99,99,99);
        lastValueP = 0.0f;
        lastValueI = 0.0f;
        lastValueD = 0.0f;
    }

    public void UpdateTrajectory()
    {
        // if (CurrentTime == 0.00f)
        // {
        //     lastTrackP = new Vector3(99,99,99);
        //     lastTrackI = new Vector3(99,99,99);
        //     lastTrackD = new Vector3(99,99,99);
        //     lastValueP = 0.0f;
        //     lastValueI = 0.0f;
        //     lastValueD = 0.0f;
        // }
        if (play)
        {

            // Debug.Log("Update Trajectory");
            // Debug.Log(CurrentTime);

            // Is Loosing
            // if (checking_level){Loosing_Check();}
            LevelUpdate_logic();    

            // Calculate error and PID terms and Simulation
            float error = targetPosition - position;
            integral += error * timeStep;
            float derivative = (error - prevError) / timeStep;
            derivative = derivertive_alpha * derivative + (1 - derivertive_alpha) * Previous_D;
            Previous_D = derivative;

            float P = Kp * error;
            float I = Ki * integral;
            float D = Kd * derivative;
            // PID 
            float voltage = P + I + D;
            voltage = Mathf.Clamp(voltage, -V_max, V_max);

            // RK4 Part 1
            float K1_Current = (voltage - (R * current - K * omega)) / L*timeStep;
            float K1_Omega = (K * current - b * omega - Load_motor) / J*timeStep;
            float K1_Position = omega * timeStep;
            // RK4 Part 2
            float K2_Current = (voltage - (R * (current+K1_Current/2) - K * (omega+K1_Omega/2))) / L*timeStep;
            float K2_Omega = (K * (current+K1_Current/2) - b * (omega+K1_Omega/2) - Load_motor) / J*timeStep;
            float K2_Position = (omega + K1_Omega/2) * timeStep;
            // RK4 Part 3
            float K3_Current = (voltage - (R * (current+K2_Current/2) - K * (omega+K2_Omega/2))) / L*timeStep;
            float K3_Omega = (K * (current+K2_Current/2) - b * (omega+K2_Omega/2) - Load_motor) / J*timeStep;
            float K3_Position = (omega + K2_Omega/2) * timeStep;
            // RK4 Part 4
            float K4_Current = (voltage - (R * (current+K3_Current/2) - K * (omega+K3_Omega/2))) / L*timeStep;
            float K4_Omega = (K * (current+K3_Current/2) - b * (omega+K3_Omega/2) - Load_motor) / J*timeStep;
            float K4_Position = (omega + K3_Omega/2) * timeStep;
            // Combine slopes
            current += (K1_Current + 2*K2_Current + 2*K3_Current + K4_Current) / 6;
            current = Mathf.Clamp(current, -Current_Max, Current_Max);
            omega += (K1_Omega + 2*K2_Omega + 2*K3_Omega + K4_Omega) / 6;
            omega = Mathf.Clamp(omega,-Omega_max,Omega_max);
            position += (K1_Position + 2*K2_Position + 2*K3_Position + K4_Position) / 6;

           
           // Update Trajectory Position
            Vector3 staticPosition = new Vector3((CurrentTime/maxTime)*podiumSize-podiumSize/2+offset_x_local, position, 0);
            transform.localPosition = staticPosition;
            OneTimeStepPosition = (0.01f/maxTime)*podiumSize;

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
                trackPosition.Add(new Vector3(staticPosition.x,staticPosition.y,staticPosition.z));
                lastPosition = staticPosition;
            }
            // trackPosition.Add(new Vector3(staticPosition.x+spawnParent_track.position.x,staticPosition.y+spawnParent_track.position.y,staticPosition.z+spawnParent_track.position.z));
            // if (CurrentTime%Store_delta==0)
            // {
            //     visualizer.Store(P,I,D,staticPosition.x);
            // }

            // Debug.Log("Updated Position");

            // //for validation ///////////////////////////////////////
            // if (new int[] { 200, 400, 600, 800, 1000, 1200, 1400, 1600, 1800, 2000 }.Contains((int)(CurrentTime * 100)))
            // {
            //     trackSomeTime.Add(( (float)(CurrentTime), (float)position ));
            // }

            if (checking_level){Loosing_Check(P,I,D);}

            //for check state game
            if ((CurrentTime > maxTime) && (state==0) && (Level!=0)) //victory
            {
                // //for valication ////////////////////////////////////////
                // foreach (var item in trackSomeTime)
                // {
                //     Debug.Log($"Time: {item.Item1}, Position: {item.Item2}");
                // }


                play = false;
                state = 1;
                end_point(P,I,D,transform.localPosition.x);
            }
            else if ((state==2)&& (Level!=0)) //game over
            {
                play = false;
                state = 2;
                end_point(P,I,D,transform.localPosition.x);
            }
            // Update Before Loop
            prevError = error;
            CurrentTime += timeStep;
            string roundedString = CurrentTime.ToString("0.00");
            CurrentTime = float.Parse(roundedString);
            lastValueP = P;
            lastValueI = I;
            lastValueD = D;
            // Debug.Log(CurrentTime);
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
        visualizer.CalculateLine();
        visualizer.SpawLine();
        // visualizer.SpawPoint();
        CreatePath();
    }

    private void CreatePath()
    {
        // Vector3 lastPosition = trackPosition[0];
        // for (int i = 0; i < trackPosition.Count; i++)
        // {
        //     GameObject point_track = Instantiate(pointPrefab_track, trackPosition[i], pointPrefab_track.transform.rotation, spawnParent_track);
        //     spawnedPoints_track.Add(point_track);
        // }
        GameObject lineObject = new GameObject("Line_Path");
        lineObject.transform.parent = spawnParent_track;
        lineObject.transform.localPosition = Vector3.zero; // Reset position to (0, 0, 0)
        lineObject.transform.localRotation = Quaternion.identity; // Reset rotation to (0, 0, 0)
        lineObject.transform.localScale = Vector3.one; // Reset scale to (1, 1, 1)
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false; // Use local space
        lineRenderer.numCornerVertices = 90;
        lineRenderer.numCapVertices = 90;
        lineRenderer.positionCount = trackPosition.Count;
        lineRenderer.startWidth = 0.004f;
        lineRenderer.endWidth = 0.004f;
        lineRenderer.material = lineMaterial_Y; // Assign red material
        lineRenderer.SetPositions(trackPosition.ToArray());
        lineRenderer.generateLightingData = true;
        spawnedPoints_track.Add(lineObject);
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


    private void LevelUpdate_logic()
    {
        if (Level == 0)
            {
                maxTime = 7f;
                podiumSize = 1.4f;
                if (CurrentTime > 2f)
                {
                    targetPosition = 0.7f;
                }
            }
        else if (Level == 1)
            {
                maxTime = 14f;
                podiumSize = 2.8f;
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
        else if (Level == 2)
        {
            maxTime = 16f;
            podiumSize = 3.2f;
            if (CurrentTime > 12.5f)
            {
                targetPosition = 0.2f;
            }
            else if (CurrentTime > 9f)
            {
                targetPosition = 0.7f;
            }
            else if (CurrentTime > 5.5f)
            {
                targetPosition = 0.3f;
            }
            else if (CurrentTime > 2f)
            {
                targetPosition = 0.8f;
            }
        }
        else if (Level == 3)
        {
            maxTime = 20f;
            podiumSize = 4.0f;
            if (CurrentTime > 17f)
            {
                targetPosition = 0.1f;
            }
            else if (CurrentTime > 14f)
            {
                targetPosition = 0.8f;
            }
            else if (CurrentTime > 11f)
            {
                targetPosition = 0.3f;
            }
            else if (CurrentTime > 8f)
            {
                targetPosition = 0.9f;
            }
            else if (CurrentTime > 5f)
            {
                targetPosition = 0.2f;
            }
            else if (CurrentTime > 2f)
            {
                targetPosition = 0.8f;
            }
        }
        else if (Level == 4)
        {
            maxTime = 22f;
            podiumSize = 4.4f;
            if (CurrentTime > 19.5f)
            {
                targetPosition = 0.1f;
            }
            else if (CurrentTime > 17f)
            {
                targetPosition = 0.9f;
            }
            else if (CurrentTime > 14.5f)
            {
                targetPosition = 0.1f;
            }
            else if (CurrentTime > 12f)
            {
                targetPosition = 0.8f;
            }
            else if (CurrentTime > 9.5f)
            {
                targetPosition = 0.3f;
            }
            else if (CurrentTime > 7f)
            {
                targetPosition = 0.9f;
            }
            else if (CurrentTime > 4.5f)
            {
                targetPosition = 0.2f;
            }
            else if (CurrentTime > 2f)
            {
                targetPosition = 0.8f;
            }
        }
    }
    private void Loosing_Check(float P, float I, float D)   //minconverge time 3 2.5 2 1.5
    {
        if (Level==0)
            {
                //pass
                //for check state game
                if ((CurrentTime > maxTime)) //reset the game
                {
                    play = false;
                    end_point(P,I,D,transform.localPosition.x);
                    state = 3;
                }
                else if ((position>0.8f)|| (position<0.2f)){
                    play = false;
                    end_point(P,I,D,transform.localPosition.x);
                    state = 2;
                }
            }

        if (((position>1f)|| (position<0f))&&(Level!=0))
            {
                play = false;
                end_point(P,I,D,transform.localPosition.x);
                state =2;
            }
        if (Level == 1)
            {
                // maxTime = 14f;
                // podiumSize = 2.8f;
                if (CurrentTime> 13f && CurrentTime < 14f)
                {
                    if (Mathf.Abs(targetPosition - position) > MaxEror_1){state = 2;}
                }
                else if (CurrentTime> 9f && CurrentTime < 10f)
                {
                    if (Mathf.Abs(targetPosition - position) > MaxEror_1){state = 2;}
                }
                else if (CurrentTime> 5f && CurrentTime < 6f)
                {
                    if (Mathf.Abs(targetPosition - position) > MaxEror_1){state = 2;}
                }
            }
        else if (Level == 2)
        {
            // maxTime = 16f;
            // podiumSize = 3.2f;
            if (CurrentTime> 15f && CurrentTime < 16f)
            { 
                if (Mathf.Abs(targetPosition - position) > MaxEror_2){state = 2;}
            }
            else if (CurrentTime> 11.5f && CurrentTime < 12.5f)
            {
                if (Mathf.Abs(targetPosition - position) > MaxEror_2){state = 2;}
            }
            else if (CurrentTime> 8f && CurrentTime < 9f)
            {
                if (Mathf.Abs(targetPosition - position) > MaxEror_2){state = 2;}
            }
            else if (CurrentTime> 4.5f && CurrentTime < 5.5f)
            {
                if (Mathf.Abs(targetPosition - position) > MaxEror_2){state = 2;}
            }
        }
        else if (Level == 3)
        {
            // maxTime = 20f;
            // podiumSize = 4.0f;
            if (CurrentTime> 19f && CurrentTime < 20f)
            {
                if (Mathf.Abs(targetPosition - position) > MaxEror_3){state = 2;}
            }
            else if (CurrentTime> 16f && CurrentTime < 17f)
            {
                if (Mathf.Abs(targetPosition - position) > MaxEror_3){state = 2;}
            }
            else if (CurrentTime> 13f && CurrentTime < 14f)
            {
                if (Mathf.Abs(targetPosition - position) > MaxEror_3){state = 2;}
            }
            else if (CurrentTime> 10f && CurrentTime < 11f)
            {
                if (Mathf.Abs(targetPosition - position) > MaxEror_3){state = 2;}
            }
            else if (CurrentTime> 7f && CurrentTime < 8f)
            {
                if (Mathf.Abs(targetPosition - position) > MaxEror_3){state = 2;}
            }
            else if (CurrentTime> 4f && CurrentTime < 5f)
            {
                if (Mathf.Abs(targetPosition - position) > MaxEror_3){state = 2;}
            }
        }
        else if (Level == 4)
        {
            // maxTime = 22f;
            // podiumSize = 4.4f;
            if (CurrentTime> 21f && CurrentTime < 22f)
            {
                if (Mathf.Abs(targetPosition - position) > MaxEror_4){state = 2;}
            }
            else if (CurrentTime> 18.5f && CurrentTime < 19.5f)
            {
                if (Mathf.Abs(targetPosition - position) > MaxEror_4){state = 2;}            
            }
            else if (CurrentTime> 16f && CurrentTime < 17f)
            {
                if (Mathf.Abs(targetPosition - position) > MaxEror_4){state = 2;}            
            }
            else if (CurrentTime> 13.5f && CurrentTime < 14.5f)
            {
                if (Mathf.Abs(targetPosition - position) > MaxEror_4){state = 2;}            
            }
            else if (CurrentTime> 11f && CurrentTime < 12f)
            {
                if (Mathf.Abs(targetPosition - position) > MaxEror_4){state = 2;}            
            }
            else if (CurrentTime> 8.5f && CurrentTime < 9.5f)
            {
                if (Mathf.Abs(targetPosition - position) > MaxEror_4){state = 2;}            
            }
            else if (CurrentTime> 6f && CurrentTime < 7f)
            {
                if (Mathf.Abs(targetPosition - position) > MaxEror_4){state = 2;}            
            }
            else if (CurrentTime> 3.5f && CurrentTime < 4.5f)
            {
                if (Mathf.Abs(targetPosition - position) > MaxEror_4){state = 2;}            
            }
        }
    }
}
