using UnityEngine;

// When clicked on will change the character choice for the clone
public class MenuClone : MonoBehaviour
{
    [Header("Hover Colour")]
    [SerializeField] protected Color hoverColour = new Color(0.8f, 0.8f, 0.8f, 1);

    private CloneMeshHandler meshHandler;
    private Renderer[] materialRenderers;

    private void Awake()
    {
        meshHandler = GetComponent<CloneMeshHandler>();
        materialRenderers = GetComponentsInChildren<Renderer>();
    }

    private void OnMouseDown()
    {
        meshHandler.ChangeCharacter();
    }
    private void OnMouseOver()
    {
        if (!IsColor(hoverColour))
        {
            ChangeColor(hoverColour);

        }
    }
    private void OnMouseExit()
    {
        ChangeColor(Color.white);
    }

    private void ChangeColor(Color newColour)
    {
        for (int i = 0; i < materialRenderers.Length; i++)
        {
            // Make sure we're not trying to change the colour of PFX (which don't have this property)
            if (materialRenderers[i].material.HasProperty("_Color"))
            {
                // Ensure alpha stays the same
                newColour.a = materialRenderers[i].material.color.a;
                materialRenderers[i].material.color = newColour;
            }
        }
    }
    private bool IsColor(Color compareColor)
    {
        // Make sure we're not trying to change the colour of PFX (which don't have this property)
        if (materialRenderers[0].material.HasProperty("_Color"))
        {
            return (materialRenderers[0].material.color == compareColor);
        }
        return false;
    }
}
