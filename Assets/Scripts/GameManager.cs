using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    [SerializeField] Slider sliderForce;
    private float timeDelaySliderUpdate = 0.05f;
    private bool isSliderForceMaxValue;
    private bool stopSliderForce;
    [SerializeField] MoveBall balls;
    [SerializeField] Button restartButton;
    [SerializeField] GameObject indicatorDirection;
    private bool offIndicatorDirection;


    void Start()
    {
        restartButton.onClick.AddListener(RestartLevel);
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (offIndicatorDirection)
            {
                stopSliderForce = true;
                balls.StartActionBall(sliderForce.value);
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
}
