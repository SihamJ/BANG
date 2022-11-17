using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Saloon : Carte
{
   int nombre;
   string figure;
   bool multi;

	int sprite;

    public string getDescription()
    {
        return "temporaire";
    }

    public Saloon(int nombre, string figure, int sprite)
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
   	return "Saloon";
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
        GameControler gameControler = GameObject.Find("Game Controler - Lancer Partie").GetComponent<GameControler>();

        if (j1 == 0 && gameControler.est_mort == false)
        {
            index = players[j1].GetComponent<Joueur>().indexCarte(this.getNomCarte());
            players[j1].GetComponent<Joueur>().main.RemoveAt(index);
        }

            foreach (GameObject joueur in players)
            {
                if (joueur.GetComponent<Joueur>().getVie() < joueur.GetComponent<Joueur>().getVieMax())
                    joueur.GetComponent<Joueur>().setVie(joueur.GetComponent<Joueur>().getVie() + 1);
            }

        defausse.Add(pioche[(pioche.Count)-1]);
       
        scene.text = players[j1].GetComponent<Joueur>().getPseudo() + " : Vous invite au Saloon ! Vous recuperez tous 1 point de vie, exceptés ceux ayant déja leurs vie maximum.";
        players[j1].GetComponent<Joueur>().setScorePartie(players[j1].GetComponent<Joueur>().getScorePartie()+2);
        historique.text += "\n\n-"+scene.text;
    }
}

