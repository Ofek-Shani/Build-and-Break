using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    GameObject board;
    GameObject border;

    GameObject[,] backgroundTiles;
    // data on which cells need to be filled in order to win
    public bool[,] goalData { get; private set; }
    // data on where the holes in the board are
    public bool[,] holeData { get; private set; }
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

    public void FillBoard(int levelNumber, int versionNumber)
    {
        board = GameObject.FindGameObjectWithTag("Board");
        border = GameObject.FindGameObjectWithTag("Border");

        Level level = Resources.Load<Level>("Version " + versionNumber + "/Level Scriptables/Level " + levelNumber);
        Texture2D tex = level.texture;
        Sprite[] tiles = Resources.LoadAll<Sprite>("Textures/Tiles/Background Tiles"); // load the sprites we need
        boardHeight = tex.height;
        boardWidth = tex.width;
        backgroundTiles = new GameObject[boardWidth, boardHeight]; // initialize the board array with size

        goalData = new bool[boardWidth, boardHeight]; // do the same for the data arrays
        holeData = new bool[boardWidth, boardHeight]; 

        // set the size of the border to the dimensions of the board
        border.GetComponent<SpriteRenderer>().size = new Vector2(boardWidth+.5f, boardHeight+.5f);

        for (int i = 0; i < boardWidth; i++)
        {
            for(int j = 0; j < boardHeight; j++)
            {
                Color32 pix = tex.GetPixel(i, j);
                // set the data array cells to the correct values
                goalData[i, j] = (pix == Color.black);
                holeData[i,j] = (pix == Color.red);

                // if there is no hole here,Make the background tile asset
                Vector3 pos = new Vector3(i - (boardWidth / 2), j - (boardHeight / 2), 0);
                backgroundTiles[i, j] = Instantiate(tilePrefab, pos, Quaternion.identity, board.transform);
                SpriteRenderer spr = backgroundTiles[i, j].GetComponentInChildren<SpriteRenderer>();
                spr.sprite = holeData[i,j] ? tiles[2]: ((goalData[i, j]) ? tiles[1] : tiles[0]);
                // backgroundTiles[i, j].GetComponentInChildren<TileController>().SetCoords(i, j);
            }
        }
    }
}
