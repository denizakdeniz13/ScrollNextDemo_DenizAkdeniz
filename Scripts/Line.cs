using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    public FallingBox[] line;

    private void OnValidate()
    {
        line = GetComponentsInChildren<FallingBox>();
    }
}
