using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
[CreateAssetMenu(menuName = " CreateButtonData / Button Data ", fileName = "NewButtonData")]
public class CreateButtonData : ScriptableObject {

    public BUTTON_TYPE buttonType = BUTTON_TYPE.NONE;

    //[Header("Property")]
    //public bool IsChangeText = true;
    //public bool IsFlexibility = true;
    //public int FontSize = 36;
    //[Header("Text Color")]
    //public Color DefaultTextColor = Color.white;
    //public Color PressTextColor = Color.white;
    //public Color HoverTextColor = Color.black;
    //public Color DisableTextColor = Color.gray;
    [Header("Button Image")]
    public Sprite DefaultImage;
    public Sprite PressedImage;
    public Sprite HoverImage;
    public Sprite DisableImage;

    [Header("New Color")]
    public Color NewDefaultColor = new Color();
    public Color NewEnterColor = new Color();
    public Color NewPressedColor = new Color();

    [Header("New Text Pressed Color")]
    public Color NewTextDefaultColor = new Color();
    public Color NewTextEnterColor = new Color();
    public Color NewTextPressedColor = new Color();

    //public string ButtonName = "";
    public string ButtonText = "";
    public string SceneName = "";
    public int SceneNumber = -1;    
    public bool isScaleChange = false;
    public bool isKeyBoardKey = false;
    public string Korean = "";
    public string English = "";
    public string ShiftKOR = "";
    public string ShiftENG = "";
    public bool isFunctionKey = false;
}
