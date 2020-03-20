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
    void Update()
    {
        //if (Time.time > next)
        //{
        //    numParticipants = GameObject.Find("People").GetComponent<Text>().text;
        //    numParticipants = numParticipants.Remove(0, 7);


        //    Debug.Log(numParticipants);
        //    //string json = JsonUtility.ToJson(objetoJson);
        //    //Connect.Instance.socket.Emit(json);
        //    Connect.Instance.socket.EmitJson("unity_number_participants", numParticipants);

        //    //Connect.Instance.socket.EmitJson("number_participants", @"{ ""number_participants"": " + numParticipants + " }");
        //    GameObject.Find("People").GetComponent<Text>().text = "Enviando p/ frondEnd..";
        //    next = Time.time + cooldownTime;
        //}
        //else
        //    GameObject.Find("People").GetComponent<Text>().text = "";


    }
}

