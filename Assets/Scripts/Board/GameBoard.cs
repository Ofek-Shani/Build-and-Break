using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameBoard : MonoBehaviour
{

    const float BOARD_BG_BORDER_SIZE = 0.5f;
    const float BOARD_BG_PADDING = 1 / 16;

    GameObject boardObj;
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

    [SerializeField] GameObject holeTilePrefab;

    // 2d array of all placed tiles. Pieces that have not had their placements confirmed
    // are not part of this array.
    public GameObject[,] data
    { get; private set; }

    /// <summary>
    /// Completely clears the game board
    /// </summary>
    public void ClearBoard()
    {
        // Note: null checks make it so that the program does not crash when the game starts for 
        // the first time (this happens before everything is initialized yet)
        if (backgroundTiles is null) return;
        foreach(GameObject g in backgroundTiles)
        {
            Destroy(g);
        }

        if (data is null) return;
        foreach (GameObject g in data) if(g) g.GetComponent<Tile>().SuperBreak();
    }

    /// <summary>
    /// Attempts to remove the tile at the given location. If the tile cannot be removed,
    /// this returns false. Otherwise it returns true.
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    public bool RemoveAt(int i, int j)
    {
        if(!data[i,j].GetComponent<Tile>().Break(i, j, this)) return false;
        data[i, j] = null;
        return true;
    }

    /// <summary>
    /// Attempts to place the given piece in the given space.
    /// If unable to place the piece, returns false.
    /// </summary>
    /// <param name="toPlace"></param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    public bool PlaceAt(GameObject toPlace, int i, int j)
    {
        Tile t = toPlace.GetComponent<Tile>();
        if (!t.CanPlaceAt(i, j, this)) return false;
        data[i, j] = toPlace;
        t.Place(i, j, this);
        return true;
    }


    public void FillBoard(int levelNumber, int versionNumber)
    {
        boardObj = GameObject.FindGameObjectWithTag("Board");
        border = GameObject.FindGameObjectWithTag("Border");

        LevelData level = Resources.Load<LevelData>("Version " + versionNumber + "/Level Scriptables/Level " + levelNumber);
        Texture2D tex = level.texture;
        Sprite[] tiles = Resources.LoadAll<Sprite>("Textures/Tiles/Background Tiles"); // load the sprites we need
        boardHeight = tex.height;
        boardWidth = tex.width;
        backgroundTiles = new GameObject[boardWidth, boardHeight]; // initialize the board array with size

        goalData = new bool[boardWidth, boardHeight]; // do the same for the data arrays
        holeData = new bool[boardWidth, boardHeight];

        data = new GameObject[boardWidth, boardHeight];

        // set the size of the border to the dimensions of the board
        border.GetComponent<SpriteRenderer>().size = new Vector2(
            boardWidth+BOARD_BG_PADDING*(boardWidth-1)+BOARD_BG_BORDER_SIZE, 
            boardHeight+BOARD_BG_PADDING*(boardWidth-1)+BOARD_BG_BORDER_SIZE);

        for (int i = 0; i < boardWidth; i++)
        {
            for(int j = 0; j < boardHeight; j++)
            {
                Color32 pix = tex.GetPixel(i, j);
                // set the data array cells to the correct values
                goalData[i, j] = (pix == Color.black);

                // if there is no hole here,Make the background tile asset
                Vector3 pos = new Vector3(i - (boardWidth / 2) + BOARD_BG_PADDING*i, j - (boardHeight / 2)+j*BOARD_BG_PADDING, 0);
                backgroundTiles[i, j] = Instantiate(tilePrefab, pos, Quaternion.identity, boardObj.transform);
                SpriteRenderer spr = backgroundTiles[i, j].GetComponentInChildren<SpriteRenderer>();
                spr.sprite = holeData[i,j] ? tiles[2]: ((goalData[i, j]) ? tiles[1] : tiles[0]);
                // backgroundTiles[i, j].GetComponentInChildren<TileController>().SetCoords(i, j);


                if (pix == Color.red) 
                {
                    GameObject holeToPlace = Instantiate(holeTilePrefab, pos, Quaternion.identity, boardObj.transform);
                    PlaceAt(holeToPlace, i, j);
                }
            }
        }
    }
}
