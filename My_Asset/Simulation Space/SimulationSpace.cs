using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SimulationSpace : MonoBehaviour
{
    public GameObject enviroment0;
    public GameObject enviroment1;
    public GameObject enviroment2;
    public GameObject enviroment3;
    public GameObject enviroment4;
    public GameObject show0;
    public GameObject show1;
    public GameObject show2;
    public GameObject show3;
    public GameObject show4;
    public GameObject Head;
    public GameObject Body1;
    public GameObject Body2;
    public GameObject Body3;
    public GameObject Body4;
    public GameObject Body5;
    public Trajectory_Rungkatta trajectory;
    private int Simulation_State = 0; // 0 is select level, 1 is playing, 3 busy
    public int Current_Level = 1; // tracking level
    public TextMesh TextP1;
    public TextMesh TextI1;
    public TextMesh TextD1;
    public TextMesh TextP2;
    public TextMesh TextI2;
    public TextMesh TextD2;
    public TextMesh TextP3;
    public TextMesh TextI3;
    public TextMesh TextD3;
    public TextMesh TextP4;
    public TextMesh TextI4;
    public TextMesh TextD4;
    public TextMesh TextInstruction;
    public TextMesh TextP0;
    public TextMesh TextI0;
    public TextMesh TextD0;
    public TextMesh TextInstructionExplain;
    public List<GameObject> gap;
    // Start is called before the first frame update
    void Start()
    {
        enviroment0.SetActive(false);
        enviroment1.SetActive(false);
        enviroment2.SetActive(false);
        enviroment3.SetActive(false);
        enviroment4.SetActive(false);
        show0.SetActive(false);
        show1.SetActive(true);
        show2.SetActive(false);
        show3.SetActive(false);
        show4.SetActive(false);
        Reset_body();
    }
    void Reset_body()
    {
        Head.transform.localPosition = new Vector3(-100, 0.5f,0);
        Body1.transform.localPosition = new Vector3(-100, 0.5f,0);
        Body2.transform.localPosition = new Vector3(-100, 0.5f,0);
        Body3.transform.localPosition = new Vector3(-100, 0.5f,0);
        Body4.transform.localPosition = new Vector3(-100, 0.5f,0);
        Body5.transform.localPosition = new Vector3(-100, 0.5f,0);
        for (int i = 0; i < gap.Count; i++)
            {
                gap[i].transform.localPosition = new Vector3(-100, 0.5f,0);
            }
        Head.SetActive(false);
        Body1.SetActive(false);
        Body2.SetActive(false);
        Body3.SetActive(false);
        Body4.SetActive(false);
        Body5.SetActive(false);
    }
    void showBody()
    {
        Head.SetActive(true);
        Body1.SetActive(true);
        Body2.SetActive(true);
        Body3.SetActive(true);
        Body4.SetActive(true);
        Body5.SetActive(true);
    }

    // Update is called once per frame
    // void Update()
    // {
        
    // }
    public int Check_State_tra()
    {
        return trajectory.state;
    }
    public void Reset_State_tra()
    {
        trajectory.state = 0;
    }
    public void Play()
    {
        // Debug.Log("pressed");
        Reset_body();
        showBody();
        trajectory.ResetTrajectory();
        trajectory.play = true;
        if (Current_Level!= 0){
            trajectory.position =0.5f;
            trajectory.targetPosition=0.5f;
        }
        else {
            trajectory.position =0.5f;
            trajectory.targetPosition=0.5f;
        }
    }
    public void Stop()
    {
        Reset_body();
        trajectory.state = 0;
        trajectory.play = false;
        trajectory.ResetTrajectory();
    }
    public void Back()
    {
        // trajectory.play = false;
        // trajectory.ResetTrajectory();
        Stop();
        if (Current_Level==1)
        {   
            enviroment1.SetActive(false);
            show1.SetActive(true);
        }
        else if (Current_Level==2)
        {
            enviroment2.SetActive(false);
            show2.SetActive(true);
        }
        else if (Current_Level==3)
        {
            enviroment3.SetActive(false);
            show3.SetActive(true);
        }
        else if (Current_Level==4)
        {
            enviroment4.SetActive(false);
            show4.SetActive(true);
        }
        else if (Current_Level==0)
        {
            enviroment0.SetActive(false);
            show0.SetActive(true);
        }
    }
    public void BackToLevel()
    {
        Stop();
        enviroment0.SetActive(false);
        enviroment1.SetActive(false);
        enviroment2.SetActive(false);
        enviroment3.SetActive(false);
        enviroment4.SetActive(false);
        if (Current_Level==1)
        {   
            show0.SetActive(false);
            show1.SetActive(true);
            show2.SetActive(false);
            show3.SetActive(false);
            show4.SetActive(false);
        }
        else if (Current_Level==2)
        {
            show0.SetActive(false);
            show1.SetActive(false);
            show2.SetActive(true);
            show3.SetActive(false);
            show4.SetActive(false);
        }
        else if (Current_Level==3)
        {
            show0.SetActive(false);
            show1.SetActive(false);
            show2.SetActive(false);
            show3.SetActive(true);
            show4.SetActive(false);
        }
        else if (Current_Level==4)
        {
            show0.SetActive(false);
            show1.SetActive(false);
            show2.SetActive(false);
            show3.SetActive(false);
            show4.SetActive(true);
        }
        else if (Current_Level==0)
        {
            show0.SetActive(true);
            show1.SetActive(false);
            show2.SetActive(false);
            show3.SetActive(false);
            show4.SetActive(false);
        }
    }

    public void SetParameter(float P, float I, float D)
    {
        trajectory.Kp = P;
        trajectory.Ki = I;
        trajectory.Kd = D;
        TextP1.text = P.ToString();
        TextI1.text = I.ToString();
        TextD1.text = D.ToString();
        TextP2.text = P.ToString();
        TextI2.text = I.ToString();
        TextD2.text = D.ToString();
        TextP3.text = P.ToString();
        TextI3.text = I.ToString();
        TextD3.text = D.ToString();
        TextP4.text = P.ToString();
        TextI4.text = I.ToString();
        TextD4.text = D.ToString();
        TextP0.text = P.ToString();
        TextI0.text = I.ToString();
        TextD0.text = D.ToString();
    }

    public void SetInstruction(int step)
    {
        if (step==0){
            TextInstructionExplain.text = "you can select parameter\nat right side of the screen\n**Click on tuning button again for ready";
            TextInstruction.text = "This is Propotional Control (P) \nThe farther you are from the target,\nthe harder the system reacts. \nTry increasing P from 1 to 5 and see what happens!\n**you can see P,I and D value at the TV above";
        }
        else if(step==1){
            TextInstructionExplain.text = "Somethime P is not enough to\nreach the target This\nintroduce I to help";
            TextInstruction.text = "This is Integral (I)\nIt react by the accumulated error over time.\nUse it to help reach the target\nby try set it from 0 to 2 and see what happen!";
        }
        else if (step==2){
            TextInstructionExplain.text = "as you can see it not\na stable reaction from our player\nthis introduce D to help";
            TextInstruction.text = "This is Derivative control (D)\nIt reacts to how fast the error is\nchanging, like if error change too fast it resist.\nuse it to make our movement smoother\ntry set it from 0 to 1.5 to see what happen!";
        }
        else if (step==3){
            TextInstructionExplain.text = "";
            TextInstruction.text = "Tutorial is Finished!\nyou can continue to experiment\nClick Back to exit the tutorial\nGood Luck!";
        }
    }
    
    public void NextLevel()
    {
        if (Current_Level==1)
        {   
            Current_Level = 2;
            show0.SetActive(false);
            show1.SetActive(false);
            show2.SetActive(true);
            show3.SetActive(false);
            show4.SetActive(false);
        }
        else if (Current_Level==2)
        {
            Current_Level = 3;
            show0.SetActive(false);
            show1.SetActive(false);
            show2.SetActive(false);
            show3.SetActive(true);
            show4.SetActive(false);
        }
        else if (Current_Level==3)
        {
            Current_Level = 4;
            show0.SetActive(false);
            show1.SetActive(false);
            show2.SetActive(false);
            show3.SetActive(false);
            show4.SetActive(true);
        }
        else if (Current_Level==4)
        {
            Current_Level = 0;
            show0.SetActive(true);
            show1.SetActive(false);
            show2.SetActive(false);
            show3.SetActive(false);
            show4.SetActive(false);
        }
        else if (Current_Level==0)
        {
            Current_Level = 1;
            show0.SetActive(false);
            show1.SetActive(true);
            show2.SetActive(false);
            show3.SetActive(false);
            show4.SetActive(false);
        }
    }

    public void PrevLevel()
    {
        if (Current_Level==1)
        {   
            Current_Level = 0;
            show0.SetActive(true);
            show1.SetActive(false);
            show2.SetActive(false);
            show3.SetActive(false);
            show4.SetActive(false);
        }
        else if (Current_Level==2)
        {
            Current_Level = 1;
            show0.SetActive(false);
            show1.SetActive(true);
            show2.SetActive(false);
            show3.SetActive(false);
            show4.SetActive(false);
        }
        else if (Current_Level==3)
        {
            Current_Level = 2;
            show0.SetActive(false);
            show1.SetActive(false);
            show2.SetActive(true);
            show3.SetActive(false);
            show4.SetActive(false);
        }
        else if (Current_Level==4)
        {
            Current_Level = 3;
            show0.SetActive(false);
            show1.SetActive(false);
            show2.SetActive(false);
            show3.SetActive(true);
            show4.SetActive(false);
        }
        else if (Current_Level==0)
        {
            Current_Level = 4;
            show0.SetActive(false);
            show1.SetActive(false);
            show2.SetActive(false);
            show3.SetActive(false);
            show4.SetActive(true);
        }
    }
    public void EnterLevel()
    {   
        show0.SetActive(false);
        show1.SetActive(false);
        show2.SetActive(false);
        show3.SetActive(false);
        show4.SetActive(false);
        trajectory.Level = Current_Level;
        if (Current_Level==1)
        {
            trajectory.Load_motor = 0.12f;
            trajectory.position = 0.5f;
            trajectory.targetPosition = 0.5f;
            trajectory.offset_x_local = 0.0f;
            enviroment0.SetActive(false);
            enviroment1.SetActive(true);
            enviroment2.SetActive(false);
            enviroment3.SetActive(false);
            enviroment4.SetActive(false);
        }
        else if (Current_Level==2)
        {
            trajectory.Load_motor = 0.12f;
            trajectory.position = 0.5f;
            trajectory.targetPosition = 0.5f;
            trajectory.offset_x_local = 0.0f;
            enviroment0.SetActive(false);
            enviroment1.SetActive(false);
            enviroment2.SetActive(true);
            enviroment3.SetActive(false);
            enviroment4.SetActive(false);
        }
        else if (Current_Level==3)
        {
            trajectory.Load_motor = 0.12f;
            trajectory.position = 0.5f;
            trajectory.targetPosition = 0.5f;
            trajectory.offset_x_local = 0.0f;
            enviroment0.SetActive(false);
            enviroment1.SetActive(false);
            enviroment2.SetActive(false);
            enviroment3.SetActive(true);
            enviroment4.SetActive(false);
        }
        else if (Current_Level==4)
        {
            trajectory.Load_motor = 0.12f;
            trajectory.position = 0.5f;
            trajectory.targetPosition = 0.5f;
            trajectory.offset_x_local = 0.0f;
            enviroment0.SetActive(false);
            enviroment1.SetActive(false);
            enviroment2.SetActive(false);
            enviroment3.SetActive(false);
            enviroment4.SetActive(true);
        }
        else if (Current_Level==0)
        {
            trajectory.Load_motor = 0.12f;
            trajectory.position = 0.3f;
            trajectory.targetPosition = 0.3f;
            trajectory.offset_x_local = 0.7f;
            enviroment0.SetActive(true);
            enviroment1.SetActive(false);
            enviroment2.SetActive(false);
            enviroment3.SetActive(false);
            enviroment4.SetActive(false);
        }
    }
}
