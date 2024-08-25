using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpKnight : DoubleJumpEnemy
{
    protected override IEnumerator FleeState()
    {
        inFleeState = true;

        //canBeHit = false;
        Flip();
        movementSpeed = initialMovementSpeed;
        movementSpeed *= fleeStateSpeedMultiplier;
        StopCoroutine(flipCycle);

        yield return new WaitForSeconds(3f);

        StartCoroutine(TripleJump(jumpForce));
        canBeHit = true;

        yield return new WaitForSeconds(3f);

        StartCoroutine(flipCycle);
        movementSpeed = initialMovementSpeed;

        inFleeState = false;
    }
}
