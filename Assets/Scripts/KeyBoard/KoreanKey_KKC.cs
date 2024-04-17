using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KoreanKeyBoard_KKC
{
    public class KoreanKey_KKC : MonoBehaviour
    {

        public ButtonFunc Btn;
        [SerializeField]
        private Text input;
        private KoreanKeyBoard_KKC keyboard;
        private List<CreateButtonData> KeyData;
        private void Awake()
        {
            Btn = GetComponent<ButtonFunc>();
        }

        public void InitFunc(Text inputfield, KoreanKeyBoard_KKC keyboard)
        {
            keyboard.LoadKeyBoardData();
            keyboard.GetKeyData(ref Btn, name);
            keyboard.GetKorean(ref Btn, name);
            input = inputfield;
            this.keyboard = keyboard;
            Btn.AddPressedListener(delegate { InputText(); });
        }

        public void LoadKeyData(ShiftState_KKC isShift, KoreanState_KKC isKor)
        {
            if (isKor == KoreanState_KKC.KOR && isShift == ShiftState_KKC.ON)
            {
                keyboard.GetShiftWord(ref Btn, name);
            }
            else if (isKor == KoreanState_KKC.KOR && isShift == ShiftState_KKC.OFF)
            {
                keyboard.GetKorean(ref Btn, name);
            }
            else if (isKor == KoreanState_KKC.ENG && isShift == ShiftState_KKC.ON)
            {
                keyboard.GetShiftWord(ref Btn, name, false);
            }
            else if (isKor == KoreanState_KKC.ENG && isShift == ShiftState_KKC.OFF)
            {
                keyboard.GetEnglish(ref Btn, name);
            }

        }

        public void InputText()
        {
            keyboard.Test(Btn.GetSetText, Btn.IsFunctionKey);
        }
    }

}