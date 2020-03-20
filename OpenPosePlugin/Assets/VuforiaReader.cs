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
    public Sprite spriteHero01;
    public Sprite spriteHero02;
    public Sprite spriteHero03;
    public Sprite spriteHero04;
    public Sprite spriteHero05;
    public Sprite spriteHero06;
    public GameObject part01;
    public GameObject part02;
    public GameObject part03;
    public GameObject part04;
    public GameObject part05;
    public GameObject part06;

    public GameObject avatarHero01;
    public GameObject avatarHero02;
    public GameObject avatarHero03;
    public GameObject avatarHero04;
    public GameObject avatarHero05;
    public GameObject avatarHero06;

    private int numeroParticipantes = -1;
    private bool part1set = false;
    private bool part2set = false;
    private bool part3set = false;
    private bool part4set = false;
    private bool part5set = false;
    private bool part6set = false;
    private bool lendoHero01 = false;
    private bool lendoHero02 = false;
    private bool lendoHero03 = false;
    private bool lendoHero04 = false;
    private bool lendoHero05 = false;
    private bool lendoHero06 = false;
    public float hero01TimmingOut = 0f;
    public float hero02TimmingOut = 0f;
    public float hero03TimmingOut = 0f;
    public float hero04TimmingOut = 0f;
    public float hero05TimmingOut = 0f;
    public float hero06TimmingOut = 0f;
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
        lendoHero01 = avatarHero01.GetComponent<MeshRenderer>().enabled ? true :false;
        lendoHero02 = avatarHero02.GetComponent<MeshRenderer>().enabled ? true : false;
        lendoHero03 = avatarHero03.GetComponent<MeshRenderer>().enabled ? true : false;
        lendoHero04 = avatarHero04.GetComponent<MeshRenderer>().enabled ? true : false;
        lendoHero05 = avatarHero05.GetComponent<MeshRenderer>().enabled ? true : false;
        lendoHero06 = avatarHero06.GetComponent<MeshRenderer>().enabled ? true : false;



        // timmers
        hero01TimmingOut += 1f * Time.deltaTime;
        hero02TimmingOut += 1f * Time.deltaTime;
        hero03TimmingOut += 1f * Time.deltaTime;
        hero04TimmingOut += 1f * Time.deltaTime;
        hero05TimmingOut += 1f * Time.deltaTime;
        hero06TimmingOut += 1f * Time.deltaTime;


        try
        {
            // N# participantes 
            numeroParticipantes = Convert.ToInt32(GameObject.Find("People").GetComponent<Text>().text.Remove(0, 7));
            Debug.Log(numeroParticipantes);

            if (part1set == true)
            {
                if (hero01TimmingOut >= timeOutLimit)
                    part01.GetComponentInChildren<Text>().color = new Color(255, 0, 0);
                else
                {
                    part01.GetComponentInChildren<Text>().color = new Color(255, 255, 255);
                }

                part01.GetComponentInChildren<Text>().text = hero01TimmingOut.ToString("F2");
                ListaParticipantes.Propriedades.Where(x => x.Cor == "green").FirstOrDefault().TimeOutoFSign = hero01TimmingOut;
            }

            if (part2set == true)
            {
                if (hero02TimmingOut >= timeOutLimit)
                    part02.GetComponentInChildren<Text>().color = new Color(255, 0, 0);
                else
                {
                    part02.GetComponentInChildren<Text>().color = new Color(255, 255, 255);
                }

                part02.GetComponentInChildren<Text>().text = hero02TimmingOut.ToString("F2");
                ListaParticipantes.Propriedades.Where(x => x.Cor == "orange").FirstOrDefault().TimeOutoFSign = hero02TimmingOut;
            }

            if (part3set == true)
            {
                if (hero04TimmingOut >= timeOutLimit)
                    part03.GetComponentInChildren<Text>().color = new Color(255, 0, 0);
                else
                {
                    part03.GetComponentInChildren<Text>().color = new Color(255, 255, 255);
                }

                part03.GetComponentInChildren<Text>().text = hero04TimmingOut.ToString("F2");
                ListaParticipantes.Propriedades.Where(x => x.Cor == "violet").FirstOrDefault().TimeOutoFSign = hero04TimmingOut;
            }

            if (part4set == true)
            {
                if (hero03TimmingOut >= timeOutLimit)
                    part04.GetComponentInChildren<Text>().color = new Color(255, 0, 0);
                else
                {
                    part04.GetComponentInChildren<Text>().color = new Color(255, 255, 255);
                }

                part04.GetComponentInChildren<Text>().text = hero03TimmingOut.ToString("F2");
                ListaParticipantes.Propriedades.Where(x => x.Cor == "yellow").FirstOrDefault().TimeOutoFSign = hero03TimmingOut;
            }

            if (lendoHero01)
            {
                hero01TimmingOut = 0;
                if (part1set == true)
                    return;
                participantesIdentificados++;
                part01.GetComponent<Image>().sprite = spriteHero01;
                ListaParticipantes.Propriedades.Add(new UnityPack.PartPropriedades { Id = participantesIdentificados, Cor = "green", TimeOutoFSign = hero01TimmingOut });
                part01.GetComponent<Image>().enabled = part1set = true;
            }
            if (lendoHero02)
            {
                hero02TimmingOut = 0;
                if (part2set == true)
                    return;
                participantesIdentificados++;
                part02.GetComponent<Image>().sprite = spriteHero02;
                ListaParticipantes.Propriedades.Add(new UnityPack.PartPropriedades { Id = participantesIdentificados, Cor = "orange", TimeOutoFSign = hero02TimmingOut });
                part02.GetComponent<Image>().enabled = part2set = true;
            }
            if (lendoHero03)
            {
                hero04TimmingOut = 0;
                if (part3set == true)
                    return;
                participantesIdentificados++;
                part03.GetComponent<Image>().sprite = spriteHero04;
                ListaParticipantes.Propriedades.Add(new UnityPack.PartPropriedades { Id = participantesIdentificados, Cor = "violet", TimeOutoFSign = hero04TimmingOut });
                part03.GetComponent<Image>().enabled = part3set = true;
            }

            if (lendoHero04)
            {
                hero03TimmingOut = 0;
                if (part4set == true)
                    return;
                participantesIdentificados++;
                part04.GetComponent<Image>().sprite = spriteHero03;
                ListaParticipantes.Propriedades.Add(new UnityPack.PartPropriedades { Id = participantesIdentificados, Cor = "yellow", TimeOutoFSign = hero03TimmingOut });
                part04.GetComponent<Image>().enabled = part4set = true;
            }


            ListaParticipantes.InSign = numeroParticipantes;


            nextCheck = Time.time + coolDownTime;
        }
        catch (Exception ex) { Debug.LogWarning(ex.Message); }

        if (Time.time > nextCheck)
        {               // Enviando para frontEnd
            var jsonJsonsoft = JsonConvert.SerializeObject(ListaParticipantes);
            Connect.Instance.socket.EmitJson("unity", jsonJsonsoft);
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
