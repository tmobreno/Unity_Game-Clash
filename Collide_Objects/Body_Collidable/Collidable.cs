using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collidable : MonoBehaviour
{
    public abstract void OnTriggerInteraction(GameObject obj);
}
