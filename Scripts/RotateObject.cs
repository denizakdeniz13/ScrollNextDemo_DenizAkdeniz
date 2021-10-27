using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObstacleType
{
    Static,
    Moving,
}
public class RotateObject : MonoBehaviour
{
    [SerializeField] private ObstacleType obstacleType;
    [Space]
    [SerializeField] private float rotatingSpeed;
    [SerializeField] private float movingSpeed;
    [SerializeField] private float borderMinX,borderMaxX;

    private Vector3 targetPositionMin,targetPositionMax;

    private bool toLeft;
    void Start()
    {
        targetPositionMin = new Vector3(borderMinX, this.transform.position.y, this.transform.position.z);
        targetPositionMax = new Vector3(borderMaxX, this.transform.position.y, this.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * rotatingSpeed * Time.deltaTime, Space.Self);

        if(obstacleType == ObstacleType.Moving)
        {
            if(toLeft == false)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPositionMax, movingSpeed * Time.deltaTime);
                if (transform.position.x >= borderMaxX)
                    toLeft = true;
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPositionMin, movingSpeed * Time.deltaTime);
                if (transform.position.x <= borderMinX)
                    toLeft = false;
            }

        }
    }


}
