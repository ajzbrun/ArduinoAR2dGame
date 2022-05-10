using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    cameraZoom mainCamera;
    SerialPort stream = new SerialPort("COM9", 9600);
    float spawnTime = 2f;
    int currentScore = 0;
    public GameObject scoreTxt;

    public GameObject gameOverObj;
    public GameObject meteorite;
    public GameObject player;
    Vector3 targetPosition;

    Coroutine spawner;
    Coroutine bonusBlinker;

    public float gameSpeed = 1f;

    public GameObject bonusTxt;
    int enemysAfterBonus = 4;
    bool bonusAvailable = false;
    bool inBonus = false;

    void Start()
    {
        OpenConnection();

        mainCamera = Camera.main.GetComponent<cameraZoom>();
        targetPosition = new Vector3();        

        StartCoroutine(PlayerFlight());
        spawner = StartCoroutine(SpawnEnemies());
    }

    void OpenConnection()
    {
        stream.ReadTimeout = 100;
        stream.Open();
        stream.WriteLine("L0");
    }

    void Update()
    {
        Vector3 newPosition = Vector3.Lerp(player.transform.position, targetPosition, Time.deltaTime);
        if (player.transform.position.y >= -4.5f && player.transform.position.y <= 4.5f)
            player.transform.position = newPosition;
        else if (player.transform.position.y < -4.5f)
            player.transform.position = new Vector3(player.transform.position.x, -4.5f);
        else if (player.transform.position.y > 4.5f)
            player.transform.position = new Vector3(player.transform.position.x, 4.5f);

        if (enemysAfterBonus <= 0 && !bonusAvailable)
        {
            bonusAvailable = true;
            bonusBlinker = StartCoroutine(BlinkBonusTxt());
        }
    }

    IEnumerator PlayerFlight()
    {
        WaitForSecondsRealtime t = new WaitForSecondsRealtime(.15f);
        targetPosition = new Vector3(player.transform.position.x, player.transform.position.y);
        while (true)
        {
            try
            {
                string value = null;
                value = stream.ReadLine();

                //print(value);
                if (value.Equals("BONUS"))
                {
                    if (bonusAvailable && !inBonus)
                        StartCoroutine(StartBonus());
                }
                else if (value != null && !inBonus)
                {
                    //print(value + " cm.");
                    float height = (((float)Convert.ToDecimal(value) - 10) / 2) - 4;
                    if (height < -4.5f)
                        height = -4.4f;
                    else if (height > 4.5f)
                        height = 4.5f;

                    if (player.transform.position.y < height)
                        targetPosition = targetPosition + new Vector3(0, 1f * gameSpeed);
                    else
                        targetPosition = targetPosition - new Vector3(0, 1f * gameSpeed);
                }
                else if (inBonus)
                {
                    //during the bonus, we center the player in the middle
                    float height = 0;

                    if (player.transform.position.y < height)
                        targetPosition = targetPosition + new Vector3(0, 1f * gameSpeed);
                    else
                        targetPosition = targetPosition - new Vector3(0, 1f * gameSpeed);
                }
            }
            catch(Exception ex)
            {
                //Debug.LogError(ex);
            }
            yield return t; //has to match with arduino delay
        }
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            GameObject enm = meteorite;
            enm.transform.position = new Vector3(enm.transform.position.x, UnityEngine.Random.Range(-4.3f, 4.3f));
            Instantiate(enm);

            if(!inBonus) //if we're not in bonus, we discount the remaining enemies for enabilng the bonus
                enemysAfterBonus -= 1;

            if (spawnTime > .8f)
                spawnTime -= .1f;

            yield return new WaitForSecondsRealtime(spawnTime);
        }
    }
    public void IncrementScore()
    {
        currentScore++;
        scoreTxt.GetComponent<Text>().text = currentScore.ToString();
    }

    public void GameOver()
    {
        stream.WriteLine("L1");
        StopCoroutine(spawner);
        StopCoroutine(bonusBlinker);
        bonusTxt.SetActive(false);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
        foreach (GameObject enemy in enemies)
            GameObject.Destroy(enemy);

        gameOverObj.SetActive(true);

        enemysAfterBonus = 15;
        SetGameSpeed(1);
    }

    public void PlayAgain()
    {
        mainCamera.SetDesiredZoom(5f); //default zoom: 5

        stream.WriteLine("L0");
        spawnTime = 2f;
        currentScore = 0;
        scoreTxt.GetComponent<Text>().text = currentScore.ToString();

        gameOverObj.SetActive(false);
        spawner = StartCoroutine(SpawnEnemies());
    }

    IEnumerator StartBonus()
    {
        int i = 0;
        float currentGameSpeed = gameSpeed;
        inBonus = true;
        StopCoroutine(bonusBlinker);
        bonusTxt.SetActive(false);
        while (i==0)
        {
            i++;
            mainCamera.SetDesiredZoom(3.5f); //zoom to the player
            player.GetComponent<EdgeCollider2D>().enabled = false; //disable collision with enemies
            SetGameSpeed(gameSpeed * 1.7f); //increase gamespeed for more velocity simulation

            yield return new WaitForSecondsRealtime(6f);

            mainCamera.SetDesiredZoom(5f);
            SetGameSpeed(currentGameSpeed);
            player.GetComponent<EdgeCollider2D>().enabled = true;
            inBonus = false;

            enemysAfterBonus = 15;
        }
    }

    IEnumerator BlinkBonusTxt()
    {
        while(true)
        {
            bonusTxt.SetActive(!bonusTxt.activeSelf);
            yield return new WaitForSecondsRealtime(.5f);
        }
    }

    public void SetGameSpeed(float speed)
    {
        gameSpeed = speed;
    }

    public float GetGameSpeed()
    {
        return gameSpeed;
    }
}
