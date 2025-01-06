using UnityEngine;

public class SceneObject : MonoBehaviour
{
    public string objectKey;

    private void Start()
    {
        if (!string.IsNullOrEmpty(objectKey))
        {
            SceneObjectRegistry.Instance.RegisterObject(objectKey, this.gameObject);
        }
    }
}