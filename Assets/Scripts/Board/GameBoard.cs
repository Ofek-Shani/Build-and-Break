using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// This class handles board state modification, as well as generation of the initial state and resetting the board.
/// 
/// Contains methods to Break, Move, and Place tiles, as well as initialize a level and clear the board.
/// </summary>
public class GameBoard : MonoBehaviour
{

    const float BOARD_BG_BORDER_SIZE = 0.5f;
    const float BOARD_BG_PADDING = 1 / 16;
    const int TILE_PHYSICS_LAYER = 3;

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
        if(!data[i,j] || !data[i,j].GetComponent<Tile>().Break(i, j, this)) return false;
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
        // f
        if (!IsInBoard(i1, j1) || !IsInBoard(i2, j2)) return false;
        // If destination is blocked OR if there is no tile to move OR if the tile we want to move is immovable, return false.
        if (data[i2, j2] || !data[i1, j1] || !data[i1, j1].GetComponent<Tile>().IsMovable()) return false;
        // if there is no path, return false
        if (!IsPathOpen(i1, j1, i2, j2)) return false;
        
        data[i2, j2] = data[i1, j1];
        data[i1, j1] = null;
        data[i2, j2].GetComponent<Tile>().MoveToPositionOnBoard(i2,j2, false, this);
        return true;
        
    }

    /// <summary>
    /// Determines if there is an open path between 2 spaces (if they can be found by moving adjacently)
    /// NOTE: If the dimensions of the tiles or their padding changes, YOU NEED TO UPDATE THE VALUES HERE.
    /// Assumes board is parallel to Z plane and tiles are at z=0
    /// </summary>
    /// <param name="i1"></param>
    /// <param name="j1"></param>
    /// <param name="i2"></param>
    /// <param name="j2"></param>
    /// <returns>true if a straight line path exists, false if there is something in the way.</returns>
    public bool IsPathOpen(int i1, int j1, int i2, int j2)
    {
        // The idea here is to "rasterize" the line of movemnet and see if any tile is in the way.
        // we do this by casting 3 rays, one through the center of the origin tile and 2 left and right of the direction of motion.
        // we do not move if any of the 3 rays are obstructed -- this means that there is not enough space for a single
        // tile to squeeze through.
        // create an offset JUST big enough so that the rays can hit the right tiles.
        float offsetDistance = 0.3f;

        // we will have 4 possible origin points for our rays, one in each corner of the tile
        List<Vector3> offsets = new List<Vector3>()
        {
            new Vector3(1, 1, 0),
            new Vector3(-1, 1, 0),
            new Vector3(-1, -1, 0),
            new Vector3(1, -1, 0),
        };
        // Ray Casting implementation -- we are doing this using 2D casting which does not take z axis into account.
        // this implementation will need to be changed if we switch to a 3D model.

        Vector3 target = backgroundTiles[i2, j2].transform.position;
        Vector2 direction = target - backgroundTiles[i1, j1].transform.position;
        float distance = direction.magnitude;
        int layerMask = 1 << TILE_PHYSICS_LAYER;
        foreach( Vector2 vOff in offsets)
        {
            // we add direction.normalized because we want the ray to start off of our home tile in the direction of casting
            // so that we do not have a false positive collision
            Vector2 origin = (Vector2)backgroundTiles[i1, j1].transform.position + offsetDistance * vOff;
            Debug.DrawRay(origin, direction, Color.red, 10f);
            // since the ray starts at our home tile, we will always have 1 hit. If we have more than 1, something is in the way
            if(Physics2D.RaycastAll(origin, direction, distance, layerMask).Length > 1) return false;
        }
        return true;
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
                backgroundTiles[i, j].name = "BG_Tile " + i + " " + j;
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


    // HELPER FUNCTIONS

    /// <summary>
    /// Returns whether the given board coordinate pair is within the bounds of the game board.
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    public bool IsInBoard(int i, int j)
    {
        return i >= 0 && i < boardWidth && j >= 0 && j < boardHeight;
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
