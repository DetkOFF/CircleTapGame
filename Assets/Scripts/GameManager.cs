using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using NaughtyAttributes;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum GameMode
{
    SinglePlayer,
    MultiPlayer
}

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
    [SerializeField] private Transform[] exitTransformsArray;
    [SerializeField] private PlayerMoving[] playerMovingsArray;
    //[SerializeField] private GameObject startBTN;
    [SerializeField] private RectTransform btnPanel;
    //Colors
    [SerializeField] private LevelColors[] levelColorsArray;
    private int currentLevelColorIndex = 0;
    [SerializeField] private SpriteRenderer[] player1SpritesRenderersArray;
    //[SerializeField] private SpriteRenderer[] player2SpritesRenderersArray;
    [SerializeField] private SpriteRenderer[] fieldSpritesRenderersArray;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Image[] player1ColorImagesArray;
    [SerializeField] private TextMeshProUGUI[] player1ColorTMPsArray;
    //MultiPlayer (game modes)
    [SerializeField] private GameMode gameMode;
    [SerializeField] private GameObject tapBtnPlayer2;
    private int player2CurrentScore;
    [SerializeField] private GameObject multiplayerScorePanel;
    [SerializeField] private TextMeshProUGUI player1ScoreTMP;
    [SerializeField] private TextMeshProUGUI player2ScoreTMP;
    private bool onePlayerLeft = false;
    private void Start()
    {
        Load();
        //btnPanel.gameObject;
        //OpenBTNPanel();
        currentScore = 0;
        UpdateScoreTMP();
        UpdateMaxScoreTMP();
        gameStatusTMP.text = "Start game :)";
        currentDifficulty = 0;
        currentLevelColorIndex = 0;
        gameMode = GameMode.SinglePlayer;
        SetSinglePlayerMode();
        ChangeLevelColors();
    }

    private void CloseBTNPanel()
    {
        btnPanel.LeanMove(new Vector3(0, -860, 0), 0.5f).setEaseOutCubic();
    }

    private void OpenBTNPanel()
    {
        btnPanel.LeanMove(new Vector3(0, 0, 0), 0.5f).setEaseOutCubic();
    }

    public void SetSinglePlayerMode()
    {
        gameMode = GameMode.SinglePlayer;
        playerMovingsArray[1].gameObject.SetActive(false);
        exitTransformsArray[1].gameObject.SetActive(false);
        tapBtnPlayer2.SetActive(false);
        multiplayerScorePanel.SetActive(false);
        scoreTMP.gameObject.SetActive(true);
        maxScoreTMP.gameObject.SetActive(true);
    }

    public void SetMultiplayerMode()
    {
        gameMode = GameMode.MultiPlayer;
        playerMovingsArray[1].gameObject.SetActive(true);
        exitTransformsArray[1].gameObject.SetActive(true);
        tapBtnPlayer2.SetActive(true);
        multiplayerScorePanel.SetActive(true);
        scoreTMP.gameObject.SetActive(false);
        onePlayerLeft = false;
        maxScoreTMP.gameObject.SetActive(false);
    }

    public void StartMultiPlayerGame()
    {
        if(gameMode == GameMode.SinglePlayer)
            SetMultiplayerMode();
        SetStartValues();
        CloseBTNPanel();
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
        if(gameMode == GameMode.MultiPlayer)
            SetSinglePlayerMode();
        SetStartValues();
        CloseBTNPanel();
    }
    
    private void SetStartValues()
    {
        currentScore = 0;
        player2CurrentScore = 0;
        onePlayerLeft = false;
        currentDifficulty = 0;
        currentLevelColorIndex = 0;
        UpdateScoreTMP();
        foreach (var playerMoving in playerMovingsArray)
        {
            playerMoving.transform.LeanMove(playerMoving.StartPosition, 0.3f).setEaseOutCubic();
            playerMoving.transform.LeanRotate(new Vector3(0, 0, 0), 0.3f).setEaseOutCubic();
        }
        
        if(gameMode == GameMode.SinglePlayer)
            exitTransformsArray[0].LeanRotate(new Vector3(0, 0, 0), 0.3f).setEaseOutCubic();
        else
        {
            exitTransformsArray[0].LeanRotate(new Vector3(0, 0, 90), 0.3f).setEaseOutCubic();
            exitTransformsArray[1].LeanRotate(new Vector3(0, 0, -90), 0.3f).setEaseOutCubic();
            playerMovingsArray[1].RotationDirection = playerMovingsArray[0].RotationDirection;
        }
        foreach (var playerMoving in playerMovingsArray)
        {
            playerMoving.Difficulty = 0;
        }
        
        ChangeLevelColors();
        StartCoroutine(DisablePause());
    }

    private IEnumerator DisablePause()
    {
        yield return new WaitForSeconds(0.3f);
        foreach (var playerMoving in playerMovingsArray)
        {
            playerMoving.IsGamePaused = false;
        }
    }

    private void ChangeLevelColors()
    {
        mainCamera.DOColor(levelColorsArray[currentLevelColorIndex].BackGroundColor, 0.5f);
        foreach (var spriteRenderer in player1SpritesRenderersArray)
        {
            //spriteRenderer.color = levelColorsArray[currentLevelColorIndex].Player1Color;
            LeanTween.color(spriteRenderer.gameObject, levelColorsArray[currentLevelColorIndex].Player1Color, 0.5f).setEaseOutCubic();
        }
        foreach (var spriteRenderer in fieldSpritesRenderersArray)
        {
            LeanTween.color(spriteRenderer.gameObject, levelColorsArray[currentLevelColorIndex].FieldColor, 0.5f).setEaseOutCubic();
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

    public void AddScore(int _amount, PlayerNumber _playerNumber)
    {
        if (gameMode == GameMode.SinglePlayer)
        {
            currentScore += _amount;
            if (currentScore % 8 == 0)
            {
                if (currentLevelColorIndex < levelColorsArray.Length - 1)
                    currentLevelColorIndex++;
                else currentLevelColorIndex = 0;
                ChangeLevelColors();
            }
        }
        else
        {
            if (_playerNumber == PlayerNumber.Player1)
                currentScore += _amount;
            else
                player2CurrentScore += _amount;
        }
        UpdateScoreTMP();
        ChangeExitPosition(_playerNumber);
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
        if(gameMode == GameMode.SinglePlayer)
            scoreTMP.text = currentScore.ToString();
        else
        {
            player1ScoreTMP.text = currentScore.ToString();
            player2ScoreTMP.text = player2CurrentScore.ToString();
        }
    }
    private void UpdateMaxScoreTMP()
    {
        maxScoreTMP.text = "max: " + maxScore;
    }

    public void GameOver(PlayerNumber _playerNumber)
    {
        if (gameMode == GameMode.SinglePlayer)
        {
            //Debug.Log("GameOver");
            foreach (var playerMoving in playerMovingsArray)
            {
                playerMoving.IsGamePaused = true;
            }

            OpenBTNPanel();
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
        else
        {
            if (onePlayerLeft)
            {
                foreach (var playerMoving in playerMovingsArray)
                {
                    playerMoving.IsGamePaused = true;
                }
                OpenBTNPanel();
                if (currentScore > player2CurrentScore)
                    gameStatusTMP.text = "Player 1 win!";
                else if (player2CurrentScore > currentScore)
                    gameStatusTMP.text = "Player 2 win!";
                else
                    gameStatusTMP.text = "Draw!";
            }
            else switch (_playerNumber)
            {
                case PlayerNumber.Player1 when currentScore >= player2CurrentScore:
                    playerMovingsArray[0].IsGamePaused = true;
                    onePlayerLeft = true;
                    break;
                case PlayerNumber.Player1:
                    foreach (var playerMoving in playerMovingsArray)
                    {
                        playerMoving.IsGamePaused = true;
                    }
                    OpenBTNPanel();
                    gameStatusTMP.text = "Player 2 win!";
                    break;
                case PlayerNumber.Player2 when player2CurrentScore >= currentScore:
                    playerMovingsArray[1].IsGamePaused = true;
                    onePlayerLeft = true;
                    break;
                case PlayerNumber.Player2:
                    foreach (var playerMoving in playerMovingsArray)
                    {
                        playerMoving.IsGamePaused = true;
                    }
                    OpenBTNPanel();
                    gameStatusTMP.text = "Player 1 win!";
                    break;
            }
        }
    }
    private void ChangeExitPosition(PlayerNumber _playerNumber)
    {
        if (_playerNumber == PlayerNumber.Player1)
        {
            exitTransformsArray[0].eulerAngles = new Vector3(
                exitTransformsArray[0].eulerAngles.x, 
                exitTransformsArray[0].eulerAngles.y, 
                exitTransformsArray[0].eulerAngles.z + Random.Range(45, 315));
        }
        else
        {
            exitTransformsArray[1].eulerAngles = new Vector3(
                exitTransformsArray[1].eulerAngles.x, 
                exitTransformsArray[1].eulerAngles.y, 
                exitTransformsArray[1].eulerAngles.z + Random.Range(45, 315));
            
        }
    }

    public void Tap_Input_Player1()
    {
        playerMovingsArray[0].Tap_Input();
        //Debug.Log("tapinput1");
    }
    public void Tap_Input_Player2()
    {
        playerMovingsArray[1].Tap_Input();
        //Debug.Log("tapinput2");
    }
 }
[Serializable]
public struct LevelColors
{
    public Color BackGroundColor;
    public Color Player1Color;
    public Color FieldColor;
}
