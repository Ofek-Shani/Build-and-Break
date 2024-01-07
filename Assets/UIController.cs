using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    // Card-related variables
    Vector3 anchor = new Vector3(-7, 0, 0); // center position of the column of cards
    [SerializeField]
    GameObject cardPrefab;
    [SerializeField]
    float cardLength, cardHeight, padding;
    List<GameObject> cards;
    List<CardController> cardControllers;

    // Phase text-related variables
    GameObject phaseTextObj, phaseSubtextObj;
    TMP_Text phaseText, phaseSubtext;

    private void Awake()
    {
        // Cards
        cards = new List<GameObject>();
        cardControllers = new List<CardController>();
        // Phase Text
        phaseTextObj = GameObject.FindGameObjectWithTag("Phase Text");
        phaseSubtextObj = GameObject.FindGameObjectWithTag("Phase Subtext");
        phaseText = phaseTextObj.GetComponent<TMP_Text>();
        phaseSubtext = phaseSubtextObj.GetComponent<TMP_Text>();
        SetPhaseText(0);
    }

    // ----------------------- PHASE TEXT -------------------------------
    
    /// <summary>
    /// Sets the phase text and subtext to the appropriate message given toRemove.
    /// Passing in a value of 0 means that the user is in Place mode, and the subtext text will not appear.
    /// 
    /// </summary>
    /// <param name="toRemove"></param>
    public void SetPhaseText(int toRemove)
    {
        bool breakMode = toRemove > 0;
        string textToSet = breakMode ? "Break Phase" : "Build Phase";
        phaseText.text = textToSet;
        if (!breakMode) phaseSubtext.text = "";
        else
        {
            phaseSubtext.text = "Break " + toRemove + " more " + (toRemove > 1 ? "Tiles": "Tile");
        }
    }






    // ----------------------- UI CARDS ---------------------------------


    // UI Card Modifications

    /// <summary>
    /// Card Factory: 
    /// handles filling up the card with all of the necessary data
    /// (this includes a duplicate of the piece to be placed on the card)
    /// </summary>
    /// <param name="piece"></param>
    public void AddCard(Piece.PieceStruct piece)
    {
        // Basic stuff
        GameObject tempCard = Instantiate(cardPrefab);
        CardController tempController = tempCard.GetComponent<CardController>();
        tempController.SetCostText(piece.cost.ToString());
        // now add the card picture
        GameObject icon = Instantiate(piece.pieceObj, tempController.anchorObject.transform);
        icon.SetActive(true);
        float scalingFactor = 1f / Mathf.Max(piece.width, piece.height);
        icon.transform.localScale = new Vector2(1f, 1f) * scalingFactor;
        // set the position of the picture
        Vector2 newPos = new Vector2(-1, 1) * scalingFactor;
        //icon.transform.localPosition = Vector2.Scale(new Vector2(-1, 1), icon.transform.localScale);
        // even and odd height and width sizes need the piece to be scaled in different ways
        if (piece.width % 2 == 0) newPos = Vector2.Scale(newPos, new Vector2(.5f, 1));
        if (piece.height % 2 == 0) newPos = Vector2.Scale(newPos, new Vector2(1, .5f));
        // we need a special case for when piece width and height are 1
        if (piece.width == 1) newPos = Vector2.Scale(newPos, new Vector2(0, 1));
        if (piece.height == 1) newPos = Vector2.Scale(newPos, new Vector2(1, 0));
        icon.transform.localPosition = newPos;
        // now make the icon look correct by making it look like it was placed
        foreach (Transform t in icon.transform) t.GetComponent<TileController>().Place();
        // Add everything to the correct list
        cards.Add(tempCard);
        cardControllers.Add(tempController);
        UpdateCardPositions();
        UpdateCardKeys();
        
    }

    /// <summary>
    /// Removes the card elements from the card and cardController lists at the given index
    /// then destroys the card gameobject removed from the list.
    /// </summary>
    /// <param name="toRemove"></param>
    public void RemoveCard(int toRemove)
    {
        cardControllers.Remove(cardControllers[toRemove]);
        GameObject toDestroy = cards[toRemove];
        cards.Remove(toDestroy);
        GameObject.Destroy(toDestroy);
        UpdateCardPositions();
        UpdateCardKeys();
    }

    /// <summary>
    /// TODO -- this function are bugged
    /// </summary>
    public void RemoveAllCards()
    {
        if (cards is null) return;
        int times = cards.Count;
        for(int i = 0; i < times; i++) RemoveCard(0);
    }

    /// <summary>
    /// Updates the positions of the cards on the play space
    /// </summary>
    void UpdateCardPositions()
    {
        for(int i = 0; i < cards.Count; i++)
        {
            float totalY = cards.Count * cardHeight;
            // we use this in order to make sure that everything is centered properly.
            float offset = .5f;
            Vector3 newPosition = anchor + new Vector3(0, -1 * ((i + offset) * cardHeight - (totalY / 2)));
            cards[i].transform.position = newPosition;
        }
    }

    /// <summary>
    /// Sets the key text on each of the cards
    /// </summary>
    void UpdateCardKeys()
    {
        for(int i = 0; i < cardControllers.Count; i++)
        {
            cardControllers[i].SetKeyText((i+1).ToString());
        }
    }

    // Card Moving 

    /// <summary>
    /// Sets all of the active cards to "in"
    /// </summary>
    public void MoveAllCardsIn()
    {
        foreach(CardController c in cardControllers)
        {
            c.MoveIn();
        }
    }

    /// <summary>
    /// Moves all cards in, then moves the selected card out to the right.
    /// The latter is not done if i is not a valid index.
    /// </summary>
    /// <param name="i"></param>
    public void MoveCardOut(int i)
    {
        MoveAllCardsIn();
        if(i >= 0 && i < cardControllers.Count) cardControllers[i].MoveOut();
    }

    // Card Enable/Disable

    public void DisableCards()
    {
        foreach (CardController c in cardControllers)
        {
            c.SetGrayOut(true);
        }
    }

    public void EnableCards()
    {
        foreach (CardController c in cardControllers)
        {
            c.SetGrayOut(false);
        }
    }





    


}
