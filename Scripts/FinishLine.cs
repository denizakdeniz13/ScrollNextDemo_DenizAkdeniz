using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<MainCharacterController>().Finish();
        }

        else if(other.gameObject.tag == "Enemy")
        {
            other.gameObject.GetComponent<EnemyAI>().DestroyObject();
        }
    }
}
