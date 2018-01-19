//THX to Caue Rego (cawas), who wrote the original: http://wiki.unity3d.com/index.php/DetectTouchMovement

using UnityEngine;

public class TouchMovement : MonoBehaviour {
    //Added rudimentry Singleton Pattern, feel free to change
    public static TouchMovement instance;

    void Awake() {
        if (!instance) {
            instance = this;
            return;
        }
        Destroy(this);
    }

    public float pinchTurnRatio = Mathf.PI / 2;
    public float minTurnAngle = 5;

    public float pinchRatio = 1;
    public float minPinchDistance = 3;

    public float panRatio = 1;
    public float minPanDistance = 3;

    /// <summary>
    ///   The delta of the angle between two touch points
    /// </summary>
    [HideInInspector]
    public float turnAngleDelta;
    /// <summary>
    ///   The angle between two touch points
    /// </summary>
    [HideInInspector]
    public float turnAngle;

    /// <summary>
    ///   The delta of the distance between two touch points that were distancing from each other
    /// </summary>
    [HideInInspector]
    public float pinchDistanceDelta;
    /// <summary>
    ///   The distance between two touch points that were distancing from each other
    /// </summary>
    [HideInInspector]
    public float pinchDistance;
    [HideInInspector]
    public Vector2 panDistance;

    /// <summary>
    ///   Calculates Pinch and Turn - This should be used inside LateUpdate
    /// </summary>
    void Update() {
        pinchDistance = pinchDistanceDelta = 0;
        turnAngle = turnAngleDelta = 0;

        if (Input.touchCount == 1) {
            var touch = Input.touches[0];
            // ... if at least one of them moved ...
            if (touch.phase == TouchPhase.Moved) {
                if (Mathf.Abs(touch.deltaPosition.magnitude) > minPanDistance) {
                    panDistance = touch.deltaPosition * panRatio;
                } else {
                    panDistance = Vector2.zero;
                }
            }
        }

        // if two fingers are touching the screen at the same time ...
        if (Input.touchCount == 2) {
            Touch touch1 = Input.touches[0];
            Touch touch2 = Input.touches[1];

            // ... if at least one of them moved ...
            if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved) {
                // ... check the delta distance between them ...
                pinchDistance = Vector2.Distance(touch1.position, touch2.position);
                float prevDistance = Vector2.Distance(touch1.position - touch1.deltaPosition,
                                                      touch2.position - touch2.deltaPosition);
                pinchDistanceDelta = pinchDistance - prevDistance;

                // ... if it's greater than a minimum threshold, it's a pinch!
                if (Mathf.Abs(pinchDistanceDelta) > minPinchDistance) {
                    pinchDistanceDelta *= pinchRatio;
                } else {
                    pinchDistance = pinchDistanceDelta = 0;
                }
                Debug.Log("Pinchdistance: " + pinchDistance);

                // ... or check the delta angle between them ...
                turnAngle = Angle(touch1.position, touch2.position);
                float prevTurn = Angle(touch1.position - touch1.deltaPosition,
                                       touch2.position - touch2.deltaPosition);
                turnAngleDelta = Mathf.DeltaAngle(prevTurn, turnAngle);

                // ... if it's greater than a minimum threshold, it's a turn!
                if (Mathf.Abs(turnAngleDelta) > minTurnAngle) {
                    turnAngleDelta *= pinchTurnRatio;
                } else {
                    turnAngle = turnAngleDelta = 0;
                }
            }
        }
    }

    private float Angle(Vector2 pos1, Vector2 pos2) {
        Vector2 from = pos2 - pos1;
        Vector2 to = new Vector2(1, 0);

        float result = Vector2.Angle(from, to);
        Vector3 cross = Vector3.Cross(from, to);

        if (cross.z > 0) {
            result = 360f - result;
        }

        return result;
    }
}
