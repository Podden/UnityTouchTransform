//2018 by Daniel Pots www.vr-bits.com

using UnityEngine;
using UnityEngine.EventSystems;

//Allows Rotating/Scaling/Moving by Touch
public class TouchTransformableObject : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler {
    public bool isSelected;
    public bool isMovable, isRotatable, isScaleable;
    bool isTouched = false;
    float maxScale;
    void Start() {
        maxScale = transform.localScale.x;

        //New Object is the Selected
        var allObjects = FindObjectsOfType<TouchTransformableObject>();
        for (int i = 0; i < allObjects.Length; i++)
            allObjects[i].isSelected = false;
        isSelected = true;

        //Initial Position in Front of the User
        var screenPosition = Camera.main.WorldToViewportPoint(Camera.main.transform.position + Camera.main.transform.forward * 10);
        Vector2 point = new Vector2(screenPosition.x, screenPosition.y);
    }
  
    void Update() {
        if (!isSelected)
            return;
        if (isMovable && isTouched) {
            if (Input.touchCount == 1) {
                var touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved) {
                    //There are two methods for moving the object, uncomment the one you want

                    //This moves the Object alongsite a plane determined by its parent

                    ////Find Parents Plane and Project Touchpoint on it
                    //var plane = new Plane(transform.forward, transform.parent.position);
                    //var ray = Camera.main.ScreenPointToRay(touch.position);
                    //float dist;
                    ////Raycast against Plane to position object under Touch
                    //if (plane.Raycast(ray, out dist))
                    //    transform.position = ray.GetPoint(dist);
                    
                    //This moves the Object alongsite its Distance from the Camera
                    transform.position = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 10));
                }
            }
        }
        if (isRotatable) {
            Quaternion desiredRotation = transform.rotation;
            if (Mathf.Abs(TouchMovement.instance.turnAngleDelta) > 0) { // rotate
                Vector3 rotationDeg = Vector3.zero;
                rotationDeg.y = -TouchMovement.instance.turnAngleDelta;
                desiredRotation *= Quaternion.Euler(rotationDeg);
            }
            // not so sure those will work:
            transform.rotation = desiredRotation;
        }
        if (isScaleable && TouchMovement.instance.pinchDistance != 0) {
            var value = Mathf.Clamp(transform.localScale.x + TouchMovement.instance.pinchDistanceDelta, .1f, maxScale);
            transform.localScale = Vector3.one * value;
        }
    }
    public void ResetPosition() {
        transform.position = Vector3.zero;
    }
    public void ResetRotation() {
        transform.rotation = Quaternion.identity;
    }
    public void ResetScale() {
        transform.localScale = Vector3.one;
    }
    public void OnPointerClick(PointerEventData eventData) {
        if (isSelected)
            return;

        //Deactivate Selection on all objects except this one
        var allObjects = FindObjectsOfType<TouchTransformableObject>();
        for (int i = 0; i < allObjects.Length; i++) {
            allObjects[i].isSelected = false;
        }
        isSelected = true;
    }

    public void OnPointerDown(PointerEventData eventData) {
        isTouched = true;
    }

    public void OnPointerUp(PointerEventData eventData) {
        isTouched = false;
    }
}
