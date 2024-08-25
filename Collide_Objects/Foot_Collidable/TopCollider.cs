using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopCollider : MonoBehaviour
{
    private Entity entity;
    private bool playerOnly;

    public void Collide(bool isPlayer)
    {
        if (!playerOnly || (playerOnly && isPlayer)) entity.OnTakeHit();
    }

    public void SetEntity(Entity e)
    {
        entity = e;
    }

    public void SetPlayerOnly(bool b)
    {
        playerOnly = b;
    }
}
