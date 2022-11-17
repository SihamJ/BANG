using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dynamite : Carte
{
    int nombre;
    string figure;
    bool multi;

	int sprite;
    public Dynamite(int nombre, string figure, int sprite)
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
        return "Dynamite";
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

        Debug.Log("Debug 1 ?");

        players[j2].GetComponent<Joueur>().plateau.Add(pioche[(pioche.Count)-1]);
        pioche.RemoveAt((pioche.Count)-1);

        if (j1 == 0 && gameControler.est_mort == false)
        {
            index = players[j1].GetComponent<Joueur>().indexCarte(this.getNomCarte());
            players[j1].GetComponent<Joueur>().main.RemoveAt(index);
        }

        Debug.Log("Debug 2 ?");
          
        scene.text = players[j1].GetComponent<Joueur>().getPseudo() + " a placer une Dynamite sur " + players[j2].GetComponent<Joueur>().getPseudo() + ", attention à l'explosion!";
        historique.text += "\n\n-"+scene.text;
    }

}
