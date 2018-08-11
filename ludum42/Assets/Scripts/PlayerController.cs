﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region CURRENT STATE
    bool isAlive = true;
    bool canPauseGame = true;
    bool isFacingRight = true;
    bool isWalking = false;
    bool isIdleA = false; // is in idle animation state A
    bool preparingIdleAnimationA = false; // true if cooldown for idle animation A is started
    #endregion

    #region MOVEMENT
    private Vector2 targetPosition;
    private Vector2 dirNormalized;
    #endregion

    #region COMPONENTS
    Rigidbody2D rigidBody2D;
    #endregion

    #region UI
    [SerializeField] Image healthBar;
    [SerializeField] GameObject defeatPanel;
    #endregion

    #region ANIMATION
    float waitTimeBeforeIdleA = 0.1f;
    float idleAnimAStartTime;
    [SerializeField] Animator playerAnimator;
    #endregion

    private void Awake()
    {
        LoadPlayerData();
    } 

    void LoadPlayerData()
    {
        if (PlayerData.current == null)
            PlayerData.current = new PlayerData();
        PlayerData.current.isGamePaused = false;
    }

    void Start()
    {
        rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        UpdateHealthBar();
        ListenForGamePause();
        ListenForPlayerDefeat();
        if (!isAlive)
            Die();
        if (PlayerData.current.isGamePaused)
        {
            return;
        }
        #region MOVEMENT
        if (Input.GetMouseButtonDown(0))
        {
            GetTargetPositionAndDirection();
            CheckIfPlayerIsWalking();
        }
        if (isWalking)
        {
            CheckIfPlayerIsWalking();
            MovePlayer(); 
        }
        CheckWherePlayerIsFacing();
        #endregion
    }

    void LateUpdate()
    {
        Debug.Log("is walking " + isWalking);
        // ANIMATION
        playerAnimator.SetBool("isWalking", isWalking);

        if (isWalking)
        {
            preparingIdleAnimationA = false;
            isIdleA = false;
        }

        // set start time for playing idle animation A
        if (!isWalking && !preparingIdleAnimationA)
        {
            preparingIdleAnimationA = true;
            idleAnimAStartTime = Time.time + waitTimeBeforeIdleA;
        }
        // start playing idle animation A
        if (Time.time > idleAnimAStartTime)
        {
            isIdleA = true;
            playerAnimator.SetBool("startIdleA", isIdleA); 
        }
    }

    void CheckWherePlayerIsFacing()
    {
        if (isFacingRight && dirNormalized.x < 0)
        {
            isFacingRight = false;
            gameObject.transform.localScale = new Vector2(-1f, 1f);
        }
        else if (!isFacingRight && dirNormalized.x > 0)
        {
            isFacingRight = true;
            gameObject.transform.localScale = new Vector2(1f, 1f);
        }
    }

    void Die()
    {
        canPauseGame = false;
        PlayerData.current.isGamePaused = true;
        defeatPanel.SetActive(true);    
    }

    void ListenForPlayerDefeat()
    {
        if (PlayerData.current.currentLife == 0)
            isAlive = false;
    }

    void ListenForGamePause()
    {
        if (Input.GetKeyDown(KeyCode.P) && canPauseGame)
        {
            PlayerData.current.isGamePaused = !PlayerData.current.isGamePaused;
        }
    }

    void UpdateHealthBar()
    {
        healthBar.fillAmount = (PlayerData.current.currentLife * 1f) / PlayerData.current.maxLife;
    }

    void GetTargetPositionAndDirection()
    {
        targetPosition = Input.mousePosition;
        targetPosition = Camera.main.ScreenToWorldPoint(targetPosition);
        dirNormalized = new Vector2(targetPosition.x - transform.position.x, targetPosition.y - transform.position.y);
        dirNormalized = dirNormalized.normalized;
    }

    void CheckIfPlayerIsWalking()
    {
        if (Vector2.Distance(targetPosition, transform.position) <= 0.01f)
        {
            isWalking = false;
        }
        else
        {
            isWalking = true;
        }
    }

    void MovePlayer()
    {
        transform.position = new Vector2(transform.position.x, transform.position.y) + dirNormalized * PlayerData.current.moveSpeed * Time.deltaTime;
    }
}
