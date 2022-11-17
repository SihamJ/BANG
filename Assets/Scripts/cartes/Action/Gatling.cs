using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gatling : Carte
{
   int nombre;
   string figure;
   bool multi;
	int sprite;
    public string getDescription()
    {
        return "temporaire";
    }

    public Gatling(int nombre, string figure, int sprite)
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
   	return "Gatling";
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
        int index = players[j1].GetComponent<Joueur>().indexCarte(this.getNomCarte());
        scene.text = players[j1].GetComponent<Joueur>().getPseudo() + " utilise son Gatling ! Vous perdez tous 1 point de vie, exceptés ceux voulant utiliser un Raté!";
        historique.text += "\n\n-"+scene.text;
        defausse.Add(players[j1].GetComponent<Joueur>().main[index]);
        players[j1].GetComponent<Joueur>().main.RemoveAt(index);

    }
}
