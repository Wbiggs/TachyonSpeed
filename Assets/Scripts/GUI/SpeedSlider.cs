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
    public Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        sliderSpeed();
        maxSpeed = player.maxSpeed;
        speedSlider.maxValue = maxSpeed;
    }

    // FixedUpdate is called once per frame
    void FixedUpdate()
    {
        sliderSpeed();
    }

    private void sliderSpeed()
    {
        currentSpeed=rb.velocity.magnitude;
        speedSlider.value = currentSpeed;
    }
}
