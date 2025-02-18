using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeMenuController : MonoBehaviour
{
    private FadeUI fadeUI;
    [SerializeField] private float fadeTime;
    // Start is called before the first frame update
    void Start()
    {
        fadeUI = GetComponent<FadeUI>();
        fadeUI.FadeUIOut(fadeTime);
    }
    public void CallFadeAndStartGame(string _sceneToLoad)
    {
        StartCoroutine(FadeAndStartGame(_sceneToLoad));
    }
    IEnumerator FadeAndStartGame(string _sceneload)
    {
        fadeUI.FadeUIIn(fadeTime);
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene( _sceneload );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
