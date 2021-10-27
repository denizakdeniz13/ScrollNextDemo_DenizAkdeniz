using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AddForce : MonoBehaviour
{
    [SerializeField] private float forceMagnitude;

    private Rigidbody rb;




    private void OnCollisionEnter(Collision collision)
    {
        rb = collision.gameObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 forceDirection = (collision.gameObject.transform.position - transform.position);
            forceDirection.y = 0;
            forceDirection.Normalize();

            rb.AddForceAtPosition(forceDirection * forceMagnitude,new Vector3(transform.position.x,0,transform.position.z),ForceMode.Force );
        }
    }

}
