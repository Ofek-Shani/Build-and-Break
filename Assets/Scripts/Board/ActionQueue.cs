using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class ActionQueue : MonoBehaviour
{
    GameBoard board;

    List<QueuableBoardAction> currentQueue = new List<QueuableBoardAction>();
    List<QueuableBoardAction> nextQueue = new List<QueuableBoardAction>();

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
        QueuableBoardAction.CombineActions(nextQueue);
    }

    private void Update()
    {
        //Debug.Log(nextQueue.Count);
        //Debug.Log(currentQueue.Count);
        if (!AreQueuesClear())
        {
            Debug.Log(CurrentQueueCount() + " " + NextQueueCount());
            foreach (QueuableBoardAction qAct in currentQueue)
            {
                qAct.Act(board);
            }
        }
        currentQueue = nextQueue;
        nextQueue = new List<QueuableBoardAction>();
      
    }
}
