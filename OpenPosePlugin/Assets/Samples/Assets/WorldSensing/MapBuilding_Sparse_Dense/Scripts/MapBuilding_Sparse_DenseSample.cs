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

namespace MapBuilding_Sparse_Dense
{
    public class MapBuilding_Sparse_DenseSample : MonoBehaviour
    {
        public Text Status;
        public ARSession Session;
        public TouchController TouchControl;

        private VIOCameraDeviceUnion vioCamera;
        private SparseSpatialMapWorkerFrameFilter sparse;
        private DenseSpatialMapBuilderFrameFilter dense;
        private bool onSparse;
        private bool onDense;
        private bool moveOnSparse = true;

        private void Awake()
        {
            vioCamera = Session.GetComponentInChildren<VIOCameraDeviceUnion>();
            sparse = Session.GetComponentInChildren<SparseSpatialMapWorkerFrameFilter>();
            dense = Session.GetComponentInChildren<DenseSpatialMapBuilderFrameFilter>();
            TouchControl.TurnOn(TouchControl.gameObject.transform, Camera.main, false, false, true, false);
        }

        private void Update()
        {
            Status.text = "VIO Device Type: " + (vioCamera.Device == null ? "-" : vioCamera.Device.DeviceType.ToString()) + Environment.NewLine +
                "Tracking Status: " + (Session.WorldRootController == null ? "-" : Session.WorldRootController.TrackingStatus.ToString()) + Environment.NewLine +
                "Sparse Point Cloud Count: " + (sparse.LocalizedMap == null ? "-" : sparse.LocalizedMap.PointCloud.Count.ToString()) + Environment.NewLine +
                "Dense Mesh Block Count: " + dense.MeshBlocks.Count + Environment.NewLine +
                "Cube Location: " + (onSparse ? "On Sparse Spatial Map" : (onDense ? "On Dense Spatial Map" : (sparse.TrackingStatus != MotionTrackingStatus.NotTracking ? "Air" : "-"))) + Environment.NewLine +
                Environment.NewLine +
                "Gesture Instruction" + Environment.NewLine +
                "\tMove to " + (moveOnSparse ? "Sparse Spatial Map Point" : "Dense Spatial Map Mesh") + ": One Finger Move" + Environment.NewLine +
                "\tScale: Two Finger Pinch";

            if (Input.touchCount == 1 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) && Input.touches[0].phase == TouchPhase.Moved)
            {
                var touch = Input.touches[0];
                if (moveOnSparse)
                {
                    var viewPoint = new Vector2(touch.position.x / Screen.width, touch.position.y / Screen.height);
                    if (sparse && sparse.LocalizedMap)
                    {
                        var points = sparse.LocalizedMap.HitTest(viewPoint);
                        foreach (var point in points)
                        {
                            onSparse = true;
                            onDense = false;
                            TouchControl.transform.position = sparse.LocalizedMap.transform.TransformPoint(point);
                            break;
                        }
                    }
                }
                else
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
                    RaycastHit hitInfo;
                    if (Physics.Raycast(ray, out hitInfo))
                    {
                        onDense = true;
                        onSparse = false;
                        TouchControl.transform.position = hitInfo.point;
                    };
                }
            }
        }

        public void SwitchMoveLocation()
        {
            moveOnSparse = !moveOnSparse;
        }
    }
}
