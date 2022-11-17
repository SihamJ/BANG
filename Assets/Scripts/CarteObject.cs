using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarteObject : MonoBehaviour
{
    public Image image;
    Carte carte;

    public void init(Sprite s,Carte carte)
    {
        image.sprite = s;
        this.carte = carte;
    }

}
