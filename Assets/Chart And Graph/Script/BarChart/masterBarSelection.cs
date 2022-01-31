#define Graph_And_Chart_PRO
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class masterBarSelection : MonoBehaviour
{
    List<customBarSelection> mSelections = new List<customBarSelection>();

    public void Register(customBarSelection sel)
    {
        mSelections.Add(sel);
    }

    public void Unregister(customBarSelection sel)
    {
        mSelections.Remove(sel);
    }

    public void ToogleBar(string category, string group)
    {
        foreach (customBarSelection s in mSelections)
            s.ToogleSelection(category, group);
    }
    public void SelectBar(string category, string group)
    {
        foreach(customBarSelection s in mSelections)
                   s.SetSelection(category, group,true);
    }

    public void DeselectBar(string category, string group)
    {
        foreach (customBarSelection s in mSelections)
            s.SetSelection(category, group, false);
    }
}
