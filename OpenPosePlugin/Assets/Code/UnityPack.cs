using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sample
{
    [Serializable]
    public class UnityPack
    {
        [Serializable]
        public class PartPropriedades
        {
            [SerializeField]
            public int Id { get; set; }
            [SerializeField]
            public string Cor { get; set; }
            [SerializeField]
            public double TimeOutoFSign { get; set; }
        }

        public int InSign = -1;
        [SerializeField]
        public List<PartPropriedades> Propriedades = new List<PartPropriedades>();
    }
}