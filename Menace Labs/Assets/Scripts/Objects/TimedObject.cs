using UnityEngine;

public class TimedObject : ConstantObject
{
    
    [Header("Timer")]
    [Tooltip("The time to use this object will be determined by how long it takes for its animations to run through")]
    [SerializeField] private bool useAnimationSetForTimeToUse = true;
    [Tooltip("If this is 0 the animation set time will be used")]
    [SerializeField] private float timeToUse = 0.0f;
    
    [Header("Results")]
    [SerializeField] private EventManager.EventType triggerEvent = EventManager.EventType.NUM_EVENT_TYPES;
    [SerializeField] private bool despawnObject;
    [Tooltip("This object will be spawned at the feet of the clone")]
    [SerializeField] private GameObject spawnObject;
    [Tooltip("This object will be spawned in the clone's hand and they will use it immediately")]
    [SerializeField] private GameObject giveObject;
    
    [Header("Variables")]
    [SerializeField] private bool affectsEnvironment = false;
    [SerializeField] private int usesFood = 0;

    private bool finished;
    public bool IsFinished() { return finished; }
    public bool AffectsEnvironment() { return affectsEnvironment; }

    protected override void Update()
    {
        base.Update();

        if (beingUsed)
        {
            int timeBeingUsed = ManagerHandler.instance.TimeM.TimeSince(startedUsingTime);

            if (timeBeingUsed > timeToUse)
            {
                finished = true;
            }
        }
    }

    private new void Start()
    {
        base.Start();

        if (useAnimationSetForTimeToUse || timeToUse == 0.0f)
        {
            timeToUse = ManagerHandler.instance.AnimationM.GetAnimationLength(animationType);
        }
    }

    public override bool StartUsing()
    {
        // If there isn't enough food for this object to be used cancel the action
        if (usesFood > 0 && !ManagerHandler.instance.FoodM.GotEnoughFood(usesFood))
        {
            return false;
        }
        else if (usesFood > 0)
        {
            ManagerHandler.instance.FoodM.ModifyFoodAmount(-usesFood);
        }

        // If the clone should face the same direction as the transform to use the object make sure they're rotated
        if (faceTransformDirection)
        {
            ManagerHandler.instance.clone.transform.rotation = requiredLocation.rotation;
        }

        beingUsed = true;
        finished = false;
        startedUsingTime = ManagerHandler.instance.TimeM.GetCurrentTime();

        if (audioSource && startSound != SoundManager.SoundName.NUM_SOUND_NAMES)
        {
            audioSource.clip = SoundManager.instance.GetClip(startSound);
            audioSource.loop = false;
            audioSource.Play();
        }
        if (particles) particles.Play();

        // If the clone is on fire and this is the shower put them out
        if (name.Equals("Shower")) ManagerHandler.instance.clone.SetOnFire(false);

        return true;
    }
    public override void FinishUsing()
    {
        if (audioSource)
        {
            audioSource.loop = false;
            if (endSound != SoundManager.SoundName.NUM_SOUND_NAMES)
            {
                audioSource.clip = SoundManager.instance.GetClip(endSound);
                audioSource.Play();
            }
            else
            {
                audioSource.Stop();
            }
        }
        if (particles) particles.Stop();

        if (beingUsed) DirtyOrBrokenCheck();

        // Update Goal Stats
        if (name.Equals("Fridge")) ManagerHandler.instance.GoalM.ModifyMealsMade(1);
        else if (name.Equals("Oven")) ManagerHandler.instance.GoalM.ModifyMealsMade(1);
        else if (name.Contains("Dirty")) ManagerHandler.instance.GoalM.ModifyTimesCleaned(1);
        else if (name.Contains("Rubbish")) ManagerHandler.instance.GoalM.ModifyTimesCleaned(1);
        else if (name.Contains("Puddle")) ManagerHandler.instance.GoalM.ModifyTimesCleaned(1);
        else if (name.Equals("Fire")) ManagerHandler.instance.GoalM.ModifyFiresSurvived(1);

        if (triggerEvent != EventManager.EventType.NUM_EVENT_TYPES) ManagerHandler.instance.EventM.CheckEventTrigger(triggerEvent);
        if (giveObject)
        {
            ManagerHandler.instance.clone.GiveObject(giveObject);
        }
        if (spawnObject)
        {
            ManagerHandler.instance.clone.SpawnObject(spawnObject);
        }
        if (despawnObject) Destroy(gameObject);

        finished = true;
        beingUsed = false;
    }
    public override void CancelUsing()
    {
        if (audioSource)
        {
            audioSource.loop = false;
            // Only play the end use sound if the object use gets cancelled while it's being used
            if (beingUsed && endSound != SoundManager.SoundName.NUM_SOUND_NAMES)
            {
                audioSource.clip = SoundManager.instance.GetClip(endSound);
                audioSource.Play();
            }
            else
            {
                audioSource.Stop();
            }
        }
        // If we cancelled fixing or cleaning don't tidy particles or change to fixed/clean object
        if (breakType != BreakType.BROKEN && dirtType != DirtType.DIRTY)
        {
            if (particles) particles.Stop();

            if (beingUsed) DirtyOrBrokenCheck();
        }

        finished = true;
        beingUsed = false;
    }

    private void OnMouseDown()
    {
        ManagerHandler.instance.ActionM.AddAction(ActionManager.ActionType.TIMED_OBJECT_USE, -1, gameObject);
    }
}
