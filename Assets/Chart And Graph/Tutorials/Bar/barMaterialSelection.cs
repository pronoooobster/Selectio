#define Graph_And_Chart_PRO
using ChartAndGraph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class barMaterialSelection : MonoBehaviour
{
    public BarChart bar;
    // Start is called before the first frame update
    void Start()
    {
        bar.BarClicked.AddListener(BarClicked);
    }

    void BarClicked(BarChart.BarEventArgs args)
    {
        var sel = bar.GetComponent<masterBarSelection>();
        if (sel != null)
        {
            sel.ToogleBar(args.Category, args.Group);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
