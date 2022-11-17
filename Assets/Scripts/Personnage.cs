using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Personnage
{
    private string nom = "PERSO";
    private int vie = 0;
    private string description = "TEMPO";
    int sprite;

    public Personnage(string nom, int vie, string description, int sprite)
    {
        this.nom = nom;
        this.vie = vie;
        this.description = description;
        this.sprite = sprite;
    }

    public int getVie()
    {
        return this.vie;
    }

    public int getSprite()
	{
		return this.sprite;
	}

    public string getNomPersonnage()
    {
        return this.nom;
    }

    public string getDescription()
    {
        return this.description;
    }
}
