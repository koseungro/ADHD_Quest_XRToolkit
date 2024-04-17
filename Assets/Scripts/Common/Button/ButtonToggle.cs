using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ToggleType
{
    NONE,
    TYPE1
}

public class ButtonToggle : Selectable {
    [SerializeField]
    public ToggleType ToggleStyle = ToggleType.NONE;

    [SerializeField]
    private List<ButtonToggle> m_GroupToggleList;
    [SerializeField]
    private Sprite m_NormalSprite;
    [SerializeField]
    private Sprite m_HoverSprite;
    [SerializeField]
    private Sprite m_PressedSprite;
    [SerializeField]
    private Color m_NormalColor = Color.white;
    [SerializeField]
    private Color m_HoverColor = Color.white;
    [SerializeField]
    private Color m_PressedColor = Color.white;

    /// <summary>
    /// Toggle시 변경할 아이콘 이미지
    /// </summary>
    private Image m_TargetImage;
    /// <summary>
    /// Toggle시 변경할 텍스트UI
    /// </summary>
    [SerializeField]
    private Text m_TargetText;
    [SerializeField]
    private string m_TargetTextStrON;
    [SerializeField]
    private string m_TargetTextStrOFF;

    private bool isToggleON = false;

    protected override void OnDisable()
    {
        if (ToggleStyle == ToggleType.NONE)
        {
            isToggleON = false;
            targetGraphic.color = m_NormalColor;
        }

        if(m_TargetImage != null)
            if(m_NormalSprite != null)
                m_TargetImage.sprite = m_NormalSprite;
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();
        if (transform.Find("Icon") != null)
        {
            m_TargetImage = transform.Find("Icon").GetComponent<Image>();
            m_TargetImage.sprite = m_NormalSprite;
            targetGraphic = m_TargetImage;            
        }
        else
        {
            targetGraphic = transform.Find("Text").GetComponent<Text>();
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        targetGraphic.color = m_PressedColor;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        if(ToggleStyle == ToggleType.NONE)
        {
            targetGraphic.color = m_NormalColor;

            if (m_PressedSprite != null)
            {
                if (isToggleON)
                {
                    ChangeGroupToggleListColor();
                }
                else
                {
                    m_TargetImage.sprite = m_PressedSprite;
                }
            }
            if (m_TargetText != null)
            {
                if (!string.IsNullOrEmpty((m_TargetText.text)))
                {
                    if (isToggleON)
                        m_TargetText.text = m_TargetTextStrON;
                    else
                        m_TargetText.text = m_TargetTextStrOFF;
                }
            }
            isToggleON = !isToggleON;
        }
        else
        {
            for (int i = 0; i < m_GroupToggleList.Count; i++)
            {
                m_GroupToggleList[i].isToggleON = false;
                m_GroupToggleList[i].targetGraphic.color = m_NormalColor;
            }
            targetGraphic.color = m_PressedColor;
            isToggleON = true;
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }
    /// <summary>
    /// 그룹안에 있는 버튼들 색상 바꾸기
    /// </summary>
    public void ChangeGroupToggleListColor()
    {
        for (int i = 0; i < m_GroupToggleList.Count; i++)
        {
            m_GroupToggleList[i].isToggleON = false;
            m_GroupToggleList[i].targetGraphic.color = m_NormalColor;
        }
        m_TargetImage.sprite = m_NormalSprite;
        targetGraphic.color = m_PressedColor;        
    }

    public void ChangeColor(Text target, Color color)
    {
        if (target != null)
            target.color = color;
    }
    public void ChangeColor(Image target, Color color)
    {
        if (target != null)
            target.color = color;
    }
    public void ChangeColor(Text target, float r, float g, float b, float a)
    {
        if (target != null)
            target.color = new Color(r, g, b, a);
    }
}
