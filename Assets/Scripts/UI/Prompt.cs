using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Prompt : MonoBehaviour
{

    public GameObject libraryRelated;

    public void ShowPrompt()
    {
        libraryRelated.SetActive(false);
        gameObject.SetActive(true);
    }

    public void HidePrompt()
    {
        libraryRelated.SetActive(true);
        gameObject.SetActive(false);
    }

    public void OnConfirm()
    {
        HidePrompt();
    }
    public void OnCancel()
    {
        HidePrompt();
    }

}
