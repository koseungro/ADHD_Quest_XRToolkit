using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class KeyBoardInput : MonoBehaviour/*, IPointerUpHandler, IPointerClickHandler*/
{
    public SceneBase Manager;
    public KoreanKeyBoard inputKeyBoard;
    public GameObject KeyBoardGO;
    public List<GameObject> OffObject = new List<GameObject>();
    public List<ButtonFunc> InputFieldList = new List<ButtonFunc>();
    public List<ButtonFunc> ToggleList = new List<ButtonFunc>();    
    public int Gender = -1;

    //public Button OutField;

    private void Start()
    {
        UIManager.Inst.LoadButtonData(string.Format("{0}", "Intro_Input"), string.Format("{0}", "SceneMainManager" + "/Intro_Input/"));
        UIManager.Inst.SettingButtonData("Intro_Input", InputFieldList[0], InputFieldList[1], InputFieldList[2], InputFieldList[3], InputFieldList[4], ToggleList[0], ToggleList[1]);
        inputKeyBoard.SetManager(Manager, OffObject, delegate { KeyboardEnterClick(); });
        
        InputFieldList[0].AddPressedListener(OnPointerClick);
        InputFieldList[1].AddPressedListener(OnPointerClick1);
        InputFieldList[2].AddPressedListener(OnPointerClick2);
        InputFieldList[3].AddPressedListener(OnPointerClick3);
        InputFieldList[4].AddPressedListener(OnOutFieldClick);

        ToggleList[0].AddTogglePressedListener(OnToggleClick);
        ToggleList[1].AddTogglePressedListener(OnToggleClick);
        //ToggleList[0].AddPressedListener(delegate { OnToggleClick(ToggleList[0], 0); });
        //ToggleList[1].AddPressedListener(delegate { OnToggleClick(ToggleList[1], 1); });

        ToggleList[0].AddEnterActionListener(delegate { OnToggleHover(ToggleList[0]); });
        ToggleList[1].AddEnterActionListener(delegate { OnToggleHover(ToggleList[1]); });
        ToggleList[0].AddExitActionListener(delegate { OnToggleExit(ToggleList[0]); });
        ToggleList[1].AddExitActionListener(delegate { OnToggleExit(ToggleList[1]); });

        for (int i = 0; i < InputFieldList.Count; i++)
        {
            //InputFieldList[i].ChangeAlpha(InputFieldList[i].mSprite, 0.5f);
            InputFieldList[i].mText = InputFieldList[i].transform.Find("Text").GetComponent<Text>();
        }
        for (int i = 0; i < ToggleList.Count; i++)
        {
            ToggleList[i].ToggleList = ToggleList;
            ToggleList[i].ToggleIndex = i;
            ToggleList[i].GetSetToggle = true;
            ToggleList[i].mText = ToggleList[i].transform.Find("Label").GetComponent<Text>();
            //ToggleList[i].ChangeAlpha(ToggleList[i].mSprite, 0.5f);
            //ToggleList[i].ChangeAlpha(ToggleList[i].mText, 0.5f);
        }
    }

    private void SelectedBtn(int index)
    {
        for(int i = 0; i < InputFieldList.Count-1; i++)
        {
            if (i == index)
            {
                InputFieldList[i].transform.Find("Image").gameObject.SetActive(true);
                InputFieldList[i].transform.Find("Placeholder").gameObject.SetActive(false);
                //InputFieldList[i].ChangeAlpha(InputFieldList[i].mSprite, 1.0f);                
            }                
            else
            {
                InputFieldList[i].transform.Find("Image").gameObject.SetActive(false);
                //InputFieldList[i].ChangeAlpha(InputFieldList[i].mSprite, 0.5f);
                if (InputFieldList[i].transform.Find("Text").GetComponent<Text>().text.Length > 0 )                
                    InputFieldList[i].transform.Find("Placeholder").gameObject.SetActive(false);                
                else                
                    InputFieldList[i].transform.Find("Placeholder").gameObject.SetActive(true);                
                
            }
        }
    }

    public void OnToggleClick(int index)
    {
        Debug.Log("index : " + index);
        for (int i = 0; i < ToggleList.Count; i++)
        {
            if (i == index)
            {
                Debug.Log("same");
                ToggleList[i].mSprite.color = ToggleList[index].NewPressedColor;
                ToggleList[i].mText.color = ToggleList[index].NewTextPressedColor;
                //ToggleList[index].ChangeAlpha(ToggleList[index].mSprite, 1.0f);
                //Gender = i;
            }
            else
            {
                Debug.Log("not same");
                ToggleList[i].IsSelected = false;
                ToggleList[i].mSprite.color =  ToggleList[index].NewDefaultColor;
                ToggleList[i].mText.color = Color.white;
                //ToggleList[i].ChangeAlpha(ToggleList[i].mText, 0.5f);
                //ToggleList[i].ChangeAlpha(ToggleList[i].mSprite, 0.5f);
            }
        }
    }

    //private void OnToggleClick(List<ButtonFunc> btn, int index)
    //{
    //    btn.mSprite.sprite = btn.PressedSprite;
    //    btn.ChangeColor(btn.mText, btn.mColor);
    //    btn.ChangeAlpha(btn.mSprite, 1.0f);
    //    for (int i = 0; i < ToggleList.Count; i++)
    //    {
    //        if (i == index)
    //        {
    //            Gender = i;
    //        }
    //        else
    //        {                
    //            ToggleList[i].IsSelected = false;
    //            ToggleList[i].mSprite.sprite = btn.DefaultSprite;
    //            ToggleList[i].ChangeColor(ToggleList[i].mText, Color.white);
    //            ToggleList[i].ChangeAlpha(ToggleList[i].mText, 0.5f);
    //            ToggleList[i].ChangeAlpha(ToggleList[i].mSprite, 0.5f);
    //        }
    //    }
    //}
    public void OnToggleHover(ButtonFunc btn)
    {
        if (!btn.IsSelected)
        {
            btn.ChangeColor(btn.mText, Color.white);
            btn.ChangeColor(btn.mSprite, btn.NewEnterColor);
        }
    }
    public void OnToggleExit(ButtonFunc btn)
    {
        if(!btn.IsSelected)
        {
            btn.ChangeColor(btn.mText, Color.white);
            btn.ChangeColor(btn.mSprite, btn.NewDefaultColor);
        }
    }
    public void OnPointerClick()
    {
        if (inputKeyBoard.inputField != null)
        {
            inputKeyBoard.CheckRange(true);
        }
        SelectedBtn(0);        
        inputKeyBoard.inputField = InputFieldList[0].transform.Find("Text").GetComponent<Text>();
        inputKeyBoard.InitWord();
        inputKeyBoard.SetLimitLength(5);
        inputKeyBoard.SetLimitRange(-1, -1);
        KeyBoardGO.SetActive(true);
        inputKeyBoard.SetActive(inputKeyBoard.NumberGOList, false);
        inputKeyBoard.SetActive(inputKeyBoard.KorGOList, true);
        SetActive(false, OffObject.ToArray());
    }

    public void OnPointerClick1()
    {
        if (inputKeyBoard.inputField != null)
        {
            inputKeyBoard.CheckRange(true);
        }
        SelectedBtn(1);        
        inputKeyBoard.inputField = InputFieldList[1].transform.Find("Text").GetComponent<Text>();
        inputKeyBoard.InitWord();
        inputKeyBoard.SetLimitLength(4);
        inputKeyBoard.SetLimitRange(1900, System.DateTime.Now.Year);        
        KeyBoardGO.SetActive(true);
        inputKeyBoard.SetActive(inputKeyBoard.KorGOList, false);
        inputKeyBoard.SetActive(inputKeyBoard.NumberGOList, true);
        SetActive(false, OffObject.ToArray());
    }

    public void OnPointerClick2()
    {
        if (inputKeyBoard.inputField != null)
        {
            inputKeyBoard.CheckRange(true);
        }
        SelectedBtn(2);
        inputKeyBoard.inputField = InputFieldList[2].transform.Find("Text").GetComponent<Text>();
        inputKeyBoard.InitWord();
        inputKeyBoard.SetLimitLength(2);
        inputKeyBoard.SetLimitRange(1, 12);
        KeyBoardGO.SetActive(true);
        inputKeyBoard.SetActive(inputKeyBoard.KorGOList, false);
        inputKeyBoard.SetActive(inputKeyBoard.NumberGOList, true);
        SetActive(false, OffObject.ToArray());
    }
    public void OnPointerClick3()
    {
        if (inputKeyBoard.inputField != null)
        {
            inputKeyBoard.CheckRange(true);
        }
        SelectedBtn(3);        
        inputKeyBoard.inputField = InputFieldList[3].transform.Find("Text").GetComponent<Text>();
        inputKeyBoard.InitWord();
        inputKeyBoard.SetLimitLength(2);
        inputKeyBoard.SetLimitRange(1, 31);
        KeyBoardGO.SetActive(true);
        inputKeyBoard.SetActive(inputKeyBoard.KorGOList, false);
        inputKeyBoard.SetActive(inputKeyBoard.NumberGOList, true);
        SetActive(false, OffObject.ToArray());
    }
    public void OnKeyFieldClick()
    {

    }

    private void KeyboardEnterClick()
    {
        if (inputKeyBoard.inputField != null)
        {
            inputKeyBoard.CheckRange(true);
        }
        SelectedBtn(-1);        
        inputKeyBoard.InitWord();
        inputKeyBoard.inputField = null;        
    }

    public void OnOutFieldClick()
    {
        Debug.Log("OnOutFieldClick");
        if (inputKeyBoard.inputField != null)
        {
            inputKeyBoard.CheckRange(true);
        }
        SelectedBtn(-1);        
        inputKeyBoard.InitWord();
        inputKeyBoard.inputField = null;
        KeyBoardGO.SetActive(false);
        SetActive(true, OffObject.ToArray());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnPointerUp");
        GameObject go = eventData.pointerCurrentRaycast.gameObject;
        Debug.Log(go.tag);

        // 이름 입력 클릭 시
        if (go.CompareTag("InputField"))
        {
            inputKeyBoard.inputField = go.GetComponent<Text>();
            inputKeyBoard.InitWord();
            inputKeyBoard.SetLimitLength(5);
            KeyBoardGO.SetActive(true);
            inputKeyBoard.SetActive(inputKeyBoard.NumberGOList, false);
            inputKeyBoard.SetActive(inputKeyBoard.KorGOList, true);
            SetActive(false, OffObject.ToArray());
        }
        else if (go.CompareTag("InputField1"))
        {
            inputKeyBoard.inputField = go.GetComponent<Text>();
            inputKeyBoard.InitWord();
            inputKeyBoard.SetLimitLength(4);
            KeyBoardGO.SetActive(true);
            inputKeyBoard.SetActive(inputKeyBoard.KorGOList, false);
            inputKeyBoard.SetActive(inputKeyBoard.NumberGOList, true);
            SetActive(false, OffObject.ToArray());
        }
        else if (go.CompareTag("InputField2"))
        {
            inputKeyBoard.inputField = go.GetComponent<Text>();
            inputKeyBoard.InitWord();
            inputKeyBoard.SetLimitLength(2);
            KeyBoardGO.SetActive(true);
            inputKeyBoard.SetActive(inputKeyBoard.KorGOList, false);
            inputKeyBoard.SetActive(inputKeyBoard.NumberGOList, true);
            SetActive(false, OffObject.ToArray());
            //OffObject.SetActive(false);
        }
        else if (go.CompareTag("KeyField"))
        {
            
        }
        else
        {
            inputKeyBoard.InitWord();
            inputKeyBoard.inputField = null;
            KeyBoardGO.SetActive(false);
            SetActive(true, OffObject.ToArray());
            //OffObject.SetActive(true);
        }
    }

    private void SetActive(bool val, params GameObject[] go)
    {
        for(int i = 0; i < go.Length; i++)
        {
            go[i].SetActive(val);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        //Debug.Log("OnSelect");
        //if (eventData.selectedObject.GetComponent<InputField>() != null)
        //{
        //    inputKeyBoard.inputField = eventData.selectedObject.GetComponent<InputField>();
        //    KeyBoardGO.SetActive(true);
        //}        
    }

    private bool ValueCheck()
    {
        bool IsInputFieldEmpty = true;
        bool IsToggleEmpty = true;
        
       if(InputFieldList[0].GetSetText != string.Empty &&
          InputFieldList[1].GetSetText != string.Empty &&
          InputFieldList[2].GetSetText != string.Empty &&
          InputFieldList[3].GetSetText != string.Empty)
        {
            IsInputFieldEmpty = false;
        }


        for (int i = 0; i < ToggleList.Count; i++)
        {
            if (ToggleList[i].GetSetSelected)
                IsToggleEmpty = false;     
        }
        
        return IsInputFieldEmpty || IsToggleEmpty;
    }

    private void ActivateButton()
    {
        Manager.ActiveLoginButton(ValueCheck());
    }

    private void Update()
    {
        // 정보를 입력했는지 체크
        ActivateButton();
    }

}
