using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Visualization : MonoBehaviour
{
    public GameObject pointPrefab_P;
    public GameObject pointPrefab_I;
    public GameObject pointPrefab_D;
    public Transform spawnParent_P; // Parent to hold all spawned points for organization
    public Transform spawnParent_I;
    public Transform spawnParent_D;

    public Material lineMaterial_R;
    public Material lineMaterial_G;
    public Material lineMaterial_B;

    public float maxHeightGraph = 0.5f;
    public float offset_y_graph = 0.0f;
    public float offset_z_graph = 0.0f;

    private List<float> pValues = new List<float>();
    private List<float> iValues = new List<float>();
    private List<float> dValues = new List<float>();
    private List<float> positions_P = new List<float>();
    private List<float> positions_I = new List<float>();
    private List<float> positions_D = new List<float>();
    private List<Vector3> positions_process_P = new List<Vector3>(); // List to track process spawned point
    private List<Vector3> positions_process_I = new List<Vector3>();
    private List<Vector3> positions_process_D = new List<Vector3>();
    private List<GameObject> spawnedPoints_P = new List<GameObject>(); // List to track Raw spawned objects
    private List<GameObject> spawnedPoints_I = new List<GameObject>();
    private List<GameObject> spawnedPoints_D = new List<GameObject>();

    public bool CalP = true;
    public bool CalI = true;
    public bool CalD = true;

    private List<GameObject> LineRenderer_track = new List<GameObject>();
    private float linewidth = 0.002f;

    // public void Store(float p, float i, float d, float position)
    // {
    //     // Add values to the lists
    //     pValues.Add(p);
    //     iValues.Add(i);
    //     dValues.Add(d);
    //     positions.Add(position);

    //     // // Spawn a visual representation of the point
    //     // if (pointPrefab != null)
    //     // {
    //     //     GameObject point = Instantiate(pointPrefab, position, Quaternion.identity, spawnParent);
    //     //     spawnedPoints.Add(point);
    //     // }
    // }

    public void Store_P(float p, float position)
    {
        pValues.Add(p);
        positions_P.Add(position);
    }
    public void Store_I(float i, float position)
    {
        iValues.Add(i);
        positions_I.Add(position);
    }
    public void Store_D(float d, float position)
    {
        dValues.Add(d);
        positions_D.Add(position);
    }

    public void Deleteline() // Further Visualization
    {
        foreach (GameObject Line in LineRenderer_track)
        {
            if (Line != null)
            {
                Destroy(Line);
            }
        }
        LineRenderer_track.Clear();
    }
    public void ResetData() // for change Level
    {
        // Clear all lists
        pValues.Clear();
        iValues.Clear();
        dValues.Clear();
        positions_P.Clear();
        positions_I.Clear();
        positions_D.Clear();


        // Destroy all spawned points
        foreach (GameObject point in spawnedPoints_P)
        {
            if (point != null)
            {
                Destroy(point);
            }
        }
        foreach (GameObject point in spawnedPoints_I)
        {
            if (point != null)
            {
                Destroy(point);
            }
        }
        foreach (GameObject point in spawnedPoints_D)
        {
            if (point != null)
            {
                Destroy(point);
            }
        }

        // Clear the list of spawned points
        spawnedPoints_P.Clear();
        spawnedPoints_I.Clear();
        spawnedPoints_D.Clear();
        Deleteline();
    }

    public void SpawPoint()
    {
        float maxP = Mathf.Max(pValues.Max(),pValues.Min()*-1);
        float maxI = Mathf.Max(iValues.Max(),iValues.Min()*-1);
        float maxD = Mathf.Max(dValues.Max(),dValues.Min()*-1);
        float overallMax = Mathf.Max(maxP, Mathf.Max(maxI, maxD));
        Vector3 LastSpawPosition = new Vector3(0f,0f,0f);
        for (int i = 0; i < pValues.Count; i++)
        {

            Vector3 SpawPosition_P = new Vector3( positions_P[i],spawnParent_P.position.y+offset_y_graph+maxHeightGraph*(pValues[i]/overallMax),offset_z_graph+spawnParent_P.position.z);
            GameObject point_p = Instantiate(pointPrefab_P, SpawPosition_P, pointPrefab_P.transform.rotation, spawnParent_P);
            spawnedPoints_P.Add(point_p);
            // if (i!=0)
            // {
            //     CreateLine_R(LastSpawPosition,SpawPosition_P);
            // }
            // LastSpawPosition = SpawPosition_P;
        }
        for (int i = 0; i < iValues.Count; i++)
        {
            Vector3 SpawPosition_I = new Vector3( positions_I[i],spawnParent_I.position.y+offset_y_graph+maxHeightGraph*(iValues[i]/overallMax),offset_z_graph+spawnParent_I.position.z);
            GameObject point_i = Instantiate(pointPrefab_I, SpawPosition_I, pointPrefab_I.transform.rotation, spawnParent_I);
            spawnedPoints_I.Add(point_i);
            // if (i!=0)
            // {
            //     CreateLine_R(LastSpawPosition,SpawPosition_P);
            // }
            // Vector3 LastSpawPosition = SpawPosition_P;
        }
        for (int i = 0; i < dValues.Count; i++)
        {
            Vector3 SpawPosition_D = new Vector3( positions_D[i],spawnParent_D.position.y+offset_y_graph+maxHeightGraph*(dValues[i]/overallMax),offset_z_graph+spawnParent_D.position.z);
            GameObject point_d = Instantiate(pointPrefab_D, SpawPosition_D, pointPrefab_D.transform.rotation, spawnParent_D);
            spawnedPoints_D.Add(point_d);
        }
    }

    public void CalculateLine()
    {
        positions_process_P.Clear();
        positions_process_I.Clear();
        positions_process_D.Clear();

        float maxP = 0f;
        float maxI = 0f;
        float maxD = 0f;

        if (CalP){maxP = Mathf.Max(pValues.Max(), pValues.Min() * -1);}
        
        if (CalI){maxI = Mathf.Max(iValues.Max(), iValues.Min() * -1);}
        
        if (CalD){maxD = Mathf.Max(dValues.Max(), dValues.Min() * -1);}
        float overallMax = Mathf.Max(maxP, Mathf.Max(maxI, maxD));
        if (overallMax == 0f){overallMax = 1f;}

        if (CalP)
        {
            for (int i = 0; i < pValues.Count; i++)
            {
                Vector3 SpawPosition_P = new Vector3(
                    positions_P[i],
                    // spawnParent_P.position.y + offset_y_graph + maxHeightGraph * (pValues[i] / overallMax),
                    offset_y_graph + maxHeightGraph * (pValues[i] / overallMax),
                    offset_z_graph// + spawnParent_P.position.z
                );
                positions_process_P.Add(SpawPosition_P);
            }
        }
        if (CalI)
        {
            for (int i = 0; i < iValues.Count; i++)
            {
                Vector3 SpawPosition_I = new Vector3(
                    positions_I[i],
                    // spawnParent_I.position.y + offset_y_graph + maxHeightGraph * (iValues[i] / overallMax),
                    offset_y_graph + maxHeightGraph * (iValues[i] / overallMax),
                    offset_z_graph// + spawnParent_I.position.z
                );
                positions_process_I.Add(SpawPosition_I);
            }
        }
        if (CalD)
        {
            for (int i = 0; i < dValues.Count; i++)
            {
                Vector3 SpawPosition_D = new Vector3(
                    positions_D[i],
                    // spawnParent_D.position.y + offset_y_graph + maxHeightGraph * (dValues[i] / overallMax),
                    offset_y_graph + maxHeightGraph * (dValues[i] / overallMax),
                    offset_z_graph// + spawnParent_D.position.z
                );
                positions_process_D.Add(SpawPosition_D);
            }
        }
        positions_process_P.Sort((a, b) => a.x.CompareTo(b.x));
        positions_process_I.Sort((a, b) => a.x.CompareTo(b.x));
        positions_process_D.Sort((a, b) => a.x.CompareTo(b.x));
    }
    public void SpawLine()
    {
        Deleteline();
        // Sort to correct line Renderer
        
        // Create LineRenderer for P
        GameObject lineObject_P = new GameObject("Line_P");
        lineObject_P.transform.parent = spawnParent_P;
        lineObject_P.transform.localPosition = Vector3.zero; // Reset position to (0, 0, 0)
        lineObject_P.transform.localRotation = Quaternion.identity; // Reset rotation to (0, 0, 0)
        lineObject_P.transform.localScale = Vector3.one; // Reset scale to (1, 1, 1)
        LineRenderer lineRenderer_P = lineObject_P.AddComponent<LineRenderer>();
        lineRenderer_P.useWorldSpace = false; // Use local space
        lineRenderer_P.positionCount = positions_process_P.Count;
        lineRenderer_P.numCornerVertices = 90;
        lineRenderer_P.numCapVertices = 90;
        lineRenderer_P.startWidth = linewidth;
        lineRenderer_P.endWidth = linewidth;
        lineRenderer_P.material = lineMaterial_R; // Assign red material
        lineRenderer_P.SetPositions(positions_process_P.ToArray());
        lineRenderer_P.generateLightingData = true;
        LineRenderer_track.Add(lineObject_P);


        // Create LineRenderer for I
        GameObject lineObject_I = new GameObject("Line_I");
        lineObject_I.transform.parent = spawnParent_I;
        lineObject_I.transform.localPosition = Vector3.zero; // Reset position to (0, 0, 0)
        lineObject_I.transform.localRotation = Quaternion.identity; // Reset rotation to (0, 0, 0)
        lineObject_I.transform.localScale = Vector3.one; // Reset scale to (1, 1, 1)
        LineRenderer lineRenderer_I = lineObject_I.AddComponent<LineRenderer>();
        lineRenderer_I.useWorldSpace = false; // Use local space
        lineRenderer_I.positionCount = positions_process_I.Count;
        lineRenderer_I.numCornerVertices = 90;
        lineRenderer_I.numCapVertices = 90;
        lineRenderer_I.startWidth = linewidth;
        lineRenderer_I.endWidth = linewidth;
        lineRenderer_I.material = lineMaterial_G; // Assign green material
        lineRenderer_I.SetPositions(positions_process_I.ToArray());
        lineRenderer_I.generateLightingData = true;
        LineRenderer_track.Add(lineObject_I);

        // Create LineRenderer for D
        GameObject lineObject_D = new GameObject("Line_D");
        lineObject_D.transform.parent = spawnParent_D;
        lineObject_D.transform.localPosition = Vector3.zero; // Reset position to (0, 0, 0)
        lineObject_D.transform.localRotation = Quaternion.identity; // Reset rotation to (0, 0, 0)
        lineObject_D.transform.localScale = Vector3.one; // Reset scale to (1, 1, 1)
        LineRenderer lineRenderer_D = lineObject_D.AddComponent<LineRenderer>();
        lineRenderer_D.useWorldSpace = false; // Use local space
        lineRenderer_D.positionCount = positions_process_D.Count;
        lineRenderer_D.numCornerVertices = 90;
        lineRenderer_D.numCapVertices = 90;
        lineRenderer_D.startWidth = linewidth;
        lineRenderer_D.endWidth = linewidth;
        lineRenderer_D.material = lineMaterial_B; // Assign blue material
        lineRenderer_D.SetPositions(positions_process_D.ToArray());
        lineRenderer_D.generateLightingData = true;
        LineRenderer_track.Add(lineObject_D);
    }

    public void PrintData()
    {
        for (int i = 0; i < pValues.Count; i++)
        {
            Debug.Log($"Index {i}: P = {pValues[i]}");
        }
    }

    public void CreateLine_R(Vector3 startPoint, Vector3 endPoint)
    {
        // Create a new GameObject to hold the LineRenderer
        GameObject lineObject = new GameObject("Line");
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
        
        // Set LineRenderer properties
        lineRenderer.positionCount = 2; // Two points for the line
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.useWorldSpace = true;

        // Set the positions of the line
        lineRenderer.SetPosition(0, startPoint); // Start point
        lineRenderer.SetPosition(1, endPoint);  // End point

        // Assign a material for visibility
        if (lineMaterial_R != null)
        {
            lineRenderer.material = lineMaterial_R;
        }
        else
        {
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }

        // Optionally, set color (can remove if material defines color)
        // lineRenderer.startColor = Color.red;
        // lineRenderer.endColor = Color.red;
    }
}
