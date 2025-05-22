using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gripper : MonoBehaviour
{
    public GameObject folow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition =  new Vector3(transform.localPosition.x, folow.transform.localPosition.y, transform.localPosition.z);
    }
}
