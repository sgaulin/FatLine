using JetBrains.Annotations;
using System;
using System.Collections.Specialized;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationUI : MonoBehaviour
{
    public static ConfirmationUI Instance { get; private set; }
    private Button yesBtn;
    private Button noBtn;
    private TextMeshProUGUI messageText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        yesBtn = transform.Find("YesButton").GetComponent<Button>();
        noBtn = transform.Find("NoButton").GetComponent<Button>();
        messageText = transform.Find("Text").GetComponent<TextMeshProUGUI>();

        Hide();
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void ShowQuestion(string questionText, Action yesAction, Action noAction)
    {
        gameObject.SetActive(true);

        messageText.text = questionText;
        yesBtn.onClick.AddListener(() =>
        {
            Hide();
            yesAction();
            RemoveListeners();
        });
        noBtn.onClick.AddListener(() =>
        {
            Hide();
            noAction();
            RemoveListeners();
        });
    }

    private void Hide()
    {
        
        gameObject.SetActive(false);
    }

    private void RemoveListeners()
    {
        yesBtn.onClick.RemoveAllListeners();
        noBtn.onClick.RemoveAllListeners();
    }


}
