using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChapterData", menuName = "Scriptable Objects/ChapterData")]
public class ChapterData : ScriptableObject
{
    public float levelTime = 180;
    public List<LevelData> levels = new List<LevelData>();
}
