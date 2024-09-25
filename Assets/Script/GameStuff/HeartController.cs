using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartController : MonoBehaviour
{
    PlayerMovement player;
    private GameObject[] heartContainers;
    private Image[] heartFills;
    public Transform heartsParent;
    public GameObject heartContainerPrefab;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerMovement.Instance;
        heartContainers = new GameObject[PlayerMovement.Instance.maxHealth];
        heartFills = new Image[PlayerMovement.Instance.maxHealth];

        PlayerMovement.Instance.onHealthChangedCallback += UpdateHeartsHUD;
        InstantiateHeartContainers();
        SetHeartContainers();
    }

    // Update is called once per frame
    void Update()
    {
        SetFilledHearts();
    }
    void SetHeartContainers()
    {
        for (int i = 0; i < heartContainers.Length; i++)
        {
            if(i < PlayerMovement.Instance.maxHealth)
            {
                heartContainers[i].SetActive(true);
            }
            else
            {
                heartContainers[i].SetActive(false);
            }    
        }
    }
    void SetFilledHearts()
    {
        for (int i = 0; i < heartFills.Length; i++)
        {
            if (i < PlayerMovement.Instance.health)
            {
                heartFills[i].fillAmount = 1;
            }
            else
            {
                heartFills[i].fillAmount = 0;
            }
        }
    }
    void InstantiateHeartContainers()
    {
        if (heartContainerPrefab == null)
        {
            Debug.LogError("C�");
            return;
        }
        if (heartsParent == null)
        {
            Debug.LogError("C�");
            return;
        }

        for (int i = 0; i < PlayerMovement.Instance.maxHealth; i++)
        {
            GameObject temp = Instantiate(heartContainerPrefab);
            temp.transform.SetParent(heartsParent, false);
            heartContainers[i] = temp;
            heartFills[i] = temp.transform.Find("HeartFill").GetComponent<Image>();
            if (heartFills[i] == null)
            {
                Debug.LogError("C�");
            }
        }
    }
    void UpdateHeartsHUD()
    {
        SetHeartContainers();
        SetFilledHearts();
    }
}
