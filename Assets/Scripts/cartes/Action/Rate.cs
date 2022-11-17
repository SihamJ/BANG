using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rate : Carte
{
	int nombre;
	string figure;
	bool multi;

	int sprite;
	public Rate(int nombre, string figure, int sprite)
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
		return "Raté!";
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
        scene.text = players[j1].GetComponent<Joueur>().getPseudo()+ " : Une carte Raté! ne peux être utilisée que pour se défendre !";
	}

}

