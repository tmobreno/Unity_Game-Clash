using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [Header("Entity Settings")]
    [SerializeField] private bool isPlayer = false;
    [SerializeField] private bool playerOnly = false;
    [SerializeField] private bool setUpTopCollider = true;
    [SerializeField] private bool setUpBottomCollider = true;
    [Range(0.1f, 1.0f)][SerializeField] private float topColliderSizeX;
    [Range(0.1f, 1.0f)][SerializeField] private float topColliderSizeY;
    [Range(0.1f, 1.0f)][SerializeField] private float bottomColliderSizeX;
    [Range(0.1f, 1.0f)][SerializeField] private float bottomColliderSizeY;


    protected GameObject topCol, botCol;

    public abstract void OnTakeHit();
    public abstract void OnHitOther();

    protected virtual void Start()
    {
        if (setUpTopCollider) SetUpTop();
        if (setUpBottomCollider) SetUpBottom();
    }

    private GameObject SetUpGameObject(string name)
    {
        GameObject obj = new GameObject();
        obj.name = name;
        obj.transform.parent = this.transform;
        obj.transform.localScale = new Vector2(1, 1);
        return obj;
    }

    private GameObject SetUpCollider(GameObject collider, float position, float sizeX, float sizeY)
    {
        collider.AddComponent<BoxCollider2D>();

        collider.transform.localPosition = new Vector2(0, position);
        collider.GetComponent<BoxCollider2D>().size = new Vector2(sizeX, sizeY);
        collider.GetComponent<BoxCollider2D>().isTrigger = true;

        return collider;
    }

    private void SetUpTop()
    {
        GameObject collider = SetUpGameObject("Top Collider");

        SetUpCollider(collider, this.GetComponent<BoxCollider2D>().size.y, topColliderSizeX, topColliderSizeY);

        collider.AddComponent<TopCollider>();
        collider.GetComponent<TopCollider>().SetEntity(this);
        collider.GetComponent<TopCollider>().SetPlayerOnly(playerOnly);

        topCol = collider;
    }

    private void SetUpBottom()
    {
        GameObject collider = SetUpGameObject("Bottom Collider");

        SetUpCollider(collider, 0f, bottomColliderSizeX, bottomColliderSizeY);

        collider.AddComponent<BottomCollider>();
        collider.GetComponent<BottomCollider>().SetEntity(this);
        collider.GetComponent<BottomCollider>().SetIsPlayer(isPlayer);

        botCol = collider;
    }
}
