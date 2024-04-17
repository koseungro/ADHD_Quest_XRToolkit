using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
[CreateAssetMenu(fileName = "New Button Data", menuName = "Stress Manager/Button Data")]
public class IS_ButtonData : ScriptableObject
{
    [Header("Property")]
    public bool IsChangeText = true;
    public bool IsFlexibility = true;
    public int FontSize = 36;
    [Header("Text Color")]
    public Color DefaultTextColor = Color.white;
    public Color PressTextColor = Color.white;
    public Color HoverTextColor = Color.black;
    public Color DisableTextColor = Color.gray;
    [Header("Button Image")]
    public Sprite Default;
    public Sprite Press;
    public Sprite Disable;
}
