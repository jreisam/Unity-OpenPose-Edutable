using Newtonsoft.Json;
using Sample;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

public class VuforiaReader : MonoBehaviour
{
    public GameObject numParticipantesUI;

    public Sprite spriteHero01;
    public Sprite spriteHero02;
    public Sprite spriteHero03;
    public Sprite spriteHero04;
    public GameObject part01;
    public GameObject part02;
    public GameObject part03;
    public GameObject part04;

    public GameObject avatar01;
    public GameObject avatar02;
    public GameObject avatar03;
    public GameObject avatar04;

    private int numeroParticipantes = -1;
    private bool part1set = false;
    private bool part2set = false;
    private bool part3set = false;
    private bool part4set = false;
    private bool lendo01 = false;
    private bool lendo02 = false;
    private bool lendo03 = false;
    private bool lendo04 = false;
    public float part01TimmingOut = 0f;
    public float part02TimmingOut = 0f;
    public float part03TimmingOut = 0f;
    public float part04TimmingOut = 0f;
    public float timeOutLimit = 5f;
    private float nextCheck = 0f;
    private float coolDownTime = 1f;

    private UnityPack ListaParticipantes = new UnityPack();
    private int participantesIdentificados = 0;

    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        for (int i = 0; i < devices.Length; i++)
            Debug.Log(devices[i].name);
    }

    private void Update()
    {
        // Reader
        lendo01 = avatar01.GetComponent<MeshRenderer>().enabled ? true : false;
        lendo02 = avatar02.GetComponent<MeshRenderer>().enabled ? true : false;
        lendo03 = avatar03.GetComponent<MeshRenderer>().enabled ? true : false;
        lendo04 = avatar04.GetComponent<MeshRenderer>().enabled ? true : false;


        // timmers
        part01TimmingOut += 1f * Time.deltaTime;
        part02TimmingOut += 1f * Time.deltaTime;
        part03TimmingOut += 1f * Time.deltaTime;
        part04TimmingOut += 1f * Time.deltaTime;


        try
        {

            if (part1set == true)
            {
                if (part01TimmingOut >= timeOutLimit)
                    part01.GetComponentInChildren<Text>().color = new Color(255, 0, 0);
                else
                {
                    part01.GetComponentInChildren<Text>().color = new Color(255, 255, 255);
                }

                part01.GetComponentInChildren<Text>().text = part01TimmingOut.ToString("F2");
                ListaParticipantes.Propriedades.Where(x => x.HeroName == "hero01").FirstOrDefault().TimeOutoFSign = part01TimmingOut;
            }

            if (part2set == true)
            {
                if (part02TimmingOut >= timeOutLimit)
                    part02.GetComponentInChildren<Text>().color = new Color(255, 0, 0);
                else
                {
                    part02.GetComponentInChildren<Text>().color = new Color(255, 255, 255);
                }

                part02.GetComponentInChildren<Text>().text = part02TimmingOut.ToString("F2");
                ListaParticipantes.Propriedades.Where(x => x.HeroName == "hero02").FirstOrDefault().TimeOutoFSign = part02TimmingOut;
            }

            if (part3set == true)
            {
                if (part04TimmingOut >= timeOutLimit)
                    part03.GetComponentInChildren<Text>().color = new Color(255, 0, 0);
                else
                {
                    part03.GetComponentInChildren<Text>().color = new Color(255, 255, 255);
                }

                part03.GetComponentInChildren<Text>().text = part04TimmingOut.ToString("F2");
                ListaParticipantes.Propriedades.Where(x => x.HeroName == "hero03").FirstOrDefault().TimeOutoFSign = part04TimmingOut;
            }

            if (part4set == true)
            {
                if (part03TimmingOut >= timeOutLimit)
                    part04.GetComponentInChildren<Text>().color = new Color(255, 0, 0);
                else
                {
                    part04.GetComponentInChildren<Text>().color = new Color(255, 255, 255);
                }

                part04.GetComponentInChildren<Text>().text = part03TimmingOut.ToString("F2");
                ListaParticipantes.Propriedades.Where(x => x.HeroName == "hero04").FirstOrDefault().TimeOutoFSign = part03TimmingOut;
            }

            if (lendo01)
            {
                part01TimmingOut = 0;
                if (part1set != true)
                {
                    participantesIdentificados++;
                    part01.GetComponent<Image>().sprite = spriteHero01;
                    ListaParticipantes.Propriedades.Add(new UnityPack.PartPropriedades { Id = participantesIdentificados, HeroName = "hero01", TimeOutoFSign = part01TimmingOut });
                    part01.GetComponent<Image>().enabled = part1set = true;
                }
            }
            if (lendo02)
            {
                part02TimmingOut = 0;
                if (part2set != true)
                {
                    participantesIdentificados++;
                    part02.GetComponent<Image>().sprite = spriteHero02;
                    ListaParticipantes.Propriedades.Add(new UnityPack.PartPropriedades { Id = participantesIdentificados, HeroName = "hero02", TimeOutoFSign = part02TimmingOut });
                    part02.GetComponent<Image>().enabled = part2set = true;
                }
            }
            if (lendo03)
            {
                part04TimmingOut = 0;
                if (part3set != true)
                {
                    participantesIdentificados++;
                    part03.GetComponent<Image>().sprite = spriteHero04;
                    ListaParticipantes.Propriedades.Add(new UnityPack.PartPropriedades { Id = participantesIdentificados, HeroName = "hero03", TimeOutoFSign = part04TimmingOut });
                    part03.GetComponent<Image>().enabled = part3set = true;
                }
            }

            if (lendo04)
            {
                part03TimmingOut = 0;
                if (part4set != true)
                {
                    participantesIdentificados++;
                    part04.GetComponent<Image>().sprite = spriteHero03;
                    ListaParticipantes.Propriedades.Add(new UnityPack.PartPropriedades { Id = participantesIdentificados, HeroName = "hero04", TimeOutoFSign = part03TimmingOut });
                    part04.GetComponent<Image>().enabled = part4set = true;
                }
            }

            // # InSign
            ListaParticipantes.InSign = ListaParticipantes.Propriedades.Where(x => x.TimeOutoFSign < 1.0).Count();
            numParticipantesUI.GetComponent<Text>().text = ListaParticipantes.InSign.ToString();
            Debug.Log("insign: " + ListaParticipantes.InSign);

        }
        catch (Exception ex) { Debug.LogWarning(ex.Message); }

        // Enviando para frontEnd
        if (Time.time > nextCheck)
        {
            var jsonJsonsoft = JsonConvert.SerializeObject(ListaParticipantes);
            Connect.Instance.socket.EmitJson("unity", jsonJsonsoft);
            nextCheck = Time.time + coolDownTime;
        }
    }

    private static Color32[] Encode(string textForEncoding, int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };
        return writer.Write(textForEncoding);
    }

    public Texture2D generateQR(string text)
    {
        var encoded = new Texture2D(256, 256);
        var color32 = Encode(text, encoded.width, encoded.height);
        encoded.SetPixels32(color32);
        encoded.Apply();
        return encoded;
    }


}
