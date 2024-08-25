using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;

    [field: Header("Jump Stats")]
    [field:Range(3f, 9f)][field: SerializeField] public float jumpForce { get; private set; }


    [field: Header("Movement Stats")]
    [field: Range(0f, 0.3f)][field: SerializeField] public float movementSmoothing { get; private set; }
    [field: Range(0f, 1f)][field: SerializeField] public float baseMovementSpeed { get; private set; }
    [field: Range(0.1f, 3f)][field: SerializeField] public float baseAirModifier { get; private set; }


    [field: Header("General Stats")]
    [field: Range(1, 5)][field: SerializeField] public int health { get; private set; }
    [field: Range(1, 20)][field: SerializeField] public int attack { get; private set; }
    public int score { get; private set; }
    public float scoreMultiplier { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        score = 0;
        scoreMultiplier = 1;
        attack = 1;

        PlayerUI.instance.UpdateScore(score);
        PlayerUI.instance.UpdateMultiplier(scoreMultiplier);
        PlayerUI.instance.UpdateHealth(health);
    }

    public void ChangeScore(float change)
    {
        score += (int)(change * scoreMultiplier);
        PlayerUI.instance.UpdateScore(score);
        LayoutManager.instance.UpdateScore((int)(change * scoreMultiplier));
    }

    public void ChangeHealth(int change)
    {
        health += change;
        if (health > 4) health = 4;
        PlayerUI.instance.UpdateHealth(health);
    }

    public void ChangeAttack(int change)
    {
        attack += change;
        if (attack < 1) attack = 1;
    }

    public void ChangeSpeed(float change)
    {
        baseMovementSpeed *= change;
        if (baseMovementSpeed > 1) baseMovementSpeed = 1;
    }

    public void ChangeAirSpeed(float change)
    {
        baseAirModifier *= change;
        if (baseAirModifier > 3) baseAirModifier = 3;
    }

    public void ChangeJumpForce(float change)
    {
        jumpForce *= change;
        if (jumpForce > 9) jumpForce = 9;
    }

    public void ChangeScoreMultiplier(float change)
    {
        scoreMultiplier += change;
        int maxMultAdj = LayoutManager.instance.GetLoopNumber() * 5;
        if (scoreMultiplier > 10 + maxMultAdj) scoreMultiplier = 10 + maxMultAdj;
        PlayerUI.instance.UpdateMultiplier(scoreMultiplier);
    }

    public void ResetScoreMultiplier()
    {
        scoreMultiplier = 1;
        PlayerUI.instance.UpdateMultiplier(scoreMultiplier);
    }
}
