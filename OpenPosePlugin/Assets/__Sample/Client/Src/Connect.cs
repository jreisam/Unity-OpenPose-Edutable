using UnityEngine;
using socket.io;
using System.Collections.Generic;

namespace Sample {

    /// <summary>
    /// The basic sample to show how to connect to a server
    /// </summary>
    public class Connect : MonoBehaviour {

        private static Connect instance;
        public string serverUrl = "http://localhost:8080";
        public Socket socket;
        private string dataResponse;
        public static Connect Instance
        {
            get
            {
                return instance;
            }
        }

        private void Awake()
        {

            socket = Socket.Connect(serverUrl);

            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Debug.Log("Multiple instances of " + this.GetType().Name + " #1", gameObject);
                Debug.Log("Multiple instances of " + instance.GetType().Name + " #2", instance.gameObject);
                Destroy(gameObject);
            }
            Debug.Log(gameObject.GetInstanceID());
        }

        void Start() {

            socket.On(SystemEvents.connect, () => {
                Debug.Log("Hello, Socket.io~");
            });

            socket.On(SystemEvents.reconnect, (int reconnectAttempt) => {
                Debug.Log("Hello, Again! " + reconnectAttempt);
            });

            socket.On(SystemEvents.disconnect, () => {
                Debug.Log("Bye~");
            });
        }

        private void Update()
        {
            //socket.On("hello", (string data) => {
            //    //Debug.Log(data);
            //    dataResponse = data;
            //    dataResponse = dataResponse.Remove(0, 2);
            //    dataResponse = dataResponse.Remove(dataResponse.Length - 2, 2);
            //    string[] parts = dataResponse.Split(',');

            //    int countPalavras = 0;
            //    foreach (var item in parts)
            //    {
            //        if (item == "255") { }
            //        // corresponde a nulo
            //        else
            //            countPalavras++;
            //    }
            //    List<string> list = new List<string>(parts);
            //    Debug.Log(list);
            //    //// Emit raw string data
            //    //socket.Emit("my other event", "{ my: data }");

            //    //// Emit json-formatted string data
            //    //socket.EmitJson("my other event", @"{ ""my"": ""data"" }");
            //});
        }

    }

}
