using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scale_up_down : MonoBehaviour
{
    public float duration = 2f; // seconds
    public float scale = 1;
    private Vector3 initialScale;
    private Vector3 TargetScale;
    private bool turn = false;

    private float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        initialScale = transform.localScale;
        TargetScale = transform.localScale*scale;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        float t = timer / duration;
        if (!turn){
            transform.localScale = Vector3.Lerp(initialScale, TargetScale, t);
        }
        else {
            transform.localScale = Vector3.Lerp(TargetScale, initialScale, t);
        }
        if (t >= 1f) {
            timer = 0f;// reset timer
            if (turn == true){
                turn = false;
            }
            else turn = true;
        }
    }
}
