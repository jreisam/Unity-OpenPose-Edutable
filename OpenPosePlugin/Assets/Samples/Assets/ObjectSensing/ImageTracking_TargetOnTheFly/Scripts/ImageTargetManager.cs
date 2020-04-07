//=============================================================================================================================
//
// Copyright (c) 2015-2019 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
// EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
// and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//=============================================================================================================================

using easyar;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ImageTracking_TargetOnTheFly
{
    public class ImageTargetManager : MonoBehaviour
    {
        public FilesManager PathManager;
        public ImageTrackerFrameFilter Tracker;
        public GameObject Cube;

        private Dictionary<string, ImageTargetController> imageTargetDic = new Dictionary<string, ImageTargetController>();

        private void Start()
        {
            if (!PathManager)
                PathManager = FindObjectOfType<FilesManager>();
            LoadTarget();
        }

        public void LoadTarget()
        {
            var imageTargetName = PathManager.GetImageFiles();
            foreach (var obj in imageTargetName.Where(obj => !imageTargetDic.ContainsKey(obj.Key)))
            {
                GameObject imageTarget = new GameObject(obj.Key);
                var controller = imageTarget.AddComponent<ImageTargetController>();
                controller.SourceType = ImageTargetController.DataSource.ImageFile;
                controller.ImageFileSource.PathType = PathType.Absolute;
                controller.ImageFileSource.Path = obj.Value.Replace(@"\", "/");
                controller.ImageFileSource.Name = obj.Key;
                controller.ImageFileSource.Scale = 1f;
                controller.Tracker = Tracker;
                imageTargetDic.Add(obj.Key, controller);
                var cube = Instantiate(Cube);
                cube.transform.parent = imageTarget.transform;
            }
        }

        public void ClearAllTarget()
        {
            foreach (var obj in imageTargetDic)
                Destroy(obj.Value.gameObject);
            imageTargetDic = new Dictionary<string, ImageTargetController>();
        }
    }
}
