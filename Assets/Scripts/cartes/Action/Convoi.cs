using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Convoi : Carte
{
   int nombre;
   string figure;
   bool multi;
	int sprite;

    public Convoi(int nombre, string figure, int sprite)
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
   	return "Convoi";
   }
   
   public string getTypeCarte()
   {
   	return "Action";
   }
   public string getDescription()
   {
        return "temporaire";
   }
    public bool getMulti()
   {
	return multi;
   }

    //TO_DO
    public void action(int j1, int j2, ref List<Carte> defausse, ref List<Carte> pioche, ref List<GameObject> players, ref Text scene, ref Text historique)
    {
        GameControler gameControler = GameObject.Find("Game Controler - Lancer Partie").GetComponent<GameControler>();

        if (j1 == 0 && gameControler.est_mort == false)
        {
            int index = players[j1].GetComponent<Joueur>().indexCarte(this.getNomCarte());
            players[j1].GetComponent<Joueur>().piocher(ref pioche, ref defausse);
            players[j1].GetComponent<Joueur>().piocher(ref pioche, ref defausse);
            defausse.Add(players[j1].GetComponent<Joueur>().main[index]);
            players[j1].GetComponent<Joueur>().main.RemoveAt(index);
        }

        scene.text = players[j1].GetComponent<Joueur>().getPseudo() + " a joué un Convoi. Il pioche deux cartes.";  
        historique.text += "\n\n-"+scene.text;
        
    }
}

