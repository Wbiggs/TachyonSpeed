using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedSlider : MonoBehaviour
{
    public float currentSpeed;
    public float maxSpeed;

    public GameObject SliderUI;
    public Slider speedSlider;
    public PlayerMovement player;
    // Start is called before the first frame update
    void Start()
    {
        sliderSpeed();
    }

    // Update is called once per frame
    void Update()
    {
        sliderSpeed();
    }

    private void sliderSpeed()
    {
        currentSpeed=player.moveSpeed;
        maxSpeed=player.maxSpeed;
        speedSlider.maxValue= maxSpeed;
        speedSlider.value = currentSpeed;
    }
}
