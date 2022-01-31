#define Graph_And_Chart_PRO
using ChartAndGraph;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class customBarSelection : MonoBehaviour
{
    void Start()
    {
        try
        {
            //register with MasterBarRotation when started
            GetComponentInParent<masterBarSelection>().Register(this);
        }
        catch(Exception)
        {

        }
    }

    void OnDestroy()
    {
        try
        {
            //unregister when destroyed
            GetComponentInParent<masterBarSelection>().Unregister(this);
        }
        catch (Exception)
        {
        }
    }

    public void ToogleSelection(string category, string group)
    {
        // get the bar info of the prefab
        var barInfo = GetComponent<BarInfo>();
        // check for a category match
        if (barInfo.Category != category)
            return;
        //check for a group match
        if (barInfo.Group != group)
            return;

        // get the bar info of the prefab
        var control = GetComponent<ChartMaterialController>();
        control.Selected = !control.Selected;
        control.Refresh();
    }
    // when rotate is called . a check is performed to see if this bar is the bar that should be rotated
    public void SetSelection(string category, string group,bool selected)
    {
        // get the bar info of the prefab
        var barInfo = GetComponent<BarInfo>();
        // check for a category match
        if (barInfo.Category != category)
            return;
        //check for a group match
        if (barInfo.Group != group)
            return;

        // get the bar info of the prefab
        var control = GetComponent<ChartMaterialController>();
        control.Selected = selected;
        control.Refresh();
    }
}
