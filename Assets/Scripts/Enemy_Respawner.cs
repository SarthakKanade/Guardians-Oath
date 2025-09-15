using UnityEngine;

public class Enemy_Respawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] respawnPoints;
    [SerializeField] public float cooldownTime;
    private float Timer;
    private Transform player;

    private void Awake()
    {
        player = GameObject.FindFirstObjectByType<Player>().transform;
    }

    private void Update()
    {
        Timer -= Time.deltaTime;

        if (Timer < 0)
        {
            Timer = cooldownTime;
            CreateNewEnemy();
        }
    }

    private void CreateNewEnemy()
    {
        int randomIndex = Random.Range(0, respawnPoints.Length);
        Vector3 spawnPosition = respawnPoints[randomIndex].position;
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        bool enemyOnRight = player.position.x < respawnPoints[randomIndex].position.x;
        if (enemyOnRight)
        {
            newEnemy.GetComponent<Enemy>().Flip(); 
        }
        
        // Notify WaveManager that an enemy was spawned
        if (WaveManager.instance != null)
        {
            WaveManager.instance.OnEnemySpawned();
        }
    } 
}
