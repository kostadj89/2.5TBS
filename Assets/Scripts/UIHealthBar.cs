using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    private Slider sliderProp;

    void Awake()
    {
        sliderProp = gameObject.GetComponent<Slider>();
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void SetCurrentHealth(int health)
    {
        sliderProp.value = health;
    }

    internal void SetMaxHealth(int health)
    {
        sliderProp.maxValue = health;
        SetCurrentHealth(health);
    }
}
