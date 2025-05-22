using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UIManagement : MonoBehaviour
{
    public SimulationSpace SimulationSpace;
    private int UI_State = 0; //0 waiting for QR, 1 selecting level, 2 In_Level, 3 Tuning, 4 Simulation 5 Winning
    public int step_tutorial = 0;

    public float Current_P = 1.0f;
    public float Current_I = 0.0f;
    public float Current_D = 0.0f;
    public int current_selected = 0; // p1 i2 d3
    public GameObject B_Play;
    public GameObject B_Select;
    public GameObject B_Prev;
    public GameObject B_Next;
    public GameObject B_Stop;
    public GameObject B_Back;
    public GameObject B_Increase;
    public GameObject B_Decrease;
    public float delta_value = 0.1f;
    public GameObject B_P;
    public GameObject B_I;
    public GameObject B_D;
    public GameObject DetectMarker;
    public GameObject Debuger;
    public GameObject B_Finish;
    public GameObject B_NumPad;
    public GameObject B_Keyboard;
    public GameObject B_OnP;
    public GameObject B_OnI;
    public GameObject B_OnD;
    public GameObject B_OffP;
    public GameObject B_OffI;
    public GameObject B_OffD;
    public Texture P_config;
    public Texture I_config;
    public Texture D_config;
    public Texture Back_config;
    private bool havePlay = false;
    private bool showP = true;
    private bool showI = true;
    private bool showD = true;
    private string stackString = "";
    private bool havingDot = false;
    public SoundManagerScript soundManager;
    private bool play_effect = false;

    public TextMeshProUGUI TextStackString;
    public GameObject frame_P;
    public GameObject frame_I;
    public GameObject frame_D;
 
    
    //for time delay
    private float updateInterval = 0.1f;  // Interval for updating trajectory
    private float elapsedTime = 0.0f;
   
    void Start()
    {
        B_Play.SetActive(false);
        B_Select.SetActive(false);
        B_Prev.SetActive(false);
        B_Next.SetActive(false);
        B_Stop.SetActive(false);
        B_Back.SetActive(false);
        B_Increase.SetActive(false);
        B_Decrease.SetActive(false);
        B_P.SetActive(false);
        B_I.SetActive(false);
        B_D.SetActive(false);
        DetectMarker.SetActive(false);
        B_Finish.SetActive(false);
        B_NumPad.SetActive(false);
        B_Keyboard.SetActive(false);
        Debuger.SetActive(false);
        OffshowPID();
        frame_P.SetActive(false);
        frame_I.SetActive(false);
        frame_D.SetActive(false);
    }

    // // Update is called once per frame
    void Update()
    {
        if (SimulationSpace == null) 
        {
            // Find an object with the tag "SimSpace" and get its SimulationSpace component
            GameObject simSpaceObject = GameObject.FindWithTag("SimSpace");
            if (simSpaceObject != null)
            {
                SimulationSpace = simSpaceObject.GetComponent<SimulationSpace>();
                UI_State = 1;
                Update_State();
            }
            else 
            {
                UI_State = 0;
                Update_State();
            }
        }
        else 
        {
            // Accumulate time
            elapsedTime += Time.deltaTime;  

            // Loop to handle time-based calling of UpdateTrajectory
            for (; elapsedTime >= updateInterval; elapsedTime -= updateInterval)
            {
                int state = SimulationSpace.Check_State_tra();
                if (state == 1) // Winning
                {
                    On_Winning();
                }
                else if (state == 2) // Losing
                {
                    On_losing();
                }
                else if (state == 3) // Reset tutorial
                {
                    // step_tutorial = +=1;
                    EndPointInTutorial();
                    //Update step tutorial
                }
            }
        }
    }



    // Moving Menu
    public void Update_Parameter()
    {
        if (SimulationSpace.Current_Level == 0) {
            if (step_tutorial == 0)
            {
                if (Current_P>5){
                    Current_P = 5f;
                }
            }
            if (step_tutorial == 1)
            {
                if (Current_I>1){
                    Current_I = 1f;
                }
            }
            if (step_tutorial == 2)
            {
                if (Current_D>1f){
                    Current_D = 1f;
                }
            }
        }
        SimulationSpace.SetParameter(Current_P, Current_I, Current_D);
        
    }
    public void Update_State()
    {
        // string result = $"P: {Current_P} I: {Current_I} D: {Current_D}";
        // Debuger.GetComponent<TextMeshProUGUI>().text = result;
        if (UI_State == 0) // looking for qr
        {
            B_Play.SetActive(false);
            B_Select.SetActive(false);
            B_Prev.SetActive(false);
            B_Next.SetActive(false);
            B_Stop.SetActive(false);
            B_Back.SetActive(false);
            B_Increase.SetActive(false);
            B_Decrease.SetActive(false);
            B_P.SetActive(false);
            B_I.SetActive(false);
            B_D.SetActive(false);
            DetectMarker.SetActive(true);
            B_Finish.SetActive(false);
            B_NumPad.SetActive(false);
            B_Keyboard.SetActive(false);
            OffshowPID();
        }
        else if (UI_State == 1) // selecting level
        {
            B_Play.SetActive(false);
            B_Select.SetActive(true);
            B_Prev.SetActive(true);
            B_Next.SetActive(true);
            B_Stop.SetActive(false);
            B_Back.SetActive(false);
            B_Increase.SetActive(false);
            B_Decrease.SetActive(false);
            B_P.SetActive(false);
            B_I.SetActive(false);
            B_D.SetActive(false);
            DetectMarker.SetActive(false);
            B_Finish.SetActive(false);
            B_NumPad.SetActive(false);
            B_Keyboard.SetActive(false);
            frame_P.SetActive(false);
            frame_I.SetActive(false);
            frame_D.SetActive(false);
            OffshowPID();
            soundManager.PlayVFX_munu();
        }
        else if (UI_State == 2) // in level
        {
            B_Play.SetActive(true);
            B_Select.SetActive(false);
            B_Prev.SetActive(false);
            B_Next.SetActive(false);
            B_Stop.SetActive(false);
            B_Back.SetActive(true);
            B_Increase.SetActive(false);
            B_Decrease.SetActive(false);
            /////PID button
            if (SimulationSpace.Current_Level == 0)
            {  
                B_P.SetActive(true);
                if (step_tutorial==0) {
                    frame_P.SetActive(true);
                    frame_I.SetActive(false);
                    frame_D.SetActive(false);
                    B_I.SetActive(false);
                    B_D.SetActive(false);
                }
                else if (step_tutorial==1){
                    frame_P.SetActive(false);
                    frame_I.SetActive(true);
                    frame_D.SetActive(false);
                    B_I.SetActive(true);
                    B_D.SetActive(false);
                }
                else {
                    frame_P.SetActive(false);
                    frame_I.SetActive(false);
                    frame_D.SetActive(true);
                    B_I.SetActive(true);
                    B_D.SetActive(true);
                }
            }
            else {
                B_P.SetActive(true);
                B_I.SetActive(true);
                B_D.SetActive(true);
                frame_P.SetActive(false);
                frame_I.SetActive(false);
                frame_D.SetActive(false);
            }


            DetectMarker.SetActive(false);
            B_Finish.SetActive(false);
            B_NumPad.SetActive(false);
            B_Keyboard.SetActive(false);
            showOnOffPid();
            if ((SimulationSpace.Current_Level==1) || (SimulationSpace.Current_Level==2))
            {
                soundManager.PlayVFX_1_2_music();
            }
            else if ((SimulationSpace.Current_Level== 3) || (SimulationSpace.Current_Level==4)) {
                soundManager.PlayVFX_3_4_music();
                }
            else if (SimulationSpace.Current_Level== 0){
                soundManager.PlayVFX_tutorial();
            }
        }
        else if (UI_State == 3) // tuning
        {
            B_Play.SetActive(false);
            B_Select.SetActive(false);
            B_Prev.SetActive(false);
            B_Next.SetActive(false);
            B_Stop.SetActive(false);
            B_Back.SetActive(false);
            B_Increase.SetActive(true);
            B_Decrease.SetActive(true);
            /////PID button
            if (SimulationSpace.Current_Level == 0)
            {  
                B_P.SetActive(true);
                if (step_tutorial==0) {
                    B_I.SetActive(false);
                    B_D.SetActive(false);
                }
                else if (step_tutorial==1){
                    B_I.SetActive(true);
                    B_D.SetActive(false);
                }
                else {
                    B_I.SetActive(true);
                    B_D.SetActive(true);
                }
            }
            else {
                B_P.SetActive(true);
                B_I.SetActive(true);
                B_D.SetActive(true);
                frame_P.SetActive(false);
                frame_I.SetActive(false);
                frame_D.SetActive(false);
            }
            DetectMarker.SetActive(false);
            B_Finish.SetActive(false);
            B_NumPad.SetActive(false);
            B_Keyboard.SetActive(true);
            showOnOffPid();
        }
        else if (UI_State == 4) //  Simulation
        {
            B_Play.SetActive(false);
            B_Select.SetActive(false);
            B_Prev.SetActive(false);
            B_Next.SetActive(false);
            B_Stop.SetActive(true);
            B_Back.SetActive(false);
            B_Increase.SetActive(false);
            B_Decrease.SetActive(false);
            B_P.SetActive(false);
            B_I.SetActive(false);
            B_D.SetActive(false);
            DetectMarker.SetActive(false);
            B_Finish.SetActive(false);
            B_NumPad.SetActive(false);
            B_Keyboard.SetActive(false);
            frame_P.SetActive(false);
            frame_I.SetActive(false);
            frame_D.SetActive(false);
            OffshowPID();
        }
        else if (UI_State == 5) //  B_Finished
        {
            B_Play.SetActive(false);
            B_Select.SetActive(false);
            B_Prev.SetActive(false);
            B_Next.SetActive(false);
            B_Stop.SetActive(false);
            B_Back.SetActive(false);
            B_Increase.SetActive(false);
            B_Decrease.SetActive(false);
            B_P.SetActive(false);
            B_I.SetActive(false);
            B_D.SetActive(false);
            DetectMarker.SetActive(false);
            B_Finish.SetActive(true);
            B_NumPad.SetActive(false);
            B_Keyboard.SetActive(false);
            frame_P.SetActive(false);
            frame_I.SetActive(false);
            frame_D.SetActive(false);
            showOnOffPid();
        }
    }

    // On Tricker button
    public void On_Keyboard()
    {
        stackString = "";
        B_Increase.SetActive(false);
        B_Decrease.SetActive(false);
        B_NumPad.SetActive(true);
        B_Keyboard.SetActive(false);
        updateStackShow(); 
        OffshowPID();
        soundManager.PlaySound_UI_Common();
    }
    public void updateStackShow()
    {
        TextStackString.text = stackString;
    }
    public void On_1(){stackString += "1";updateStackShow();soundManager.PlaySound_UI_Common();}
    public void On_2(){stackString += "2";updateStackShow();soundManager.PlaySound_UI_Common();}
    public void On_3(){stackString += "3";updateStackShow();soundManager.PlaySound_UI_Common();}
    public void On_4(){stackString += "4";updateStackShow();soundManager.PlaySound_UI_Common();}
    public void On_5(){stackString += "5";updateStackShow();soundManager.PlaySound_UI_Common();}
    public void On_6(){stackString += "6";updateStackShow();soundManager.PlaySound_UI_Common();}
    public void On_7(){stackString += "7";updateStackShow();soundManager.PlaySound_UI_Common();}
    public void On_8(){stackString += "8";updateStackShow();soundManager.PlaySound_UI_Common();}
    public void On_9(){stackString += "9";updateStackShow();soundManager.PlaySound_UI_Common();}
    public void On_0(){stackString += "0";updateStackShow();soundManager.PlaySound_UI_Common();}
    public void On_dot()
    {
        if (havingDot){return;}
        havingDot=true;
        stackString += ".";
        updateStackShow();
        soundManager.PlaySound_UI_Common();
    }
    public void Pop_out()
    {
        if (stackString.Length > 0)
        {
            // Get the last character
            char lastChar = stackString[stackString.Length - 1];

            // Remove the last character
            stackString = stackString.Substring(0, stackString.Length - 1);
            if (lastChar == '.'){havingDot = false;}
        }
        updateStackShow();
        soundManager.PlaySound_UI_Common();
    }
    public void Cancel()
    {
        B_Increase.SetActive(true);
        B_Decrease.SetActive(true);
        B_NumPad.SetActive(false);
        B_Keyboard.SetActive(true);
        havingDot = false;
        showOnOffPid();
        soundManager.PlaySound_UI_Common();
    }
    public void On_Enter_Num()
    {
        if (current_selected == 1)
        {
            Current_P = GetFloat();
        }
        else if (current_selected == 2)
        {
            Current_I = GetFloat();
        }
        else if (current_selected == 3)
        {
            Current_D = GetFloat();
        }
        // stackString = "";
        Cancel();
        Update_Parameter();
        soundManager.PlaySound_UI_Common();
    }

    public float GetFloat()
    {
        // Convert string to float safely
        if (float.TryParse(stackString, out float result))
        {
            return result;
        }
        return 0f; // Return 0 if conversion fails
    }

    public void Back()
    {
        step_tutorial =0;
        Current_P = 1.0f;
        Current_I= 0.0f;
        Current_D = 0.0f;
        SimulationSpace.Back();
        UI_State = 1;
        Update_State();
        havePlay = false;
        soundManager.PlayVFX_munu();
        soundManager.PlaySound_UI_Common();
    }
    public void PlayButton()
    {
        play_effect=false;
        SimulationSpace.Play();
        UI_State = 4;
        havePlay = true;
        Update_State();
        soundManager.PlaySound_UI_Common();
        
    }
    public void PrevLevel()
    {
        SimulationSpace.PrevLevel();
        soundManager.PlaySound_UI_Common();
    }
    public void NextLevel()
    {
        SimulationSpace.NextLevel();
        soundManager.PlaySound_UI_Common();
    }
    public void EnterLevel()
    {
        SimulationSpace.EnterLevel();
        UI_State = 2;
        Update_State();
        havePlay = false;
        soundManager.PlaySound_UI_Selected();
        Update_Parameter();
        SimulationSpace.SetInstruction(step_tutorial);
    }
    public void Stop()
    {
        play_effect=false;
        havePlay = false;
        SimulationSpace.Stop();
        UI_State = 2;
        Update_State();
        havePlay=false;
        soundManager.PlaySound_UI_Common();
    }
    public void Increase()
    {
        if (current_selected == 1)
        {
            Current_P+=delta_value;
            // Current_P = (float)Mathf.Round(Current_P);
            string roundedString = Current_P.ToString("0.0");
            Current_P = float.Parse(roundedString);
            
        }
        else if (current_selected == 2)
        {
            Current_I+=delta_value;
            // Current_I = (float)Mathf.Round(Current_I);
            string roundedString = Current_I.ToString("0.0");
            Current_I = float.Parse(roundedString);
        }
        else if (current_selected == 3)
        {
            Current_D+=delta_value;
            // Current_D = (float)Mathf.Round(Current_D);
            string roundedString = Current_D.ToString("0.0");
            Current_D = float.Parse(roundedString);
        }
        Update_State();
        Update_Parameter();
        soundManager.PlaySound_UI_Common();
    }
    public void Decrease()
    {
        if (current_selected == 1)
        {
            Current_P-=delta_value;
            string roundedString = Current_P.ToString("0.0");
            Current_P = float.Parse(roundedString);
        }
        else if (current_selected == 2)
        {
            Current_I-=delta_value;
            string roundedString = Current_I.ToString("0.0");
            Current_I = float.Parse(roundedString);
        }
        else if (current_selected == 3)
        {
            Current_D-=delta_value;
            string roundedString = Current_D.ToString("0.0");
            Current_D = float.Parse(roundedString);
        }
        Update_State();
        Update_Parameter();
        soundManager.PlaySound_UI_Common();
    }

    public void P_Click()
    {
        if (current_selected == 1 )
        {
            current_selected = 0;
            B_P.GetComponent<RawImage>().texture = P_config; 
            UI_State = 2;
            Update_State();
        }
        else
        {
            /// Set all image for P
            B_P.GetComponent<RawImage>().texture = Back_config; 
            B_I.GetComponent<RawImage>().texture = I_config;
            B_D.GetComponent<RawImage>().texture = D_config; 
            current_selected = 1;
            UI_State = 3;
            Update_State();
        }
        soundManager.PlaySound_UI_Config();
    }
    public void I_Click()
    {
        if (current_selected == 2)
        {
            current_selected = 0;
            B_I.GetComponent<RawImage>().texture = I_config; 
            UI_State = 2;
            Update_State();
        }
        else
        {
            /// Set all image for I
            B_P.GetComponent<RawImage>().texture = P_config; 
            B_I.GetComponent<RawImage>().texture = Back_config; 
            B_D.GetComponent<RawImage>().texture = D_config;
            current_selected = 2;
            UI_State = 3;
            Update_State();
        }
        soundManager.PlaySound_UI_Config();
    }
    public void D_Click()
    {
        if (current_selected == 3)
        {
            current_selected = 0;
            B_D.GetComponent<RawImage>().texture = D_config; 
            UI_State = 2;
            Update_State();
        }
        else 
        {
            /// Set all image for D
            B_P.GetComponent<RawImage>().texture = P_config;
            B_I.GetComponent<RawImage>().texture = I_config;
            B_D.GetComponent<RawImage>().texture = Back_config; 
            current_selected = 3;
            UI_State = 3;
            Update_State();

        }
        soundManager.PlaySound_UI_Config();
    }

    public void On_losing()
    {
        UI_State = 2;
        Update_State();
        SimulationSpace.Reset_State_tra();
        if (play_effect==false)
        {
            soundManager.PlayEffect_lossing();
            play_effect=true;
        }
    }

    public void EndPointInTutorial()
    {
        UI_State = 2;
        SimulationSpace.Reset_State_tra();
        if (SimulationSpace.Current_Level == 0) {
            if (step_tutorial == 0){
                if (Current_P>3.9){
                    step_tutorial = 1;
                    // frame_P.SetActive(false);
                    // frame_I.SetActive(true);
                    // frame_D.SetActive(false);
                }
            }
            if (step_tutorial == 1){
                if (Current_I>0.9){
                    step_tutorial = 2;
                    // frame_I.SetActive(false);
                    // frame_P.SetActive(false);
                    // frame_D.SetActive(true);
                }
            }
            if (step_tutorial == 2){
                if (Current_D>0.9){
                    step_tutorial = 3;
                    // frame_D.SetActive(true);
                    // frame_I.SetActive(false);
                    // frame_P.SetActive(false);
                }
            }
        }
        SimulationSpace.SetInstruction(step_tutorial);
        Update_State();

    }

    public void On_Winning()
    {
        UI_State = 5;
        Update_State();
        if (play_effect==false) 
        {
            soundManager.PlayEffect_winning();
            play_effect=true;
        }
    }
    public void Return_to_select()
    {
        UI_State = 1;
        play_effect=false;
        Update_State();
        SimulationSpace.BackToLevel();
        SimulationSpace.Reset_State_tra();
        soundManager.PlaySound_UI_Selected();
    }

    public void showOnOffPid()
    {
        if (havePlay)
        {
            if (showP)
                {
                    B_OnP.SetActive(false);
                    B_OffP.SetActive(true);
                }
            else 
                {
                    B_OnP.SetActive(true);
                    B_OffP.SetActive(false);
                }

            if(showI)
                {
                    B_OnI.SetActive(false);
                    B_OffI.SetActive(true);
                }
            else 
                {
                    B_OnI.SetActive(true);
                    B_OffI.SetActive(false);
                }

            if(showD)
                {
                    B_OnD.SetActive(false);
                    B_OffD.SetActive(true);
                }
            else 
                {
                    B_OnD.SetActive(true);
                    B_OffD.SetActive(false);
                }
        }
        else 
        {
            B_OnP.SetActive(false);
            B_OffP.SetActive(false);
            B_OnI.SetActive(false);
            B_OffI.SetActive(false);
            B_OnD.SetActive(false);
            B_OffD.SetActive(false);   
        }
        soundManager.PlaySound_UI_Common();
    }
    public void OffshowPID()
    {
        B_OnP.SetActive(false);
        B_OffP.SetActive(false);
        B_OnI.SetActive(false);
        B_OffI.SetActive(false);
        B_OnD.SetActive(false);
        B_OffD.SetActive(false);
    }
    public void On_OnP(){SimulationSpace.trajectory.calPOn();showP = true;showOnOffPid();soundManager.PlaySound_UI_Common();}
    public void On_OnI(){SimulationSpace.trajectory.calIOn();showI = true;showOnOffPid();soundManager.PlaySound_UI_Common();}
    public void On_OnD(){SimulationSpace.trajectory.calDOn();showD = true;showOnOffPid();soundManager.PlaySound_UI_Common();}
    public void On_OffP(){SimulationSpace.trajectory.calPOff();showP=false;showOnOffPid();soundManager.PlaySound_UI_Common();}
    public void On_OffI(){SimulationSpace.trajectory.calIOff();showI=false;showOnOffPid();soundManager.PlaySound_UI_Common();}
    public void On_OffD(){SimulationSpace.trajectory.calDOff();showD=false;showOnOffPid();soundManager.PlaySound_UI_Common();}
}
