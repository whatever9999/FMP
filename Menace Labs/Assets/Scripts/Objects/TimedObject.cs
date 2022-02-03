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

    private float useTimer;
    private bool finished;
    public bool IsFinished() { return finished; }
    public bool AffectsEnvironment() { return affectsEnvironment; }

    private void Update()
    {
        if (beingUsed)
        {
            useTimer += Time.deltaTime;

            if (useTimer > timeToUse)
            {
                useTimer = 0.0f;
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
            ManagerHandler.instance.ActionM.AddAction(ActionManager.ActionType.REFUSE, 0);
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
        useTimer = 0.0f;

        if (audioSource && startSound != SoundManager.SoundName.NUM_SOUND_NAMES)
        {
            audioSource.clip = SoundManager.instance.GetClip(startSound);
            audioSource.loop = false;
            audioSource.Play();
        }
        if (particles) particles.Play();

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

        finished = true;
        beingUsed = false;
    }

    private void OnMouseDown()
    {
        ManagerHandler.instance.ActionM.AddAction(ActionManager.ActionType.TIMED_OBJECT_USE, -1, gameObject);
    }
}
