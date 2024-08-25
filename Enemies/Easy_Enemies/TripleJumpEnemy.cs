using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleJumpEnemy : Enemy
{
    protected override IEnumerator JumpCycle()
    {
        while (true)
        {
            float randTimer = Random.Range(jumpCycleTimer - 1, jumpCycleTimer + 1);
            yield return new WaitForSeconds(randTimer);
            int tripleJumpChance = Random.Range(1, 3);
            if (tripleJumpChance == 1) StartCoroutine(TripleJump(jumpForce));
            else
            {
                Jump(jumpForce);
            }
        }
    }
}
