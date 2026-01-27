using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Level Data")]
    [SerializeField] private float m_totalLevelTime;
    [SerializeField] private LevelData m_level;

    [Header("References")]
    [SerializeField] private CustomerManager m_customerManager;

    [Header("UI References")]
    [SerializeField] private LevelClock m_clock;
    [SerializeField] private Score m_score;

    private bool m_levelStarted;
    private float m_currentLevelTime;
    private int m_waveIndex;
    private bool m_wavesSpawned;

    private int m_maxScore;
    private int m_currentScore;

    private void Start()
    {
        StartLevel();
    }

    public void StartLevel()
    {
        #region Set Score
        m_maxScore = (int)(m_level.oneStarGoal / LevelData.oneStarScorePercentage);
        m_score.SetScoreSlider(m_maxScore);
        #endregion
        #region Set Clock
        m_clock.UpdateClock(m_totalLevelTime);
        m_currentLevelTime = m_totalLevelTime;
        #endregion
        m_levelStarted = true;
    }

    private void Update()
    {
        if (!m_levelStarted || m_currentLevelTime == 0)
        {
            return;
        }

        m_currentLevelTime = Math.Max(m_currentLevelTime - Time.deltaTime, 0);
        m_clock.UpdateClock(m_currentLevelTime);

        //Closing so no new customers anymore
        if (m_currentLevelTime == 0)
        {
            Debug.Log("TIME UP");
            m_customerManager.StopSpawning();
            return;
        }
        if (m_wavesSpawned)
        {
            return;
        }

        float timePercentage = m_currentLevelTime / m_totalLevelTime;
        if (timePercentage <= m_level.waves[m_waveIndex].spawnTimePercentage)
        {
            Debug.Log($"[LevelManager] Spawn wave [{m_waveIndex}]");
            m_customerManager.AddWave(m_level.waves[m_waveIndex]);
            m_waveIndex++;

            if (m_waveIndex > m_level.waves.Length - 1)
            {
                m_wavesSpawned = true;
            }
        }
    }

    public void AddScore()
    {
        m_currentScore += 10; //Magic number
        m_score.UpdateScore(m_currentScore);
    }
}
