using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePad : MonoBehaviour
{
    [SerializeField] private float bounce;
    private Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player collided with the elevator.");
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * bounce, ForceMode2D.Impulse);
            anim.SetBool("Bounce", true);
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(WaitAndResetBounce());
        }
    }

    private IEnumerator WaitAndResetBounce()
    {
        // Đợi một khoảng thời gian
        yield return new WaitForSeconds(1f);

        // Tắt trạng thái Bounce
        anim.SetBool("Bounce", false);
    }
}