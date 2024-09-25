using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeptofField : MonoBehaviour
{
    // Start is called before the first frame update
    public Material blurMaterial; // Gán Material có shader
    [Range(0, 1)]
    public float blurAmount = 0.5f; // Độ mờ

    void Update()
    {
        if (blurMaterial != null)
        {
            blurMaterial.SetFloat("_BlurAmount", blurAmount); // Điều chỉnh độ mờ
        }
    }
}
