using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExit : Collidable
{
    private bool activated;

    private void Awake()
    {
        this.GetComponent<BoxCollider2D>().enabled = false;
        this.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void OnEnable()
    {
        LayoutManager.ActivateExit += ActivateExit;
    }

    private void OnDisable()
    {
        LayoutManager.ActivateExit -= ActivateExit;
    }

    public override void OnTriggerInteraction(GameObject obj)
    {
        if (activated) return;
        if (obj.GetComponent<PlayerController>())
        {
            AudioManager.instance.PlaySound("Level_Transition");
            activated = true;
            LayoutManager.instance.SwapLayout();
        }
    }

    private void ActivateExit()
    {
        this.GetComponent<BoxCollider2D>().enabled = true;
        this.GetComponent<SpriteRenderer>().enabled = true;
    }
}
