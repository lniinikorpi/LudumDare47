using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyButton : MonoBehaviour
{
    public ItemButton itemButton;
    public void PressButton()
    {
        if(itemButton != null)
        {
            itemButton.BuyItem();
        }
    }
}
