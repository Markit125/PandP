using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Start : MonoBehaviour
{
    public GameObject Welcome, StartButton, Header;

    public void OnClick()
    {
        StartCoroutine(Bigger());
    }

    IEnumerator Bigger()
    {
        
        while (Welcome.GetComponent<Text>().fontSize < 300)
        {
            if (Welcome.GetComponent<Text>().fontSize > 20)
            {
                if (!GetComponent<Text>().enabled)
                    GetComponent<Text>().enabled = true;
                GetComponent<Transform>().localScale += new Vector3(0.2f, 0.2f, 0);
                GetComponent<Text>().fontSize += 2;
            }
            Header.GetComponent<Transform>().position += new Vector3(0f, 0.55f, 0);
            StartButton.GetComponent<Transform>().position += new Vector3(0.55f, 0f, 0);
            Welcome.GetComponent<Transform>().localScale += new Vector3(0.05f, 0.05f, 0);
            Welcome.GetComponent<Text>().fontSize += 5;
            yield return new WaitForSeconds(0.01f);
        }
        SceneManager.LoadScene("Welcome 1");
    }
}
