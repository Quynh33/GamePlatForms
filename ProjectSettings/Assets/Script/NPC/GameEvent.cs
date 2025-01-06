using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEvent : MonoBehaviour
{
    public void ContinueDialogue()
    {
        DialogueManager.GetInstance().ContinueStory();
    }

    public void EndGame()
    {
        Invoke("LoadEndGameScene", 2f); // Gọi hàm LoadEndGameScene sau 2 giây
    }

    private void LoadEndGameScene()
    {
        SceneManager.LoadScene("Endgame");
    }
}