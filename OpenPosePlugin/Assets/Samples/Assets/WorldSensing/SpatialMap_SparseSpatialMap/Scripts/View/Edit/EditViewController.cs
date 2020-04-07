//================================================================================================================================
//
//  Copyright (c) 2015-2019 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using System.Collections.Generic;
using UnityEngine;

namespace SpatialMap_SparseSpatialMap
{
    public class EditViewController : MonoBehaviour
    {
        public Dragger PropDragger;
        public GameObject Tips;

        private MapSession.MapData mapData;
        private bool isTipsOn;

        private void Awake()
        {
            PropDragger.CreateObject += (gameObj) =>
            {
                if (gameObj)
                {
                    gameObj.transform.parent = mapData.Controller.transform;
                    mapData.Props.Add(gameObj);
                }
            };
            PropDragger.DeleteObject += (gameObj) =>
            {
                if (gameObj)
                {
                    mapData.Props.Remove(gameObj);
                }
            };
        }

        private void OnEnable()
        {
            Tips.SetActive(false);
            isTipsOn = false;
        }

        public void SetMapSession(MapSession session)
        {
            mapData = session.Maps[0];
            PropDragger.SetMapSession(session);
        }

        public void ShowTips()
        {
            isTipsOn = !isTipsOn;
            Tips.SetActive(isTipsOn);
        }

        public void Save()
        {
            if (mapData == null)
            {
                return;
            }

            var propInfos = new List<MapMeta.PropInfo>();

            foreach (var prop in mapData.Props)
            {
                var position = prop.transform.localPosition;
                var rotation = prop.transform.localRotation;
                var scale = prop.transform.localScale;

                propInfos.Add(new MapMeta.PropInfo()
                {
                    Name = prop.name,
                    Position = new float[3] { position.x, position.y, position.z },
                    Rotation = new float[4] { rotation.x, rotation.y, rotation.z, rotation.w },
                    Scale = new float[3] { scale.x, scale.y, scale.z }
                });
            }
            mapData.Meta.Props = propInfos;

            MapMetaManager.Save(mapData.Meta);
        }
    }
}
