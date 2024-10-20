using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Escalada : MonoBehaviour
{
    private bool Terminei;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FinishedEscalada()
    {   
        SceneManager.LoadScene("Game");
        Debug.Log("teste");
    }
}
