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
    [SerializeField] TextMeshProUGUI ballsCounterText;
    [SerializeField] GameObject[] pins;
    private byte amountFallenPins;
    private int[] startPosPins;
    private int score;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI bonusScoreText;
    private byte numberOfRemainingPin;
    private byte bonusStrike = 4;
    private byte bonusMoreThanAHalf = 3;
    private byte bonusHalf = 2;
    private float delayBonusText = 3f;
    [SerializeField] GameObject totalPanel;
    [SerializeField] TextMeshProUGUI gameOverText;
    private bool isDisplayBonusTextOn;
    private int record;
    [SerializeField] Button resetRecordsButton;

    void Start()
    {
        restartButton.onClick.AddListener(RestartLevel);//listens to a button press, if pressed, it will restart
        resetRecordsButton.onClick.AddListener(ResetRecord);
        startSpeedIndicatorDirection = indicatorDirection.GetComponent<MovingIndicatorDirection>().speedIndicatorDirection;

        for (int i = 0; i < balls.Length; i++)
        {
            balls[i].SetActive(false);
        }

        startPosPins = new int[pins.Length];
        for (int i = 0; i < pins.Length; i++)
        {
            startPosPins[i] = (int)Mathf.Round(pins[i].transform.position.y);
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

    //Controls the throw force slider
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

    private void ResetRecord()
    {
        PlayerPrefs.DeleteKey("Record");
        gameOverText.SetText("Your Score:\r\n" + 0 + "\n\nYour Records:\r\n" + PlayerPrefs.GetInt("Record", 0));
    }

    private void BallChoice()
    {
        if(index < balls.Length -1)
        {
            index++;
            ballsCounterText.SetText("Balls: " + (balls.Length - index));
            selectedBall = balls[index];
            selectedBall.SetActive(true);
            stopSliderForce = false;
            offIndicatorDirection = false;
            indicatorDirection.GetComponent<MovingIndicatorDirection>().speedIndicatorDirection = startSpeedIndicatorDirection;
        }
        else
        {
            GameOver();
        }
    }

    //Scoring and display of points
    public void CountingFallenPins()
    {
        for (int i = 0; i < pins.Length; i++)
        {
            if (Mathf.Round(pins[i].transform.position.y) != startPosPins[i])
            {
                if (pins[i].gameObject.activeSelf)
                {
                    pins[i].gameObject.SetActive(false);
                    amountFallenPins++;
                }
            }
        }

        if(index == 0)
        {
            if (amountFallenPins > 0)
            {
                switch (pins.Length / amountFallenPins)
                {
                    case 1:
                        if (pins.Length % amountFallenPins == 0)
                        {
                            StartCoroutine(DisplayBonusText("STRIKE! \n score x " + bonusStrike, 150));
                            score = amountFallenPins * 1000 * bonusStrike;
                            break;
                        }
                        else
                        {
                            StartCoroutine(DisplayBonusText("More than a half \n score x " + bonusMoreThanAHalf, 60));
                            score = amountFallenPins * 1000 * bonusMoreThanAHalf;
                            break;
                        }
                    case 2:
                        StartCoroutine(DisplayBonusText("Half \n score x " + bonusHalf, 150));
                        score = amountFallenPins * 1000 * bonusHalf;
                        break;
                    default:
                        score = amountFallenPins * 1000;
                        break;
                }
            }
            else
            {
                StartCoroutine(DisplayBonusText("Miss", 150));
            }

            numberOfRemainingPin = (byte)(pins.Length - amountFallenPins);
        }
        else
        {
            score += (numberOfRemainingPin - (pins.Length - amountFallenPins)) * 1000;
            numberOfRemainingPin = (byte)(pins.Length - amountFallenPins);
        }
        scoreText.SetText("Score: " + score);

        if(!isDisplayBonusTextOn)
        {
            NextAction();
        }
    }

    private void NextAction()
    {
        if (amountFallenPins != pins.Length)
        {
            BallChoice();
        }
        else
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        if (score > PlayerPrefs.GetInt("Record", 0))
        {
            PlayerPrefs.SetInt("Record", score);
        }
        totalPanel.SetActive(true);
        gameOverText.SetText("Your Score:\r\n" + score + "\n\nYour Records:\r\n" + PlayerPrefs.GetInt("Record", score));
    }

    //Displaying text points (Strike, etc.)
    IEnumerator DisplayBonusText(string text, int fontSize)
    {
        isDisplayBonusTextOn = true;
        bonusScoreText.gameObject.SetActive(true);
        bonusScoreText.fontSize = fontSize;
        bonusScoreText.SetText(text);
        yield return new WaitForSeconds(delayBonusText);
        isDisplayBonusTextOn = false;
        bonusScoreText.gameObject.SetActive(false);
        NextAction();
    }
}
