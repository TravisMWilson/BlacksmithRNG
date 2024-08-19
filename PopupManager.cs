using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public enum PopupType {
    OK,
    YesNo
}

public class PopupManager : MonoBehaviour {
    #region PublicVariables

    public GameObject popupWindow;
    public TextMeshProUGUI messageText;
    public Button yesButton;
    public Button noButton;
    public Button okButton;

    #endregion
    #region PrivateVariables

    private Action yesAction;
    private Action noAction;
    private Action okAction;

    #endregion
    #region Singleton

    public static PopupManager Instance;

    void Awake() {
        HidePopup();

        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    #endregion
    #region PublicMethods

    public void ShowPopup(string message, PopupType popupType, Action yesCallback = null, Action noCallback = null, Action okCallback = null) {
        messageText.text = message;
        yesAction = yesCallback;
        noAction = noCallback;
        okAction = okCallback;

        switch (popupType) {
            case PopupType.OK:
                okButton.gameObject.SetActive(true);
                yesButton.gameObject.SetActive(false);
                noButton.gameObject.SetActive(false);
                break;
            case PopupType.YesNo:
                okButton.gameObject.SetActive(false);
                yesButton.gameObject.SetActive(true);
                noButton.gameObject.SetActive(true);
                break;
        }

        popupWindow.SetActive(true);
    }

    public void HidePopup() => popupWindow.SetActive(false);

    public void OnYesButtonClicked() {
        yesAction?.Invoke();
        HidePopup();
    }

    public void OnNoButtonClicked() {
        noAction?.Invoke();
        HidePopup();
    }

    public void OnOKButtonClicked() {
        okAction?.Invoke();
        HidePopup();
    }

    #endregion
}