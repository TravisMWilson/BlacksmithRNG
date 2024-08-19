using UnityEngine;

public class UIMoverScript : MonoBehaviour {
    #region PublicVariables

    public float MoveDistance;

    #endregion
    #region PrivateVariables

    private bool hidden = false;

    #endregion
    #region PublicMethods

    public void MoveUIPanel() {
        Transform button = gameObject.transform.Find("Button");
        button.Rotate(0f, 0f, 180f);

        if (!hidden) {
            gameObject.transform.position += new Vector3(MoveDistance, 0f, 0f);
            hidden = true;
        } else {
            gameObject.transform.position -= new Vector3(MoveDistance, 0f, 0f);
            hidden = false;
        }
    }

    #endregion
}
