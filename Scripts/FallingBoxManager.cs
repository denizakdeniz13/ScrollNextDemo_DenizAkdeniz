using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBoxManager : MonoBehaviour
{
    public List<Line> lines = new List<Line>();

    private Line[] tempLines;

    private void OnEnable()
    {
        tempLines = GetComponentsInChildren<Line>();
        AddToGenericList();
    }

    private void AddToGenericList()
    {
        foreach(Line line in tempLines)
        {
            lines.Add(line);
        }
    }

    private void OnDisable()
    {
        lines.Clear();
    }

}
