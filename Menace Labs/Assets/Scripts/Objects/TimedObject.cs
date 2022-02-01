using UnityEngine;

public class TimedObject : ConstantObject
{
    [SerializeField] private float timeToUse;
    // TODO: Enable us to trigger actions or events at the end of using an object
    //[SerializeField] private ActionType triggerAction;
    [SerializeField] private EventManager.EventType triggerEvent = EventManager.EventType.NUM_EVENT_TYPES;
    [SerializeField] private bool despawnObject;
    [SerializeField] private bool affectsEnvironment = false;

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

    public override bool StartUsing()
    {
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

        if (triggerEvent != EventManager.EventType.NUM_EVENT_TYPES) ManagerHandler.instance.EventM.CheckEventTrigger(triggerEvent);
        if (despawnObject) Destroy(gameObject);

        finished = true;
        beingUsed = false;
    }

    private void OnMouseDown()
    {
        ManagerHandler.instance.ActionM.AddAction(ActionManager.ActionType.MOVEMENT, false);
        ManagerHandler.instance.ActionM.AddAction(this);
    }
}
