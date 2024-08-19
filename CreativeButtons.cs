using UnityEngine.UI;
using UnityEngine;

public class CreativeButtons : MonoBehaviour {
    #region LifecycleMethods

    void Start() => GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;

    #endregion
}
