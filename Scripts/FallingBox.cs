using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBox : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] Material redMaterial;

    private MeshRenderer meshRenderer;

    public bool isFall;

    private void OnEnable()
    {
        rb = this.GetComponent<Rigidbody>();
        rb.useGravity = false;

        meshRenderer = this.GetComponent<MeshRenderer>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

    }

    private void Start()
    {

        if (meshRenderer.enabled == false)
            StartCoroutine(FallBox());
        else
            isFall = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            StartCoroutine(FallBox());
        }
    }
    private IEnumerator FallBox()
    {
        isFall = true;
        yield return new WaitForSeconds(0.1f);
        this.GetComponent<MeshRenderer>().material = redMaterial;
        yield return new WaitForSeconds(0.1f);
        rb.useGravity = true;
 

    }


}
