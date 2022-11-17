using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lunette :  Carte
{
    int nombre;
    string figure;
    bool multi;

	int sprite;
    public string getDescription()
    {
        return "temporaire";
    }

    public Lunette(int nombre, string figure, int sprite)
    {
        this.nombre = nombre;
        this.figure = figure;
        this.multi = false;
		this.sprite = sprite;
    }
	
	public int getSprite()
	{
		return this.sprite;
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
        return "Lunette";
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
        int index;

        GameControler gameControler = GameObject.Find("Game Controler - Lancer Partie").GetComponent<GameControler>();

        if (players[j1].GetComponent<Joueur>().possedePlateau(this.getNomCarte()))
        {
            if (j1 == 0)
            {
                scene.text = players[j1].GetComponent<Joueur>().getPseudo() + ", vous ne pouvez pas avoir deux fois le même objet sur le plateau.";
                historique.text += "\n\n"+scene.text;
                players[j1].GetComponent<Joueur>().Mise_a_jour_carte();
            }
        }
        else
        {
            if (j1 == 0 && gameControler.est_mort == false)
            {
                index = players[j1].GetComponent<Joueur>().indexCarte(this.getNomCarte());
                players[j1].GetComponent<Joueur>().main.RemoveAt(index);
            }

            players[j1].GetComponent<Joueur>().plateau.Add(pioche[pioche.Count-1]);
            pioche.RemoveAt(pioche.Count-1);
            scene.text = players[j1].GetComponent<Joueur>().getPseudo() + " possède désormais une lunette, il voit donc tout les joueurs à une distance réduite de 1";
            historique.text += "\n\n-"+scene.text;
            
        }
    }

}
