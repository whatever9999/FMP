using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActionManager : MonoBehaviour
{
    public enum ActionType
    {
        CONSTANT_OBJECT_USE,
        TIMED_OBJECT_USE,
        MOVE,
        MOVE_TO_USE,
        TEST,
        DIE,
        REACT,
        REFUSE,
        BOREDOM,
        NUM_ACTION_TYPES,
    }

    [Header("Clone Animations")]
    [SerializeField] private AnimationManager cloneAnimationManager;

    [Header("Action Bar Limit")]
    [SerializeField] private int maxNumActions = 4;

    [Header("Move Action Layers")]
    [SerializeField] private LayerMask clickableLayer;
    [SerializeField] private LayerMask objectLayer;

    [Header("Action Prefabs")]
    [SerializeField] private GameObject constantObjectUseActionPrefab;
    [SerializeField] private GameObject timedObjectUseActionPrefab;
    [SerializeField] private GameObject movementActionPrefab;
    [SerializeField] private GameObject testActionPrefab;
    [SerializeField] private GameObject dieActionPrefab;
    [SerializeField] private GameObject reactActionPrefab;
    [SerializeField] private GameObject refuseActionPrefab;
    [SerializeField] private GameObject boredomActionPrefab;

    [Header("Special Action Required Locations")]
    [SerializeField] private GameObject testRequiredLocation;
    private bool cloneTesting = false;
    public bool IsCloneTesting() { return cloneTesting; }

    private List<GameObject> currentActions = new List<GameObject>();
    private Dictionary<ActionType, GameObject> actions = new Dictionary<ActionType, GameObject>();

    private Action currentAction;
    public Action GetCurrentAction() { return currentAction; }

    private void Start()
    {
        // Add actions to the dictionary
        actions.Add(ActionType.CONSTANT_OBJECT_USE, constantObjectUseActionPrefab);
        actions.Add(ActionType.TIMED_OBJECT_USE, timedObjectUseActionPrefab);
        actions.Add(ActionType.MOVE, movementActionPrefab);
        actions.Add(ActionType.MOVE_TO_USE, movementActionPrefab);
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
                // If starting the action fails then cancel it and the clone will refuse
                if (!currentAction.StartAction())
                {
                    CancelAction(currentActions[0]);
                    ManagerHandler.instance.ActionM.AddAction(ActionManager.ActionType.REFUSE, 0);
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

    // If index is -1 the action is added to the end
    public void AddAction(ActionType type, int index, GameObject usedObject = null)
    {
        // Don't add an action if we're over the max actions unless it's a move before use action or if it's already in the bar
        if (type == ActionType.MOVE_TO_USE || (currentActions.Count < maxNumActions && !AlreadyGotAction(usedObject)))
        {
            bool addAction = true;
            GameObject action;
            actions.TryGetValue(type, out action);
            if (action)
            {
                // Create the action button as a child of the action bar (adding it to the front if needed)
                GameObject button = Instantiate(action, transform);
                if (index == -1)
                {
                    // Add to end if index is -1
                    button.transform.SetAsLastSibling();
                }
                else
                {
                    button.transform.SetSiblingIndex(index);
                }

                // When the clone is on fire they can't use anything but the shower!
                bool cloneOnFire = ManagerHandler.instance.clone.IsOnFire();
                // Ensure action data is set
                switch (type)
                {
                    case ActionType.MOVE:
                        addAction = CreateMoveAction(button);
                        break;
                    case ActionType.MOVE_TO_USE:
                        {
                            ConstantObject constantObject;
                            if (usedObject.TryGetComponent<ConstantObject>(out constantObject) && constantObject.GetRequiredLocation())
                            {
                                addAction = CreateMoveToUseAction(button, constantObject.GetRequiredLocation().position);
                            }
                            else
                            {
                                // Special Action Movement
                                addAction = CreateMoveToUseAction(button, usedObject.transform.position);
                            }
                        }
                        break;
                    case ActionType.DIE:
                        ManagerHandler.instance.EventM.CheckEventTrigger(EventManager.EventType.DEATH);
                        break;
                    case ActionType.CONSTANT_OBJECT_USE:
                        {
                            if (!cloneOnFire || (cloneOnFire && usedObject.name.Equals("Shower")))
                            {
                                ConstantObjectUseAction useAction = button.GetComponent<ConstantObjectUseAction>();
                                ConstantObject constantObject = usedObject.GetComponent<ConstantObject>();
                                useAction.SetObject(constantObject);
                                useAction.SetAnimationType(constantObject.GetAnimationType());
                            }
                            else
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundName.FAILURE);
                                addAction = false;
                            }    
                        }
                        break;
                    case ActionType.TIMED_OBJECT_USE:
                        {
                            if (!cloneOnFire || (cloneOnFire && usedObject.name.Equals("Shower")))
                            {
                                TimedObjectUseAction useAction = button.GetComponent<TimedObjectUseAction>();
                                TimedObject timedObject = usedObject.GetComponent<TimedObject>();
                                useAction.SetObject(timedObject);
                                useAction.SetAnimationType(timedObject.GetAnimationType());
                            }
                            else
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundName.FAILURE);
                                addAction = false;
                            }
                        }
                        break;
                    case ActionType.TEST:
                        CancelAllActions();
                        ManagerHandler.instance.ActionM.AddAction(ActionManager.ActionType.MOVE_TO_USE, 0, testRequiredLocation);
                        break;
                }

                // Add the button to the action list
                if (addAction && index != -1) currentActions.Insert(index, button);
                else if (addAction) currentActions.Add(button);
                else Destroy(button);

                // If we're using an object make sure we move to the required location first
                if (addAction && (type == ActionType.CONSTANT_OBJECT_USE || type == ActionType.TIMED_OBJECT_USE))
                {
                    Action addedAction = button.GetComponent<Action>();

                    int i = 0;
                    for(; i < currentActions.Count; i++)
                    {
                        if (addedAction.gameObject == currentActions[i]) break;
                    }

                    // Add the move action to the spot before the just added action
                    AddAction(ActionType.MOVE_TO_USE, i, usedObject);
                }

                if (addAction)
                {
                    // Track if the clone is testing or not
                    UpdateCloneTesting();
                    ManagerHandler.instance.PerformanceMetric.IncrementPerformanceAttribute(PerformanceMetric.PerformanceData.ACTIONS_TRIGGERED);
                }
            }
            else
            {
                Debug.LogError("Failed to get action of type " + type);
            }
        }
    }
    // Only checking for object use since other actions aren't added by player
    public bool AlreadyGotAction(GameObject usedObject)
    {
        // We've already got this action if we're trying to use an object we're already planning to use in our actions
        if (usedObject)
        {
            for (int i = 0; i < currentActions.Count; i++)
            {
                ConstantObjectUseAction constantAction;
                TimedObjectUseAction timedAction;
                if (currentActions[i].TryGetComponent<ConstantObjectUseAction>(out constantAction))
                {
                    return (usedObject == constantAction.GetUsedObject().gameObject);
                }
                else if (currentActions[i].TryGetComponent<TimedObjectUseAction>(out timedAction))
                {
                    return (usedObject == timedAction.GetUsedObject().gameObject);
                }
            }
        }
        return false;
    }

    #region Stopping Actions
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
            cancellingAction.CancelAction();
            if (cancellingAction == currentAction)
            {
                currentAction = null;
                cloneAnimationManager.SetAnimation(cancellingAction.GetAnimationType(), false);
            }

            // Remove it from the action list
            currentActions.Remove(button);

            // Destroy the button
            Destroy(button);

            // Track if the clone is testing or not
            UpdateCloneTesting();
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

            // Track if the clone is testing or not
            UpdateCloneTesting();
        }
    }
    #endregion // Stopping Actions

    #region Move Actions
    public bool CreateMoveAction(GameObject button)
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
            if (Physics.Raycast(ray, out hit, 100, clickableLayer))
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
    public bool CreateMoveToUseAction(GameObject button, Vector3 destination)
    {
        MovementAction action = button.GetComponent<MovementAction>();
        action.SetActionType(ActionType.MOVE_TO_USE);
        action.SetDestination(destination);
        action.SetCancellable(false);

        return true;
    }
    #endregion // Move Actions

    // Cancel all actions from end to start
    public void CancelAllActions()
    {
        while(currentActions.Count > 0)
        {
            CancelAction(currentActions[currentActions.Count - 1]);
        }
    }

    public void ModifyTestTime(float amount)
    {
        // Check if the current action is a TestAction (if not then something has gone wrong)
        TestAction action = (TestAction)currentAction;
        if (action)
        {
            action.ModifyTestTime(amount);
        }
        else
        {
            Debug.LogError("Trying to modify test time but the current action isn't a test!");
        }
    }
    public void EndTest()
    {
        // Check if the current action is a TestAction (if not then something has gone wrong)
        TestAction action = (TestAction)currentAction;
        if (action)
        {
            action.EndTest();
        }
        else
        {
            Debug.LogError("Trying to end test but the current action isn't a test!");
        }
    }

    // The clone is testing if they're doing a test or on their way to do one
    public void UpdateCloneTesting()
    {
        cloneTesting = false;
        for (int i = 0; i < currentActions.Count; i++)
        {
            Action checkingAction = currentActions[i].GetComponent<Action>();
            if (checkingAction.GetActionType() == ActionType.TEST)
            {
                cloneTesting = true;
                break;
            }
        }
    }
}
