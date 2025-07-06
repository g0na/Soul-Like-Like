using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject enemy;

    private void Start()
    {
        SpawnEnemies();
    }

    public void SpawnEnemies()
    {

        enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
    }
}
