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

    public Vector2 moveValue;
    public Vector3 position;
    public Vector3 oldPosition;
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
    public Light fillLight;

    enum modes { Nromal, Distance, Vision};
    int mode = 1;

    void Start()
    {
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
        if (mode < 2) { mode++; }
        else mode = 0;
    }
    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(moveValue.x, 0.0f, moveValue.y);
        GetComponent<Rigidbody>().AddForce(movement * speed * Time.fixedDeltaTime);
        oldPosition = position;
        //newPosition = transform.position;
        position = transform.position;
        velocity = CountDistance(oldPosition, position) / Time.fixedDeltaTime;
        
        if (mode == 0)
        {
            positionText.text = null;
            distanceText.text = null;
            velocityText.text = null;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position);
            
            fillLight.color = Color.white;
            for (int n = 0; n < pickUp.Length; n++)
            {              
                pickUp[n].GetComponent<Renderer>().material.color = Color.white;
            }
        }
        if (mode == 1)
        {
            lineRenderer.gameObject.SetActive(true);
            
            fillLight.color = Color.blue;
            SetPositionText();
            SetDistanceText();
            SetVelocityText();
        }
        if(mode == 2)
        {
            lineRenderer.gameObject.SetActive(true);
            positionText.text = null;
            distanceText.text = null;
            velocityText.text = null;
            fillLight.color = Color.green;
            mode3();
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
        isActive[0] = pickUp1.activeSelf;
        isActive[1] = pickUp2.activeSelf;
        isActive[2] = pickUp3.activeSelf;
        isActive[3] = pickUp4.activeSelf;
        isActive[4] = pickUp5.activeSelf;
       
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
        private float CountDistance2(Vector3 d1, Vector3 d2)
    {
        float distance = 0;
        distance = (float)(Math.Pow(Math.Abs(d1.x * d2.z - d2.x * d1.z), 0.5) / Math.Pow(Math.Pow(d1.x, 2) + Math.Pow(d1.z, 2), 0.5));
        return distance;
    }

    private void mode3()
    {
        Vector3 towards = (transform.position - oldPosition) * 50;
      
        //double distance = 0;

        isActive[0] = pickUp1.activeSelf;
        isActive[1] = pickUp2.activeSelf;
        isActive[2] = pickUp3.activeSelf;
        isActive[3] = pickUp4.activeSelf;
        isActive[4] = pickUp5.activeSelf;

        minDistance = 99999;
        int min = 9;
        for (int n = 0; n < pickUp.Length; n++)
        {
            if (isActive[n])
            {
                if (CountDistance2(towards, pickUp[n].transform.position - transform.position) <= minDistance && (towards.x* (pickUp[n].transform.position - transform.position).x+ towards.z * (pickUp[n].transform.position - transform.position).z) >= 0)
                {
                    minDistance = CountDistance2(towards, pickUp[n].transform.position - transform.position);
                    min = n;
                }                
            }
        }
        for (int n = 0; n < pickUp.Length; n++)
        {
            if (n == min)
            {
                pickUp[n].GetComponent<Renderer>().material.color = Color.green;
                pickUp[n].transform.LookAt(transform.position);
           
            }
            else
            {
                pickUp[n].GetComponent<Renderer>().material.color = Color.white;
            }
        }
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position+towards);
    }
}
