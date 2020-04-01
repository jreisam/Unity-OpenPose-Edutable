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
            public string HeroName { get; set; }
            [SerializeField]
            public string Cor { get; set; }
            [SerializeField]
            public double TimeOutoFSign { get; set; }
            [SerializeField]
            public bool IsIdle { get; set; }
            [SerializeField]
            public Vector3 LastPosition { get; set; }
            [SerializeField]
            public Vector3 CurrentPosition { get; set; }
            [SerializeField]
            public string ICanSeeYouPhrase { get; set; }
            [SerializeField]
            public string SeeYouPhrase { get; set; }
        }

        public int InSign = -1;

        public string ICanSeeYouPhraseTotal;

        public string SomeoneIsOutOfFramePhraseTotal;

        public string AllOfThemIsIdlePhraseTotal;

        [SerializeField]
        public List<PartPropriedades> Propriedades = new List<PartPropriedades>();
    }
}