using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float hitForce;
    [SerializeField] float speed;
    [SerializeField] float lifetime = 1;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifetime );
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += speed * transform.right;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag =="Enemy")
        {
            other.GetComponent<Enemy>().EnemyGetHit(damage, (other.transform.position - transform.position).normalized, -hitForce);
        }
    }
}
