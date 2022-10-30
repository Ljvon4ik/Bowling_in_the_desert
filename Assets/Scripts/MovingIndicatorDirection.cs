using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingIndicatorDirection : MonoBehaviour
{
    private float zLimit = 4f;
    public float speedIndicatorDirection = 5f;
    private bool isMovingRight;

    void Update()
    {
        if (isMovingRight)
        {
            transform.Translate(Vector3.forward * speedIndicatorDirection * Time.deltaTime, Space.World);
        }
        else
        {
            transform.Translate(Vector3.forward * -speedIndicatorDirection * Time.deltaTime, Space.World);

        }

        if (transform.position.z > zLimit)
        {
            isMovingRight = false;
        }

        if (transform.position.z < -zLimit)
        {
            isMovingRight = true;
        }    
    }
}
