using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenMe : MonoBehaviour
{
    public Animation anim;
    public Text[] texts;
    
    private bool isOpen;
    private bool isUsing;
    private bool isLooked;

    public void Use()
    {
        if (!isOpen)
            Open();
        else
            Close();
    }

    private void Open()
    {
        
        if (!isUsing)
            anim["Door"].time  = 0;
        
        anim["Door"].speed = 1;
        anim.Play();
        isOpen = true;
    }

    private void Close()
    {
        if (!isUsing)
            anim["Door"].time = anim["Door"].length;
            
        anim["Door"].speed = -1;
        anim.Play();
        isOpen = false;
    }

    public void Opened()
    {
        isUsing = !isUsing;     
        foreach (var text in texts)
            text.text = "CLOSE";
    }

    public void Closed()
    {
        isUsing = !isUsing;  
        foreach (var text in texts)
            text.text = "OPEN";
    }


}
