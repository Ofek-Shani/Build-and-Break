using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PieceScriptableObject", order = 1)]
public class PieceSO : ScriptableObject
{
    public Texture2D texture; // sprite representing the tiles this piece contains
    public int cost;// how many pieces need to removed after this is placed?
    public int level;
    public bool[,] data; // which cells does this piece occupy?
    public int width, height;
    int pieceNumber; // used only for getting the text

    int versionNumber = 2;

    void SetValues()
    {
        if (name != "Remove Piece")
        {
            pieceNumber = int.Parse(name.Split(" ")[1]);
            texture = Resources.Load<Texture2D>("Version " + versionNumber + "/Puzzle Data/Pieces/Level " + level + "/Piece " + pieceNumber);
        }
        else
        {
            pieceNumber = 0;
            texture = Resources.Load<Texture2D>("Version " + versionNumber + "/Puzzle Data/Pieces/RemovePiece");
        }
        width = texture.width;
        height = texture.height;
        data = new bool[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                data[i, j] = texture.GetPixel(i, j) == Color.black;
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
