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

    private void Start()
    {
        rbBall = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(rbBall.velocity.magnitude < minSpeed && ballRoll || transform.position.x < xLimit)
        {
            gameManager.BallChoice();
            gameObject.SetActive(false);
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
        yield return new WaitForSeconds(0.05f);
        ballRoll = true;
    }
}
