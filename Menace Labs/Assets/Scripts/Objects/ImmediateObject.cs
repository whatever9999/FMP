using UnityEngine;

public class ImmediateObject : ConstantObject
{
    [SerializeField] private float timeToUse;
    // TODO: Enable us to trigger actions or events at the end of using an object
    //[SerializeField] private ActionType triggerAction;
    [SerializeField] private EventManager.EventType triggerEvent = EventManager.EventType.NUM_EVENT_TYPES;
    [SerializeField] private bool despawnObject;

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

        if (triggerEvent != EventManager.EventType.NUM_EVENT_TYPES) EventManager.instance.CheckEventTrigger(triggerEvent);
        if (despawnObject) Destroy(gameObject);

        beingUsed = false;
    }
}
