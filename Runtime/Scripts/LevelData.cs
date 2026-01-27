using System;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Scriptable Objects/Level")]
public class LevelData : ScriptableObject
{
    [ReadOnly(true)] public static float oneStarScorePercentage = 0.6f;
    [Tooltip("one star is about 60% of the total bar and the second star will be 85%")] public int oneStarGoal;
    public CustomerWave[] waves;
}

[Serializable]
public class CustomerWave
{
    [Range(0, 1)] public float spawnTimePercentage; //If the level is 1 minute long then at 50 this wave will spawn 30 seconds into the level
    public CustomerType[] customerType;
}

[Serializable]
public enum CustomerType
{
    Line,
    Table
}