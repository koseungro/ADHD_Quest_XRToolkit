/// 작성자: 백인성 
/// 작성일: 2018-07-31 
/// 수정일: 2018-07-31
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력 
/// 

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 드롭다운 박스에 목록을 자동으로 추가 해 줍니다.
/// </summary>
public class IS_AddDropdownList : MonoBehaviour
{
    #region public field
    public int startNum;
    public int endNum;
    #endregion

    #region protected / private field
    private Dropdown m_dropdown;
    #endregion
    
    #region Unity base method
    void Start ()
    {
        m_dropdown = GetComponent<Dropdown>();

        int max = Mathf.Max(startNum, endNum);
        int min = Mathf.Min(startNum, endNum);

        for (int cnt = 0; cnt < max - min; cnt++)
        {
            m_dropdown.options.Add(new Dropdown.OptionData((min + cnt).ToString()));
        }
    }
    #endregion
}
