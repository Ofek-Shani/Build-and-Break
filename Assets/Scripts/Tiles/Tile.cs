using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Controller Implementation used by the tiles placed by the player.
/// Note: This controls visual tiles, not the model implementation.
/// DOES NOT apply to the background tiles!
/// </summary>
public class Tile : MonoBehaviour
{

    // changed in the inspector -- determines type of visual to use for the tile.
    // Options are "Basic", "Unbreakable", "Hole"
    [SerializeField] string tileVisualType = "Basic";
    /// <summary>
    /// Should this tile be ignored by the game manager when checking for a win?
    /// </summary>
    [SerializeField] bool ignoreDuringWinCheck = false;

    public bool GetIgnoreDuringWinCheck() { return ignoreDuringWinCheck; }

public enum TileStatus { Unplaceable, Correct, Incorrect}
    SpriteRenderer spr;

    Sprite[] sprites; // used by the enum and SetSprite to set sprites
    // Start is called before the first frame update
    void Awake()
    {
        InitializeSprites();
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
        CheckForNullSprites();
        // set initial state
        SetSprite(TileStatus.Correct);
    }

    void CheckForNullSprites()
    {
        int fails = 0;
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] is null) fails++;
        }
        if (fails > 0) Debug.LogWarning("File Warning: " + fails + "/" + sprites.Length + " tile sprites failed to load.");
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
        spr.sprite = sprites[(int)ts];
        //Debug.Log(spr.sprite.name + " " + gameObject.name);
    }
}
