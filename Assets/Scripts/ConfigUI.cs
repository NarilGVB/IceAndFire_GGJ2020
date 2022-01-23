using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigUI : MonoBehaviour
{

    //Animator anim;
    [SerializeField] Slider sliderMaster, sliderMusic, sliderFx;

    private void Start()
    {
        //sliderMaster.value = 0.2f;
        //AudioManager.instance.ChangeVolume(0.2f, "Master Volume");
        /*
        if (PlayerPrefs.HasKey("MasterSliderValue"))
        {
            sliderMaster.value = PlayerPrefs.GetFloat("MasterSliderValue");
            sliderMusic.value = PlayerPrefs.GetFloat("MusicSliderValue");
            sliderFx.value = PlayerPrefs.GetFloat("FxSliderValue");
        }
        else
        {
            sliderMaster.value = 0.2f;
        }
        */

    }


    public void SaveConfig()
    {
        PlayerPrefs.SetFloat("MasterSliderValue", sliderMaster.value);
        PlayerPrefs.SetFloat("MusicSliderValue", sliderMusic.value);
        PlayerPrefs.SetFloat("FxSliderValue", sliderFx.value);

    }

    public void ChangeVolume(int sliderNumber)
    {
        if (sliderNumber == 0)
            AudioManager.instance.ChangeVolume(sliderMaster.value, "Master Volume");
        else if (sliderNumber == 1)
            AudioManager.instance.ChangeVolume(sliderMusic.value, "Music Volume");
        else if (sliderNumber == 2)
        {
            AudioManager.instance.ChangeVolume(sliderFx.value, "Fx Volume");
            //AudioManager.instance.PlayStar();
        }
    }

    

}
