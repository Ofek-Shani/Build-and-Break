using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Controller Implementation used by the tiles placed by the player.
/// Note: This controls visual tiles, not the model implementation.
/// DOES NOT apply to the background tiles!
/// </summary>
public class TileController : MonoBehaviour
{
    public enum TileStatus { Unplaceable, Correct, Incorrect}
    SpriteRenderer spr;

    // changed in the inspector -- determines type of visual to use for the tile.
    // Options are "Basic", "Unbreakable"
    [SerializeField] string tileVisualType; 

    Sprite[] sprites; // used by the enum and SetSprite to set sprites
    // Start is called before the first frame update
    void Awake()
    {
        InitializeSprites();
    }

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
        for(int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] is null) fails++;
        }
        if (fails > 0) Debug.LogWarning("File Warning: " + fails + "/" + sprites.Length + " tile sprites failed to load.");
    }

    /// <summary>
    /// Changes the appearance/location of the tile so that it is "placed" properly.
    /// </summary>
    public void Place()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
        spr.color = Color.white; // turn off the transparency.
    }

    /// <summary>
    /// Gets the status of the tile if it were placed at position i,j
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="bc"></param>
    public TileStatus GetStatus(int i, int j, GameObject[,] tiles, BoardController bc)
    {
        if (bc.holeData[i, j] == true || tiles[i, j] is not null)
        {
            return TileStatus.Unplaceable;
        }
        else
        {
            return bc.goalData[i, j] ? TileStatus.Correct: TileStatus.Incorrect;
        }
    }

    public bool CanPlaceAt(int i, int j, GameObject[,] tiles, BoardController bc)
    {
        return GetStatus(i, j, tiles, bc) != TileStatus.Unplaceable;
    }

    public void SetSprite(TileStatus ts)
    {
        spr.sprite = sprites[(int)ts];
        //Debug.Log(spr.sprite.name + " " + gameObject.name);
    }
}
