using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PracticeObject : MonoBehaviour {

    private UIAnimation mUIAni;
    private Image myImage;
    
    private Image FigureImage;
    public Image SetFigureImage
    {
        set { FigureImage = value; }
    }
    public Image GetFigureImage
    {
        get { return FigureImage; }
    }
    private Image mBorder;
    public Image GetBorder
    {
        get { return mBorder; }
    }
    private Image TitleImage;
    public Image GetTitleImage
    {
        get { return TitleImage; }
    }        
    //private Text mBorderText;

    /// <summary>
    /// 2-Back Sprite
    /// </summary>
    private Sprite NbackTextSprite;
    /// <summary>
    /// Current Sprite
    /// </summary>
    private Sprite CurrentTextSprite;

    private void Awake()
    {
        mUIAni = GetComponent<UIAnimation>();
        myImage = GetComponent<Image>();
        FigureImage = transform.Find("FigureImage").GetComponent<Image>();
        mBorder = transform.Find("BorderImage").GetComponent<Image>();
        TitleImage = transform.Find("TitleImage").GetComponent<Image>();
        //mBorderText = transform.Find("TitleImage/TitleText").GetComponent<Text>();

        NbackTextSprite = Resources.Load<Sprite>("UI/FocusGame/Tutorial/NbackTextImage");
        CurrentTextSprite = Resources.Load<Sprite>("UI/FocusGame/Tutorial/CurrentTextImage");

        SetTitleImageActive(false);
        SetBorderActive(false);
    }

    public void SetMyImageColor(Color color)
    {
        myImage.color = color;
    }
    public void SetMyFigureImageColor(Color color)
    {
        FigureImage.color = color;
    }
    public void SetTitleSprite(bool isNback)
    {
        if (isNback)
            TitleImage.sprite = NbackTextSprite;
        else
            TitleImage.sprite = CurrentTextSprite;
    }
    public void SetBorderActive(bool toggle)
    {
        mBorder.gameObject.SetActive(toggle);
    }
    public void SetTitleImageActive(bool toggle)
    {
        TitleImage.gameObject.SetActive(toggle);
    }
    public void UIMove(Transform target, Vector3 StartVec, Vector3 EndVec, float duration)
    {
        mUIAni.UIMove(target, StartVec, EndVec, duration);
    }
    public void UIFadeIn(Graphic target, double MilliSecDuration)
    {
        mUIAni.UIFadeIn(target, MilliSecDuration);
    }
    public void UIFadeOut(Graphic target, double MilliSecDuration, params UnityAction[] callback)
    {
        mUIAni.UIFadeOut(target, MilliSecDuration, callback);
    }
    public void UIBlink(List<Color> targetColor, List<Graphic> target)
    {
        mUIAni.UIBlink(target, targetColor);
    }
    public void UIStopBlink(Graphic target)
    {
        mUIAni.UIStopBlink(target);
    }
    public void UITransToTrans(Transform target, Vector3 startVec, Vector3 endVec, float Term)
    {
        mUIAni.UITransToTrans(target, startVec, endVec, Term);
    }
}
