using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private int totalWaves = 5;
    [SerializeField] private float cooldownDecrease = 0.25f; // Decrease by 0.25 seconds each wave
    // No pause between waves
    
    [Header("Enemy Counts Per Wave")]
    [SerializeField] private int[] enemiesPerWave = {10, 15, 20, 25, 30}; // Wave 1-5 enemy counts
    
    [Header("UI References")]
    // UI updates are handled by the UI script
    
    [Header("Enemy Spawner Reference")]
    [SerializeField] private Enemy_Respawner enemyRespawner;
    
    private int currentWave = 1;
    private float baseCooldownTime;
    private bool isInfiniteWave = false;
    // No pause system
    
    // Enemy tracking variables
    private int enemiesToSpawn = 15; // Wave 1 starts with 15 enemies
    private int enemiesSpawned = 0;
    private int enemiesKilled = 0;
    private bool waveInProgress = false;
    
    public static WaveManager instance;
    
    // Use GameData for UI access
    
    private void Awake()
    {
        instance = this;
        
        // Find enemy respawner if not assigned
        if (enemyRespawner == null)
        {
            enemyRespawner = FindFirstObjectByType<Enemy_Respawner>();
        }
        
        // Get the base cooldown time from the enemy respawner
        if (enemyRespawner != null)
        {
            baseCooldownTime = GetEnemyRespawnerCooldown();
            Debug.Log($"WaveManager: Base cooldown time set to {baseCooldownTime}");
        }
        else
        {
            Debug.LogError("WaveManager: No Enemy_Respawner found! Please assign one in the inspector.");
        }
        
        // Initialize first wave
        enemiesToSpawn = enemiesPerWave[0]; // Set enemies for wave 1
        GameData.currentWave = currentWave;
        waveInProgress = true;
        ShowWaveAnnouncement();
    }
    
    private void Update()
    {
        if (waveInProgress)
        {
            // Check if wave is complete (all enemies spawned and killed)
            if (enemiesSpawned >= enemiesToSpawn && enemiesKilled >= enemiesToSpawn)
            {
                CompleteWave();
            }
        }
    }
    
    private void CompleteWave()
    {
        waveInProgress = false;
        Debug.Log($"WaveManager: Wave {currentWave} completed! Enemies killed: {enemiesKilled}/{enemiesToSpawn}");
        
        // Move to next wave
        currentWave++;
        GameData.currentWave = currentWave;
        
        if (currentWave > totalWaves)
        {
            // Start infinite wave
            isInfiniteWave = true;
            GameData.isInfiniteWave = true;
            currentWave = 999; // Show as "Wave Max" in UI
            GameData.currentWave = 999;
            enemiesToSpawn = int.MaxValue; // Infinite enemies
            Debug.Log("WaveManager: Starting infinite wave (Wave Max)");
        }
        else
        {
            // Set enemies for next wave
            enemiesToSpawn = enemiesPerWave[currentWave - 1];
            Debug.Log($"WaveManager: Next wave will have {enemiesToSpawn} enemies");
        }
        
        // Reset counters
        enemiesSpawned = 0;
        enemiesKilled = 0;
        
        // Show wave announcement and start immediately
        ShowWaveAnnouncement();
    }
    
    private void ShowWaveAnnouncement()
    {
        // Show wave announcement immediately
        string announcementText = "";
        if (isInfiniteWave)
        {
            announcementText = "WAVE MAX";
        }
        else
        {
            announcementText = $"WAVE {currentWave}";
        }
        
        if (UI.instance != null)
        {
            UI.instance.ShowWaveAnnouncement(announcementText);
        }
        
        // Start the wave immediately
        waveInProgress = true;
        
        // Resume enemy spawning
        if (enemyRespawner != null)
        {
            enemyRespawner.enabled = true;
        }
        
        Debug.Log($"WaveManager: Wave {currentWave} started! Need to spawn {enemiesToSpawn} enemies");
        
        // Update enemy spawner cooldown
        UpdateEnemySpawnerCooldown();
        
        // Hide announcement after 1 second
        Invoke(nameof(HideAnnouncement), 1f);
    }
    
    private void HideAnnouncement()
    {
        if (UI.instance != null)
        {
            UI.instance.HideWaveAnnouncement();
        }
    }
    
    private void UpdateEnemySpawnerCooldown()
    {
        if (enemyRespawner != null)
        {
            float newCooldown = baseCooldownTime - (cooldownDecrease * (currentWave - 1));
            
            // Ensure cooldown doesn't go below 0.75 seconds (as per your change)
            newCooldown = Mathf.Max(newCooldown, 0.75f);
            
            SetEnemyRespawnerCooldown(newCooldown);
            Debug.Log($"WaveManager: Updated enemy spawn cooldown to {newCooldown} seconds for Wave {currentWave}");
        }
    }
    
    
    // Helper methods to interact with Enemy_Respawner
    private float GetEnemyRespawnerCooldown()
    {
        return enemyRespawner.cooldownTime;
    }
    
    private void SetEnemyRespawnerCooldown(float newCooldown)
    {
        enemyRespawner.cooldownTime = newCooldown;
    }
    
    // Public methods for enemy tracking
    public void OnEnemySpawned()
    {
        if (waveInProgress && !isInfiniteWave)
        {
            enemiesSpawned++;
            Debug.Log($"WaveManager: Enemy spawned! {enemiesSpawned}/{enemiesToSpawn}");
            
            // Stop spawning if we've reached the limit for this wave
            if (enemiesSpawned >= enemiesToSpawn)
            {
                if (enemyRespawner != null)
                {
                    enemyRespawner.enabled = false;
                }
                Debug.Log($"WaveManager: All {enemiesToSpawn} enemies spawned for Wave {currentWave}");
            }
        }
    }
    
    public void OnEnemyKilled()
    {
        if (waveInProgress)
        {
            enemiesKilled++;
            Debug.Log($"WaveManager: Enemy killed! {enemiesKilled}/{enemiesToSpawn}");
        }
    }
    
    // Public getters for other scripts
    public int GetCurrentWave() => currentWave;
    public bool IsInfiniteWave() => isInfiniteWave;
    public int GetEnemiesSpawned() => enemiesSpawned;
    public int GetEnemiesKilled() => enemiesKilled;
    public int GetEnemiesToSpawn() => enemiesToSpawn;
}
