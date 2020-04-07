//================================================================================================================================
//
//  Copyright (c) 2015-2019 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================


using UnityEngine;
using UnityEngine.EventSystems;
using easyar;
using UnityEngine.UI;
using System;
using Common;

namespace SpatialMap_Sparse_ImageTarget
{
    public class SparseSpatailMapSample : MonoBehaviour
    {
        public Text Status;
        public ARSession Session;
        public TouchController TouchControl;
        public ImageTargetController ImageTarget;

        private VIOCameraDeviceUnion vioCamera;
        private SparseSpatialMapWorkerFrameFilter sparse;
        private bool onSparse;
        private bool found;

        private void Awake()
        {
            vioCamera = Session.GetComponentInChildren<VIOCameraDeviceUnion>();
            sparse = Session.GetComponentInChildren<SparseSpatialMapWorkerFrameFilter>();
            TouchControl.TurnOn(TouchControl.gameObject.transform, Camera.main, false, false, true, false);
            ImageTarget.TargetFound += () => { found = true; };
            ImageTarget.TargetLost += () => { found = false; };
        }

        private void Update()
        {
            Status.text = "VIO Device Type: " + (vioCamera.Device == null ? "-" : vioCamera.Device.DeviceType.ToString()) + Environment.NewLine +
                "Tracking Status: " + (Session.WorldRootController == null ? "-" : Session.WorldRootController.TrackingStatus.ToString()) + Environment.NewLine +
                "Sparse Point Cloud Count: " + (sparse.LocalizedMap == null ? "-" : sparse.LocalizedMap.PointCloud.Count.ToString()) + Environment.NewLine +
                "Cube Location: " + (onSparse ? "On Sparse Spatial Map" : (sparse.TrackingStatus != MotionTrackingStatus.NotTracking ? "Air" : "-")) + (found ? " + On " + ImageTarget.name : "") + Environment.NewLine +
                Environment.NewLine +
                "Gesture Instruction" + Environment.NewLine +
                "\tMove to Sparse Spatial Map Point: One Finger Move" + Environment.NewLine +
                "\tScale: Two Finger Pinch";

            if (Input.touchCount == 1 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                var touch = Input.touches[0];
                if (touch.phase == TouchPhase.Moved)
                {
                    var viewPoint = new Vector2(touch.position.x / Screen.width, touch.position.y / Screen.height);
                    if (sparse && sparse.LocalizedMap)
                    {
                        var points = sparse.LocalizedMap.HitTest(viewPoint);
                        foreach (var point in points)
                        {
                            onSparse = true;
                            TouchControl.transform.position = sparse.LocalizedMap.transform.TransformPoint(point);
                            break;
                        }
                    }
                }
            }
        }
    }
}
