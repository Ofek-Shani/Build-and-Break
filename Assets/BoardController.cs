using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    GameObject board;

    GameObject[,] backgroundTiles;
    // data on which cells need to be filled in order to win
    public bool[,] boardData { get; private set; }
    public int boardWidth { get; private set; }
    public int boardHeight { get; private set; }
    [SerializeField]
    GameObject tilePrefab;


    public void ClearBoard()
    {
        if (backgroundTiles is null) return;
        foreach(GameObject g in backgroundTiles)
        {
            Destroy(g);
        }
    }

    public void FillBoard(int levelNumber)
    {
        board = GameObject.FindGameObjectWithTag("Board");

        Level level = Resources.Load<Level>("Level Scriptables/Level " + levelNumber);
        Texture2D tex = level.texture;
        Sprite[] tiles = Resources.LoadAll<Sprite>("Textures/Tiles"); // load the sprites we need
        boardHeight = tex.height;
        boardWidth = tex.width;
        backgroundTiles = new GameObject[boardWidth, boardHeight]; // initialize the board array with size
        boardData = new bool[boardWidth, boardHeight]; // do the same for the data array
        for (int i = 0; i < boardWidth; i++)
        {
            for(int j = 0; j < boardHeight; j++)
            {
                Color32 pix = tex.GetPixel(i, j);
                // set the boardData cell to the correct value
                boardData[i, j] = (pix == Color.black);
                // Make the background tile asset
                Vector3 pos = new Vector3(i - (boardWidth / 2), j - (boardHeight / 2), 0);
                backgroundTiles[i,j] = Instantiate(tilePrefab, pos, Quaternion.identity, board.transform);
                SpriteRenderer spr = backgroundTiles[i, j].GetComponentInChildren<SpriteRenderer>();
                spr.sprite = (boardData[i,j]) ? tiles[1] : tiles[0];
                // backgroundTiles[i, j].GetComponentInChildren<TileController>().SetCoords(i, j);
            }
        }
    }
}
