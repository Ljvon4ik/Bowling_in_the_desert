using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBall : MonoBehaviour
{
    private Rigidbody rbBall;
    [SerializeField] GameObject indicatorDirection;
    public bool ballRoll;
    private float xLimit = -3f;
    private float minSpeed = 1.5f;
    [SerializeField] GameManager gameManager;
    public float delayBallSpeedCheck = 0.05f;
    public float delayStartCountingFallenPins = 4f;

    private void Start()
    {
        rbBall = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //Ball removal condition
        if (rbBall.velocity.magnitude < minSpeed && ballRoll || transform.position.x < xLimit)
        {
            StartCoroutine(StartCountingFallenPins());
        }
    }

    public void StartActionBall(float throwForce)
    {
        Vector3 loocDirection = (indicatorDirection.transform.position - transform.position).normalized;
        rbBall.AddForce(loocDirection * throwForce, ForceMode.Impulse);
        StartCoroutine(ActionBall());
    }

    IEnumerator ActionBall()
    {
        yield return new WaitForSeconds(delayBallSpeedCheck);
        ballRoll = true;
    }

    IEnumerator StartCountingFallenPins()
    {
        yield return new WaitForSeconds(delayStartCountingFallenPins);
        gameObject.SetActive(false);
        gameManager.CountingFallenPins();
    }
}
