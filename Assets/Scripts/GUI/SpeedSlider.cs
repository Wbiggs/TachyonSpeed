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
        Vector3 magnitude = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        currentSpeed=magnitude.magnitude;
        speedSlider.value = currentSpeed;
    }
}
