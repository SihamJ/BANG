using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Diligence : Carte
{
   int nombre;
   string figure;
   bool multi;
	int sprite;

    public string getDescription()
    {
        return "temporaire";
    }

    public Diligence(int nombre, string figure, int sprite)
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
   	return "Diligence";
   }
   
   public string getTypeCarte()
   {
   	return "Action";
   }
   
   public bool getMulti()
   {
	return multi;
   }

    //TO_DO
    public void action(int j1, int j2, ref List<Carte> defausse, ref List<Carte> pioche, ref List<GameObject> players, ref Text scene, ref Text historique)
    {
        GameControler gameControler = GameObject.Find("Game Controler - Lancer Partie").GetComponent<GameControler>();

        if (j1 == 0 &&  gameControler.est_mort == false)
        {
            int index = players[j1].GetComponent<Joueur>().indexCarte(this.getNomCarte());
            players[j1].GetComponent<Joueur>().piocher(ref pioche, ref defausse);
            players[j1].GetComponent<Joueur>().piocher(ref pioche, ref defausse);
            players[j1].GetComponent<Joueur>().piocher(ref pioche, ref defausse);
            defausse.Add(players[j1].GetComponent<Joueur>().main[index]);
            players[j1].GetComponent<Joueur>().main.RemoveAt(index);
        }
        //modifier le joueur directements et faire la défausse

        scene.text = players[j1].GetComponent<Joueur>().getPseudo() + " a posé une Diligence";
        historique.text += "\n\n-"+scene.text;

        //ajouter dans historique
        
    }
}
