using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;
using System;

public class PlayerController : MonoBehaviour
{
    //public GameObject player;
    public Vector2 moveValue;
    public Vector3 position;
    public Vector3 newPosition;
    public float speed;
    public float velocity;
    private int count;
    private int numPickups = 5;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI winText;
    public TextMeshProUGUI positionText;
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI velocityText;
    
    public GameObject pickUp1;
    public GameObject pickUp2;
    public GameObject pickUp3;
    public GameObject pickUp4;
    public GameObject pickUp5;
    public GameObject[] pickUp = new GameObject[5];
    private bool[] isActive=new bool[5];
    float minDistance = 99999;

    private LineRenderer lineRenderer;

    enum modes { Nromal, Distance, Vision};
    int mode = 1;

    void Start()
    {
        Debug.Log("0");
        count = 0;
        pickUp[0] = pickUp1;
        pickUp[1] = pickUp2;
        pickUp[2] = pickUp3;
        pickUp[3] = pickUp4;
        pickUp[4] = pickUp5;
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        position = transform.position;
        winText.text = " ";
        SetCountText();
        SetPositionText();
        SetVelocityText();
    }

    void OnMove(InputValue value)
    {
        moveValue = value.Get<Vector2>();
    }

    void OnChangeMode(InputValue value)
    {
        Debug.Log("mode");
        if (mode < 2) { mode++; }
        else mode = 0;
        Debug.Log(mode);
    }
    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(moveValue.x, 0.0f, moveValue.y);
        GetComponent<Rigidbody>().AddForce(movement * speed * Time.fixedDeltaTime);
        newPosition = transform.position;
        velocity = CountDistance(newPosition, position) / Time.fixedDeltaTime;
        position = transform.position;
        if (mode == 0)
        {
            positionText.text = null;
            distanceText.text = null;
            velocityText.text = null;
        }
        if (mode == 1)
        {
            SetPositionText();
            SetDistanceText();
            SetVelocityText();
        }

        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PickUp")
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();
        }

    }
    private void SetCountText()
    {
        scoreText.text = "Score: " + count.ToString();
        if (count >= numPickups)
        {
            winText.text = "You win!";
            Time.timeScale = 0;
            //Destroy(lineRenderer);
        }
    }

    private void SetPositionText()
    {
        positionText.text = "Position: " + transform.position.x.ToString("0.00") + " , " + transform.position.z.ToString("0.00");
    }

    private void SetDistanceText()
    {
        //int i = 0;
        isActive[0] = pickUp1.activeSelf;
        isActive[1] = pickUp2.activeSelf;
        isActive[2] = pickUp3.activeSelf;
        isActive[3] = pickUp4.activeSelf;
        isActive[4] = pickUp5.activeSelf;
        /*if (pickUp1.activeSelf == true)
        {
            isActive[0] = 1;
            i++;
        }
        if (pickUp2.activeSelf == true)
        {
            isActive[1] = 1;
            i++;
        }
        if (pickUp3.activeSelf == true)
        {
            isActive[2] = 1;
            i++;
        }
        if (pickUp4.activeSelf == true)
        {
            isActive[3] = 1;
            i++;
        }
        if (pickUp5.activeSelf == true)
        {
            isActive[4] = 1;
        }*/

        //float[] array = { CountDistance(pickUp1.transform.position, position), CountDistance(pickUp2.transform.position, position), CountDistance(pickUp3.transform.position, position), CountDistance(pickUp4.transform.position, position), CountDistance(pickUp5.transform.position, position) };
        minDistance = 99999;
        int min = 0;
        for (int n = 0; n < pickUp.Length; n++)
        {
            if (isActive[n])
            {
                if (CountDistance(pickUp[n].transform.position, transform.position) <= minDistance)
                {
                    minDistance = CountDistance(pickUp[n].transform.position, transform.position);
                    min = n;
                }
            }
        }
        for (int n = 0; n < pickUp.Length; n++)
        {
            if (n == min)
            {
                pickUp[n].GetComponent<Renderer>().material.color = Color.blue;
                if(lineRenderer != null)
                {
                    lineRenderer.SetPosition(0, transform.position);
                    lineRenderer.SetPosition(1, pickUp[n].transform.position);
                }
                
            }
            else
            {
                pickUp[n].GetComponent<Renderer>().material.color = Color.white;
            }
        }
        
        distanceText.text = "Distance: " +  minDistance.ToString("0.00");
    }
 
    private void SetVelocityText()
    {
        velocityText.text = "Velocity: " + velocity.ToString("0.00");
    }

   
    private float CountDistance(Vector3 d1, Vector3 d2)
    {
        float distance = 0;
        distance = (float)Math.Pow(Math.Pow((d1.x - d2.x), 2) + Math.Pow((d1.z - d2.z), 2), 0.5);
        return distance;
    }
}
