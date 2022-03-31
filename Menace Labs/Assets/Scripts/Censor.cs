using UnityEngine;

public class Censor : MonoBehaviour
{
    private float offsetDuration = 0.2f;
    private float offsetTimer;

    MeshRenderer renderer;

    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        offsetTimer += Time.unscaledDeltaTime;

        if (offsetTimer > offsetDuration)
        {
            float randX = Random.Range(0.0f, 10.0f);
            float randY = Random.Range(0.0f, 10.0f);
            renderer.material.SetTextureOffset("_BaseMap", new Vector2(randX, randY));

            offsetTimer = 0;
        }
    }
}
