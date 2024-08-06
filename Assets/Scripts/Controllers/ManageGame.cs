using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ManageGame : MonoBehaviour
{
    // Use this to determine which sets of levels to draw from.
    int versionNumber = 3;


    [SerializeField] int startingLevel = 1, numLevels = 8;
    int currentLevel;
    List<PieceData> pieces;
    List<Piece> pieceComponents;
    [SerializeField]
    GameObject tilePrefab;

    GameBoard board;
    GameObject boardObj;
    GameObject eraser;

    UIController ui;

    ActionQueue actionQueue;

    // Game controller important variables
    GameObject controlledPiece = null;
    Piece activePiece;
    Vector2 boardPosition = Vector2.zero;
    // how many  more pieces need to be broken before we can place the next piece.
    int toRemove = 0; // if 0, we are in place mode

    int GetPressedNumber()
    {
        for (int number = 1; number <= 9; number++)
        {
            if (Input.GetKeyDown(number.ToString()))
                return number;
        }

        return -1;
    }

    private void Awake()
    { 
        board = gameObject.GetComponent<GameBoard>();
        actionQueue = gameObject.GetComponent<ActionQueue>();
        boardObj = GameObject.FindGameObjectWithTag("Board");
        eraser = GameObject.FindGameObjectWithTag("Eraser");
        ui = GetComponent<UIController>();
        currentLevel = startingLevel;

    }

    private void Start()
    {
        // Time to load up the first level!
        LoadLevel(startingLevel);
    }

    void DestroyAllPieces()
    {
        if (pieceComponents is null) return;
        foreach (Piece p in pieceComponents) Destroy(p.pieceObj);
    }

    void LoadLevel(int levelNumber)
    {
        Debug.Log("Loading Level " + levelNumber);
        // first, let's clear everything up
        ui.RemoveAllCards();
        board.ClearBoard();
        ClearActivePiece();
        DestroyAllPieces();
        // make sure that we snap back to place mode
        ToggleBreakMode(false);
        toRemove = 0;
        ui.SetPhaseText(toRemove);

        // Now let's prep the new level.
        // get the board ready...
        board.FillBoard(levelNumber, versionNumber);
        // now let's put together all of the pieces.
        pieces = new List<PieceData>(Resources.LoadAll<PieceData>("Version " + versionNumber + "/Piece Scriptables/Level " + levelNumber));
        pieceComponents = new List<Piece>(); // list of all piece components attached to gameobjects

        
        int counter = 0;
        Vector3 spawnPoint = new Vector3(-10, 7, 0);
        foreach (PieceData pData in pieces)
        {
            GameObject toAdd = new GameObject("Piece " + (counter));
            toAdd.transform.position = spawnPoint;
            spawnPoint += new Vector3(0, -1 * (pData.height + 1));
            // fill the gameobject with tiles!
            Piece pStruct;
            try
            {
                pStruct = PieceFactory.MakePiece(pData, ++counter, toAdd);
                pieceComponents.Add(pStruct);
                // and let's take care of the UI while we're at it.
                ui.AddCard(pStruct);
            }
            catch (Exception e)
            {
                Debug.LogError("ERR " + pData.name + e);
            }
        }
        // make all of the actual pieces invisible so they don't clog the screen
        foreach (Piece p in pieceComponents) p.pieceObj.SetActive(false);
    }

    /// <summary>
    /// Changes the current level to the next level, if there is one.
    /// </summary>
    void LoadNextLevel()
    {
        if (currentLevel <= numLevels)
        {
            Debug.Log("Loading Next Level");
            LoadLevel(++currentLevel);
        }
    }

    /// <summary>
    /// Restarts the current level
    /// </summary>
    void RestartLevel()
    {
        LoadLevel(currentLevel);
    }

    /// <summary>
    /// Changes the level to the previous level, if there is one.
    /// </summary>
    void LoadPreviousLevel()
    {
        if (currentLevel > startingLevel)
        {
            Debug.Log("Loading Previous Level");
            LoadLevel(--currentLevel);
        }
    }

    /// <summary>
    /// Handles inputs relating to skipping a level, restarting a level, or going back to the previous level.
    /// </summary>
    void HandleLevelManipInputs()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                LoadNextLevel();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                LoadPreviousLevel();
            }
        }
        else if (Input.GetKeyDown(KeyCode.R)) RestartLevel();
    }
    // Update is called once per frame
    void Update()
    {
        HandleLevelManipInputs();

        // Win Detection
        if (pieceComponents.Count == 0 && toRemove == 0 && actionQueue.AreQueuesClear())
        {
            if (CheckForWin()) LoadNextLevel();
            else RestartLevel();
        }

        // Set which piece we are controlling
        int keynum = GetPressedNumber() - 1; // subtract 1 to make it more usable as an index for pieces
        if (keynum >= 0 && keynum < pieceComponents.Count && toRemove == 0)
        {
            foreach (Piece p in pieceComponents) p.pieceObj.SetActive(false);
            activePiece = pieceComponents[keynum];
            activePiece.pieceObj.SetActive(true);
            controlledPiece = activePiece.pieceObj;
            ui.MoveCardOut(keynum);
        }
        if (controlledPiece is not null) ControlPiece();
    }

    bool CheckForWin()
    {
        for(int i = 0; i < board.boardWidth; i++)
        {
            for(int j = 0; j < board.boardHeight; j++)
            {
                if (board.goalData[i, j] != (board.data[i, j] is not null)) {
                    if(!board.data[i, j].GetComponent<Tile>().GetIgnoreDuringWinCheck()) return false;
                }
            }
        }
        return true;
    }


    /// <summary>
    /// Deletes/Destroys the currently active piece
    /// </summary>
    void ClearActivePiece()
    {
        if (controlledPiece is null) return;
        if (controlledPiece != eraser)
        {
            pieceComponents.Remove(activePiece);
            GameObject toDestroy = controlledPiece;
            Destroy(toDestroy);
        }
        else 
        { 
            ToggleBreakMode(false);
        }
        controlledPiece = null;
    }

    /// <summary>
    /// REQUIRES controlledPiece is not null
    /// Makes sure piece is not out of bounds, updates piece tiles, and places the piece when enter is pressed.
    /// </summary>
    void ControlPiece()
    {
        // clamp board position so that the piece never goes off of the board.
        // Handle Cursor Movement
        if (controlledPiece == eraser) HandleCursorMovement(1, 1);
        else HandleCursorMovement(activePiece.width, activePiece.height);
        controlledPiece.transform.position = (Vector3)Vector2.Scale(boardPosition, new Vector2(1, -1))
            + new Vector3(-board.boardWidth / 2, board.boardHeight / 2, -.5f); //-.5f so that the piece is closer to camera
        // update each tile in the piece to reflect if it is placed in the correct spot.
        if (controlledPiece != eraser)
        {
            //Debug.Log((board is not null) + " " + (board.data is not null));
            activePiece.UpdateTiles(Vector2Int.RoundToInt(new Vector2(boardPosition.x, board.boardHeight - boardPosition.y - activePiece.height)), board.data, board);
        }
        // handle piece placement -- set each tile object to be a child of the board, then delete the piece parent.
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (toRemove >0) // aka if in break mode
            {
                if (board.data[(int)boardPosition.x, (int)(board.boardHeight - boardPosition.y - 1)] is not null)
                {
                    // Attempt to erase the tile at the given position
                    if (board.CanBreakAt((int)boardPosition.x, (int)(board.boardHeight - boardPosition.y - 1)))
                    {
                        actionQueue.QueueAction(new QueuableBreak(new Vector2Int((int)boardPosition.x, (int)(board.boardHeight - boardPosition.y - 1)), gameObject));
                        // decrement toRemove and check to see if we can switch back to Build Phase
                        toRemove--;
                        if (toRemove == 0)
                        {
                            ToggleBreakMode(false);
                            //Debug.Log("Tile erased. You can now place another piece.");
                        }
                        ui.SetPhaseText(toRemove);
                    }
                }
                
            }
            else if(controlledPiece != null) // aka if we are in build mode and have a piece to place
            {
                toRemove = activePiece.cost;
                // for some reason we need to place the pieces in normal x order but reverse y order -- just roll with it.
                if (PlacePieceAt(activePiece, (int)boardPosition.x, (int)(board.boardHeight - boardPosition.y - activePiece.height)))
                {
                    ToggleBreakMode(true);
                    ui.SetPhaseText(toRemove);
                    if (toRemove == 0) ToggleBreakMode(false);
                }
                else toRemove = 0;
            }
        }
    }

    /// <summary>
    /// Places a piece at a given point. Returns false if the placement was unsuccessful.
    /// </summary>
    /// <param name="p"></param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    bool PlacePieceAt(Piece p, int i, int j)
    {
        // try to place the piece and return false if it fails.
        if(!p.AddTilesToBoard(new Vector2Int(i, j), board.data, board, boardObj.transform)) { return false; }
        // destroy the piece's card and gray remaining cards out (we are about to enter Break Phase)
        ui.RemoveCard(pieceComponents.IndexOf(p)); // we can use this because index i in pieceObjects, pieces, and UIControllers card lists all correspond to components of the same piece
        pieceComponents.Remove(p);
        Destroy(controlledPiece);
        return true;
    }

    /// <summary>
    /// Handles the transitions in and out of break mode.
    /// </summary>
    /// <param name="on"></param>
    void ToggleBreakMode(bool breakModeOn)
    {
        if(breakModeOn)
        {
            controlledPiece = eraser;
            eraser.SetActive(true);
            ui.DisableCards();
        }
        else
        {
            controlledPiece = null;
            eraser.SetActive(false);
            ui.EnableCards();
        }
    }

    /// <summary>
    /// takes in keyboard input and moves the cursor accordingly
    /// </summary>
    /// <param name="pieceWidth"></param>
    /// <param name="pieceHeight"></param>
    void HandleCursorMovement(int pieceWidth, int pieceHeight)
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) boardPosition += Vector2.down;
        if (Input.GetKeyDown(KeyCode.DownArrow)) boardPosition += Vector2.up;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) boardPosition += Vector2.left;
        if (Input.GetKeyDown(KeyCode.RightArrow)) boardPosition += Vector2.right;

        boardPosition = new Vector2(Mathf.Clamp(boardPosition.x, 0, board.boardWidth - pieceWidth),
            Mathf.Clamp(boardPosition.y, 0, board.boardHeight - pieceHeight));
    }
}
