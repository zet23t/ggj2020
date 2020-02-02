using UnityEngine;

[CreateAssetMenu(fileName = "LevelRegistry", menuName = "GJ2020/LevelRegistry", order = 0)]
public class LevelRegistry : ScriptableObject {
    public LevelPattern[] Patterns;
}