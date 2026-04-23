using UnityEngine;

public class ScrollingStarfield : MonoBehaviour
{
    [Header("Scroll Settings")]
    public float scrollSpeed = 0.2f;
    public Vector2 scrollDirection = new Vector2(0f, -1f);

    private Renderer rend;
    private Vector2 currentOffset;

    void Start()
    {
        rend = GetComponent<Renderer>();

        if (rend != null)
        {
            if (rend.material.HasProperty("_BaseMap"))
            {
                currentOffset = rend.material.GetTextureOffset("_BaseMap");
            }
            else
            {
                currentOffset = rend.material.mainTextureOffset;
            }
        }
    }

    void Update()
    {
        if (rend == null) return;

        currentOffset += scrollDirection.normalized * scrollSpeed * Time.deltaTime;

        if (rend.material.HasProperty("_BaseMap"))
        {
            rend.material.SetTextureOffset("_BaseMap", currentOffset);
        }
        else
        {
            rend.material.mainTextureOffset = currentOffset;
        }
    }
}