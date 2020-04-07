//================================================================================================================================
//
//  Copyright (c) 2015-2019 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using System;
using UnityEngine;

namespace SpatialMap_SparseSpatialMap
{
    public class OneShot : MonoBehaviour
    {
        private bool mirror;
        private Action<Texture2D> callback;
        private bool capturing;

        public void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination);
            if (!capturing) { return; }

            var destTexture = new RenderTexture(Screen.width, Screen.height, 0);
            if (mirror)
            {
                var mat = Instantiate(Resources.Load<Material>("Sample_MirrorTexture"));
                mat.mainTexture = source;
                Graphics.Blit(null, destTexture, mat);
            }
            else
            {
                Graphics.Blit(source, destTexture);
            }

            RenderTexture.active = destTexture;
            var texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            texture.Apply();
            RenderTexture.active = null;
            Destroy(destTexture);

            callback(texture);
            Destroy(this);
        }

        public void Shot(bool mirror, Action<Texture2D> callback)
        {
            if (callback == null) { return; }
            this.mirror = mirror;
            this.callback = callback;
            capturing = true;
        }
    }
}
