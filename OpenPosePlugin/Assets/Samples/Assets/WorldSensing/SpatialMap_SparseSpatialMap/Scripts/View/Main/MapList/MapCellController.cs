//================================================================================================================================
//
//  Copyright (c) 2015-2019 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using System;
using UnityEngine;
using UnityEngine.UI;

namespace SpatialMap_SparseSpatialMap
{
    public class MapCellController : MonoBehaviour
    {
        public Button DeleteButton;

        private Text text;
        private Image sprite;

        public event Action PointerDown;
        public event Action Delete;

        public bool Selected { get; private set; }
        public MapMeta MapMeta { get; private set; }

        public void Awake()
        {
            text = GetComponentInChildren<Text>();
            sprite = GetComponent<Image>();
            Cancel();
        }

        public void SetData(MapMeta meta)
        {
            MapMeta = meta;
            text.text = meta.Map.Name;
        }

        public void OnPointerDown()
        {
            if (Selected)
            {
                Cancel();
            }
            else
            {
                Selete();
            }
            if (PointerDown != null)
            {
                PointerDown();
            }
        }

        public virtual void OnDelete()
        {
            if (Delete != null)
            {
                Delete();
            }
        }

        private void Selete()
        {
            sprite.color = new Color(0.2f, 0.58f, 0.99f);
            text.color = new Color(1, 1, 1);
            DeleteButton.gameObject.SetActive(true);
            Selected = true;
        }

        private void Cancel()
        {
            sprite.color = new Color(1, 1, 1);
            text.color = new Color(0.2f, 0.58f, 0.99f);
            DeleteButton.gameObject.SetActive(false);
            Selected = false;
        }
    }
}
