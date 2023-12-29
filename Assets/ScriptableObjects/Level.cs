using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level ", menuName = "ScriptableObjects/LevelScriptableObject", order = 1)]
public class Level : ScriptableObject
{
    public int levelNumber; // level number
    public Texture2D texture; // sprite representing the tiles this piece contains

    private void OnValidate()
    {
        texture = Resources.Load<Texture2D>("Puzzle Data/Levels/Level " + levelNumber);
        //Debug.Log("TEST");
    }
}
