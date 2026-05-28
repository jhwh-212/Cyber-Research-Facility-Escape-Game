using UnityEngine;

public class Soul : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager.Instance.CollectSoul();
        Destroy(gameObject);
    }
}
