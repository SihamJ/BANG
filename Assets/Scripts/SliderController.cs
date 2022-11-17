using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public Slider slider;
    public AudioSource son;
    public Text name_bouton_son;

    // Update is called once per frame
    void Update()
    {
        if(slider.value > 0f)
        {
            son.mute = false;
            name_bouton_son.text = "DESACTIVER";
        }
        else{
            son.mute = true;
            name_bouton_son.text = "ACTIVER";
        }
    }
}
