using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public static ActionManager instance;

    public enum ActionType
    {
        CONSTANT_OBJECT_USE,
        TIMED_OBJECT_USE,
        MOVEMENT,
        TEST,
        DIE,
        REACT,
        REFUSE,
        BOREDOM,
    }

    [SerializeField] private GameObject constantObjectUseActionPrefab;
    [SerializeField] private GameObject timedObjectUseActionPrefab;
    [SerializeField] private GameObject movementActionPrefab;
    [SerializeField] private GameObject testActionPrefab;
    [SerializeField] private GameObject dieActionPrefab;
    [SerializeField] private GameObject reactActionPrefab;
    [SerializeField] private GameObject refuseActionPrefab;
    [SerializeField] private GameObject boredomActionPrefab;

    private List<GameObject> currentActions = new List<GameObject>();
    private Dictionary<ActionType, GameObject> actions = new Dictionary<ActionType, GameObject>();

    private Action currentAction;

    private void Start()
    {
        instance = this;

        // Add actions to the dictionary
        actions.Add(ActionType.CONSTANT_OBJECT_USE, constantObjectUseActionPrefab);
        actions.Add(ActionType.TIMED_OBJECT_USE, timedObjectUseActionPrefab);
        actions.Add(ActionType.MOVEMENT, movementActionPrefab);
        actions.Add(ActionType.TEST, testActionPrefab);
        actions.Add(ActionType.DIE, dieActionPrefab);
        actions.Add(ActionType.REACT, reactActionPrefab);
        actions.Add(ActionType.REFUSE, refuseActionPrefab);
        actions.Add(ActionType.BOREDOM, boredomActionPrefab);
    }

    private void Update()
    {
        // If we cancelled/ended the last action and there's one in the queue get the new one
        if (!currentAction && currentActions.Count > 0)
        {
            currentAction = currentActions[0].GetComponent<Action>();
        }
        if (currentAction)
        {
            // If the current action hasn't started then start it
            if (!currentAction.HasStarted())
            {
                // If starting the action fails then cancel it
                if (!currentAction.StartAction())
                {
                    CancelAction(currentActions[0]);
                    currentAction = null;
                }
            }
            // If the current action hasn't completed then continue it
            else if (!currentAction.HasCompleted())
            {
                // If continuing the action fails then cancel it
                if (!currentAction.ContinueAction())
                {
                    CancelAction(currentActions[0]);
                    currentAction = null;
                }
            }
            // End the action if it's completed
            else
            {
                EndAction(currentActions[0]);
                currentAction = null;
            }
        }
    }

    public void AddAction(ActionType type)
    {
        GameObject action;
        actions.TryGetValue(type, out action);
        if (action)
        {
            // Create the action button as a child of the action bar 
            GameObject button = Instantiate(action, transform);

            // Add the button to the action list
            currentActions.Add(button);
        }
        else
        {
            Debug.LogError("Failed to get action of type " + type);
        }
    }
    public void AddAction(ConstantObject usedObject)
    {
        GameObject action;
        actions.TryGetValue(ActionType.CONSTANT_OBJECT_USE, out action);
        if (action)
        {
            // Create the action button as a child of the action bar
            GameObject button = Instantiate(action, transform);
            button.GetComponent<ConstantObjectUseAction>().SetObject(usedObject);

            // Add the button to the action list
            currentActions.Add(button);
        }
        else
        {
            Debug.LogError("Failed to get action of type " + ActionType.CONSTANT_OBJECT_USE);
        }
    }
    public void AddAction(TimedObject usedObject)
    {
        GameObject action;
        actions.TryGetValue(ActionType.TIMED_OBJECT_USE, out action);
        if (action)
        {
            // Create the action button as a child of the action bar
            GameObject button = Instantiate(action, transform);
            button.GetComponent<TimedObjectUseAction>().SetObject(usedObject);


            // Add the button to the action list
            currentActions.Add(button);
        }
        else
        {
            Debug.LogError("Failed to get action of type " + ActionType.TIMED_OBJECT_USE);
        }
    }

    public void CancelAction(GameObject button)
    {
        if (currentActions.Contains(button))
        {
            // Cancel the action
            button.GetComponent<Action>().CancelAction();

            // Remove it from the action list
            currentActions.Remove(button);

            // Destroy the button
            Destroy(button);
        }
    }

    public void EndAction(GameObject button)
    {
        if (currentActions.Contains(button))
        {
            // End the action
            button.GetComponent<Action>().EndAction();

            // Remove it from the action list
            currentActions.Remove(button);

            // Destroy the button
            Destroy(button);
        }
    }
}
