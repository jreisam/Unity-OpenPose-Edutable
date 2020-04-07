//================================================================================================================================
//
//  Copyright (c) 2015-2019 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using Common;
using easyar;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SurfaceTracking_ImageTarget
{
    public class UIController : MonoBehaviour
    {
        public Text Status;
        public ARSession Session;
        public TouchController TouchControl;

        private SurfaceTrackerFrameFilter tracker;
        private ImageTargetController controllerNamecard;

        private void Awake()
        {
            tracker = Session.GetComponentInChildren<SurfaceTrackerFrameFilter>();
            controllerNamecard = GameObject.Find("ImageTarget").GetComponent<ImageTargetController>();
            TouchControl.TurnOn(TouchControl.transform, Camera.main, false, false, true, true);
        }

        private void Update()
        {
            Status.text = "CenterMode: " + Session.CenterMode + Environment.NewLine +
                Environment.NewLine +
                "Gesture Instruction" + Environment.NewLine +
                "\tMove on Surface: One Finger Move" + Environment.NewLine +
                "\tRotate: Two Finger Horizontal Move" + Environment.NewLine +
                "\tScale: Two Finger Pinch";

            if (Input.touchCount == 1 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                var touch = Input.touches[0];
                if (touch.phase == TouchPhase.Moved)
                {
                    var viewPoint = new Vector2(touch.position.x / Screen.width, touch.position.y / Screen.height);
                    if (tracker && tracker.Tracker != null && Session.FrameCameraParameters.OnSome)
                    {
                        var coord = EasyARController.Instance.Display.ImageCoordinatesFromScreenCoordinates(viewPoint, Session.FrameCameraParameters.Value, Session.Assembly.Camera);
                        tracker.Tracker.alignTargetToCameraImagePoint(coord.ToEasyARVector());
                    }
                }
            }
        }

        public void SwitchCenterMode()
        {
            while (true)
            {
                Session.CenterMode = (ARSession.ARCenterMode)(((int)Session.CenterMode + 1) % Enum.GetValues(typeof(ARSession.ARCenterMode)).Length);
                if (Session.CenterMode == ARSession.ARCenterMode.SpecificTarget)
                {
                    Session.CenterTarget = controllerNamecard;
                }
                if (Session.CenterMode == ARSession.ARCenterMode.FirstTarget ||
                    Session.CenterMode == ARSession.ARCenterMode.Camera ||
                    Session.CenterMode == ARSession.ARCenterMode.SpecificTarget ||
                    Session.CenterMode == ARSession.ARCenterMode.WorldRoot)
                {
                    break;
                }
            }
        }
    }
}

