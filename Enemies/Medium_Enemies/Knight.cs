using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Enemy
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

        StartCoroutine(DoubleJump(jumpForce));
        canBeHit = true;

        yield return new WaitForSeconds(3f);

        StartCoroutine(flipCycle);
        movementSpeed = initialMovementSpeed;

        inFleeState = false;
    }
}
