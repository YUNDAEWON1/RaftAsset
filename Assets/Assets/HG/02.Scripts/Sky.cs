using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sky : MonoBehaviour
{
    public float dayTime = 24f;
    public Material dayMaterial;
    public Material nightMaterial;

    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        dayTime -= Time.deltaTime;
        if (dayTime <= 0)
        {
            dayTime = 24f;
        }

        if (dayTime < 12f)
        {
            meshRenderer.material = nightMaterial;
        }
        else
        {
            meshRenderer.material = dayMaterial;
        }
    }
}
