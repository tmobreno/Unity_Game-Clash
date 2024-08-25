using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private IEnumerator spawnCycle;
    private IEnumerator waitToSpawn;

    [Header("Enemy Initialization")]
    [SerializeField] private Enemy enemy;
    [SerializeField] private bool facingRight;
    [SerializeField] private bool jumpOnStart;
    [SerializeField] private float initialJumpForce;
    [SerializeField] private bool startActive;
    [Range(10, 120)][SerializeField] private float timeToStart;

    [Header("Timers")]
    [Range(1, 60)][SerializeField] private float spawnTimer;
    [SerializeField] private bool randomizedTimer;

    private void Awake()
    {
        spawnCycle = StartSpawning();
        waitToSpawn = WaitToSpawn();
    }

    private void Start()
    {
        if (startActive) StartCoroutine(spawnCycle);
        else StartCoroutine(waitToSpawn);
    }

    public IEnumerator StartSpawning()
    {
        while (true)
        {
            SpawnEnemy();
            float timer = spawnTimer;
            if (randomizedTimer) timer = Random.Range(spawnTimer - 2, spawnTimer + 2);
            yield return new WaitForSeconds(timer);
        }
    }

    public void SpawnEnemy()
    {
        if (LayoutManager.instance.enemiesOnLayout >= (4 + (2 * LayoutManager.instance.GetLoopNumber()))) return;

        if (enemy == null)
        {
            Debug.Log(this.name + " is missing an enemy to spawn on " + this.transform.parent.transform.parent.name);
            return;
        }

        LayoutManager.instance.ChangeEnemyAmount(1);
        Enemy e = Instantiate(enemy, this.transform);
        if (jumpOnStart) e.Jump(initialJumpForce);
        if (!facingRight) e.Flip();
    }

    private IEnumerator WaitToSpawn()
    {
        yield return new WaitForSeconds(timeToStart);
        StartCoroutine(spawnCycle);
    }
}
