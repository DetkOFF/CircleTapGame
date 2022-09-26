using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreTMP;
    [SerializeField] private TextMeshProUGUI maxScoreTMP;
    private int currentScore;
    private int maxScore;
    [SerializeField] private Transform exitTransform;
    [SerializeField] private PlayerMoving playerMoving;
    [SerializeField] private GameObject startBTN;

    private void Start()
    {
        Load();
        startBTN.SetActive(true);
        currentScore = 0;
        UpdateScoreTMP();
        UpdateMaxScoreTMP();
    }

    private void Load()
    {
        maxScore = (PlayerPrefs.HasKey("MaxScore")) ? PlayerPrefs.GetInt("MaxScore") : 0;
    }

    private void Save()
    {
        PlayerPrefs.SetInt("MaxScore", maxScore);
    }

    public void StartGame()
    {
        SetStartValues();
        startBTN.SetActive(false);
        maxScoreTMP.gameObject.SetActive(false);
    }
    
    private void SetStartValues()
    {
        currentScore = 0;
        UpdateScoreTMP();
        playerMoving.transform.position = playerMoving.StartPosition;
        exitTransform.rotation = new Quaternion(0, 0, 0, 0);
        playerMoving.Difficulty = 0f;
        StartCoroutine(DisablePause());
    }

    private IEnumerator DisablePause()
    {
        yield return new WaitForSeconds(0.1f);
        playerMoving.IsGamePaused = false;
    }

    public void AddScore(int _amount)
    {
        currentScore += _amount;
        UpdateScoreTMP();
        ChangeExitPosition();
    }

    private void UpdateScoreTMP()
    {
        scoreTMP.text = currentScore.ToString();
    }
    private void UpdateMaxScoreTMP()
    {
        maxScoreTMP.text = "max: " + maxScore;
    }

    public void GameOver()
    {
        //Debug.Log("GameOver");
        playerMoving.IsGamePaused = true;
        startBTN.SetActive(true);
        maxScoreTMP.gameObject.SetActive(true);
        if (currentScore > maxScore)
        {
            maxScore = currentScore;
            Save();
            UpdateMaxScoreTMP();
        }
        
    }

    private void ChangeExitPosition()
    {
        exitTransform.eulerAngles = new Vector3(
            exitTransform.eulerAngles.x,
            exitTransform.eulerAngles.y,
            exitTransform.eulerAngles.z + Random.Range(55, 325)
        );
    }
 }
