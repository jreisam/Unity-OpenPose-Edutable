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

public class QRCodeReader : MonoBehaviour
{
    public Sprite spriteVerde;
    public Sprite spriteLaranja;
    public Sprite spriteAmarelo;
    public Sprite spriteRoxo;
    public GameObject participanteVerde;
    public GameObject participanteLaranja;
    public GameObject participanteRoxo;
    public GameObject participanteAmarelo;

    private int numParticipants = -1;
    private bool part1set = false;
    private bool part2set = false;
    private bool part3set = false;
    private bool part4set = false;
    public float verdeTimmingOut = 0f;
    public float laranjaTimmingOut = 0f;
    public float amareloTimmingOut = 0f;
    public float roxoTimmingOut = 0f;
    public float timeOutLimit = 5f;
    private float nextCheck = 0f;
    private float coolDownTime = 1f;
    private WebCamTexture camTexture;
    private WebCamTexture camTextureB;
    private Rect screenRect;
    private Rect screenRectB;
    private string textReceived;
    private IBarcodeReader barcodeReader = new BarcodeReader();
    private UnityPack ListaParticipantes = new UnityPack();
    private int identificados = 0;

    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        for (int i = 0; i < devices.Length; i++)
            Debug.Log(devices[i].name);

        screenRect = new Rect(0, 0, Screen.width, Screen.height);
        camTexture = new WebCamTexture(devices[1].name);
        camTexture.requestedHeight = Screen.height;
        camTexture.requestedWidth = Screen.width;
        if (camTexture != null)
        {
            camTexture.Play();
        }

        camTextureB = new WebCamTexture(devices[3].name);
        screenRectB = new Rect(0, 0, Screen.width, Screen.height);
        camTextureB.requestedHeight = Screen.height;
        camTextureB.requestedWidth = Screen.width;
        if (camTextureB != null)
        {
            camTextureB.Play();
        }

    }

    private void Update()
    {
        // QR Code decoder
        var leituraQRCodeA = barcodeReader.Decode(camTexture.GetPixels32(),
          camTexture.width, camTexture.height);
        if (leituraQRCodeA != null)
            Debug.Log("DECODED QR A: " + leituraQRCodeA.Text);
        var leituraQRCodeB = barcodeReader.Decode(camTextureB.GetPixels32(),
          camTextureB.width, camTextureB.height);
        if (leituraQRCodeB != null)
            Debug.Log("DECODED QR B: " + leituraQRCodeB.Text);

        if (Time.time > nextCheck)
        {
            try
            {
                // N# participantes 
                numParticipants = Convert.ToInt32(GameObject.Find("People").GetComponent<Text>().text.Remove(0, 7));
                Debug.Log(numParticipants);

                // timmers
                verdeTimmingOut += Mathf.Clamp(1f * Time.deltaTime, 0, 100);
                laranjaTimmingOut += Mathf.Clamp(1f * Time.deltaTime, 0, 10);
                roxoTimmingOut += 1f * Time.deltaTime;
                amareloTimmingOut += 1f * Time.deltaTime;

                if (part1set == true)
                {
                    if (verdeTimmingOut >= timeOutLimit)
                        participanteVerde.GetComponentInChildren<Text>().color = new Color(255, 0, 0);
                    else
                    {
                        participanteVerde.GetComponentInChildren<Text>().color = new Color(255, 255, 255);
                    }

                    participanteVerde.GetComponentInChildren<Text>().text = verdeTimmingOut.ToString();
                    ListaParticipantes.Propriedades.Where(x => x.Cor == "green").FirstOrDefault().TimeOutoFSign = verdeTimmingOut;
                }

                if (part2set == true)
                {
                    if (laranjaTimmingOut >= timeOutLimit)
                        participanteLaranja.GetComponentInChildren<Text>().color = new Color(255, 0, 0);
                    else
                    {
                        participanteLaranja.GetComponentInChildren<Text>().color = new Color(255, 255, 255);
                    }

                    participanteLaranja.GetComponentInChildren<Text>().text = laranjaTimmingOut.ToString();
                    ListaParticipantes.Propriedades.Where(x => x.Cor == "orange").FirstOrDefault().TimeOutoFSign = laranjaTimmingOut;
                }

                if (part3set == true)
                {
                    if (roxoTimmingOut >= timeOutLimit)
                        participanteRoxo.GetComponentInChildren<Text>().color = new Color(255, 0, 0);
                    else
                    {
                        participanteRoxo.GetComponentInChildren<Text>().color = new Color(255, 255, 255);
                    }

                    participanteRoxo.GetComponentInChildren<Text>().text = roxoTimmingOut.ToString();
                    ListaParticipantes.Propriedades.Where(x => x.Cor == "violet").FirstOrDefault().TimeOutoFSign = roxoTimmingOut;
                }

                if (part4set == true)
                {
                    if (amareloTimmingOut >= timeOutLimit)
                        participanteAmarelo.GetComponentInChildren<Text>().color = new Color(255, 0, 0);
                    else
                    {
                        participanteAmarelo.GetComponentInChildren<Text>().color = new Color(255, 255, 255);
                    }

                    participanteAmarelo.GetComponentInChildren<Text>().text = amareloTimmingOut.ToString();
                    ListaParticipantes.Propriedades.Where(x => x.Cor == "yellow").FirstOrDefault().TimeOutoFSign = amareloTimmingOut;
                }

                // QRCode leu algo
                if (leituraQRCodeA != null)
                {

                    switch (leituraQRCodeA.Text)
                    {
                        case "Verde":
                            verdeTimmingOut = 0;
                            if (part1set == true)
                                break;
                            identificados++;
                            participanteVerde.GetComponent<Image>().sprite = spriteVerde;
                            ListaParticipantes.Propriedades.Add(new UnityPack.PartPropriedades { Id = identificados, Cor = "green", TimeOutoFSign = verdeTimmingOut });
                            participanteVerde.GetComponent<Image>().enabled = part1set = true;
                            break;
                        case "Laranja":
                            laranjaTimmingOut = 0;
                            if (part2set == true)
                                break;
                            identificados++;
                            participanteLaranja.GetComponent<Image>().sprite = spriteLaranja;
                            ListaParticipantes.Propriedades.Add(new UnityPack.PartPropriedades { Id = identificados, Cor = "orange", TimeOutoFSign = laranjaTimmingOut });
                            participanteLaranja.GetComponent<Image>().enabled = part2set = true;
                            break;
                        case "Roxo":
                            roxoTimmingOut = 0;
                            if (part3set == true)
                                break;
                            identificados++;
                            participanteRoxo.GetComponent<Image>().sprite = spriteRoxo;
                            ListaParticipantes.Propriedades.Add(new UnityPack.PartPropriedades { Id = identificados, Cor = "violet", TimeOutoFSign = roxoTimmingOut });
                            participanteRoxo.GetComponent<Image>().enabled = part3set = true;
                            break;
                        case "Amarelo":
                            amareloTimmingOut = 0;
                            if (part4set == true)
                                break;
                            identificados++;
                            participanteAmarelo.GetComponent<Image>().sprite = spriteAmarelo;
                            ListaParticipantes.Propriedades.Add(new UnityPack.PartPropriedades { Id = identificados, Cor = "yellow", TimeOutoFSign = amareloTimmingOut });
                            participanteAmarelo.GetComponent<Image>().enabled = part4set = true;

                            break;
                        default:
                            break;
                    }
                }

                if (leituraQRCodeB != null)
                {

                    switch (leituraQRCodeB.Text)
                    {
                        case "Verde":
                            verdeTimmingOut = 0;
                            if (part1set == true)
                                break;
                            identificados++;
                            participanteVerde.GetComponent<Image>().sprite = spriteVerde;
                            ListaParticipantes.Propriedades.Add(new UnityPack.PartPropriedades { Id = identificados, Cor = "green", TimeOutoFSign = verdeTimmingOut });
                            participanteVerde.GetComponent<Image>().enabled = part1set = true;
                            break;
                        case "Laranja":
                            laranjaTimmingOut = 0;
                            if (part2set == true)
                                break;
                            identificados++;
                            participanteLaranja.GetComponent<Image>().sprite = spriteLaranja;
                            ListaParticipantes.Propriedades.Add(new UnityPack.PartPropriedades { Id = identificados, Cor = "orange", TimeOutoFSign = laranjaTimmingOut });
                            participanteLaranja.GetComponent<Image>().enabled = part2set = true;
                            break;
                        case "Roxo":
                            roxoTimmingOut = 0;
                            if (part3set == true)
                                break;
                            identificados++;
                            participanteRoxo.GetComponent<Image>().sprite = spriteRoxo;
                            ListaParticipantes.Propriedades.Add(new UnityPack.PartPropriedades { Id = identificados, Cor = "violet", TimeOutoFSign = roxoTimmingOut });
                            participanteRoxo.GetComponent<Image>().enabled = part3set = true;
                            break;
                        case "Amarelo":
                            amareloTimmingOut = 0;
                            if (part4set == true)
                                break;
                            identificados++;
                            participanteAmarelo.GetComponent<Image>().sprite = spriteAmarelo;
                            ListaParticipantes.Propriedades.Add(new UnityPack.PartPropriedades { Id = identificados, Cor = "yellow", TimeOutoFSign = amareloTimmingOut });
                            participanteAmarelo.GetComponent<Image>().enabled = part4set = true;

                            break;
                        default:
                            break;
                    }
                }

                ListaParticipantes.InSign = numParticipants;
                // Enviando para frontEnd
                var jsonJsonsoft = JsonConvert.SerializeObject(ListaParticipantes);
                Connect.Instance.socket.EmitJson("unity", jsonJsonsoft);

                nextCheck = Time.time + coolDownTime;
            }
            catch (Exception ex) { Debug.LogWarning(ex.Message); }
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
