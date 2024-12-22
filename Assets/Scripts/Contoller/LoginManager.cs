using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public string username = "admin";
    public string password = "dcbolt2024";

    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    public void Login()
    {
        if (usernameInput.text == username && passwordInput.text == password)
        {
            SceneManager.LoadScene("ResettingScene");
            print("next scene");
        }
        else
        {
            print("Wrong password");
        }
    }
}
