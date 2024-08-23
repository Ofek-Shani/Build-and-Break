using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class ActionQueue : MonoBehaviour
{
    // how many FixedUpdates should we wait in between queue clears?
    // we do this to add padding between each "cycle" in the aftermath of a Break.
    const int FRAMES_BETWEEN_PROCESSES = 3;
    int framesUntilNextProcess = 0;

    GameBoard board;

    List<QueuableBoardAction> currentQueue = new List<QueuableBoardAction>();
    List<QueuableBoardAction> nextQueue = new List<QueuableBoardAction>();
    Dictionary<Vector2Int, List<QueuableBoardAction>> nextQueuePerTile = new();

    public static ActionQueue Instance { get; private set; }

    public int CurrentQueueCount() { return currentQueue.Count; }
    public int NextQueueCount() { return nextQueue.Count; }

    private void Awake()
    {
        Instance = this;
        board = gameObject.GetComponent<GameBoard>();
    }

    public bool AreQueuesClear()
    {
        return currentQueue.Count == 0 && nextQueue.Count == 0;
    }

    public void QueueAction(QueuableBoardAction toAdd)
    {
        nextQueue.Add(toAdd);
    }

    private void FixedUpdate()
    {
        //Debug.Log(framesUntilNextProcess);
        if (!AreQueuesClear())
        {
            if (framesUntilNextProcess <= 0)
            {
                //Debug.Log("Current Queue: " + currentQueue.Count + ", Next Queue: " + nextQueue.Count);
                currentQueue = QueuableBoardAction.CombineActions(currentQueue);
                //Debug.Log("Processing Current Queue...");
                //Debug.Log(CurrentQueueCount() + " " + NextQueueCount());
                foreach (QueuableBoardAction qAct in currentQueue)
                {
                    //Debug.Log("Executing " + qAct.ToString());
                    qAct.Act(board);
                }
                currentQueue = nextQueue;
                nextQueue = new List<QueuableBoardAction>();
                //Debug.Log("Next Queue loaded");
                framesUntilNextProcess = FRAMES_BETWEEN_PROCESSES;
            }
        }
        framesUntilNextProcess--;

    }
}
