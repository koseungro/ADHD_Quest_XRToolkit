using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class KoreanKeyBoard : MonoBehaviour {


    public SceneBase Manager;
    public List<GameObject> OffObject = new List<GameObject>();
    public List<KoreanKey> KeyList = new List<KoreanKey>();
    public Text inputField;    
    
    public int limitLength = 5;
    public int limitMax = -1;
    public int limitMin = -1;
    private int BASE_CODE = 0xAC00;
    
    public List<char> inputChar = new List<char>();
    public List<int> combines = new List<int>();
    private List<char> first = new List<char>() { 'ㄱ', 'ㄲ', 'ㄴ', 'ㄷ', 'ㄸ', 'ㄹ', 'ㅁ', 'ㅂ', 'ㅃ', 'ㅅ', 'ㅆ', 'ㅇ', 'ㅈ', 'ㅉ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ' };
    private List<char> middle = new List<char>() { 'ㅏ', 'ㅐ', 'ㅑ', 'ㅒ', 'ㅓ', 'ㅔ', 'ㅕ', 'ㅖ', 'ㅗ', 'ㅘ', 'ㅙ', 'ㅚ', 'ㅛ', 'ㅜ', 'ㅝ', 'ㅞ', 'ㅟ', 'ㅠ', 'ㅡ', 'ㅢ', 'ㅣ' };
    private List<char> last = new List<char>() { ' ', 'ㄱ', 'ㄲ', 'ㄳ', 'ㄴ', 'ㄵ', 'ㄶ', 'ㄷ', 'ㄹ', 'ㄺ', 'ㄻ', 'ㄼ', 'ㄽ', 'ㄾ', 'ㄿ', 'ㅀ', 'ㅁ', 'ㅂ', 'ㅄ', 'ㅅ', 'ㅆ', 'ㅇ', 'ㅈ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ' };
    public List<KoreanKey> KorGOList = new List<KoreanKey>();
    public List<KoreanKey> NumberGOList = new List<KoreanKey>();

    public Word PreWord = new Word();
    public Word CurrentWord = new Word();
    public WordType wordType = WordType.NONE;
    public Finish IsComplete = Finish.NONE;

    public ShiftState isShift = ShiftState.OFF;
    public KoreanState isKor = KoreanState.KOR;

    private UnityAction callbackFunc;

    public List<CreateButtonData> KeyData = new List<CreateButtonData>();

    [System.Serializable]
    public struct Word
    {
        public WordType type;
        public Finish finish;

        public char Cho;
        public char juong;
        public char jong;
        public char InputChar;

        public void AllClearWord()
        {
            Cho = new char();
            juong = new char();
            jong = new char();
        }

    }
    public enum WordType
    {
        NONE,
        CHO,
        JUONG,
        JONG        
    }
    public enum Finish
    {
        NONE,
        FINISH
    }
    
    // Use this for initialization
    void Start () {        
        for (int i = 0; i < KeyList.Count; i++)
        {
            KeyList[i].InitFunc(inputField, this);
        }
    }
	public void SetManager(SceneBase mng, List<GameObject> offOBJ, UnityAction callback)
    {
        Manager = mng;
        callbackFunc = callback;
        OffObject = offOBJ;
        
    }
    public void LoadKeyBoardData()
    {
        CreateButtonData[] array = Resources.LoadAll<CreateButtonData>("ScriptableData/Button/SceneMainManager/KeyBoard/");
        for (int i = 0; i < array.Length; i++)
            KeyData.Add(array[i]);
    }
    public void GetKeyData(ref ButtonFunc btn, string name)
    {
        if (btn == null) return;
        CreateButtonData temp = KeyData.Find(x => x.name.Equals(name));

        btn.IsFunctionKey = temp.isFunctionKey;
        //btn.GetSetImage.sprite = temp.DefaultImage;
        //btn.DefaultSprite = temp.DefaultImage;
        //btn.PressedSprite = temp.PressedImage;
        //btn.HoverSprite = temp.HoverImage;
        btn.buttonType = temp.buttonType;
        btn.NewDefaultColor = temp.NewDefaultColor;
        btn.NewEnterColor = temp.NewEnterColor;
        btn.NewPressedColor = temp.NewPressedColor;
        btn.NewTextPressedColor = temp.NewTextPressedColor;
        btn.ChangeColor(btn.mSprite, temp.NewDefaultColor);
        btn.ChangeColor(btn.mText, Color.white);
    }
    public void GetKorean(ref ButtonFunc btn, string name)
    {
        if (btn == null) return;
        CreateButtonData temp = KeyData.Find(x => x.name.Equals(name));
        btn.GetSetText = temp.Korean;
    }
    public void GetEnglish(ref ButtonFunc btn, string name)
    {
        if (btn == null) return;
        CreateButtonData temp = KeyData.Find(x => x.name.Equals(name));
        btn.GetSetText = temp.English;
    }
    public void GetShiftWord(ref ButtonFunc btn, string name, bool isKor = true)
    {
        if (btn == null) return;
        CreateButtonData temp = KeyData.Find(x => x.name.Equals(name));
        if (isKor)
        {
            btn.GetSetText = temp.ShiftKOR;
        }
        else
        {
            btn.GetSetText = temp.ShiftENG;
        }
    }
    public void SetActive(List<KoreanKey> list, bool val)
    {
        int i = 0;
        for(i =0; i < list.Count; i++)
        {
            list[i].Btn.enabled = val;
            if (!val)
            {
                list[i].Btn.dummyInteractable = false;
                //list[i].Btn.ChangeAlpha(list[i].Btn.mSprite, 0.5f);
                //list[i].Btn.ChangeAlpha(list[i].Btn.mText, 0.5f);
            }
            else
            {
                list[i].Btn.dummyInteractable = true;
                //list[i].Btn.ChangeAlpha(list[i].Btn.mSprite, 1.0f);
                //list[i].Btn.ChangeAlpha(list[i].Btn.mText, 1.0f);
            }
        }
    }
    public void InitWord()
    {
        PreWord.type = WordType.NONE;
        PreWord.finish = Finish.NONE;
        PreWord.AllClearWord();
        CurrentWord.type = WordType.NONE;
        CurrentWord.finish = Finish.NONE;
        CurrentWord.AllClearWord();
    }
    public void SetLength()
    {
        if(limitLength < inputField.text.Length)
        {
            inputField.text.Remove(limitLength, inputField.text.Length - limitLength);
        }
    }
    public void SetLimitRange(int min, int Max)
    {
        limitMin = min;
        limitMax = Max;
    }
    public void SetLimitLength(int limit)
    {
        limitLength = limit;
    }
    public void LoadKeyData(ShiftState isShift, KoreanState isKor = KoreanState.KOR)
    {
        for (int i = 0; i < KeyList.Count; i++)
        {
            KeyList[i].LoadKeyData(isShift, isKor);
        }
    }
    public void Test(string text, bool isFunctionKey = false)
    {

        if(isFunctionKey)
        {
            if (text.Equals("Del"))
            {
                if (inputField.text.Length > 0)
                {
                    PreWord.AllClearWord();
                    PreWord.type = WordType.NONE;
                    PreWord.finish = Finish.NONE;
                    inputField.text = inputField.text.Remove(inputField.text.Length - 1);
                }
                return;
            }
            else if (text.Equals("Shift"))
            {
                Debug.Log("Input Shift");
                if (isShift == ShiftState.OFF)
                {
                    isShift = ShiftState.ON;
                    LoadKeyData(isShift, isKor);
                }
                else if (isShift == ShiftState.ON)
                {
                    isShift = ShiftState.OFF;
                    LoadKeyData(isShift, isKor);
                }
                return;
            }
            else if (text.Equals("ABC") || text.Equals("한글"))
            {
                Debug.Log("Input Trans");

                isShift = ShiftState.OFF;

                if (isKor == KoreanState.KOR)
                {
                    isKor = KoreanState.ENG;
                    LoadKeyData(isShift, isKor);
                }
                else if (isKor == KoreanState.ENG)
                {
                    isKor = KoreanState.KOR;
                    LoadKeyData(isShift, isKor);
                }
                return;
            }
            else if (text.Equals("Space"))
            {
                inputField.text += " ";
                PreWord.type = WordType.NONE;
                PreWord.finish = Finish.NONE;
                PreWord.AllClearWord();

            }
            else if(text.Equals("Enter"))
            {
                if(OffObject != null)
                {
                    for (int i = 0; i < OffObject.Count; i++)
                    {
                        Manager.SetActive(OffObject[i], true);
                    }
                    if(callbackFunc != null)
                        callbackFunc.Invoke();
                }
                gameObject.SetActive(false);
            }
            return;
        }
        else
        {
            if (inputField.text.Length >= limitLength)
            {
                return;
            }

            char[] CheckKor = text.ToCharArray();
            bool IsContain = false;

            for (int i = 0; i < CheckKor.Length; i++)
            {
                if (char.GetUnicodeCategory(CheckKor[i]) != System.Globalization.UnicodeCategory.OtherLetter)
                {
                    IsContain = true;
                    inputField.text += text;
                    break;
                }
            }

            // 입력된 값이 최소값보다 작거나 최대값보다 큰지 체크
            CheckRange();

            if (IsContain)
                return;

            List<char> list = Seper(text);

            if (IsComplete == Finish.NONE)
            {
                if (wordType == WordType.CHO)
                {
                    CurrentWord.type = WordType.CHO;
                    CurrentWord.Cho = list[0];
                }
                else if (wordType == WordType.JUONG)
                {
                    CurrentWord.type = WordType.JUONG;
                    CurrentWord.juong = list[0];
                }
                else if (wordType == WordType.JONG)
                {
                    CurrentWord.type = WordType.JONG;
                    CurrentWord.jong = list[0];
                }
                CurrentWord.InputChar = list[0];
            }
            else
            {
                CurrentWord.Cho = list[0];
                CurrentWord.juong = list[1];
                CurrentWord.jong = list[2];
                CurrentWord.InputChar = text.ToCharArray()[0];
            }

            string complete = combine2(PreWord, CurrentWord);

            if (complete.Length == 0)
            {

            }
            else
            {
                inputField.text = complete;
            }

            if (isShift == ShiftState.ON)
            {
                isShift = ShiftState.OFF;
                LoadKeyData(isShift, isKor);
            }
        }
    }

    public void CheckRange(bool ignore = false)
    {
        //Debug.Log(inputField.text);
        if(ignore)
        {
            if (limitMax != -1 && limitMin != -1)
            {
                if (inputField.text.Length > 0 && !string.IsNullOrEmpty(inputField.text))
                {
                    int inputNum = int.Parse(inputField.text);                    
                    if (inputNum > limitMax || inputNum < limitMin)
                    {
                        inputField.text = "";
                        return;
                    }
                }
                else if (string.IsNullOrEmpty(inputField.text))
                {
                    inputField.text = "";
                    return;
                }
            }
        }
        else
        {
            if (limitMax != -1 && limitMin != -1)
            {
                if (limitLength == inputField.text.Length)
                {
                    int inputNum = int.Parse(inputField.text);
                    if (inputNum > limitMax || inputNum < limitMin)
                    {
                        inputField.text = "";
                        return;
                    }
                }
            }
        }
    }

/*
    private void Seperating(string input)
    {
        combines.Clear();
        inputChar.Clear();
        char[] temp = input.ToCharArray();
        Debug.Log(temp.Length);

        for (int i = 0; i < temp.Length; i++)
        {
            int uniCode = temp[i] - BASE_CODE;
            Debug.Log(temp[i]);
            Debug.Log(uniCode);
            if (uniCode < 0)
            {
                if (isGetFirst(temp[i]))
                {
                    combines.Add(GetFirst(temp[i]));
                    
                }
                else if (isGetMiddle(temp[i]))
                {
                    combines.Add(GetMiddle(temp[i]));
                    
                }
                else if (isGetLast(temp[i]))
                {                    
                    combines.Add(GetLast(temp[i]));
                    
                }
                inputChar.Add(temp[i]);
            }
            else
            {
                Debug.Log((uniCode / 28) / 21);
                Debug.Log((uniCode / 28) % 21);
                Debug.Log(uniCode % 28);
                combines.Add((uniCode / 28) / 21);
                combines.Add((uniCode / 28) % 21);
                inputChar.Add(first[combines[0]]);
                inputChar.Add(middle[combines[1]]);

                if (uniCode % 28 == 0)
                {
                    if (temp[i + 1] != ' ')
                    {
                        Debug.Log(temp[i + 1]);
                        if (!isGetMiddle(temp[i + 1]) && isGetLast(temp[i + 1]))
                        {
                            Debug.Log("Next is Last");
                            Debug.Log(GetLast(temp[i + 1]));                            
                            combines.Add(GetLast(temp[i+1]));
                            inputChar.Add(last[combines[2]]);
                            break;
                        }
                    }
                }
                else
                {                    
                    combines.Add(uniCode % 28);
                    inputChar.Add(last[combines[2]]);
                }
                
                
                
            }
            
        }
    }
 */

    public List<char> Seper(string input)
    {
        //Debug.Log("Seper : " + input);
        char[] temp = input.ToCharArray();
        List<char> list = new List<char>();

        //Debug.Log((int)temp[0]);

        int uniCode = temp[0] - BASE_CODE;

        if (temp[0] - BASE_CODE < 0)
        {
            if (isGetFirst(temp[0]))
            {
                wordType = WordType.CHO;
                list.Add(temp[0]);
            }
            else if (isGetMiddle(temp[0]))
            {
                wordType = WordType.JUONG;
                list.Add(temp[0]);
            }
            else if (isGetLast(temp[0]))
            {
                wordType = WordType.JONG;
                list.Add(temp[0]);
            }
            IsComplete = Finish.NONE;
        }
        else
        {
            list.Add(GetFirst((uniCode / 28) / 21));
            list.Add(GetMiddle((uniCode / 28) % 21));
            list.Add(GetMiddle((uniCode % 28)));
            IsComplete = Finish.FINISH;
        }
        return list;
    }

    public List<char> Seper2(string input)
    {
        Debug.Log("Seper2 : " + input);
        char[] temp = input.ToCharArray();
        List<char> list = new List<char>();

        Debug.Log(temp[0]);

        int uniCode = temp[0] - BASE_CODE;

        Debug.Log(uniCode);

        if (temp[0] - BASE_CODE < 0)
        {
            if (isGetFirst(temp[0]))
            {                
                list.Add(temp[0]);
            }
            else if (isGetMiddle(temp[0]))
            {
                list.Add(temp[0]);
            }
            else if (isGetLast(temp[0]))
            {
                list.Add(temp[0]);
            }
        }
        else
        {
            Debug.Log(uniCode);
            list.Add(GetFirst((uniCode / 28) / 21));
            list.Add(GetMiddle((uniCode / 28) % 21));
            list.Add(GetLast((uniCode % 28)));            
        }
        return list;
    }

    public string combine2(Word preWord, Word CurrentWord)
    {
        string ch = "";
        if (preWord.type != WordType.NONE)
        {            
            // 이전 문자가 완성된 타입이냐
            if (preWord.finish == Finish.NONE)
            {
                // 이전 문자가 초성이면서 현재 입력된 문자가 중성이냐
                if(preWord.type == WordType.CHO && CurrentWord.type == WordType.JUONG)
                {
                    int cho = GetFirst(preWord.Cho);
                    int juong = GetMiddle(CurrentWord.juong);

                    PreWord.finish = Finish.FINISH;

                    ch = ((char)(BASE_CODE + cho * 21 * 28 + juong * 28 + 0)).ToString();
                    char ch2 = (char)(BASE_CODE + cho * 21 * 28 + juong * 28 + 0);

                    Debug.Log("ch  : " + ch);
                    Debug.Log("ch2 : " + ch2);

                    if (inputField.text.Length > 1)
                    {
                        Debug.Log("inputField : " + inputField.text);
                        Debug.Log("inputField Length : " + inputField.text.Length);
                        Debug.Log("inputField Length : " + inputField.text[inputField.text.Length - 1]);
                        Debug.Log("Replace : " + inputField.text.Replace(inputField.text[inputField.text.Length - 1], ch2));
                        string temp1 = inputField.text;
                        StringBuilder sb = new StringBuilder(temp1);
                        sb.Remove(sb.Length - 1, 1);
                        sb.Append(ch2);
                        inputField.text = sb.ToString();
                        //inputField.text = inputField.text.Replace(inputField.text[inputField.text.Length - 1], ch2);
                    }
                    else
                    {                        
                        inputField.text = ch;
                    }   

                    List<char> list = Seper2(ch.ToString());

                    PreWord.Cho = list[0];
                    PreWord.juong = list[1];
                    PreWord.jong = list[2];

                    return "";
                }
                // 이전 문자가 초성이면서 입력된 타입이 초성이냐
                else if (preWord.type == WordType.CHO && CurrentWord.type == WordType.CHO)
                {
                    PreWord.AllClearWord();
                    PreWord.finish = Finish.NONE;
                    PreWord.type = CurrentWord.type;

                    if (PreWord.type == WordType.CHO)
                        PreWord.Cho = CurrentWord.InputChar;
                    else if (PreWord.type == WordType.JUONG)
                        PreWord.juong = CurrentWord.InputChar;
                    else if (PreWord.type == WordType.JONG)
                        PreWord.jong = CurrentWord.InputChar;

                    inputField.text += CurrentWord.InputChar;

                    return "";
                }
                // 이전 문자 타입이 중성이냐
                else if(preWord.type == WordType.JUONG)
                {
                    PreWord.AllClearWord();
                    PreWord.finish = Finish.NONE;
                    PreWord.type = CurrentWord.type;

                    if (PreWord.type == WordType.CHO)
                        PreWord.Cho = CurrentWord.InputChar;
                    else if(PreWord.type == WordType.JUONG)
                        PreWord.juong = CurrentWord.InputChar;
                    else if (PreWord.type == WordType.JONG)
                        PreWord.jong = CurrentWord.InputChar;

                    inputField.text += CurrentWord.InputChar;

                    return "";
                }
                // 이전 문자 타입이 종성이냐
                else if (preWord.type == WordType.JONG)
                {
                    // 입력 문자가 중성이냐
                    if(CurrentWord.type == WordType.JUONG)
                    {
                        int cho = GetFirst(preWord.jong);
                        int juong = GetMiddle(CurrentWord.juong);
                        
                        PreWord.finish = Finish.FINISH;

                        ch = ((char)(BASE_CODE + cho * 21 * 28 + juong * 28 + 0)).ToString();
                        char ch2 = (char)(BASE_CODE + cho * 21 * 28 + juong * 28 + 0);

                        if (inputField.text.Length > 1)
                        {                            
                            string temp1 = inputField.text;
                            StringBuilder sb = new StringBuilder(temp1);
                            sb.Remove(sb.Length - 1, 1);
                            sb.Append(ch2);
                            inputField.text = sb.ToString();                         
                        }
                        else
                        {                            
                            inputField.text = ch;
                        }

                        List<char> list = Seper2(ch.ToString());

                        PreWord.Cho = list[0];
                        PreWord.juong = list[1];
                        PreWord.jong = list[2];

                        return "";

                    }
                    // 초성이냐 || 종성이냐
                    else if (CurrentWord.type == WordType.CHO || CurrentWord.type == WordType.CHO)
                    {
                        PreWord.AllClearWord();
                        PreWord.finish = Finish.NONE;
                        PreWord.type = CurrentWord.type;
                        inputField.text += CurrentWord.InputChar;
                    }                    
                    return "";
                }

            }
            else
            {
                // 받침이 없으며
                if(preWord.jong == ' ')
                {                    
                    // 입력된 타입이 초성이면
                    if(wordType == WordType.CHO)
                    {
                        Debug.Log(preWord.Cho);
                        Debug.Log(preWord.juong);
                        Debug.Log(CurrentWord.Cho);
                        int cho = GetFirst(preWord.Cho);
                        int juong = GetMiddle(preWord.juong);
                        int jong = GetLast(CurrentWord.Cho);
                        Debug.Log("jong : " + jong);

                        ch = ((char)(BASE_CODE + cho * 21 * 28 + juong * 28 + jong)).ToString();
                        char ch2 = (char)(BASE_CODE + cho * 21 * 28 + juong * 28 + jong);
                        if (inputField.text.Length > 1)
                        {
                            Debug.Log("ch  : " + ch);
                            Debug.Log("inputField : " + inputField.text);
                            Debug.Log("inputField Length : " + inputField.text.Length);
                            Debug.Log("inputField Length : " + inputField.text[inputField.text.Length-1]);
                            Debug.Log("ch2 : " + ch2);

                            if (jong == -1)
                            {
                                jong = 0;
                                
                                ch = CurrentWord.InputChar.ToString();
                                inputField.text += ch;

                                PreWord.AllClearWord();
                                PreWord.type = WordType.JONG;
                                PreWord.finish = Finish.NONE;
                                PreWord.jong = CurrentWord.InputChar;                                
                            }
                            else
                            {
                                string temp1 = inputField.text;
                                StringBuilder sb = new StringBuilder(temp1);
                                sb.Remove(sb.Length - 1, 1);
                                sb.Append(ch2);
                                inputField.text = sb.ToString();

                                List<char> list = Seper2(ch);

                                PreWord.Cho = list[0];
                                PreWord.juong = list[1];
                                PreWord.jong = list[2];                                
                            }
                            return "";
                        }
                        else
                        {   
                            Debug.Log("ch 0 : " + ch);
                            Debug.Log("inputField : " + inputField.text);

                            if (jong == -1)
                            {
                                jong = 0;
                                ch = ((char)(BASE_CODE + cho * 21 * 28 + juong * 28 + jong)).ToString();
                                inputField.text = ch;                                
                                ch = CurrentWord.InputChar.ToString();
                                inputField.text += ch;

                                PreWord.AllClearWord();
                                PreWord.type = WordType.JONG;
                                PreWord.finish = Finish.NONE;
                                PreWord.jong = CurrentWord.InputChar;
                                return "";
                            }
                            else
                            {
                                ch = ((char)(BASE_CODE + cho * 21 * 28 + juong * 28 + jong)).ToString();
                                inputField.text = ch;

                                List<char> list = Seper2(ch);

                                PreWord.Cho = list[0];
                                PreWord.juong = list[1];
                                PreWord.jong = list[2];
                                return "";
                            }
                        }
                        
                    }
                    // 입력된 타입이 중성이면
                    else if (wordType == WordType.JUONG)
                    {
                        int cho = GetFirst(preWord.Cho);
                        int juong = GetMiddle(preWord.juong);

                        if (preWord.juong == 'ㅗ')
                        {
                            if (CurrentWord.juong == 'ㅏ')
                            {
                                juong = 9;                                
                            }
                            else if (CurrentWord.juong == 'ㅐ')
                            {
                                juong = 10;
                            }
                            else if (CurrentWord.juong == 'ㅣ')
                            {
                                juong = 11;
                            }
                            else
                            {
                                PreWord.AllClearWord();

                                PreWord.finish = Finish.NONE;
                                PreWord.type = WordType.JUONG;
                                PreWord.juong = CurrentWord.InputChar;
                                inputField.text += CurrentWord.juong;
                                return "";
                            }
                        }
                        else if (preWord.juong == 'ㅜ')
                        {
                            if (CurrentWord.juong == 'ㅓ')
                            {
                                juong = 14;
                            }
                            else if (CurrentWord.juong == 'ㅔ')
                            {
                                juong = 15;
                            }
                            else if (CurrentWord.juong == 'ㅣ')
                            {
                                juong = 16;
                            }
                            else
                            {
                                PreWord.AllClearWord();

                                PreWord.finish = Finish.NONE;
                                PreWord.type = WordType.JUONG;
                                PreWord.juong = CurrentWord.InputChar;
                                inputField.text += CurrentWord.juong;
                                return "";
                            }
                        }
                        else if (preWord.juong == 'ㅡ')
                        {
                            if (CurrentWord.juong == 'ㅣ')
                            {
                                juong = 19;
                            }
                            else
                            {
                                PreWord.AllClearWord();

                                PreWord.finish = Finish.NONE;
                                PreWord.type = WordType.JUONG;
                                PreWord.juong = CurrentWord.InputChar;
                                inputField.text += CurrentWord.juong;
                                return "";
                            }
                        }
                        else
                        {
                            PreWord.AllClearWord();

                            PreWord.finish = Finish.NONE;
                            PreWord.type = WordType.JUONG;
                            PreWord.juong = CurrentWord.InputChar;
                            inputField.text += CurrentWord.juong;
                            return "";
                        }

                        ch = ((char)(BASE_CODE + cho * 21 * 28 + juong * 28 + 0)).ToString();
                        char ch2 = (char)(BASE_CODE + cho * 21 * 28 + juong * 28 + 0);

                        if (inputField.text.Length > 1)
                        {
                            string temp1 = inputField.text;
                            StringBuilder sb = new StringBuilder(temp1);
                            sb.Remove(sb.Length - 1, 1);
                            sb.Append(ch2);
                            inputField.text = sb.ToString();
                        }
                        else
                            inputField.text = ch;

                        PreWord.type = WordType.JUONG;
                        PreWord.finish = Finish.FINISH;
                        PreWord.juong = GetMiddle(juong);

                        return "";
                    }
                }
                // 받침이 있으며
                else
                {
                    // 입력된 타입이 초성이면
                    if(wordType == WordType.CHO)
                    {
                        
                        Debug.Log("ch 3 : " + ch);
                        Debug.Log("inputField : " + inputField.text);
                        
                        PreWord.AllClearWord();

                        PreWord.finish = Finish.NONE;
                        PreWord.type = WordType.CHO;
                        PreWord.Cho = CurrentWord.Cho;

                        inputField.text += CurrentWord.Cho;

                        return "";

                    }
                    // 입력된 타입이 종성이면
                    else if(wordType == WordType.JUONG)
                    {
                        int cho = GetFirst(preWord.Cho);
                        int juong = GetMiddle(preWord.juong);
                        int jong = 0;
                        ch = ((char)(BASE_CODE + cho * 21 * 28 + juong * 28 + jong)).ToString();
                        Debug.Log("ch 1 : " + ch);
                        Debug.Log("inputField : " + inputField.text);
                        if(inputField.text.Length > 1)
                        {
                            char ch2 = (char)(BASE_CODE + cho * 21 * 28 + juong * 28 + jong);
                            string temp1 = inputField.text;
                            StringBuilder sb = new StringBuilder(temp1);
                            sb.Remove(sb.Length - 1, 1);
                            sb.Append(ch2);
                            inputField.text = sb.ToString();                            
                        }
                        else
                        {
                            inputField.text = ch;
                        }

                        cho = GetFirst(preWord.jong);
                        juong = GetMiddle(CurrentWord.juong);
                        jong = 0;
                        ch = ((char)(BASE_CODE + cho * 21 * 28 + juong * 28 + jong)).ToString();
                        
                        Debug.Log("ch 2 : " + ch);
                        Debug.Log("inputField : " + inputField.text);
                        
                        inputField.text += ch;

                        List<char> list = Seper2(ch.ToString());

                        PreWord.Cho = list[0];
                        PreWord.juong = list[1];
                        PreWord.jong = list[2];
                        return "";
                    }
                }
            }
        }        
        else if (CurrentWord.type != WordType.NONE)
        {
            Debug.Log("1");
            if (CurrentWord.finish == Finish.NONE)
            {                
                if (CurrentWord.type == WordType.CHO)
                {
                    PreWord.type = WordType.CHO;
                    PreWord.Cho = CurrentWord.Cho;

                    if(inputField.text.Length > 0)
                    {
                        inputField.text += CurrentWord.Cho;
                        return "";
                    }
                    else
                    {
                        ch += CurrentWord.Cho;
                    }
                }
                else if (CurrentWord.type == WordType.JUONG)
                {
                    PreWord.type = WordType.JUONG;
                    PreWord.juong = CurrentWord.juong;
                    if (inputField.text.Length > 0)
                    {
                        inputField.text += CurrentWord.juong;
                        return "";
                    }
                    else
                    {
                        ch += CurrentWord.juong;
                    }
                }
                else if (CurrentWord.type == WordType.JONG)
                {
                    PreWord.type = WordType.JONG;
                    PreWord.jong = CurrentWord.jong;
                    if (inputField.text.Length > 0)
                    {
                        inputField.text += CurrentWord.jong;
                        return "";
                    }
                    else
                    {
                        ch += CurrentWord.jong;
                    }
                }
            }
        }

        return ch;
    }

    private int GetFirst(char ch)
    {
        int ret = -1;
        ret = first.FindIndex(x => x.Equals(ch));
        return ret;
    }

    private char GetFirst(int ch)
    {
        char ret = ' ';
        ret = first[ch];
        return ret;
    }
    private bool isGetFirst(char ch)
    {        
        bool ret = false;
        ret = first.Contains(ch);
        return ret;
    }
    private bool isGetFirst(int ch)
    {        
        bool ret = false;
        
        for(int i = 0; i < first.Count; i++)
        {
            if(first[i].Equals(first[ch]))
            {
                ret = true;
                break;
            }
        }

        return ret;
    }

    private int GetMiddle(char ch)
    {
        int ret = -1;
        ret = middle.FindIndex(x => x.Equals(ch));
        return ret;
    }
    private char GetMiddle(int ch)
    {
        char ret = ' ';
        ret = middle[ch];
        return ret;
    }
    private bool isGetMiddle(char ch)
    {
        bool ret = false;
        ret = middle.Contains(ch);
        return ret;
    }
    private bool isGetMiddle(int ch)
    {
        bool ret = false;
        //ret = middle.Contains(middle[ch]);
        for (int i = 0; i < middle.Count; i++)
        {
            if (middle[i].Equals(middle[ch]))
            {
                ret = true;
                break;
            }
        }
        return ret;
    }

    private int GetLast(char ch)
    {
        int ret = 0;
        ret = last.FindIndex(x => x.Equals(ch));
        return ret;
    }
    private char GetLast(int ch)
    {
        char ret = ' ';
        ret = last[ch];
        return ret;
    }
    private bool isGetLast(char ch)
    {
        bool ret = false;
        ret = last.Contains(ch);
        return ret;
    }
    private bool isGetLast(int ch)
    {
        bool ret = false;
        //ret = last.Contains(last[ch]);
        for (int i = 0; i < last.Count; i++)
        {
            if (last[i].Equals(last[ch]))
            {
                ret = true;
                break;
            }
        }
        return ret;
    }

}