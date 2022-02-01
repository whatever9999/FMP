using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActionManager : MonoBehaviour
{
    public static ActionManager instance;

    public enum ActionType
    {
        CONSTANT_OBJECT_USE,
        TIMED_OBJECT_USE,
        MOVEMENT,
        MOVE_TO_USE,
        MOVE_TO_ENTRANCE,
        TEST,
        DIE,
        REACT,
        REFUSE,
        BOREDOM,
        NUM_ACTION_TYPES,
    }

    [SerializeField] private AnimationManager cloneAnimationManager;

    [SerializeField] private int maxNumActions = 4;

    [SerializeField] private LayerMask clickableForMovement;
    [SerializeField] private LayerMask objectLayer;

    [SerializeField] private Transform entranceLocation;
    public Vector3 GetEntranceLocation() { return entranceLocation.position; }

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
    public Action GetCurrentAction() { return currentAction; }

    private void Start()
    {
        instance = this;

        // Add actions to the dictionary
        actions.Add(ActionType.CONSTANT_OBJECT_USE, constantObjectUseActionPrefab);
        actions.Add(ActionType.TIMED_OBJECT_USE, timedObjectUseActionPrefab);
        actions.Add(ActionType.MOVEMENT, movementActionPrefab);
        actions.Add(ActionType.MOVE_TO_USE, movementActionPrefab);
        actions.Add(ActionType.MOVE_TO_ENTRANCE, movementActionPrefab);
        actions.Add(ActionType.TEST, testActionPrefab);
        actions.Add(ActionType.DIE, dieActionPrefab);
        actions.Add(ActionType.REACT, reactActionPrefab);
        actions.Add(ActionType.REFUSE, refuseActionPrefab);
        actions.Add(ActionType.BOREDOM, boredomActionPrefab);
    }

    private void Update()
    {
        // If there's an action in the queue get the one at the front
        if (currentActions.Count > 0)
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
                else
                {
                    cloneAnimationManager.SetAnimation(currentAction.GetAnimationType(), true);
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

    public void AddAction(ActionType type, bool toStart)
    {
        // Don't add an action if we're over the max actions unless it's a move before use action
        if (type == ActionType.MOVE_TO_USE || currentActions.Count < maxNumActions)
        {
            bool addAction = true;
            GameObject action;
            actions.TryGetValue(type, out action);
            if (action)
            {
                // Create the action button as a child of the action bar (adding it to the front if needed)
                GameObject button;
                if (toStart)
                {
                    button = Instantiate(action, transform);
                    button.transform.SetAsFirstSibling();
                }
                else
                {
                    button = Instantiate(action, transform);
                }

                // Ensure action data is set
                switch (type)
                {
                    case ActionType.MOVEMENT:
                        addAction = CheckMoveAction(button);
                        break;
                    case ActionType.MOVE_TO_USE:
                        addAction = CheckMoveBeforeObjectUseAction(button);
                        break;
                    case ActionType.MOVE_TO_ENTRANCE:
                        addAction = MoveToEntranceAction(button);
                        break;
                }

                // Add the button to the action list
                if (addAction && toStart) currentActions.Insert(0, button);
                else if (addAction) currentActions.Add(button);
                else Destroy(button);
            }
            else
            {
                Debug.LogError("Failed to get action of type " + type);
            }
        }
    }
    public void AddAction(ConstantObject usedObject)
    {
        // Don't add an action if we're over the max actions
        if (currentActions.Count < maxNumActions)
        {
            // Move before carrying out the action
            AddAction(ActionType.MOVE_TO_USE, false);

            GameObject action;
            actions.TryGetValue(ActionType.CONSTANT_OBJECT_USE, out action);
            if (action)
            {
                // Create the action button as a child of the action bar
                GameObject button = Instantiate(action, transform);
                ConstantObjectUseAction useAction = button.GetComponent<ConstantObjectUseAction>();
                useAction.SetObject(usedObject);
                useAction.SetAnimationType(usedObject.GetAnimationType());

                // Add the button to the action list
                currentActions.Add(button);
            }
            else
            {
                Debug.LogError("Failed to get action of type " + ActionType.CONSTANT_OBJECT_USE);
            }
        }
    }
    public void AddAction(TimedObject usedObject)
    {
        // Don't add an action if we're over the max actions
        if (currentActions.Count < maxNumActions)
        {
            // Move before carrying out the action
            AddAction(ActionType.MOVE_TO_USE, false);

            GameObject action;
            actions.TryGetValue(ActionType.TIMED_OBJECT_USE, out action);
            if (action)
            {
                // Create the action button as a child of the action bar
                GameObject button = Instantiate(action, transform);
                TimedObjectUseAction useAction = button.GetComponent<TimedObjectUseAction>();
                useAction.SetObject(usedObject);
                useAction.SetAnimationType(usedObject.GetAnimationType());


                // Add the button to the action list
                currentActions.Add(button);
            }
            else
            {
                Debug.LogError("Failed to get action of type " + ActionType.TIMED_OBJECT_USE);
            }
        }
    }

    public void CancelAction(GameObject button)
    {
        if (currentActions.Contains(button))
        {
            // If the action before this is a MOVE_TO_USE action then cancel that too
            int cancellingActionIndex = currentActions.IndexOf(button);
            if (cancellingActionIndex > 0)
            {
                Action previousAction = currentActions[cancellingActionIndex - 1].GetComponent<Action>();
                if (previousAction.GetActionType() == ActionType.MOVE_TO_USE)
                {
                    CancelAction(currentActions[cancellingActionIndex - 1]);
                }
            }

            // Cancel the action
            Action cancellingAction = button.GetComponent<Action>();
            // If the cancelled action is the current one make sure to stop the animation
            if (cancellingAction == currentAction) cloneAnimationManager.SetAnimation(cancellingAction.GetAnimationType(), false);
            cancellingAction.CancelAction();

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
            Action endingAction = button.GetComponent<Action>();
            cloneAnimationManager.SetAnimation(endingAction.GetAnimationType(), false);
            endingAction.EndAction();

            // Remove it from the action list
            currentActions.Remove(button);

            // Destroy the button
            Destroy(button);
        }
    }

    public bool CheckMoveAction(GameObject button)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Don't add the action if we're clicking on the UI
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return false;
        }
        else
        {
            if (Physics.Raycast(ray, out hit, 100, clickableForMovement))
            {
                RaycastHit tempHit;
                button.GetComponent<MovementAction>().SetDestination(hit.point);

                // If we hit an object before hitting the destination then don't add the action
                if (Physics.Raycast(ray, out tempHit, hit.distance, objectLayer)) { return false; }
            }
            else
            {
                return false;
            }
        }

        return true;
    }
    public bool CheckMoveBeforeObjectUseAction(GameObject button)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Don't add the action if we're clicking on the UI
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return false;
        }
        else
        {
            if (Physics.Raycast(ray, out hit, 100, objectLayer))
            {
                MovementAction action = button.GetComponent<MovementAction>();
                action.SetActionType(ActionType.MOVE_TO_USE);
                action.SetDestination(hit.point);
                action.SetCancellable(false);
            }
            else
            {
                return false;
            }
        }

        return true;
    }
    public bool MoveToEntranceAction(GameObject button)
    {
        MovementAction action = button.GetComponent<MovementAction>();
        action.SetActionType(ActionType.MOVE_TO_ENTRANCE);
        action.SetDestination(entranceLocation.position);
        action.SetCancellable(false);

        return true;
    }
}
