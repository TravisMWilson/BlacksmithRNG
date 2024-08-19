using UnityEngine;

public class MoveWithMouseWheel : MonoBehaviour {
    #region PublicVariables

    public float MoveSpeed;
    public float MinY;
    public float MaxY;

    #endregion
    #region LifecycleMethods

    void Update() {
        float mouseWheel = Input.GetAxis("Mouse ScrollWheel");

        if (mouseWheel != 0) {
            Vector3 newPosition = transform.position;
            newPosition.y -= mouseWheel * MoveSpeed;
            newPosition.y = Mathf.Clamp(newPosition.y, MinY, MaxY);
            transform.position = newPosition;
        }
    }

    #endregion
}
