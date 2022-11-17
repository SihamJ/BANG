using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bang : Carte
{
    int nombre;
    string figure;
    bool multi;

	int sprite;
    public Bang(int nombre, string figure, int sprite)
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
        return "Bang!";
    }

    public string getTypeCarte()
    {
        return "Action";
    }

    public bool getMulti()
    {
        return multi;
    }
    public string getDescription()
    {
        return "temporaire";
    }





    //Trouver une solution, pour 


    public void action(int j1, int j2, ref List<Carte> defausse, ref List<Carte> pioche, ref List<GameObject> players,  ref Text scene, ref Text historique)
    {
        if (j1 == 0)
        {
            Debug.Log("presque bon xd ");
        }

        Debug.Log("Ca bug ?");

        players[j2].GetComponent<Joueur>().setVie(players[j2].GetComponent<Joueur>().getVie()-1);
        scene.text += players[j1].GetComponent<Joueur>().getPseudo() + " a tiré sur "+players[j2].GetComponent<Joueur>().getPseudo()+", il perd 1 PV.";
        players[j1].GetComponent<Joueur>().setScorePartie(players[j1].GetComponent<Joueur>().getScorePartie()+3);
        historique.text += "\n\n-"+scene.text;

        Debug.Log("Ca bug encore");

        /*
        //Personnage BART CASSIDY : 
        if (Equals(players[j2].GetComponent<Joueur>().getPersonnage(), "BART CASSIDY"))
        {
           if(j2 == 0)
                players[j2].GetComponent<Joueur>().piocher(ref pioche, ref defausse);
        }

        //Personnage EL GRINGO :


        // Mettre ceci en dehors de la fonctions
        if (Equals(players[j2].GetComponent<Joueur>().getPersonnage(), "EL GRINGO"))
        {
            int nb_carte = players[1].GetComponent<Joueur>().main.Count;
            int index_carte = UnityEngine.Random.Range(0, nb_carte);


            players[j2].GetComponent<Joueur>().main.Add(players[j1].GetComponent<Joueur>().main[index_carte]);
        
        
           players[j1].GetComponent<Joueur>().main.RemoveAt(index_carte);

            scene.text += "\n" + players[j2].GetComponent<Joueur>().getPseudo() +
                              " a volé une carte de " +
                              players[j1].GetComponent<Joueur>().getPseudo();
            historique.text += scene.text;

        */


            //mettre en dehors de la fonction fairemulti avant l'envoi du client rpc Vous_êtes jouer 2
     }

}
