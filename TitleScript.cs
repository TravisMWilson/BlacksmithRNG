using System.Collections.Generic;
using UnityEngine;

public class TitleScript : MonoBehaviour {
    #region PublicVariables

    public List<GameObject> WorldCanvasList = new List<GameObject>();

    #endregion
    #region PrivateVariables

    private string titleBackground;

    #endregion
    #region LifecycleMethods

    void Start() {
        titleBackground = LoadScript.LoadStringValue("TitleBackground");
        if (titleBackground == string.Empty) titleBackground = "World1";

        foreach (GameObject worldCanvas in WorldCanvasList) {
            worldCanvas.SetActive(false);
            if (worldCanvas.name.Contains(titleBackground[..6])) worldCanvas.SetActive(true);
        }
    }

    #endregion
}
