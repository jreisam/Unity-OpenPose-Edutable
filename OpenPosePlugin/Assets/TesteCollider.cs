using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TesteCollider : MonoBehaviour
{
    public GameObject UITocou;
    // Start is called before the first frame update
    void Start()
    {
        UITocou.GetComponent<Text>().text = "nada";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("acertou o " + collision.gameObject.name);
        UITocou.GetComponent<Text>().text = "acertou o " + collision.gameObject.name;
    }
}
