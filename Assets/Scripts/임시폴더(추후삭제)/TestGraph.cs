using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGraph : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
        DataManager.Inst.Player.Name = "김기철";

        FocusData data = new FocusData("TEST", LevelType.CAFE_LEVEL0);

        for (int i = 0; i < 360; i++)
        {
            data.ValueAdd(Random.Range(0,2), Random.Range(0,2), FocusState.RIGHT, i, Random.Range(0, 600));
        }
        DataManager.Inst.AddFocusData(data);
        //DataManager.Inst.saveCSV.CreateBP_CTP(1);

        Debug.Log(System.DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss"));
        List<float> list1 = new List<float>();
        List<float> list2 = new List<float>();
        List<float> list3 = new List<float>();
        List<float> list4 = new List<float>();
        List<float> list5 = new List<float>();

        for (int i = 0; i < 3; i++)
        {
            list1.Add(Random.Range(0, 101));
            Debug.Log(list1[i]);
        }

        for (int i = 0; i < 3; i++)
        {
            list2.Add(Random.Range(0, 101));
            Debug.Log(list2[i]);
        }

        for (int i = 0; i < 3; i++)
        {
            list3.Add(Random.Range(0, 101));
            Debug.Log(list3[i]);
        }
        for (int i = 0; i < 3; i++)
        {
            list4.Add(Random.Range(0, 101));
            Debug.Log(list4[i]);
        }

        for (int i = 0; i < 3; i++)
        {
            list5.Add(Random.Range(0, 101));
            Debug.Log(list5[i]);
        }

        WindowGraph.Inst.CreateBarGraph(0,list1);
        WindowGraph.Inst.CreateBarGraph(1, list2);
        WindowGraph.Inst.CreateBarGraph(2, list3);
        WindowGraph.Inst.ShowGraph(0, list1);
        WindowGraph.Inst.ShowGraph(1, list2);
        WindowGraph.Inst.ShowGraph(2, list3);
        //WindowGraph.Inst.CreateBarGraph(4, list4);
        //WindowGraph.Inst.CreateBarGraph(4, list5);
        //CreateRandomSequence();
    }

    public void CreateRandomSequence()
    {
        int a = 1;
        int b = 1;
        int c = 1;
        int d = 1;

        int aR = UnityEngine.Random.Range(1, 8);
        int bR = UnityEngine.Random.Range(1, 11 - aR - c - d);
        int cR = UnityEngine.Random.Range(1, 11 - aR - bR - d);
        int dR = (10 - aR - bR - cR);
        int total = (aR*36) + (bR*36) + (cR*36) + (dR*36);
        Debug.Log(aR + " " + bR + " " + cR + " " + dR + " = " + total);
        Debug.Log(System.DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss"));
    }

}
