using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Controller Implementation used by the tiles placed by the player.
/// Note: This controls visual tiles, not the model implementation.
/// DOES NOT apply to the background tiles!
/// </summary>
public class Tile : MonoBehaviour
{

    // changed in the inspector -- determines type of visual to use for the tile.
    // Options are "Basic", "Unbreakable", "Hole", "Burst", "Gust"
    [SerializeField] string tileVisualType = "Basic";
    /// <summary>
    /// Should this tile be ignored by the game manager when checking for a win?
    /// </summary>
    [SerializeField] bool ignoreDuringWinCheck = false;

    public bool GetIgnoreDuringWinCheck() { return ignoreDuringWinCheck; }

    public enum TileStatus { Unplaceable, Correct, Incorrect}
    SpriteRenderer spr;

    [SerializeField] Sprite[] sprites; // used by the enum and SetSprite to set sprites
    // Start is called before the first frame update
    void Awake()
    {
        InitializeSprites();
        targetPos = transform.localPosition;
    }

    // POSITION HANDLING
    Vector3 targetPos;
    private void FixedUpdate()
    {
        //transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, 0.1f);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="newPos"></param> Position to move to
    /// <param name="snapToPos"></param> // whether the movemnet should snap instead of lerp
    public void MoveTo(Vector3 newPos, bool snapToPos)
    {

    }

    // -- GAME LOGIC ---

    /// <summary>
    /// Changes the appearance/location of the tile so that it is "placed" properly.
    /// </summary>
    public virtual void Place(int i, int j, GameBoard board)
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
        spr.color = Color.white; // turn off the transparency.
    }

    /// <summary>
    /// Removes the tile from the board (if possible) and triggers the tile's Break effect.
    /// If the tile cannot be removed, this function returns false. Otherwise returns true
    /// </summary>
    /// <param name="board"></param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    public virtual bool Break(int i, int j, GameBoard board)
    {
        Destroy(gameObject);
        return true;
    }


    /// <summary>
    /// Destroys the tile without modifying anything else or triggering Break effects.
    /// </summary>
    /// <param name="board"></param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    public bool SuperBreak()
    {
        Destroy(gameObject);
        return true;
    }

    public bool CanPlaceAt(int i, int j, GameBoard board)
    {
        return GetStatus(i, j, board) != TileStatus.Unplaceable;
    }
    public virtual bool CanBreakAt(int i, int j)
    {
        return true;
    }

    public virtual bool IsMovable()
    {
        return true;
    }

    /// <summary>
    /// Gets the status of the tile if it were placed at position i,j
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="board"></param>
    public TileStatus GetStatus(int i, int j, GameBoard board)
    {
        if (board.data[i, j] != null)
        {
            return TileStatus.Unplaceable;
        }
        else
        {
            return board.goalData[i, j] ? TileStatus.Correct: TileStatus.Incorrect;
        }
    }

    // -- DISPLAY LOGIC ---

    /// <summary>
    /// Loads the correct sprites and sets tile sprite to initial state
    /// </summary>
    void InitializeSprites()
    {
        // load sprites
        sprites = Resources.LoadAll<Sprite>("Textures/Tiles/" + tileVisualType + " Tiles");
        spr = GetComponentInChildren<SpriteRenderer>();
        ValidateSprites();
        // set initial state
        SetSprite(TileStatus.Correct);
    }

    void ValidateSprites()
    {
        int fails = 0;
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] is null) fails++;
        }
        if (fails > 0) Debug.LogWarning("Tile Sprite Loading Warning: " + fails + "/" + sprites.Length + " tile sprites failed to load.");

        if (sprites.Length < 3)
        {
            Debug.LogWarning(transform.gameObject.name + " Tile: Tile Sprite Loading Warning: Should have 3 sprites but only has " + sprites.Length + ".");
        }
    }

    /// <summary>
    /// Makes the tile look like it has been placed -- used by UI controllers to create piece display on cards
    /// </summary>
    public void PlaceVisually()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
        spr.color = Color.white; // turn off the transparency.
    }

    public void SetSprite(TileStatus ts)
    {
        ValidateSprites();
        if ((int)ts > sprites.Length)
        {
            Debug.LogWarning("index too big (" + (int)ts + " > " + sprites.Length + ")");
        }
        else if ((int)ts < 0) Debug.LogWarning("<0");
        spr.sprite = sprites[(int)ts];
        //Debug.Log(spr.sprite.name + " " + gameObject.name);
    }
}
