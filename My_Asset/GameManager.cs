using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Trajectory_Rungkatta trajectory;  // Reference to the Trajectory object
    private float updateInterval = 0.01f;  // Interval for updating trajectory
    private float elapsedTime = 0.0f;     // Timer to keep track of elapsed time

    // Start is called before the first frame update
    void Start()
    {
        // Check if the trajectory object is assigned
        // if (trajectory == null)
        // {
        //     Debug.LogError("Trajectory object not assigned to GameManager.");
        // }
    }

    // Update is called once per frame
    void Update()
    {
        if (trajectory == null) 
        {
            GameObject obj= GameObject.FindWithTag("Player");
            if (obj != null)
            {
                trajectory = obj.GetComponent<Trajectory_Rungkatta>();
            }

        }
        else 
        {
            // Accumulate time
            elapsedTime += Time.deltaTime;

            // Loop to handle time-based calling of UpdateTrajectory
            for (; elapsedTime >= updateInterval; elapsedTime -= updateInterval)
            {
                // Call the trajectory update method every 0.1 seconds
                trajectory.UpdateTrajectory();
                // Debug.Log("Time Updated");
            }   
        }
    }
}