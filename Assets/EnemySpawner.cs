using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;
    public float initCooldown; //Time before the first spawn
    public float minSpawnCooldown = 4;
    public float maxSpawnCooldown = 8;

    public int maxEnemySpawn = 10; //Nombre d'ennemis spawné avant la destruction du spawner
    private int spawnedEnemyCount = 0;

    private void Awake()
    {
        StartCoroutine(SpawnEnemy_C(initCooldown));
    }

    IEnumerator SpawnEnemy_C(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        GameObject newEnemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count - 1)]);
        newEnemy.transform.position = this.transform.position;

        spawnedEnemyCount++;
        if (spawnedEnemyCount < maxEnemySpawn)
        {
            StartCoroutine(SpawnEnemy_C(Random.Range(minSpawnCooldown, maxSpawnCooldown)));
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
