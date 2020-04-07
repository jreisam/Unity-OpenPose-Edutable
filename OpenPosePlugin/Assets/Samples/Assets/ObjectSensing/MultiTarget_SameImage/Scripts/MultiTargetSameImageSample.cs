//================================================================================================================================
//
//  Copyright (c) 2015-2019 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using easyar;
using System;
using System.Collections;
using UnityEngine;

namespace MultiTarget_SameImage
{
    public class MultiTargetSameImageSample : MonoBehaviour
    {
        public ImageTrackerFrameFilter ImageTracker;
        public GameObject Cube;

        private void Start()
        {
            StartCoroutine(FileUtil.LoadFile("namecard.jpg", PathType.StreamingAssets, (buffer) =>
            {
                StartCoroutine(LoadImageBuffer(buffer.Clone(), "namecard", 0.09f));
            }));
        }

        private IEnumerator LoadImageBuffer(easyar.Buffer buffer, string name, float scale)
        {
            using (buffer)
            {
                Optional<Image> imageOptional = null;
                bool taskFinished = false;
                EasyARController.Instance.Worker.Run(() =>
                {
                    imageOptional = ImageHelper.decode(buffer);
                    taskFinished = true;
                });

                while (!taskFinished)
                {
                    yield return 0;
                }
                if (imageOptional.OnNone)
                {
                    throw new Exception("invalid buffer");
                }
                using (var image = imageOptional.Value)
                {
                    CreateMultipleTargetsFromOneImage(image, 10, name, scale);
                }
            }
        }

        private void CreateMultipleTargetsFromOneImage(Image image, int count, string name, float scale)
        {
            for (int i = 0; i < count; i++)
            {
                using (var param = new ImageTargetParameters())
                {
                    param.setImage(image);
                    param.setName(name);
                    param.setScale(scale);
                    param.setUid(Guid.NewGuid().ToString());
                    param.setMeta(string.Empty);
                    var targetOptional = ImageTarget.createFromParameters(param);
                    if (targetOptional.OnSome)
                    {
                        var target = targetOptional.Value;
                        GameObject Target = new GameObject(name + " <" + i + ">");
                        var controller = Target.AddComponent<ImageTargetController>();
                        AddTargetControllerEvents(controller);

                        controller.SourceType = ImageTargetController.DataSource.Target;
                        controller.TargetSource = target;
                        controller.Tracker = ImageTracker;

                        if (Cube)
                        {
                            var cube = Instantiate(Cube);
                            cube.transform.parent = controller.transform;
                        }
                    }
                    else
                    {
                        throw new Exception("invalid parameter");
                    }
                }
            }
        }

        private void AddTargetControllerEvents(ImageTargetController controller)
        {
            if (!controller)
            {
                return;
            }

            controller.TargetFound += () =>
            {
                Debug.LogFormat("Found target {{id = {0}, name = {1}}}", controller.Target.runtimeID(), controller.Target.name());
            };
            controller.TargetLost += () =>
            {
                Debug.LogFormat("Lost target {{id = {0}, name = {1}}}", controller.Target.runtimeID(), controller.Target.name());
            };
            controller.TargetLoad += (Target target, bool status) =>
            {
                Debug.LogFormat("Load target {{id = {0}, name = {1}, size = {2}}} into {3} => {4}", target.runtimeID(), target.name(), controller.Size, controller.Tracker.name, status);
            };
            controller.TargetUnload += (Target target, bool status) =>
            {
                Debug.LogFormat("Unload target {{id = {0}, name = {1}}} => {2}", target.runtimeID(), target.name(), status);
            };
        }
    }
}
