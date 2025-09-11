using System.Collections;
using UnityEngine;
using System.Linq;

/// <summary>
/// Improved WaveManager: modular, extensible, and clear English comments.
/// </summary>
public class WaveManagerChange : MonoBehaviour
{
    private GameObject enemyPrefab;
    private Transform spawnPoint;
    private Transform[] waypoints;
    public int enemiesPerWave = 5;
    public float timeBetweenSpawns = 1f;
    public float timeBetweenWaves = 5f;

    void Start()
    {
        enemyPrefab = Resources.Load<GameObject>("Enemy");
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab not found in Resources.");
            return;
        }
    }

    public void StartWave()
    {
        StartCoroutine(StartWaveCycle());
    }

    private IEnumerator StartWaveCycle()
    {
        while (true)
        {
            waypoints = HexGridManagerChange.Instance?.GetPathWaypoints();
            if (waypoints == null || waypoints.Length == 0)
            {
                Debug.LogWarning("No enemy path defined. Enemies cannot spawn.");
                yield return new WaitForSeconds(timeBetweenWaves);
                continue;
            }
            System.Array.Reverse(waypoints);
            spawnPoint = waypoints.FirstOrDefault();
            yield return StartCoroutine(SpawnWave());
            Debug.Log($"Wave finished. Next wave in {timeBetweenWaves} seconds.");
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    private IEnumerator SpawnWave()
    {
        if (spawnPoint == null)
        {
            Debug.LogError("Spawn point not found. Cannot spawn enemies.");
            yield break;
        }
        for (int i = 0; i < enemiesPerWave; i++)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            EnemyMoverChange enemyMover = newEnemy.GetComponent<EnemyMoverChange>();
            if (enemyMover != null)
            {
                enemyMover.waypoints = waypoints;
            }
            else
            {
                Debug.LogError("Enemy prefab missing EnemyMoverChange script!");
                Destroy(newEnemy);
                yield break;
            }
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }
}
