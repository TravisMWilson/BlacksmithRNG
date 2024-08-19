using UnityEngine.UI;
using UnityEngine;

public class Scroller : MonoBehaviour {
    #region PublicVariables

    public float X;

    #endregion
    #region PrivateVariables

    private RawImage image;

    #endregion
    #region LifecycleMethods

    void Start() => image = transform.GetComponent<RawImage>();
    void Update() => image.uvRect = new Rect(image.uvRect.position + (new Vector2(X, 0) * Time.deltaTime), image.uvRect.size);

    #endregion
}
