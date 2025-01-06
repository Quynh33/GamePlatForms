using System.Collections;
using UnityEngine;

public class SpawnBoss : MonoBehaviour
{
    public static SpawnBoss Instance;

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private Animator doorLeftAnimator;
    [SerializeField] private BoxCollider2D roomCollider;

    private bool callOnce = false;
    private GameObject spawnedBoss;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !callOnce)
        {
            callOnce = true;
            StartCoroutine(TriggerBossRoom());
        }
    }

    private IEnumerator TriggerBossRoom()
    {
        // Đóng cửa
        doorLeftAnimator.SetBool("Close", true);


        // Chờ 1 giây trước khi spawn Boss
        yield return new WaitForSeconds(1f);

        // Đặt collider để ngăn player thoát khỏi phòng
        roomCollider.isTrigger = false;

        // Spawn Boss
        spawnedBoss = Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity);

        // Theo dõi trạng thái của Boss
        StartCoroutine(CheckBossDefeated());
    }

    private IEnumerator CheckBossDefeated()
    {
        while (spawnedBoss != null)
        {
            yield return null; // Chờ cho đến khi Boss bị phá hủy
        }

        // Mở cửa sau khi Boss bị tiêu diệt
        doorLeftAnimator.SetBool("Open", true);

        // Bật lại trigger cho phòng
        roomCollider.isTrigger = true;
    }
}