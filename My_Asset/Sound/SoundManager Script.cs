using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    public AudioClip Menu_music;
    public AudioClip tutorial_music;
    public AudioClip music_1_2;
    public AudioClip music_3_4;
    public AudioClip button_select;
    public AudioClip button_config;
    public AudioClip button_common;
    public AudioClip loss_sound;
    public AudioClip win_sound;

    public AudioSource vfx_source;
    public AudioSource ui_source;
    public AudioSource effect_source;

    public void PlayVFX_munu()
    {
        if ((vfx_source.isPlaying) && (vfx_source.clip != Menu_music))
        {
            vfx_source.Stop();
        }
        if (vfx_source.clip != Menu_music)
        { 
            vfx_source.volume = 0.4f;
            vfx_source.clip = Menu_music;
            vfx_source.Play();
        }
        vfx_source.Play();
    }
    public void PlayVFX_tutorial()
    {
        if ((vfx_source.isPlaying) && (vfx_source.clip != tutorial_music))
        {
            vfx_source.Stop();
        }
        if (vfx_source.clip != tutorial_music)
        {
            vfx_source.volume = 0.5f;
            vfx_source.clip = tutorial_music;
            vfx_source.Play();
        }
    }

    public void PlayVFX_1_2_music()
    {
        if ((vfx_source.isPlaying) && (vfx_source.clip != music_1_2))
        {
            vfx_source.Stop();
        }
        if (vfx_source.clip != music_1_2)
        {
            vfx_source.volume = 0.5f;
            vfx_source.clip = music_1_2;
            vfx_source.Play();
        }
    }

    public void PlayVFX_3_4_music()
    {
        if ((vfx_source.isPlaying) && (vfx_source.clip != music_3_4))
        {
            vfx_source.Stop();
        }
        if (vfx_source.clip != music_3_4)
        {
            vfx_source.clip = music_3_4;
            vfx_source.volume = 1f;
            vfx_source.Play();
        }
    }

    public void PlaySound_UI_Config()
    {
        if ((ui_source.isPlaying))
        {
            ui_source.Stop();
        }
        
        ui_source.clip = button_config;
        ui_source.volume = 0.65f;
        ui_source.Play();
        
    }
    public void PlaySound_UI_Common()
    {
        if ((ui_source.isPlaying))
        {
            ui_source.Stop();
        }
        
        ui_source.clip = button_common;
        ui_source.volume = 0.9f;
        ui_source.Play();
        
    }
    public void PlaySound_UI_Selected()
    {
        if ((ui_source.isPlaying))
        {
            ui_source.Stop();
        }
        
        ui_source.clip = button_select;
        ui_source.volume = 0.65f;
        ui_source.Play();
      
    }

    public void PlayEffect_winning()
    {
        if ((effect_source.isPlaying))
        {
            effect_source.Stop();
        }
        
        effect_source.clip = win_sound;
        effect_source.volume = 1f;
        effect_source.Play();
        
    }
    public void PlayEffect_lossing()
    {
        if ((effect_source.isPlaying))
        {
            effect_source.Stop();
        }
        effect_source.clip = loss_sound;
        effect_source.volume = 1f;
        effect_source.Play();
    }

}
