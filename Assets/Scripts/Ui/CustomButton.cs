using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Serializable] public class ButtonClickedEvent : UnityEvent {}

    public bool Interactable
    {
        get => _interactable;
        set
        {
            text.alpha = value ? 1 : 0.5f;
            transform.localScale = new Vector3(1f, 1f, 1f);
            _interactable = value;
        }
    }
        
    [SerializeField] private TMP_Text text = null;
    [SerializeField] private bool changeScale = false;
    [SerializeField] private ButtonClickedEvent onClick = new ButtonClickedEvent();
    
    public ButtonClickedEvent OnClick => onClick;

    private bool _interactable = true;

    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (changeScale && Interactable)
        {
            transform.localScale = new Vector3(1.2f, 1.2f, 1f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (changeScale && Interactable)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!Interactable) return;
        
        onClick.Invoke();
    }
}
