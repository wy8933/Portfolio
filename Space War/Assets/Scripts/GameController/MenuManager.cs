using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    public void StartButton() {
        SceneManager.LoadScene("Plane");
    }

    public void InstructionButton() { 
    
    }
}
