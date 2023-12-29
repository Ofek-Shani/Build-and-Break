using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PieceScriptableObject", order = 1)]
public class PieceSO : ScriptableObject
{
    public Texture2D texture; // sprite representing the tiles this piece contains
    public int cost;// how many pieces need to removed after this is placed?
    public int level;
    public bool[,] data; // which cells does this piece occupy?
    public int width, height;
    int pieceNumber; // used only for getting the texture
    private void OnValidate()
    {
        pieceNumber = int.Parse(name.Split(" ")[1]);
        texture = Resources.Load<Texture2D>("Puzzle Data/Pieces/Level " + level + "/Piece " + pieceNumber);
        width = texture.width; 
        height = texture.height;
        data = new bool[width, height];
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                data[i,j] = texture.GetPixel(i, j) == Color.black;
            }
        }
    }
}
