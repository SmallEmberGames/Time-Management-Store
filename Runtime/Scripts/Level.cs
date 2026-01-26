using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Scriptable Objects/Level")]
public class Level : ScriptableObject
{
	public Customer[] customers;

	[Serializable]
	public enum CustomerType
	{
		Line,
		Table
	}

	[Serializable]
	public struct Customer
	{
		[HideInInspector] public string name;
		public CustomerType customerType;
		[Range(0,100)] public float spawnTime;
    }
}
