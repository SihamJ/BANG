using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface Carte 
{
	int getSprite();
    string getNomCarte();
    string getTypeCarte();
    string getDescription();
    string getFigure();
    int getNombre();
    bool getMulti();
    void action(int j1, int j2, ref List<Carte> defausse, ref List<Carte> pioche, ref List<GameObject> joueurs, ref Text scene, ref Text historique);

}









