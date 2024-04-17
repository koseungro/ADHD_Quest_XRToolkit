/// 작성자: 백인성 
/// 작성일: 2018-07-31 
/// 수정일: 2018-07-31
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력 
/// 

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 사용자 정보를 정의 합니다.
/// </summary>
public class IS_Infomation : Singleton<IS_Infomation>
{
    #region delegate, event, enum
    #endregion
    #region public property
    /// <summary>
    /// 유저 이름입니다.
    /// </summary>
    public string UserName
    {
        get { return m_nameInput.text; }
    }
    /// <summary>
    /// 유저의 성별
    /// </summary>
    public string Gender
    {
        get { return m_gender.options[m_gender.value].text; }
    }
    /// <summary>
    /// 유저의 생일
    /// </summary>
    public string Birthday
    {
        get
        {
            string birthday = string.Format("{0}{1}{2}", m_yaer.options[m_yaer.value].text,
                                                         m_month.options[m_month.value].text,
                                                         m_day.options[m_day.value].text);
            return birthday;
        }
    }
    /// <summary>
    /// 유저의 만 나이
    /// </summary>
    public string Age
    {
        get { return m_ageText.text; }
    }
    #endregion

    #region protected / private field
    private InputField m_nameInput;
    private Dropdown m_gender;

    private Dropdown m_yaer;
    private Dropdown m_month;
    private Dropdown m_day;
    private Text     m_ageText;
    #endregion

    #region Unity base method
    void Start ()
    {
        //Transform info = transform.Find("Info");
        //Transform age = transform.Find("Age");

        //m_nameInput = info.transform.Find("Name/InputField").GetComponent<InputField>();
        //m_gender = info.transform.Find("Name/Dropdown").GetComponent<Dropdown>();

        //m_yaer = info.transform.Find("Age/Year").GetComponent<Dropdown>();
        //m_month = info.transform.Find("Age/Month").GetComponent<Dropdown>();
        //m_day = info.transform.Find("Age/Day").GetComponent<Dropdown>();

        //m_ageText = info.transform.Find("Age/Image_Age/Text").GetComponent<Text>();

        //m_yaer.onValueChanged.AddListener(SetBirthYear);
        //m_month.onValueChanged.AddListener(SetBirthMonth);
        //m_day.onValueChanged.AddListener(SetBirthDay);

        //m_ageText.text = "0";
    }
    #endregion

    #region protected / private method
    private void SetBirthYear(int num)
    {
        calculateManAge(m_yaer.options[num].text, m_month.options[m_month.value].text, m_day.options[m_day.value].text);
    }
    private void SetBirthMonth(int num)
    {
        calculateManAge(m_yaer.options[m_yaer.value].text, m_month.options[num].text, m_day.options[m_day.value].text);
    }
    private void SetBirthDay(int num)
    {
        calculateManAge(m_yaer.options[m_yaer.value].text, m_month.options[m_month.value].text, m_day.options[num].text);
    }
    private void calculateManAge(string birthY = "1990", string birthM = "1", string birthD = "1")
    {
        int manAge; //만 나이

        DateTime birthDay = new DateTime(int.Parse(birthY), int.Parse(birthM), int.Parse(birthD));
        DateTime today = DateTime.Today;
        
        manAge = today.Year - birthDay.Year;

        if (today.Month < birthDay.Month)
        { //생년월일 "월"이 지났는지 체크
            manAge--;
        }
        else if (today.Month == birthDay.Month)
        { //생년월일 "일"이 지났는지 체크
            if (today.Day < birthDay.Day)
            {
                manAge--; //생일 안지났으면 (만나이 - 1)
            }
        }

        m_ageText.text = manAge.ToString();
    }
    #endregion
}
