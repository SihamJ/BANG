using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Prison : Carte
{
    int nombre;
    string figure;
    bool multi;

	int sprite;
   

    public Prison(int nombre, string figure, int sprite)
    {
        this.nombre = nombre;
        this.figure = figure;
        this.multi = true;
		this.sprite = sprite;
    }

	public int getSprite()
	{
		return this.sprite;
	}

    public string getDescription()
    {
        return "temporaire";
    }

    public int getNombre()
    {
        return nombre;
    }

    public string getFigure()
    {
        return figure;
    }

    public string getNomCarte()
    {
        return "Prison";
    }

    public string getTypeCarte()
    {
        return "Objet";
    }

    public bool getMulti()
    {
        return multi;
    }

    public void action(int j1, int j2, ref List<Carte> defausse, ref List<Carte> pioche, ref List<GameObject> players, ref Text scene, ref Text historique)
    {
        Debug.Log("Vous recevez  0?");
        int index;
        GameControler gameControler = GameObject.Find("Game Controler - Lancer Partie").GetComponent<GameControler>();

        if (players[j2].GetComponent<Joueur>().possedePlateau(this.getNomCarte()))
        {
            if (j1 == 0 && gameControler.est_mort == false)
            {
                scene.text = players[j2].GetComponent<Joueur>().getPseudo() + ", vous ne pouvez pas avoir deux fois le même objet sur le plateau.";
                players[j1].GetComponent<Joueur>().Mise_a_jour_carte();
            }
        }
        else
        {
            Debug.Log("Vous recevez  1?");
            if (j1 == 0 && gameControler.est_mort == false)
            {
                index = players[j1].GetComponent<Joueur>().indexCarte(this.getNomCarte());
                players[j1].GetComponent<Joueur>().main.RemoveAt(index);
            }
            Debug.Log("Vous recevez  2?");

            players[j2].GetComponent<Joueur>().plateau.Add(pioche[(pioche.Count)-1]);
            pioche.RemoveAt((pioche.Count)-1);
            scene.text = players[j1].GetComponent<Joueur>().getPseudo() + " a placé une prison sur " + players[j2].GetComponent<Joueur>().getPseudo();
            historique.text += "\n\n-"+scene.text;
        }

    }
}

