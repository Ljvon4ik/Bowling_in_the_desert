using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class GameManager : MonoBehaviour
{
    [SerializeField] Slider sliderForce;
    private float timeDelaySliderUpdate = 0.05f;
    private bool isSliderForceMaxValue;
    private bool stopSliderForce;
    [SerializeField] GameObject[] balls;
    [SerializeField] Button restartButton;
    [SerializeField] GameObject indicatorDirection;
    private bool offIndicatorDirection;
    private GameObject selectedBall;
    private int index = -1;
    private float startSpeedIndicatorDirection;
    [SerializeField] TextMeshProUGUI ballsCounter;


    void Start()
    {
        restartButton.onClick.AddListener(RestartLevel);
        startSpeedIndicatorDirection = indicatorDirection.GetComponent<MovingIndicatorDirection>().speedIndicatorDirection;
        for (int i = 0; i < balls.Length; i++)
        {
            balls[i].SetActive(false);
        }

        BallChoice();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0) && !selectedBall.GetComponent<MoveBall>().ballRoll)
        {
            if (offIndicatorDirection)
            {
                stopSliderForce = true;
                selectedBall.GetComponent<MoveBall>().StartActionBall(sliderForce.value);
                sliderForce.value = 0;

            }
            else
            {
                offIndicatorDirection = true;
                indicatorDirection.GetComponent<MovingIndicatorDirection>().speedIndicatorDirection = 0f;
                StartCoroutine(SliderValue());
            }
        }

    }

    IEnumerator SliderValue()
    {
        while(!stopSliderForce)
        {
            if(!isSliderForceMaxValue)
            {
                sliderForce.value++;
                yield return new WaitForSeconds(timeDelaySliderUpdate);
                if (sliderForce.value == sliderForce.maxValue)
                    isSliderForceMaxValue = true;
            }
            else
            {
                sliderForce.value--;
                yield return new WaitForSeconds(timeDelaySliderUpdate);
                if (sliderForce.value == sliderForce.minValue)
                    isSliderForceMaxValue = false;
            }
        }
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BallChoice()
    {
        if(index < balls.Length -1)
        {
            index++;
            ballsCounter.SetText("Balls: " + (balls.Length - index));
            selectedBall = balls[index];
            selectedBall.SetActive(true);
            stopSliderForce = false;
            offIndicatorDirection = false;
            indicatorDirection.GetComponent<MovingIndicatorDirection>().speedIndicatorDirection = startSpeedIndicatorDirection;
        }
        else
        {
            Debug.Log("Game Over!");
        }
    }
}
