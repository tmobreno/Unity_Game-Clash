using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : ScriptableObject
{
    [Header("Stats")]
    [SerializeField] public string name;
    [SerializeField] public string description;
    [Range(1, 10)][SerializeField] public int health = 2;
    [SerializeField] public bool canJump = true;
    [SerializeField] public bool canFlip = true;

    [Header("Movement")]
    [Range(3f, 15f)][SerializeField] public float jumpForce = 10;
    [Range(0f, 0.5f)][SerializeField] public float movementSpeed = 0.05f;
    [Range(0f, 0.3f)][SerializeField] public float movementSmoothing = 0.05f;

    [Header("Timers")]
    [Range(1f, 20f)][SerializeField] public float jumpCycleTimer = 7;
    [Range(1f, 20f)][SerializeField] public float flipCycleTimer = 11;

    [Header("Rewards")]
    [Range(10, 1000)][SerializeField] public int scoreValue = 50;
    [Range(1, 5)][SerializeField] public int scoreMultiplierValue = 1;
}
