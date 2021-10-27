using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target = null;
    [SerializeField] private float cameraSpeed = 1f;

    private Vector3 offset;

    void Start()
    {
        offset = transform.position - target.position;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(target.position.x/2, target.position.y , target.position.z) + offset, Time.deltaTime * cameraSpeed );
    }

}
