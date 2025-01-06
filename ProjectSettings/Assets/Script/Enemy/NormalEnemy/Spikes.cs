using System.Collections;
using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] private Transform assignedRespawnPoint;

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            StartCoroutine(RespawnPlayer(_other));
        }
    }

    private IEnumerator RespawnPlayer(Collider2D player)
    {
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement == null) yield break;

        playerMovement.pState.cutscene = true;
        playerMovement.pState.invincible = true;
        playerMovement.rb.velocity = Vector2.zero;

        // Hiệu ứng fade out
        yield return UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.In);

        playerMovement.TakeDamage(1);
        yield return new WaitForSecondsRealtime(1);

        // Đặt vị trí của nhân vật tại điểm hồi sinh tương ứng
        player.transform.position = assignedRespawnPoint.position;

        // Hiệu ứng fade in
        yield return UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.Out);
        yield return new WaitForSecondsRealtime(UIManager.Instance.sceneFader.fadeTime);

        playerMovement.pState.cutscene = false;
        playerMovement.pState.invincible = false;
    }
}