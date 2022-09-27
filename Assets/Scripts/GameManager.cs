using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using NaughtyAttributes;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    
    
    //TMPs
    [SerializeField] private TextMeshProUGUI scoreTMP;
    [SerializeField] private TextMeshProUGUI maxScoreTMP;
    [SerializeField] private TextMeshProUGUI gameStatusTMP;
    //Score
    private int currentScore;
    private int maxScore;
    private float currentDifficulty;
    [SerializeField] private int maxDifficulty = 6;
    //Restart components
    [SerializeField] private Transform exitTransform;
    [SerializeField] private PlayerMoving[] playerMovingsArray;
    [SerializeField] private GameObject startBTN;
    //Colors
    [SerializeField] private LevelColors[] levelColorsArray;
    private int currentLevelColorIndex = 0;
    [SerializeField] private SpriteRenderer[] player1SpritesRenderersArray;
    //[SerializeField] private SpriteRenderer[] player2SpritesRenderersArray;
    [SerializeField] private SpriteRenderer[] fieldSpritesRenderersArray;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Image[] player1ColorImagesArray;
    [SerializeField] private TextMeshProUGUI[] player1ColorTMPsArray;
    
    
    private void Start()
    {
        Load();
        startBTN.SetActive(true);
        currentScore = 0;
        UpdateScoreTMP();
        UpdateMaxScoreTMP();
        gameStatusTMP.text = "Start game :)";
        currentDifficulty = 0;
        currentLevelColorIndex = 0;
        //ChangeLevelColors();
    }
    
    [Button("ResetPlayerPref")]
    public void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
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
        
    }
    
    private void SetStartValues()
    {
        currentScore = 0;
        currentDifficulty = 0;
        currentLevelColorIndex = 0;
        UpdateScoreTMP();
        foreach (var playerMoving in playerMovingsArray)
        {
            playerMoving.transform.position = playerMoving.StartPosition;
        }
        exitTransform.rotation = new Quaternion(0, 0, 0, 0);
        foreach (var playerMoving in playerMovingsArray)
        {
            playerMoving.Difficulty = 0;
        }

        ChangeLevelColors();
        StartCoroutine(DisablePause());
    }

    private IEnumerator DisablePause()
    {
        yield return new WaitForSeconds(0.1f);
        foreach (var playerMoving in playerMovingsArray)
        {
            playerMoving.IsGamePaused = false;
        }
    }

    private void ChangeLevelColors()
    {
        mainCamera.backgroundColor = levelColorsArray[currentLevelColorIndex].BackGroundColor;
        foreach (var spriteRenderer in player1SpritesRenderersArray)
        {
            spriteRenderer.color = levelColorsArray[currentLevelColorIndex].Player1Color;
        }
        foreach (var spriteRenderer in fieldSpritesRenderersArray)
        {
            spriteRenderer.color = levelColorsArray[currentLevelColorIndex].FieldColor;
        }
        foreach (var image in player1ColorImagesArray)
        {
            image.color = levelColorsArray[currentLevelColorIndex].Player1Color;
        }
        foreach (var tmp in player1ColorTMPsArray)
        {
            tmp.color = levelColorsArray[currentLevelColorIndex].Player1Color;
        }
        
    }

    public void AddScore(int _amount)
    {
        currentScore += _amount;
        if (currentScore % 8 == 0)
        {
            if (currentLevelColorIndex < levelColorsArray.Length - 1)
                currentLevelColorIndex++;
            else currentLevelColorIndex = 0;
            ChangeLevelColors();
        }
        UpdateScoreTMP();
        ChangeExitPosition();
        if (currentDifficulty < maxDifficulty)
        {
            currentDifficulty+=0.25f;
            foreach (var playerMoving in playerMovingsArray)
            {
                playerMoving.Difficulty = (int) currentDifficulty;
            }
        }
            
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
        foreach (var playerMoving in playerMovingsArray)
        {
            playerMoving.IsGamePaused = true;
        }

        startBTN.SetActive(true);
        gameStatusTMP.text = "Game Over :((";
        if (currentScore > maxScore)
        {
            maxScore = currentScore;
            Save();
            maxScoreTMP.text = "new record!";
        }
        else
        {
            UpdateMaxScoreTMP();
        }
        
    }
    private void ChangeExitPosition()
    {
        var eulerAngles = exitTransform.eulerAngles;
        eulerAngles = new Vector3(
            eulerAngles.x,
            eulerAngles.y,
            eulerAngles.z + Random.Range(45, 315)
        );
        exitTransform.eulerAngles = eulerAngles;
    }

    public void Tap_Input()
    {
        foreach (var playerMoving in playerMovingsArray)
        {
            playerMoving.Tap_Input();
        }
    }
 }
[Serializable]
public struct LevelColors
{
    public Color BackGroundColor;
    public Color Player1Color;
    public Color FieldColor;
}
