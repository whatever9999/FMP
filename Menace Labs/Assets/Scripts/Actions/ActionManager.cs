using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActionManager : MonoBehaviour
{
    public enum ActionType
    {
        OBJECT_USE,
        MOVE,
        MOVE_TO_USE,
        TEST,
        DIE,
        REACT,
        REFUSE,
        BOREDOM,
        PASS_OUT,
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
    [SerializeField] private GameObject objectUseActionPrefab;
    [SerializeField] private GameObject movementActionPrefab;
    [SerializeField] private GameObject testActionPrefab;
    [SerializeField] private GameObject dieActionPrefab;
    [SerializeField] private GameObject reactActionPrefab;
    [SerializeField] private GameObject refuseActionPrefab;
    [SerializeField] private GameObject boredomActionPrefab;
    [SerializeField] private GameObject passOutActionPrefab;

    [Header("Special Action Required Locations")]
    [SerializeField] private GameObject testRequiredLocation;
    private bool cloneTesting = false;
    public bool IsCloneTesting() { return cloneTesting; }

    private bool cloneDying = false;
    public bool IsCloneDying() { return cloneDying; }

    private List<GameObject> currentActions = new List<GameObject>();
    private Dictionary<ActionType, GameObject> actions = new Dictionary<ActionType, GameObject>();

    private Action currentAction;
    public Action GetCurrentAction() { return currentAction; }

    [Header("Boredom Action Timer")]
    [SerializeField] private int timeToTriggerBoredom = 60;
    private int boredomTimer;

    [Header("Clone Put Out Self (fire) Action UI")]
    [SerializeField] private Sprite fireSprite;
    [SerializeField] private string fireTooltip;
    public Sprite GetFireSprite() { return fireSprite; }
    public string GetFireTooltip() { return fireTooltip; }

    [Header("Current Action Aesthetics")]
    [SerializeField] private Color currentActionColor = Color.gray;
    [SerializeField] private Vector3 currentActionScale = new Vector3(1.1f, 1.1f, 1.1f);

    private void Start()
    {
        // Add actions to the dictionary
        actions.Add(ActionType.OBJECT_USE, objectUseActionPrefab);
        actions.Add(ActionType.MOVE, movementActionPrefab);
        actions.Add(ActionType.MOVE_TO_USE, movementActionPrefab);
        actions.Add(ActionType.TEST, testActionPrefab);
        actions.Add(ActionType.DIE, dieActionPrefab);
        actions.Add(ActionType.REACT, reactActionPrefab);
        actions.Add(ActionType.REFUSE, refuseActionPrefab);
        actions.Add(ActionType.BOREDOM, boredomActionPrefab);
        actions.Add(ActionType.PASS_OUT, passOutActionPrefab);

        boredomTimer = ManagerHandler.instance.TimeM.GetCurrentTime();
    }

    private void Update()
    {
        // Update the boredom timer
        if (currentActions.Count > 0)
        {
            // Update the boredom timer
            boredomTimer = ManagerHandler.instance.TimeM.GetCurrentTime();
        }

        if (ManagerHandler.instance.TimeM.TimeSince(boredomTimer) >= timeToTriggerBoredom)
        {
            AddAction(ActionType.BOREDOM, -1);
        }
    }

    // Done in fixed update so affected by timescale
    private void FixedUpdate()
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
                    ManagerHandler.instance.ActionM.AddAction(ActionManager.ActionType.REFUSE, 1);
                }
                else
                {
                    currentAction.transform.localScale = currentActionScale;
                    currentAction.SetColour(currentActionColor);
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
                    ManagerHandler.instance.ActionM.AddAction(ActionManager.ActionType.REFUSE, 1);
                }
            }
            // End the action if it's completed
            else if (!currentAction.IsEnding() && !currentAction.IsCancelling())
            {
                EndAction(currentActions[0]);
            }
            else if (currentAction.HasFinalised())
            {
                RemoveAction(currentActions[0]);
                currentAction = null;
            }
        }
    }

    // If index is -1 the action is added to the end
    public void AddAction(ActionType type, int index, GameObject usedObject = null)
    {
        // Don't add an action if we're over the max actions unless it's a move before use action or if it's already in the bar
        if (!ManagerHandler.instance.UIM.PauseMenuOpen() && (type == ActionType.MOVE_TO_USE || (currentActions.Count < maxNumActions && !AlreadyGotAction(usedObject))))
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
                    // If the current action is still cancelling actions have to be added to index 1 but if there isn't a current action it can go to 0
                    if (!currentAction && index == 1)
                    {
                        index = 0;
                    }

                    button.transform.SetSiblingIndex(index);
                }

                // When the clone is on fire they can't use anything but the shower!
                bool cloneOnFire = ManagerHandler.instance.clone.IsOnFire();
                // And when there's a fire on the lot they can only put it out!
                bool thereIsFire = ManagerHandler.instance.director.IsFirePresent();
                // Ensure action data is set
                switch (type)
                {
                    case ActionType.MOVE:
                        addAction = CreateMoveAction(button);

                        if (addAction) CancelAdditionalActions();
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

                            if (addAction) CancelAdditionalActions();
                        }
                        break;
                    case ActionType.DIE:
                        // Make sure we're not already dying
                        if (IsPlanning(ActionType.DIE)) addAction = false;
                        
                        if (addAction)
                        {
                            CancelAllActions();
                            ManagerHandler.instance.EventM.CheckEventTrigger(EventManager.EventType.DEATH);
                        }
                        break;
                    case ActionType.OBJECT_USE:
                        {
                            if ((!cloneOnFire && !thereIsFire) ||
                                (cloneOnFire && usedObject.name.Contains("Shower")) ||
                                (thereIsFire && !cloneOnFire && usedObject.name.Contains("Fire")))
                            {
                                ObjectUseAction useAction = button.GetComponent<ObjectUseAction>();
                                ConstantObject constantObject = usedObject.GetComponent<ConstantObject>();
                                useAction.SetObject(constantObject);
                                useAction.SetAnimationType(constantObject.GetAnimationType());
                            }
                            else
                            {
                                if (cloneOnFire)
                                {
                                    ManagerHandler.instance.NotificationM.AddNotification(NotificationManager.NotificationType.CLONE_ON_FIRE);
                                }
                                else if (thereIsFire)
                                {
                                    ManagerHandler.instance.NotificationM.AddNotification(NotificationManager.NotificationType.THERE_IS_FIRE);
                                }
                                SoundManager.instance.PlayClip(SoundManager.SoundName.FAILURE);
                                addAction = false;
                            }    
                        }
                        break;
                    case ActionType.TEST:
                        CancelAllActions();
                        ManagerHandler.instance.ActionM.AddAction(ActionManager.ActionType.MOVE_TO_USE, 1, testRequiredLocation);
                        break;
                    case ActionType.REACT:
                        CancelAllActions();
                        break;
                    case ActionType.PASS_OUT:
                        // Make sure we're not already passed out
                        if (IsPlanning(ActionType.PASS_OUT)) addAction = false;

                        if (addAction) CancelAllActions();
                        break;
                }

                // Add the button to the action list
                if (addAction && index != -1) currentActions.Insert(index, button);
                else if (addAction) currentActions.Add(button);
                else Destroy(button);

                // If we're using an object make sure we move to the required location first
                if (addAction && type == ActionType.OBJECT_USE)
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
                    UpdateCloneActionStates();
                    ManagerHandler.instance.PerformanceMetric.IncrementPerformanceAttribute(PerformanceMetric.PerformanceData.ACTIONS_TRIGGERED);
                }
            }
            else
            {
                Debug.LogError("Failed to get action of type " + type);
            }
        }
        else if (currentActions.Count > maxNumActions)
        {
            ManagerHandler.instance.NotificationM.AddNotification(NotificationManager.NotificationType.MAX_ACTIONS);
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
                ObjectUseAction action;
                if (currentActions[i].TryGetComponent<ObjectUseAction>(out action))
                {
                    return (usedObject == action.GetUsedObject().gameObject);
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
            if (cancellingAction == currentAction)
            {
                cloneAnimationManager.SetAnimation(cancellingAction.GetAnimationType(), false);
            }

            cancellingAction.CancelAction();

            // If the cancelled action isn't the current action remove it from the action list
            if (cancellingAction != currentAction)
            {
                RemoveAction(button);
            }
        }
    }
    public void EndAction(GameObject button)
    {
        if (currentActions.Contains(button))
        {
            // End the action
            Action endingAction = button.GetComponent<Action>();

            // Ensure the animation stops
            cloneAnimationManager.SetAnimation(endingAction.GetAnimationType(), false);

            endingAction.EndAction();
        }
    }
    // When the animation for the current action ends it will be marked as finalised and removed from the list
    public void RemoveAction(GameObject button)
    {
        // Remove it from the action list
        currentActions.Remove(button);

        // Destroy the button
        Destroy(button);

        // Track if the clone is testing or not
        UpdateCloneActionStates();
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
        // Cancel all actions that haven't started yet
        while (currentActions.Count > 1)
        {
            CancelAction(currentActions[currentActions.Count - 1]);
        }
        // Cancel the current action
        if (currentAction) CancelAction(currentAction.gameObject);
    }
    // Cancel basic MOVE, react, bored actions etc for when another action is executed
    public void CancelAdditionalActions()
    {
        // Cancel all actions that haven't started yet
        for (int i = 1; i < currentActions.Count; i++)
        {
            ActionType checkAction = currentActions[currentActions.Count - 1].GetComponent<Action>().GetActionType();
            if (checkAction == ActionType.MOVE || checkAction == ActionType.REACT || checkAction == ActionType.BOREDOM)
            {
                CancelAction(currentActions[currentActions.Count - 1]);
            }
        }
        // Cancel the current action
        if (currentAction)
        {
            ActionType checkAction = currentAction.GetActionType();
            if (checkAction == ActionType.MOVE || checkAction == ActionType.REACT || checkAction == ActionType.BOREDOM)
            {
                CancelAction(currentAction.gameObject);
            }
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
    public void UpdateCloneActionStates()
    {
        cloneTesting = false;
        cloneDying = false;
        for (int i = 0; i < currentActions.Count; i++)
        {
            Action checkingAction = currentActions[i].GetComponent<Action>();
            if (checkingAction.GetActionType() == ActionType.TEST)
            {
                cloneTesting = true;
            }
            if (checkingAction.GetActionType() == ActionType.DIE)
            {
                cloneDying = true;
            }
        }
    }

    public bool IsCurrently(ActionType doingAction)
    {
        if (!currentAction) return false;
        return (currentAction.GetActionType() == doingAction);
    }
    public bool IsPlanning(ActionType toDoAction)
    {
        for (int i = 0; i < currentActions.Count; i++)
        {
            Action checkAction = currentActions[i].GetComponent<Action>();
            if (checkAction.GetActionType() == toDoAction)
            {
                return true;
            }
        }
        return false;
    }
    public bool IsSleeping()
    {
        ObjectUseAction objectUseAction = currentAction as ObjectUseAction;
        if (objectUseAction)
        {
            if (objectUseAction.GetUsedObject().name == "Bed") return true;
        }
        return false;
    }
}
