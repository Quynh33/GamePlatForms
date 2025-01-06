using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEvent : MonoBehaviour
{

    void SlashDamagePlayer()
    {
        Debug.Log("Boss hit player with damage: " + Boss.Instance.damage);

        if (PlayerMovement.Instance.transform.position.x > transform.position.x || PlayerMovement.Instance.transform.position.x < transform.position.x)
        {
            Debug.Log("Side attack triggered");
            Hit(Boss.Instance.SideAttackTransform, Boss.Instance.SideAttackArea);
        }
        else if (PlayerMovement.Instance.transform.position.y > transform.position.y)
        {
            Debug.Log("Up attack triggered");
            Hit(Boss.Instance.UpAttackTransform, Boss.Instance.UpAttackArea);
        }
        else if (PlayerMovement.Instance.transform.position.y < transform.position.y)
        {
            Debug.Log("Down attack triggered");
            Hit(Boss.Instance.DownAttackTransform, Boss.Instance.DownAttackArea);
        }
    }
    void Hit(Transform _attackTransform, Vector2 _attackArea)
    {
        // Tính toán vùng tấn công (pointA và pointB là hai góc của hình chữ nhật)
        Vector2 pointA = (Vector2)_attackTransform.position - (_attackArea / 2);
        Vector2 pointB = (Vector2)_attackTransform.position + (_attackArea / 2);

        // Vẽ vùng tấn công trong Scene View để debug
        Debug.DrawLine(pointA, new Vector2(pointA.x, pointB.y), Color.red, 1f); // Đường bên trái
        Debug.DrawLine(pointA, new Vector2(pointB.x, pointA.y), Color.red, 1f); // Đường bên dưới
        Debug.DrawLine(pointB, new Vector2(pointA.x, pointB.y), Color.red, 1f); // Đường bên phải
        Debug.DrawLine(pointB, new Vector2(pointB.x, pointA.y), Color.red, 1f); // Đường bên trên

        // Phát hiện các đối tượng trong vùng tấn công
        Collider2D[] _objectsToHit = Physics2D.OverlapAreaAll(pointA, pointB, LayerMask.GetMask("Player"));

        // Log số lượng đối tượng phát hiện được
        Debug.Log("Objects hit count: " + _objectsToHit.Length);

        // Lặp qua các đối tượng phát hiện và xử lý sát thương
        for (int i = 0; i < _objectsToHit.Length; i++)
        {
            Debug.Log("Object hit: " + _objectsToHit[i].name);

            // Kiểm tra nếu đối tượng là Player
            if (_objectsToHit[i].GetComponent<PlayerMovement>() != null)
            {
                Debug.Log("Player detected, applying damage...");
                _objectsToHit[i].GetComponent<PlayerMovement>().TakeDamage(Boss.Instance.damage);
            }
        }
    }
    void Parrying()
    {
        Boss.Instance.parrying = true;
    }
    void BendDownCheck()
    {
        if (Boss.Instance.barrageAttack)
        {

            StartCoroutine(BarrageAttackTransition());
        }
        if (Boss.Instance.outbreakAttack)
        {
            StartCoroutine(OutbreakAttackTrasittion());
        }
        if(Boss.Instance.bounceAttack)
        {
            Debug.Log("BarragePutCall");
            Boss.Instance.anmin.SetTrigger("Bounce1");

        }
    }
    void BarrageOrOutbreak()
    {
        Debug.Log("BarragePutCall");

        if (Boss.Instance.barrageAttack)
        {
            Boss.Instance.StartCoroutine(Boss.Instance.Barrage());
        }
        if (Boss.Instance.outbreakAttack)
        {
            Boss.Instance.StartCoroutine(Boss.Instance.OutBreak());
        }

    }
    IEnumerator BarrageAttackTransition()
    {
        yield return new WaitForSeconds(1f);
        Boss.Instance.anmin.SetBool("Cast", true);
    }
    IEnumerator OutbreakAttackTrasittion()
    {
        yield return new WaitForSeconds(1f);
        Boss.Instance.anmin.SetBool("Cast", true);
    }
    void DestroyAfterDeath()
    {
        Boss.Instance.DestroyAfterDeath();
    }
}
