using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField] private Slider m_scoreSlider;

    public void SetScoreSlider(int maxScore)
    {
        m_scoreSlider.maxValue = maxScore;
        m_scoreSlider.value = 0;
    }

    public void UpdateScore(int newScore)
    {
        m_scoreSlider.value = newScore;
    }
}
