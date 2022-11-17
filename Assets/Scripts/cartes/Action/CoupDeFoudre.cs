using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoupDeFoudre : Carte
{
    int nombre;
    string figure;
    bool multi;
	int sprite;

    public CoupDeFoudre(int nombre, string figure, int sprite)
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
        return "CoupDeFoudre";
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
        int index = players[j1].GetComponent<Joueur>().indexCarte(this.getNomCarte());
        scene.text = players[j1].GetComponent<Joueur>().getPseudo() + " invoque la foudre sur " + players[j2].GetComponent<Joueur>().getPseudo() + " !";
        players[j1].GetComponent<Joueur>().setScorePartie(players[j1].GetComponent<Joueur>().getScorePartie()+1);
        historique.text += "\n\n-"+scene.text;

      //  defausse.Add(players[j1].GetComponent<Joueur>().main[index]);
      // players[j1].GetComponent<Joueur>().main.RemoveAt(index);
    }

    public string getDescription()
    {
        return "temporaire";
    }

}

