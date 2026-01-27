using UnityEngine;
using UnityEngine.UI;

public class LevelClock : MonoBehaviour
{
    [SerializeField] private Slider m_levelClock;

    private bool m_clockStarted;
    private float m_levelClockTime;
    private float m_currentTime;

    private void Start()
    {
        m_levelClock.value = 1;
    }

    public void UpdateClock(float levelTime)
    {
        if (!m_clockStarted) 
        {
            m_levelClock.maxValue = levelTime;
            m_clockStarted = true;
        }

        m_levelClock.value = levelTime;
    }
}
