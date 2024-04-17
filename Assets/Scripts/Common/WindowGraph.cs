using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowGraph : Singleton<WindowGraph> {

    [SerializeField]
    private Sprite AnswercircleSprite;
    [SerializeField]
    private Sprite AOIcircleSprite;
    [SerializeField]
    private Sprite LineSprite;
    [SerializeField]
    private Sprite BarSprite;
    /// <summary>
    /// EEG 범례?
    /// </summary>
    [SerializeField]
    private GameObject Legend2;
    public GameObject GetBioLegend { get { return Legend2; } }

    private Color AnswerColor = new Color(0.372f, 0.741f, 0.956f);
    private Color AOIColor = new Color(0.956f, 0.65f, 0.65f);
    private Color EEGColor = new Color(0.239f, 0.913f, 0.709f);

    [SerializeField]
    private Material dotMat;
    [SerializeField]
    private Material lineMat;
    private RectTransform graphContainer;
    private RectTransform mRact;

    private GameObject circleGameObject = null;
    private GameObject lastGO = null;

    private Vector2 dotSize = new Vector2(10, 10);
    private Vector2 AnswerPivot = new Vector2(0.5f, 0.5f);    
    private Vector2 AOIPivot = new Vector2(0.5f, 0.5f);
    

    private float width = 1200.0f;
    private float height = 495.0f;
    private float xSpacing = 100.0f;
    private float ySpacing = 20.0f;

    private float BarWidth = 35.0f;
    private float order = 40.0f;
    private float groupWidth = 85.0f;

    private List<Transform> AreaParentList = new List<Transform>();
    private List<Transform> CreateObjectList = new List<Transform>();
    
    protected override void Awake()
    {        
        base.Awake();        
        graphContainer = transform.Find("GraphContainer").GetComponent<RectTransform>();
        Transform temp = graphContainer.transform.Find("AreaList").transform;
        for(int i = 0; i < temp.childCount; i++)
        {
            AreaParentList.Add(temp.GetChild(i));
        }
        mRact = GetComponent<RectTransform>();
    }
    private void Start()
    {
        if (MainBioSignal.Inst == null) return;
        if (MainBioSignal.Inst.GetConnectMng.IsConnected())
            Legend2.SetActive(true);
        else
            Legend2.SetActive(false);

    }
    public void ClearGraph()
    {   
        for(int i = 0; i < CreateObjectList.Count; i++)
        {
            Destroy(CreateObjectList[i].gameObject);
        }
        CreateObjectList.Clear();
    }

    private Color SwitchColor(int index)
    {
        Color BarColor = new Color();
        switch (index)
        {
            case 0:
                BarColor = AnswerColor;
                break;
            case 1:
                BarColor = AOIColor;
                break;
            case 2:
                BarColor = EEGColor;
                break;
        }

        return BarColor;
    }

    public void CreateBarGraph(int index, List<float> valueList)
    {
        Color BarColor = new Color();
        float yMax = 100.0f;

        float gap = 0;

        BarColor = SwitchColor(index);

        for (int i = 0; i < valueList.Count; i++)
        {
            float xPos = 0.0f;
            float yPos = 0.0f;

            //xPos = (float)(i) / (float)(valueList.Count) * width + /*xSpacing*/ gap;
            yPos = (valueList[i] / yMax) * height;
            CreateBar(i, yPos, BarColor);
        }
    }

    //public void CreateBarGraph(int index, List<float> valueList)
    //{
    //    Color BarColor = new Color();
    //    float yMax = 100.0f;
        
    //    float gap = 0;

    //    if(index == 0)
    //    {
    //        BarColor = AnswerColor;
    //    }
    //    else if(index == 1)
    //    {
    //        BarColor = AOIColor;            
    //    }
    //    else if(index == 2)
    //    {
    //        BarColor = EEGColor;            
    //    }

    //    for (int i = 0; i < valueList.Count; i++)
    //    {
    //        float xPos = 0.0f;
    //        float yPos = 0.0f;

    //        //xPos = (float)(i) / (float)(valueList.Count) * width + /*xSpacing*/ gap;
    //        yPos = (valueList[i] / yMax) * height;
    //        CreateBar(index, yPos, i == 0 ? AnswerColor : i == 1 ? AOIColor : i == 2 ? EEGColor : Color.white);
    //    }
    //}

    private void CreateBar(int ParentIndex, float yHeight, Color color)
    {
        GameObject go = new GameObject("Bar", typeof(Image));
        go.transform.SetParent(AreaParentList[ParentIndex], false);
        go.GetComponent<Image>().sprite = BarSprite;
        go.GetComponent<Image>().color = color;
        RectTransform rtTr = go.GetComponent<RectTransform>();
        //rtTr.anchoredPosition = new Vector2(position.x, 5);
        rtTr.pivot = new Vector2(0.5f, 0);
        rtTr.sizeDelta = new Vector2(BarWidth, yHeight);
        go.AddComponent<CurvedUI.CurvedUIVertexEffect>();
        CreateObjectList.Add(go.transform);
    }

    //private void CreateBar(int ParentIndex, float yHeight, Color color)
    //{
    //    GameObject go = new GameObject("Bar", typeof(Image));
    //    go.transform.SetParent(AreaParentList[ParentIndex], false);
    //    go.GetComponent<Image>().sprite = BarSprite;
    //    go.GetComponent<Image>().color = color;
    //    RectTransform rtTr = go.GetComponent<RectTransform>();
    //    //rtTr.anchoredPosition = new Vector2(position.x, 5);
    //    rtTr.pivot = new Vector2(0.5f, 0);
    //    rtTr.sizeDelta = new Vector2(BarWidth, yHeight);
    //    go.AddComponent<CurvedUI.CurvedUIVertexEffect>();
    //    CreateObjectList.Add(go.transform);
    //}

    public void ShowAOIGraph(int index, List<float> valueList)
    {        
        float yMax = 100.0f;        
        lastGO = null;

        Color lineColor = SwitchColor(index);

        for (int i = 0; i < valueList.Count; i++)
        {
            //if (valueList[i].enterTime > totalTime)
            //    break;
            Debug.Log("Count : "+valueList.Count);
            Debug.Log("graphWidth: " + height);
            float xPos = 0.0f;
            float yPos = 0.0f;

            // 시작점
            xPos =(float)(i) / (float)(valueList.Count) * width + xSpacing;
            yPos = (valueList[i] / yMax) * height + ySpacing;
            Debug.Log("xPos : " + xPos);
            DrawDotAndLine(xPos, yPos, dotSize, lineColor, AOIcircleSprite, AOIPivot, AOIPivot);
        }
    }

    public void ShowGraph(int index, List<float> valueList)
    {        
        float yMax = 100.0f;
        lastGO = null;

        Color lineColor = SwitchColor(index);

        for (int i = 0; i < valueList.Count; i++)
        {
            float xPos = 0.0f;
            float yPos = 0.0f;

            xPos = (float)(i) / (float)(valueList.Count) * width + xSpacing;
            yPos = (valueList[i]/ yMax) * height + ySpacing;

            DrawDotAndLine(xPos, yPos, dotSize, lineColor, AnswercircleSprite, AnswerPivot, AnswerPivot);
        }
    }

    private GameObject CreateCircle(Vector2 anchorPos, Color color, Vector2 size, Sprite Circle, Vector2 pivot)
    {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = Circle;
        gameObject.GetComponent<Image>().color = color;
        //gameObject.GetComponent<Image>().material = dotMat;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchorPos;
        rectTransform.sizeDelta = size;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);// pivot;
        gameObject.AddComponent<CurvedUI.CurvedUIVertexEffect>();
        return gameObject;
    }

    private void DrawDotAndLine(float xPos, float yPos, Vector2 dotSize, Color color, Sprite Circle, Vector2 pivot, Vector2 LinePivot)
    {
        circleGameObject = CreateCircle(new Vector2(xPos, yPos), color, new Vector2(14, 14), Circle, pivot);
        if (lastGO != null)
        {
            CreateDotConnection(lastGO.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition, color, LinePivot);
        }
        lastGO = circleGameObject;
    }

    private void CreateDotConnection(Vector2 dotA, Vector2 dotB, Color lineColor, Vector2 pivot)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        //gameObject.GetComponent<Image>().material = lineMat;
        gameObject.GetComponent<Image>().color = lineColor;        
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotB - dotA).normalized;
        float dist = Vector2.Distance(dotB, dotA);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.pivot = pivot;
        rectTransform.sizeDelta = new Vector2(dist-10, 4.0f);
        rectTransform.anchoredPosition = dotA + dir * dist * 0.5f;
        float deltaX = dotB.x - dotA.x;
        float deltaY = dotB.y - dotA.y;
        float c = GetDegree(deltaX, deltaY);
        rectTransform.localEulerAngles = new Vector3(0,0,c);
        gameObject.AddComponent<CurvedUI.CurvedUIVertexEffect>();
    }

    private float GetDegree(float x, float y)
    {
        float angle = Mathf.Atan2(y, x) * 180 / 3.14f;
        if (angle < 0) angle += 360;
        return angle;
    }    
}
