﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public static PlayerData current;

    #region GAME STATE
    public bool isGamePaused = false;
    public bool canPlayBackground = false;
    #endregion

    #region MOVEMENT
    public float moveSpeed = 0.8f;
    #endregion

    #region LIFE
    public int currentLife;
    public int maxLife = 100;
    private int lifePerLevel = 5;
    #endregion

    #region MANA
    public int currentMana = 50;
    public int maxMana = 50;
    private int defaultMaxMana = 50;
    private int manaPerLevel = 3;
    public int manaRegenPerSecond = 5;
    private int defaultManaRegenPerSecond = 5;
    public float manaRegenInterval = 0.2f;
    public int manaRegenPerInterval;
    #endregion

    #region SPELLS
    public int fireballDamage = 6;
    public int fireballManaCost = 20;
    public float fireballCastCooldown = 0.3f;
    #endregion

    #region MELEE ATTACK
    public int meleeDamage = 8;
    private int defaultMeleeDamage = 8;
    private int wrathMeleeDamageIncrease = 1;
    public float meleeAttackCooldown = 0.3f;
    #endregion

    #region LEVELLING
    public int currentExp = 0;
    public int currentLevel = 1;
    public int requiredExp = 50;
    #endregion

    #region SKILLS
    public int skillPoints = 0;

    public int wrath = 1;
    

    public int pride = 1;
    private float prideMaxManaIncrease = 0.05f;

    public int lust = 1;
    private int lustManaRegenIncrease = 1;
    #endregion
    public PlayerData()
    {
        isGamePaused = false;
        Reset();
    }

    public void Reset()
    {
        isGamePaused = false;
        currentLife = maxLife;
        currentMana = maxMana;
        GetManaRegenPerInterval();
    }

    public void Pause(bool isPaused)
    {
        isGamePaused = isPaused;
    }

    public void AddExp(int expGained)
    {
        currentExp += expGained;
        if (currentExp >= requiredExp)
        {
            currentExp = currentExp - requiredExp; 
            LevelUp();
        }
    }

    public void AddWrath(int amount)
    {
        wrath += amount;

        // calculate melee damage increase
        meleeDamage = defaultMeleeDamage + (wrath - 1) * wrathMeleeDamageIncrease;
        Debug.Log("Melee damage " + meleeDamage);
    }

    public void AddPride(int amount)
    {
        pride += amount;
    }

    public void AddLust(int amount)
    {
        lust += amount;
    }

    private void LevelUp()
    {
        currentLevel++;
        skillPoints += 2;
        maxLife += lifePerLevel;
        maxMana += manaPerLevel;
        currentLife = maxLife;
        currentMana = maxMana;
        requiredExp = (int)(requiredExp * 1.3f);
    }

    void GetManaRegenPerInterval()
    {
        manaRegenPerInterval = (int)(manaRegenPerSecond * (manaRegenInterval/1f));
    }

    public void DamagePlayer(int damageAmount)
    {
        if (damageAmount > currentLife)
            currentLife = 0;
        else
            currentLife -= damageAmount;
    }
}
