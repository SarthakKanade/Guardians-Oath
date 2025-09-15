using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

// Simple data class to avoid compilation issues
public static class GameData
{
    public static int currentWave = 1;
    public static bool isInfiniteWave = false;
    public static bool isPaused = false;
}

public class UI : MonoBehaviour
{
    [SerializeField] private GameObject gameoverUI;
    [Header("UI Elements")]
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject protectText;
    [SerializeField] private TextMeshProUGUI timertext;
    [SerializeField] private TextMeshProUGUI killcounttext;
    [SerializeField] private TextMeshProUGUI wavetext; // This should be Wave_Value or Wave_Text
    [SerializeField] private TextMeshProUGUI waveannouncementtext; // This should be WaveAnnouncement
    [SerializeField] private TextMeshProUGUI lifeCountText; // Reference to the Life_Count TextMeshProUGUI
    [SerializeField] private Player player; // Reference to the Player

    public static UI instance;
     
    private int killcount;
    private float gameStartTime;

    private void Awake()
    {
        instance = this;
        
        // Initialize UI state
        if (startScreen != null) startScreen.SetActive(true);
        if (protectText != null) protectText.SetActive(false);
        if (gameoverUI != null) gameoverUI.SetActive(false);
        
        // Pause game at start
        Time.timeScale = 0f;
        gameStartTime = Time.unscaledTime; // Record when the game started
        
        // Reset game data when game starts
        GameData.currentWave = 1;
        GameData.isInfiniteWave = false;
        GameData.isPaused = true; // Game starts paused
    }

    private void Update()
    {
        UpdateTimer();
        UpdateWaveDisplay();
        UpdateHealthDisplay();
    }
    
    private void UpdateHealthDisplay()
    {
        if (player != null && lifeCountText != null)
        {
            lifeCountText.text = $"{player.GetCurrentHealth()}/{player.GetMaxHealth()}";
        }
    }

    public void EnableGameOverUI()
    {
        Time.timeScale = 0.5f;
        gameoverUI.SetActive(true);
    }

    public void StartGame()
    {
        if (startScreen != null)
        {
            startScreen.SetActive(false);
            Time.timeScale = 1f; // Unpause the game
            GameData.isPaused = false;
            gameStartTime = Time.unscaledTime; // Reset the game start time
            
            // Show protect text for 2 seconds
            if (protectText != null)
            {
                StartCoroutine(ShowProtectText());
            }
        }
    }
    
    private System.Collections.IEnumerator ShowProtectText()
    {
        protectText.SetActive(true);
        yield return new WaitForSeconds(2f);
        protectText.SetActive(false);
    }
    
    public void RestartGame()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneIndex);
    }

    private void UpdateTimer()
    {
        if (timertext != null)
        {
            // Calculate elapsed time since game started
            float elapsedTime = Time.unscaledTime - gameStartTime;
            timertext.text = elapsedTime.ToString("F2");
        }
    }

    private void UpdateWaveDisplay()
    {
        if (wavetext != null)
        {
            // Use GameData - this works without compilation issues
            if (GameData.isInfiniteWave)
            {
                wavetext.text = "Max";
            }
            else
            {
                wavetext.text = $"{GameData.currentWave}";
            }
        }
        else
        {
            Debug.LogError("UI: wavetext is null! Please assign Wave_Value or Wave_Text to wavetext field in UI script.");
        }
    }
    
    public void ShowWaveAnnouncement(string waveText)
    {
        if (waveannouncementtext != null)
        {
            waveannouncementtext.text = waveText;
            waveannouncementtext.gameObject.SetActive(true);
            Debug.Log($"UI: Showing wave announcement: {waveText}");
        }
        else
        {
            Debug.LogError("UI: waveannouncementtext is null! Please assign WaveAnnouncement to waveannouncementtext field in UI script.");
        }
    }
    
    public void HideWaveAnnouncement()
    {
        if (waveannouncementtext != null)
        {
            waveannouncementtext.gameObject.SetActive(false);
            Debug.Log("UI: Hiding wave announcement");
        }
    }

    public void UpdateKillCount()
    {
        killcount++;
        killcounttext.text = killcount.ToString();
    }
}
