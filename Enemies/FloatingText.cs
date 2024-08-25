using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private float DestroyTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, DestroyTime);
    }
}
