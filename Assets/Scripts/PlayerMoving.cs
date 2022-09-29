using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum PlayerState
{
    Field,
    Exit
}

public enum PlayerNumber
{
    Player1,
    Player2
}

public class PlayerMoving : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Vector2 centerPoint;
    [SerializeField] private float rotationSpeed = 5f; //degrees per second
    [SerializeField] public int RotationDirection = 1;

    [SerializeField] private string exitTag;
    [SerializeField] private PlayerNumber playerNumber;

    [SerializeField] private bool godMode = false; 
    
    //private const string exitTagPlayer1 = "Exit_Player1";
    //private const string exitTagPlayer2 = "Exit_Player2";
    private bool gotPoint = false;
    private PlayerState playerState;
    public int Difficulty = 0;
    public bool IsGamePaused = true;

    public Vector3 StartPosition = new Vector3(0,0,0);
    private void Start()
    {
        playerState = PlayerState.Field;
        StartPosition = transform.position;
    }

    private void Update()
    {
        if (IsGamePaused) return;
        //Rotation
        transform.RotateAround(centerPoint, Vector3.back, (rotationSpeed+(int)Difficulty*20)*Time.deltaTime*RotationDirection);
        
        //Input
        //Input_ChangeDirection();
        /*if (Input_Touch())
        {
            switch (playerState)
            {
                case PlayerState.Field:
                    gameManager.GameOver();
                    break;
                case PlayerState.Exit:
                    if (gotPoint) break;
                    gameManager.AddScore(1);
                    rotationDirection *= -1;
                    gotPoint = true;
                    if (Difficulty < maxDifficulty)
                        Difficulty+=0.3f;
                    break;
                default: 
                    gameManager.GameOver(); 
                    break;
            }
        }*/
        
    }

    public void Tap_Input()
    {
        if(IsGamePaused)
            return;
        //Debug.Log(playerState);
        switch (playerState)
        {
            case PlayerState.Field:
                if(godMode) break;
                gameManager.GameOver(playerNumber);
                break;
            case PlayerState.Exit:
                if (gotPoint) break;
                gameManager.AddScore(1, playerNumber);
                RotationDirection *= -1;
                gotPoint = true;
                
                break;
            default: 
                if(godMode) break;
                gameManager.GameOver(playerNumber); 
                break;
        }
    }
    
    /*private bool Input_Touch()
    {
        if (Input.touchCount > 0 && !isTouching)
        {
            //Debug.Log("input");
            isTouching = true;
            StartCoroutine(TouchWait());
            
            return true;
        }
        return (false);
    }*/
    /*private void Input_ChangeDirection()
    {
        if (Input_Touch())
            rotationDirection *= -1;
    }*/
    private void OnTriggerEnter2D(Collider2D _other)
    {
        if(IsGamePaused)
            return;
        if (_other.CompareTag(exitTag))
            playerState = PlayerState.Exit;
    }
    private void OnTriggerExit2D(Collider2D _other)
    {
        if(IsGamePaused)
            return;
        if (_other.CompareTag(exitTag))
        {
            playerState = PlayerState.Field;
            if (!gotPoint && !godMode)
                gameManager.GameOver(playerNumber);
            else gotPoint = false;
        }
    }
}
