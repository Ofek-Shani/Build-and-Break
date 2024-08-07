using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PieceScriptableObject", order = 1)]
/// <summary>
/// Scriptable Object used to store all the data needed by the Piece class to function.
/// This file handles processing piece data when building the game. For information on 
/// Piece class implementation, go to Piece.cs
/// </summary>
public class PieceData : ScriptableObject
{
    public Texture2D texture; // sprite representing the tiles this piece contains
    public int cost;// how many pieces need to removed after this is placed?
    public int level, levelGroupNumber;
    public bool[,] data; // which cells does this piece occupy?
    public int width, height;
    int pieceNumber; // used only for getting the text

    

    /// <summary>
    /// Sets the values of the piece
    /// </summary>
    void SetValues()
    {
        string path;

        if (name != "Remove Piece")
        {
            pieceNumber = int.Parse(name.Split(" ")[1]);
            path = "Level Group " + levelGroupNumber + "/Puzzle Data/Pieces/Level " + level + "/Piece " + pieceNumber;
            texture = Resources.Load<Texture2D>(path);
        }
        else
        {
            pieceNumber = 0;
            path = "Level Group " + levelGroupNumber + "/Puzzle Data/Pieces/RemovePiece";
            texture = Resources.Load<Texture2D>(path);
        }

        if (texture == null) Debug.LogWarning(name + " tried to load texture " + path + " and failed.");

        width = texture.width;
        height = texture.height;
        
        data = new bool[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                data[i, j] = texture.GetPixel(i, j) != Color.white;
            }
        }
    }

    private void OnValidate()
    {
        SetValues();
    }

    private void Awake()
    {
        SetValues();
    }
}
