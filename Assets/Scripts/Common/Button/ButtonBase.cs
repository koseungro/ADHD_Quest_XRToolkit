using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public delegate void PressedEvent();
public delegate void PressedEvent2(int param);
public delegate void PressedEvent3(string param);

public class ButtonBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    // dummy 버튼 Component
    private Button dummyButton;

    public bool Interactable = true;
    protected bool isOn = false;

    // 버튼 Action담당 UnityAction Component
    protected UnityAction pressedAction;
    protected UnityAction EnterAction;
    protected UnityAction ExitAction;
    protected UnityAction ClickSoundAction;

    public PressedEvent EventPressed;
    public PressedEvent2 EventTogglePressed;
    public PressedEvent3 EventChangeScenePressed;

    public virtual bool isInteractable
    {
        set
        {
            Interactable = value;
        }
        get
        {
            return Interactable;
        }
    }
    public bool dummyInteractable
    {
        set
        {
            if(dummyButton != null)
                dummyButton.interactable = value;
        }
        get
        {
            if (dummyButton != null)
                return dummyButton.interactable;
            return false;
        }
    }
    public bool isPointerEnter
    {
        get
        {
            return isOn;
        }
    }

    protected virtual void Awake()
    {        
        dummyButton = GetComponent<Button>();
        if (!dummyButton)
        {
            //dummyButton.gameObject.AddComponent<Button>();
            //dummyButton.transition = Selectable.Transition.None;
            //dummyButton.interactable = false;
        }

        

    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {        
        
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {        
        isOn = true;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {        
        isOn = false;
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        
    }

    public virtual void AddPressedListener(params PressedEvent[] someEvent)
    {
        EventPressed = null;
        for(int i = 0; i < someEvent.Length; i++)
        {
            EventPressed += someEvent[i];
        }
        //pressedAction = null;
        //for (int i = 0; i < action.Length; i++)
        //    pressedAction += action[i];
    }
    public virtual void AddTogglePressedListener(PressedEvent2 someEvent)
    {
        EventTogglePressed = null;
        EventTogglePressed += someEvent;
    }
    public virtual void AddChangeScenePressedListener(PressedEvent3 someEvent)
    {
        EventChangeScenePressed = null;
        EventChangeScenePressed += someEvent;
    }

    public virtual void AddPressedListener2(PressedEvent someEvent)
    {
        EventPressed += someEvent;
    }

    public virtual void AddEnterActionListener(params UnityAction[] action)
    {
        EnterAction = null;
        for (int i = 0; i < action.Length; i++)
            EnterAction += action[i];
    }
    public virtual void AddClickSoundActionListener(params UnityAction[] action)
    {
        ClickSoundAction = null;
        for (int i = 0; i < action.Length; i++)
            ClickSoundAction += action[i];
    }
    public virtual void AddExitActionListener(params UnityAction[] action)
    {
        ExitAction = null;
        for (int i = 0; i < action.Length; i++)
            ExitAction += action[i];
    }
}
