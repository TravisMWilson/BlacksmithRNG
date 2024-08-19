using UnityEngine.UI;
using UnityEngine;

public class DisplaySword : MonoBehaviour {
    #region PrivateVariables

    private GameObject pommelDisplay;
    private GameObject handleDisplay;
    private GameObject guardDisplay;
    private GameObject bladeDisplay;

    private Transform pommelDisplayShadow;
    private Transform handleDisplayShadow;
    private Transform guardDisplayShadow;
    private Transform bladeDisplayShadow;

    #endregion
    #region Singleton

    public static DisplaySword Instance;

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    #endregion
    #region PublicMethods

    public void UpdateSword(Item swordItem) {
        GameObject displaySword = GameObject.Find("DisplaySword");

        CacheDisplayObjects(displaySword);
        UpdateDisplayImages(swordItem);
        RepositionDisplaySword();
    }

    public void RepositionDisplaySword() {
        GetPartOffsets(pommelDisplay, 5, 0, out int pommelDisplayStartPos, out _);
        GetPartOffsets(handleDisplay, 15, 20, out int handleDisplayStartPos, out int handleDisplayEndPos);
        GetPartOffsets(guardDisplay, 8, 1, out int guardDisplayStartPos, out int guardDisplayEndPos);
        GetPartOffsets(bladeDisplay, 24, 0, out int bladeDisplayStartPos, out _);

        SetAnchoredPositions(pommelDisplay, (pommelDisplayStartPos + handleDisplayStartPos + handleDisplayEndPos + guardDisplayStartPos) * -2);
        SetAnchoredPositions(handleDisplay, (handleDisplayEndPos + guardDisplayStartPos) * -2);
        SetAnchoredPositions(bladeDisplay, (bladeDisplayStartPos + guardDisplayEndPos) * 2);

        if (pommelDisplayShadow != null) {
            GetPartOffsets(pommelDisplayShadow.gameObject, 5, 0, out pommelDisplayStartPos, out _);
            GetPartOffsets(handleDisplayShadow.gameObject, 15, 20, out handleDisplayStartPos, out handleDisplayEndPos);
            GetPartOffsets(guardDisplayShadow.gameObject, 8, 1, out guardDisplayStartPos, out guardDisplayEndPos);
            GetPartOffsets(bladeDisplayShadow.gameObject, 24, 0, out bladeDisplayStartPos, out _);

            SetAnchoredPositions(pommelDisplayShadow.gameObject, (int)((pommelDisplayStartPos + handleDisplayStartPos + handleDisplayEndPos + guardDisplayStartPos) * -2.4));
            SetAnchoredPositions(handleDisplayShadow.gameObject, (int)((handleDisplayEndPos + guardDisplayStartPos) * -2.4));
            SetAnchoredPositions(bladeDisplayShadow.gameObject, (int)((bladeDisplayStartPos + guardDisplayEndPos) * 2.4));
        }
    }

    public void CacheDisplayObjects(GameObject sword) {
        pommelDisplay = sword.transform.Find("PommelImage").gameObject;
        handleDisplay = sword.transform.Find("HandleImage").gameObject;
        guardDisplay = sword.transform.Find("GuardImage").gameObject;
        bladeDisplay = sword.transform.Find("BladeImage").gameObject;

        pommelDisplayShadow = sword.transform.Find("PommelShadow");
        handleDisplayShadow = sword.transform.Find("HandleShadow");
        guardDisplayShadow = sword.transform.Find("GuardShadow");
        bladeDisplayShadow = sword.transform.Find("BladeShadow");
    }

    #endregion
    #region PrivateMethods

    private void UpdateDisplayImages(Item swordItem) {
        pommelDisplay.GetComponent<Image>().sprite = swordItem.SwordItem.Pommel.Icon;
        handleDisplay.GetComponent<Image>().sprite = swordItem.SwordItem.Handle.Icon;
        guardDisplay.GetComponent<Image>().sprite = swordItem.SwordItem.Guard.Icon;
        bladeDisplay.GetComponent<Image>().sprite = swordItem.SwordItem.Blade.Icon;

        if (pommelDisplayShadow != null) {
            pommelDisplayShadow.gameObject.GetComponent<Image>().sprite = swordItem.SwordItem.Pommel.Icon;
            handleDisplayShadow.gameObject.GetComponent<Image>().sprite = swordItem.SwordItem.Handle.Icon;
            guardDisplayShadow.gameObject.GetComponent<Image>().sprite = swordItem.SwordItem.Guard.Icon;
            bladeDisplayShadow.gameObject.GetComponent<Image>().sprite = swordItem.SwordItem.Blade.Icon;
        }
    }

    private void GetPartOffsets(GameObject display, int defaultStartPos, int defaultEndPos, out int startPos, out int endPos) {
        string materialType = display.GetComponent<Image>().sprite.ToString().Replace(" (UnityEngine.Sprite)", "");

        if (materialType.Contains("Anvil")) {
            startPos = defaultStartPos;
            endPos = defaultEndPos;
        } else {
            PartOffsetList.SetPartOffset(materialType, out startPos, out endPos);
        }
    }

    private void SetAnchoredPositions(GameObject display, int position)
        => display.GetComponent<RectTransform>().anchoredPosition = new Vector2(display.GetComponent<RectTransform>().anchoredPosition.x, position);

    #endregion
}
