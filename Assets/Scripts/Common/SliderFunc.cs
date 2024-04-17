using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderFunc : MonoBehaviour {

    public Slider slider;
    public Text valueText;
    public Image Handle;
    public float SliderValue
    {
        get
        {
            return slider.value;
        }
        set
        {
            slider.value = value;
        }
    }

    private void Update()
    {
        if (valueText != null)
        {   
            SetValText(Math.Truncate(SliderValue * 100.0f).ToString());
        }
            
    }

    public void SetValText(string text)
    {
        valueText.text = text;
    }

}
