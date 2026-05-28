using UnityEngine;

public class Switch : MonoBehaviour
{
    [Header("Lights this switch controls")]
    public GameObject[] controlledLights;

    [Header("Optional: sprite to show when used")]
    public Sprite activatedSprite;

    private bool used = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (used || !other.CompareTag("Player")) return;

        used = true;

        foreach (GameObject light in controlledLights)
        {
            if (light != null)
                light.SetActive(false);
        }

        // Swap sprite if one is assigned
        if (activatedSprite != null)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.sprite = activatedSprite;
        }

        Debug.Log("Switch activated — lights disabled.");
    }
}
