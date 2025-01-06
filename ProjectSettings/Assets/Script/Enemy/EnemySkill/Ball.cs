using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private float damage = 1f ; // Sát thương của đạn

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player hit!");
            PlayerMovement.Instance.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
