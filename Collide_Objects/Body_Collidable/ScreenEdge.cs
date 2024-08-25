using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenEdge : Collidable
{
    [SerializeField] private GameObject oppositeEdge;
    [SerializeField] private float offsetAmount;

    public override void OnTriggerInteraction(GameObject obj)
    {
        float offset = 0;
        if (this.gameObject.transform.position.y == 0)
        {
            offset = oppositeEdge.transform.position.x < 0 ? offsetAmount : -offsetAmount;
            obj.transform.position = new Vector2(oppositeEdge.transform.position.x + offset, obj.transform.position.y);
        }
        else if (this.gameObject.transform.position.x == 0)
        {
            offset = oppositeEdge.transform.position.y < 0 ? offsetAmount : -offsetAmount;
            obj.transform.position = new Vector2(obj.transform.position.x, oppositeEdge.transform.position.y + offset);
        }
    }
}
