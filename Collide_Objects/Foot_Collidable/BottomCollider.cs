using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomCollider : MonoBehaviour
{
    private Entity entity;
    private bool isPlayer = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<TopCollider>())
        {
            collision.GetComponent<TopCollider>().Collide(isPlayer);
            entity.OnHitOther();
        }
    }

    public void SetEntity(Entity e)
    {
        entity = e;
    }

    public void SetIsPlayer(bool b)
    {
        isPlayer = b;
    }
}
