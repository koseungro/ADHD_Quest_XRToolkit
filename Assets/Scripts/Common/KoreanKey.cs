using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class KoreanKey : MonoBehaviour {

    public ButtonFunc Btn;    
    [SerializeField]
    private Text input;
    public KoreanKeyBoard keyboard;
    private List<CreateButtonData> KeyData;
    private void Awake()
    {
        Btn = GetComponent<ButtonFunc>();        
    }

    public void InitFunc(Text inputfield, KoreanKeyBoard keyboard)
    {
        keyboard.LoadKeyBoardData();        
        keyboard.GetKeyData(ref Btn, name);
        keyboard.GetKorean(ref Btn, name);
        input = inputfield;
        this.keyboard = keyboard;
        Btn.AddPressedListener(InputText);
    }

    public void LoadKeyData(ShiftState isShift, KoreanState isKor)
    {
        if(isKor == KoreanState.KOR && isShift == ShiftState.ON)
        {
            UIManager.Inst.GetShiftWord(ref Btn, name);
        }
        else if(isKor == KoreanState.KOR && isShift == ShiftState.OFF)
        {
            UIManager.Inst.GetKorean(ref Btn, name);
        }
        else if (isKor == KoreanState.ENG && isShift == ShiftState.ON)
        {
            UIManager.Inst.GetShiftWord(ref Btn, name, false);
        }
        else if (isKor == KoreanState.ENG && isShift == ShiftState.OFF)
        {
            UIManager.Inst.GetEnglish(ref Btn, name);
        }

    }

    public void InputText()
    {
        Debug.Log("InputText");        
        Debug.Log(this.Btn.GetSetText);
        keyboard.Test(Btn.GetSetText, Btn.IsFunctionKey);
    }
}
