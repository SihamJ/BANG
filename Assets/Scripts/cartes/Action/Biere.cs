using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Biere : Carte
{
    int nombre;
    string figure;
    bool multi;
	int sprite;

    public Biere(int nombre, string figure, int sprite)
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
        return "Bière";
    }

    public string getTypeCarte()
    {
        return "Action";
    }

    public bool getMulti()
    {
        return multi;
    }


    public void action(int j1, int j2, ref List<Carte> defausse, ref List<Carte> pioche, ref List<GameObject> players, ref Text scene, ref Text historique)
    {
        int index;
        Debug.Log("Erreur num 1");
        GameControler gameControler = GameObject.Find("Game Controler - Lancer Partie").GetComponent<GameControler>();

        if (players[j1].GetComponent<Joueur>().getVie() == 5){
            scene.text = players[j1].GetComponent<Joueur>().getPseudo() +" vous avez 5 points de vie, vous ne pouvez pas boire une Bière.";

            if(j1 == 0 && gameControler.est_mort == false)
                players[j1].GetComponent<Joueur>().Mise_a_jour_carte();
        }
        else
        {
            if (j1 == 0 && gameControler.est_mort == false)
            {
                Debug.Log("vous êtes le joueur 1");
                index = players[j1].GetComponent<Joueur>().indexCarte(this.getNomCarte());
                players[j1].GetComponent<Joueur>().main.RemoveAt(index);
                Debug.Log("Fini");
            }

            defausse.Add(pioche[(pioche.Count) - 1]);
            pioche.RemoveAt((pioche.Count)-1);
            Debug.Log("Erreur num 2");
            players[j1].GetComponent<Joueur>().setVie(players[j1].GetComponent<Joueur>().getVie() + 1);
            Debug.Log("Erreur num 3");

            scene.text = players[j1].GetComponent<Joueur>().getPseudo() +" a bu une Bière. Il possède désormais" + players[j1].GetComponent<Joueur>().getVie().ToString() + " points de vie";
            players[j1].GetComponent<Joueur>().setScorePartie(players[j1].GetComponent<Joueur>().getScorePartie()+1);
            historique.text += "\n\n-"+scene.text;
        }
    }

    public string getDescription()
    {
        return "temporaire";
    }

}

