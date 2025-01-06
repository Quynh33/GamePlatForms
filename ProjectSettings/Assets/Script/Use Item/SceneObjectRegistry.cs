using System.Collections.Generic;
using UnityEngine;

public class SceneObjectRegistry : MonoBehaviour
{
    public static SceneObjectRegistry Instance;

    private Dictionary<string, GameObject> registeredObjects = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Đăng ký đối tượng vào registry với key duy nhất
    public void RegisterObject(string key, GameObject obj)
    {
        if (!registeredObjects.ContainsKey(key))
        {
            registeredObjects.Add(key, obj);
        }
    }

    // Phương thức lấy đối tượng từ registry dựa vào key
    public GameObject GetObject(string key)
    {
        registeredObjects.TryGetValue(key, out GameObject obj);
        return obj;
    }
}
