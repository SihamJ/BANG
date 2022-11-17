using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Arme : Carte
{
	private int nombre;
	private string figure;
	private int portee;
	string nomArme;
	private bool multi;
	int sprite;

	public Arme(int portee, string nomArme, int nombre, string figure, int sprite)
	{	
     		this.portee = portee;
     		this.nomArme = nomArme;
     		this.multi = false;
     		this.nombre = nombre;
     		this.figure = figure;
			this.sprite= sprite;
    }
	public int getSprite()
	{
		return this.sprite;
	}
	public void init(int portee, string figure)
	{
		this.portee = portee;
		this.figure = figure;
		this.multi = false;
	}
	
	public string getNomCarte()
	{
		return nomArme;
	}

	public string getTypeCarte()
	{
		return "Arme";
	}
	
	public string getDescription()
	{
		return "temporaire";
	}

	public bool getMulti()
	{
		return multi;
	}

	public int getNombre()
	{
		return this.nombre;
	}  

	public string getFigure()
	{
		return this.figure;
	}


	public void action(int j1, int j2, ref List<Carte> defausse, ref List<Carte> pioche, ref List<GameObject> players, ref Text scene, ref Text historique)
	{
		GameControler gameControler = GameObject.Find("Game Controler - Lancer Partie").GetComponent<GameControler>();

		int index;

		if (j1 == 0 && gameControler.est_mort == false)
		{
			index = players[j1].GetComponent<Joueur>().indexCarte(this.getNomCarte());
			players[j1].GetComponent<Joueur>().main.RemoveAt(index);
		}

	    players[j1].GetComponent<Joueur>().setPortee(this.portee);
		players[j1].GetComponent<Joueur>().setArme(this.getNomCarte(), this.sprite);


		Debug.Log("Vous recevez ?");

	   		if(players[j1].GetComponent<Joueur>().possedeArme())
	   		{
	   			for(int i = 0; i < players[j1].GetComponent<Joueur>().plateau.Count; i++)
	   			{
				if (Equals(players[j1].GetComponent<Joueur>().plateau[i].getTypeCarte(), "Arme"))
					players[j1].GetComponent<Joueur>().plateau.RemoveAt(i);
				// #TO_DO: défausser ?
	   			}
	   		}


		Debug.Log("Vous recevez 2 ?");


	   		players[j1].GetComponent<Joueur>().plateau.Add(pioche[pioche.Count-1]);
		   pioche.RemoveAt(pioche.Count-1);

		Debug.Log("Vous recevez 3 ?");

	   		scene.text = players[j1].GetComponent<Joueur>().getPseudo() + " : Possède maintenant un " + this.getNomCarte() + ". Il a désormais " + players[j1].GetComponent<Joueur>().getPortee() + " de portée.";
	   		historique.text += "\n\n"+scene.text;

	}
}

public class Volcanic : Arme
{
	public Volcanic(int nombre, string figure, int sprite)
		: base(1,"Volcanic",nombre,figure,sprite)
	{
	}
}

public class Schofield : Arme
{
	public Schofield(int nombre, string figure, int sprite)
		: base(2,"Schofield",nombre,figure,sprite)
	{
	}
}

public class Remington : Arme
{
	public Remington(int nombre, string figure, int sprite)
		: base(3,"Remington",nombre,figure,sprite)
	{
	}
}

public class Carabine : Arme
{
	public Carabine(int nombre, string figure, int sprite)
		: base(4,"Carabine",nombre,figure,sprite)
	{
	}
}

public class Winchester : Arme
{
	public Winchester(int nombre, string figure, int sprite)
		: base(5,"Winchester",nombre,figure,sprite)
	{
	}
}
