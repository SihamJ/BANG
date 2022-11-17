using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Button_devoiler : NetworkBehaviour
{

        public GameControler serveur;

        private void Start()
        {
            serveur = GameObject.Find("Game Controler - Lancer Partie").GetComponent<GameControler>();
        }

        public void OnClick()
        {

            if (serveur.indiceJoueurCourant == 0)
            {
                serveur.CmdDevoilerJoueur();
            }
        }

    // Update is called once per frame
    void Update()
    {
        
    }
}
