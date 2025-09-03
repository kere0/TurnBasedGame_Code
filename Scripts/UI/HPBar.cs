using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    Slider slider;
    private Vector3 cameraDistance;
    //private Vector3 cameraPositionZ;
    private float cameraPositionX;
    private float cameraPositionZ;
    private void Awake()
    {
        transform.GetChild(0).TryGetComponent(out slider);
    }
    void Update()
    {
        Transform parent = gameObject.transform.parent;
         cameraDistance = Camera.main.transform.position - parent.position;
         cameraPositionX = cameraDistance.x;
         cameraPositionZ = cameraDistance.z;
        transform.position = parent.position + new Vector3(cameraPositionX/10 - cameraPositionZ/20, 0f, 0f) + Vector3.up * (parent.GetComponent<Collider>().bounds.size.y + 0.5f);
        transform.rotation = Camera.main.transform.rotation;
    }
    public void SetHpRatio(float ratio)
    {
        slider.value = ratio;
    }
}
