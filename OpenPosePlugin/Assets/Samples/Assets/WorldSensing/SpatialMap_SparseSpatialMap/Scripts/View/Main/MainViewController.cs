//================================================================================================================================
//
//  Copyright (c) 2015-2019 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpatialMap_SparseSpatialMap
{
    public class MainViewController : MonoBehaviour
    {
        public Button Edit;
        public Button Preview;
        public Button Create;
        public GameObject ClearPupup;
        public MapGridController MapGrid;

        private void OnEnable()
        {
            StopAllCoroutines();
            var colors = Create.colors;
            colors.normalColor = new Color(0.2f, 0.58f, 0.988f);
            colors.highlightedColor = new Color(0.192f, 0.557f, 0.949f);
            colors.pressedColor = new Color(0.157f, 0.455f, 0.773f);
            Create.colors = colors;
            ClearPupup.SetActive(false);
            StartCoroutine(Twinkle(Create));
        }

        public void EnableEdit(bool enable)
        {
            Edit.interactable = enable;
        }

        public void EnablePreview(bool enable)
        {
            Preview.interactable = enable;
        }

        private IEnumerator Twinkle(Button button)
        {
            if (MapGrid.CellCount > 0) { yield break; }

            var colors = button.colors;
            var olist = new List<Color>
            {
                colors.normalColor,
                colors.highlightedColor,
                colors.pressedColor
            };

            var clist = new List<Vector3>();
            foreach (var color in olist)
            {
                Vector3 hsv;
                Color.RGBToHSV(color, out hsv.x, out hsv.y, out hsv.z);
                clist.Add(hsv);
            }

            float smin = 0.2f;
            float smax = clist[0].y;
            bool increase = false;

            while (MapGrid.CellCount <= 0)
            {
                for (int i = 0; i < clist.Count; ++i)
                {
                    var hsv = clist[i];
                    hsv.y += increase ? 0.2f * Time.deltaTime : -0.2f * Time.deltaTime;
                    clist[i] = hsv;
                }
                if (clist[0].y >= smax)
                {
                    for (int i = 0; i < clist.Count; ++i)
                    {
                        clist[i] = new Vector3(clist[i].x, smax, clist[i].z);
                    }
                    increase = false;
                }
                else if (clist[0].y < smin)
                {
                    for (int i = 0; i < clist.Count; ++i)
                    {
                        clist[i] = new Vector3(clist[i].x, smin, clist[i].z);
                    }
                    increase = true;
                }
                colors.normalColor = Color.HSVToRGB(clist[0].x, clist[0].y, clist[0].z);
                colors.highlightedColor = Color.HSVToRGB(clist[1].x, clist[1].y, clist[1].z);
                colors.pressedColor = Color.HSVToRGB(clist[2].x, clist[2].y, clist[2].z);
                button.colors = colors;
                yield return 0;
            }
            colors.normalColor = olist[0];
            colors.highlightedColor = olist[1];
            colors.pressedColor = olist[2];
            button.colors = colors;
        }
    }
}
