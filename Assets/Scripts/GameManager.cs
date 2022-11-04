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
    private Vector3[] startPosPins;
    private int score;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI bonusScoreText;
    private byte bonusStrike = 4;
    private byte bonusMoreThanAHalf = 3;
    private byte bonusHalf = 2;
    private byte standartBonusMultiplier = 1;
    private float delayBonusText = 3f;
    [SerializeField] GameObject totalPanel;
    [SerializeField] TextMeshProUGUI gameOverText;
    [SerializeField] Button resetRecordsButton;
    private bool stayPins;
    readonly int multiplier = 1000;

    void Start()
    {
        restartButton.onClick.AddListener(RestartLevel);//listens to a button press, if pressed, it will restart
        resetRecordsButton.onClick.AddListener(ResetRecord);
        startSpeedIndicatorDirection = indicatorDirection.GetComponent<MovingIndicatorDirection>().speedIndicatorDirection;

        for (int i = 0; i < balls.Length; i++)
        {
            balls[i].SetActive(false);
        }

        startPosPins = new Vector3[pins.Length];

        for (int i = 0; i < pins.Length; i++)
        {
            startPosPins[i] = pins[i].transform.position;
        }

        BallChoice();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !selectedBall.GetComponent<MoveBall>().ballRoll)
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
        while (!stopSliderForce)
        {
            if (!isSliderForceMaxValue)
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

    //Scoring and display of points
    public void CountingFallenPins()
    {
        for (int i = 0; i < pins.Length; i++)
        {
            if (Mathf.Round(pins[i].transform.position.y) != Mathf.Round(startPosPins[i].y))
            {
                if (pins[i].gameObject.activeSelf)
                {
                    pins[i].gameObject.SetActive(false);
                    amountFallenPins++;
                }
            }
            else
            {
                stayPins = true;
            }
        }

        if (amountFallenPins > 0)
        {
            switch (pins.Length / amountFallenPins)
            {
                case 1:
                    if (pins.Length % amountFallenPins == 0)
                    {
                        StartCoroutine(DisplayBonusText("STRIKE! \n score x " + bonusStrike, 150, bonusStrike));
                        break;
                    }
                    else
                    {
                        StartCoroutine(DisplayBonusText("More than a half \n score x " + bonusMoreThanAHalf, 60, bonusMoreThanAHalf));
                        break;
                    }
                case 2:
                    StartCoroutine(DisplayBonusText("Half \n score x " + bonusHalf, 150, bonusHalf));
                    break;
                default:
                    StartCoroutine(DisplayBonusText(amountFallenPins + " x " + standartBonusMultiplier, 150, standartBonusMultiplier));
                    score += amountFallenPins * multiplier;
                    break;
            }
        }
        else
        {
            StartCoroutine(DisplayBonusText("Miss", 150, standartBonusMultiplier));
        }
    }

    private void NextAction()
    {
        if (index < balls.Length - 1)
        {
            if (stayPins)
            {
                BallChoice();
            }
            else
            {
                RespawnPins();
            }
            stayPins = false;
            amountFallenPins = 0;
        }
        else
        {
            GameOver();
        }
    }

    private void BallChoice()
    {
        index++;
        ballsCounterText.SetText("Balls: " + (balls.Length - index));
        selectedBall = balls[index];
        selectedBall.SetActive(true);
        stopSliderForce = false;
        offIndicatorDirection = false;
        indicatorDirection.GetComponent<MovingIndicatorDirection>().speedIndicatorDirection = startSpeedIndicatorDirection;
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
    IEnumerator DisplayBonusText(string text, int fontSize, byte bonusMultiplier)
    {
        score += amountFallenPins * multiplier * bonusMultiplier;
        scoreText.SetText("Score: " + score);
        bonusScoreText.gameObject.SetActive(true);
        bonusScoreText.fontSize = fontSize;
        bonusScoreText.SetText(text);
        yield return new WaitForSeconds(delayBonusText);
        bonusScoreText.gameObject.SetActive(false);
        NextAction();
    }

    private void RespawnPins()
    {
        for (int i = 0; i < pins.Length; i++)
        {
            pins[i].gameObject.SetActive(true);
            pins[i].transform.position = startPosPins[i];
            pins[i].transform.rotation = new Quaternion(0, 0, 0, 0);

            //When restoring the original position, the pins fell, so I applied a freeze
            pins[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            pins[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
        BallChoice();
    }
}
