using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    private void Awake() {
        if(Instance == null) {
            Instance = this;
        }
    }

    public float GetScrollOrMobileZoom() {
#if UNITY_STANDALONE || UNITY_EDITOR
        return Input.GetAxis("Mouse ScrollWheel"); ;
#elif UNITY_ANDROID || UNITY_IPHONE
        return _ZoomInMobile();
#endif
    }

    public Vector3 GetMouseTouchPosition() {
#if UNITY_STANDALONE || UNITY_EDITOR
        var mousePosition = Input.mousePosition;
        return mousePosition;
#elif UNITY_ANDROID || UNITY_IPHONE
        if (Input.touchCount > 0) {
            return Input.GetTouch(0).position;
        }
#endif
        return Vector3.zero;
    }

    public bool GetMouseOrTouchBegin() {
#if UNITY_STANDALONE || UNITY_EDITOR
        return Input.GetMouseButtonDown(0);
#elif UNITY_ANDROID || UNITY_IPHONE
        foreach (Touch touch in Input.touches) {
            return (touch.phase == TouchPhase.Began);
        }
        return false;
#endif
    }

    public bool GetMouseOrTouchEnd() {
#if UNITY_STANDALONE || UNITY_EDITOR
        return Input.GetMouseButtonUp(0);
#elif UNITY_ANDROID || UNITY_IPHONE
        foreach (Touch touch in Input.touches) {
            return (touch.phase == TouchPhase.Ended);
        }
        return false;
#endif
    }

#if UNITY_ANDROID || UNITY_IPHONE
    private float _ZoomInMobile() {

        float zoomInOut = 0f;

        // TODO : Only 2 fingers can "zoom in / out" not quite elegant. Make it "all fingers".
        if(Input.touchCount == 2) {
            var firstFinger = Input.touches[0];
            var secondFinger = Input.touches[1];

            // Getting the current touch - previous one
            var differenceFirstFingerPos = firstFinger.position - firstFinger.deltaPosition;
            var differenceSecondFingerPos = secondFinger.position - secondFinger.deltaPosition;

            // Give us the lenght of our vector / zoom in / out
            var magnitudePrevious = (differenceFirstFingerPos - differenceSecondFingerPos).magnitude;
            var magnitudeCurrent = (firstFinger.position - secondFinger.position).magnitude;

            zoomInOut = (magnitudePrevious - magnitudeCurrent) * 0.01f;
        }

        return zoomInOut;
    }
#endif
}
