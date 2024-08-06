using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level ", menuName = "ScriptableObjects/LevelScriptableObject", order = 1)]
public class LevelData : ScriptableObject
{
    public int levelNumber; // level number
    public Texture2D texture; // sprite representing the tiles this piece contains
    public int versionNumber;

    private void Awake()
    {
        texture = Resources.Load<Texture2D>("Version " + versionNumber + "/Puzzle Data/Levels/Level " + levelNumber);
        //Debug.Log("TEST");
    }

    private void OnValidate()
    {
        texture = Resources.Load<Texture2D>("Version " + versionNumber + "/Puzzle Data/Levels/Level " + levelNumber);
        //Debug.Log("TEST");
    }
}
