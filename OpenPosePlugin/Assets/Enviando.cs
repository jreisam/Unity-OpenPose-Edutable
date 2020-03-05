using Sample;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enviando : MonoBehaviour
{
    private string numParticipants = "";
    public float cooldownTime = 1;
    private float next = 0;
    private MyJson objetoJson = new MyJson();
    void Start()
    {
        objetoJson.Msg = "unity_number_participants";
        objetoJson.Value = "0";
    }
    void Update()
    {
        if (Time.time > next)
        {
            numParticipants = GameObject.Find("People").GetComponent<Text>().text;
            numParticipants = numParticipants.Remove(0, 7);
            objetoJson.Value = numParticipants;


            Debug.Log(numParticipants);
            //string json = JsonUtility.ToJson(objetoJson);
            //Connect.Instance.socket.Emit(json);
            Connect.Instance.socket.EmitJson(objetoJson.Msg, objetoJson.Value);

            //Connect.Instance.socket.EmitJson("number_participants", @"{ ""number_participants"": " + numParticipants + " }");
            this.gameObject.GetComponent<TextMesh>().text = "Enviando p/ frondEnd..";
            next = Time.time + cooldownTime;
        }
        else
            this.gameObject.GetComponent<TextMesh>().text = "";


    }
}

class MyJson
{
    string msg;
    string value;

    public string Msg
    {
        get
        {
            return msg;
        }

        set
        {
            msg = value;
        }
    }

    public string Value
    {
        get
        {
            return value;
        }

        set
        {
            this.value = value;
        }
    }
}
