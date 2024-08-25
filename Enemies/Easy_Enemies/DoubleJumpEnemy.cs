using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpEnemy : Enemy
{
    protected override IEnumerator JumpCycle()
    {
        while (true)
        {
            float randTimer = Random.Range(jumpCycleTimer - 1, jumpCycleTimer + 1);
            yield return new WaitForSeconds(randTimer);
            int doubleJumpChance = Random.Range(1, 3);
            if (doubleJumpChance == 1) StartCoroutine(DoubleJump(jumpForce));
            else
            {
                Jump(jumpForce);
            }
        }
    }
}
