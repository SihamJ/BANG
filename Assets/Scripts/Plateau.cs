using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Plateau : MonoBehaviour
{
    [SerializeField] public Text pseudoText;
    [SerializeField] public Text vieText;
    [SerializeField] public Image imageArme;
    [SerializeField] public Image imagePerso;
    //[SerializeField] public Text cartes;
    [SerializeField] public List<Sprite> sprites; // 0-Dynamite  1-Lunette  2-Mustang   3-Planque   4-Prison   5-Volcanic  
                                                  // 6-Schofield  7-Remington   8-Carabine   9-Winchester   
    [SerializeField] public List<Image> icones;

    public void init(List<Carte> cartes)
    {
        int i = 0;

        foreach (var carte in cartes)
        {
            if (Equals(carte.getNomCarte(), "Dynamite"))
                icones[i].sprite = sprites[0];
            else if (Equals(carte.getNomCarte(), "Lunette"))
                icones[i].sprite = sprites[1];
            else if (Equals(carte.getNomCarte(), "Mustang"))
                icones[i].sprite = sprites[2];
            else if (Equals(carte.getNomCarte(), "Planque"))
                icones[i].sprite = sprites[3];
            else if (Equals(carte.getNomCarte(), "Prison"))
                icones[i].sprite = sprites[4];
            else if (Equals(carte.getNomCarte(), "Volcanic"))
                icones[i].sprite = sprites[5];
            else if (Equals(carte.getNomCarte(), "Schofield"))
                icones[i].sprite = sprites[6];
            else if (Equals(carte.getNomCarte(), "Remington"))
                icones[i].sprite = sprites[7];
            else if (Equals(carte.getNomCarte(), "Carabine"))
                icones[i].sprite = sprites[8];
            else if (Equals(carte.getNomCarte(), "Winchester"))
                icones[i].sprite = sprites[9];
            i++;
        }
    }


}
