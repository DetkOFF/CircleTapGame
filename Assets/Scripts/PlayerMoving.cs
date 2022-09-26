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

public class PlayerMoving : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Vector2 centerPoint;
    [SerializeField] private float rotationSpeed = 5f; //degrees per second
    [SerializeField] private int rotationDirection = 1;
    private bool isTouching = false;
    private bool gotPoint = false;
    private PlayerState playerState;
    [SerializeField] private int maxDifficulty = 6;
    public float Difficulty = 0f;
    public bool IsGamePaused = true;

    public Vector3 StartPosition = new Vector3(0,0,0);
    private void Start()
    {
        playerState = PlayerState.Field;
        StartPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (IsGamePaused) return;
        //Rotation
        transform.RotateAround(centerPoint, Vector3.back, (rotationSpeed+(int)Difficulty*20)*Time.fixedDeltaTime*rotationDirection);
        
        //Input
        //Input_ChangeDirection();
        if (Input_Touch())
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
        }
        
    }

    private bool Input_Touch()
    {
        if (Input.touchCount > 0 && !isTouching)
        {
            //Debug.Log("input");
            isTouching = true;
            StartCoroutine(TouchWait());
            
            return true;
        }
        return (false);
    }
    private void Input_ChangeDirection()
    {
        if (Input_Touch())
            rotationDirection *= -1;
    }
    private IEnumerator TouchWait()
    {
        yield return new WaitForSeconds(0.2f);
        isTouching = false;
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        switch (_other.tag)
        {
            case "Exit":
                playerState = PlayerState.Exit;
                break;
            default: break;
        }
    }
    private void OnTriggerExit2D(Collider2D _other)
    {
        switch (_other.tag)
        {
            case "Exit":
                playerState = PlayerState.Field;
                if (!gotPoint)
                    gameManager.GameOver();
                else gotPoint = false;
                break;
            default: break;
        }
    }
}
