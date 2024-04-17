using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Scene Data", menuName = "Stress Manager/Scene Data")]
public class IS_SceneData : ScriptableObject
{
    public AudioClip[] narrations;
    public ButtonsData[] buttons;
}

[System.Serializable]
public struct ButtonsData
{
    public IS_ButtonData button;
    public Vector2 buttonPos;
}
