using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Duel : Carte
{
    int nombre;
    string figure;
    bool multi;
	int sprite;
    public string getDescription()
    {
        return "temporaire";
    }

    public Duel(int nombre, string figure, int sprite)
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
        return "Duel";
    }

    public string getTypeCarte()
    {
        return "Objet";
    }

    public bool getMulti()
    {
        return multi;
    }

    //TO_DO: A revoir l'affichage, ajouter le temps
    public void action(int j1, int j2, ref List<Carte> defausse, ref List<Carte> pioche, ref List<GameObject> players, ref Text scene, ref Text historique)
    {
        int index = players[j1].GetComponent<Joueur>().indexCarte(this.getNomCarte());

        scene.text = players[j1].GetComponent<Joueur>().getPseudo() + " à provoqué " + players[j2].GetComponent<Joueur>().getPseudo() + " en Duel !";
        historique.text += "\n\n"+scene.text;
        if (players[j2].GetComponent<Joueur>().possede("Bang!"))
        {
            scene.text += players[j2].GetComponent<Joueur>().getPseudo() + " BANG! " + players[j1].GetComponent<Joueur>().getPseudo();
            historique.text += " - " + scene.text;
            players[j2].GetComponent<Joueur>().main.RemoveAt(players[j2].GetComponent<Joueur>().indexCarte("Bang!"));
            int i = 0;
            while (i == 0)
            {
                if (players[j1].GetComponent<Joueur>().possede("Bang!"))
                {
                    scene.text += players[j1].GetComponent<Joueur>().getPseudo() + " BANG! " + players[j2].GetComponent<Joueur>().getPseudo();
                    historique.text += " - " + scene.text;
                    players[j1].GetComponent<Joueur>().main.RemoveAt(players[j1].GetComponent<Joueur>().indexCarte("Bang!"));
                    if (players[j2].GetComponent<Joueur>().possede("Bang!"))
                    {
                        scene.text = players[j2].GetComponent<Joueur>().getPseudo() + " BANG! " + players[j1].GetComponent<Joueur>().getPseudo();
                        historique.text += " - " + scene.text;
                        players[j2].GetComponent<Joueur>().main.RemoveAt(players[j2].GetComponent<Joueur>().indexCarte("Bang!"));
                    }
                    else
                    {
                        players[j2].GetComponent<Joueur>().setVie(players[j2].GetComponent<Joueur>().getVie() - 1);
                        scene.text = players[j2].GetComponent<Joueur>().getPseudo() + " a perdu le Duel. Il n'a plus que " + players[j2].GetComponent<Joueur>().getVie() + " points de vie(s)";
                        i = 1;
                    }
                }
                else
                {
                    players[j1].GetComponent<Joueur>().setVie(players[j1].GetComponent<Joueur>().getVie() - 1);
                    scene.text = players[j1].GetComponent<Joueur>().getPseudo() + " a perdu le Duel. Il n'a plus que " + players[j1].GetComponent<Joueur>().getVie() + " points de vie(s)";
                    i = 1;
                    historique.text += "\n-" + scene.text;
                }
            }
        }
        else
        {
            players[j2].GetComponent<Joueur>().setVie(players[j2].GetComponent<Joueur>().getVie() - 1);
            scene.text = players[j2].GetComponent<Joueur>().getPseudo() + " a perdu le Duel. Il n'a plus que " + players[j2].GetComponent<Joueur>().getVie() + " points de vie(s)";
            historique.text += "\n-" + scene.text;
        }
        players[j1].GetComponent<Joueur>().main.RemoveAt(index);

    }

}


