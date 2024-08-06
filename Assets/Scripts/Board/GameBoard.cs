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
    /// Attempts to break a tile at the given location.
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns>If the tile cannot be removed,
    /// this returns false. Otherwise it returns true.</returns>
    public bool BreakAt(int i, int j)
    {
        // first, check to see if the tile in question is actually ON the board
        if (i < 0 || j < 0 || i > boardWidth || j > boardHeight) return false;
        // now we can actually do things
        if(!data[i,j].GetComponent<Tile>().Break(i, j, this)) return false;
        data[i, j] = null;
        return true;
    }
   
    /// <summary>
    /// TODO: Make the tile visual update to match the movement
    /// </summary>
    /// <param name="i1"></param>
    /// <param name="j1"></param>
    /// <param name="i2"></param>
    /// <param name="j2"></param>
    /// <returns></returns>
    public bool MoveTo(int i1, int j1, int i2, int j2)
    {
        if (!data[i2,j2] && data[i1,j1].GetComponent<Tile>().IsMovable())
        {
            data[i2, j2] = data[i1, j1];
            data[i1, j1] = null;
            data[i2, j2].GetComponent<Tile>().MoveTo(BoardToWorldCoordinates(i2, j2), false);
            return true;
        }
        return false;
    }

    public bool CanBreakAt(int i, int j)
    {
        return data[i, j].GetComponent<Tile>().CanBreakAt(i, j);
    }

    /// <summary>
    /// Attempts to place the given piece in the given space.
    /// </summary>
    /// <param name="toPlace"></param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns>If unable to place the piece, returns false. Otherwise returns true.</returns>
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
                Vector3 pos = BoardToWorldCoordinates(i, j);
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

    /// <summary>
    /// Returns the world coordinates of a given tile on the board in Vector3 form (z=0).
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    public Vector3 BoardToWorldCoordinates(int i, int j)
    {
        return new Vector3(i - (boardWidth / 2) + BOARD_BG_PADDING * i, j - (boardHeight / 2) + j * BOARD_BG_PADDING, 0);
    }
}
