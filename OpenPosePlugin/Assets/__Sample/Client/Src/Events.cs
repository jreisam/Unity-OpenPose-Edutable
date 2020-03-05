using UnityEngine;
using socket.io;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Sample {
    
    /// <summary>
    /// The basic sample to show how to send and receive messages.
    /// </summary>
    public class Events : MonoBehaviour {
        private string dataResponse;

        private void Update()
        {
            var socket = Connect.Instance.socket;

            socket.On("hello", (string data) => {
             
                Debug.Log("hello recebido: " + data);
                //// Emit raw string data
                //socket.Emit("my other event", "{ my: data }");

                //// Emit json-formatted string data
                //socket.EmitJson("my other event", @"{ ""my"": ""data"" }");
            });
        }

        public void EnviarNumPessoas()
        {
            //// Emit raw string data
            Connect.Instance.socket.Emit("number_participants", GameObject.Find("People").GetComponent<Text>().text);
            Connect.Instance.socket.EmitJson("number_participants", @"{ ""number_participants"": "+ GameObject.Find("People").GetComponent<Text>().text + " }");
        }

    }

}