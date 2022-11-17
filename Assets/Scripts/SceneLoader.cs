using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// GameObject qui permet de changer de scène en fin de partie et pour rejouer
public class SceneLoader : MonoBehaviour
{
    int gameScene;
    public void LoadStartScene()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadEndScene()
    {
        SceneManager.LoadScene(2); //fin de jeu
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        GameControler gameControler = GameObject.Find("Game Controler - Lancer Partie").GetComponent<GameControler>();

        if (gameControler.est_mort == false)
        {
            gameControler.CmdJoueurQuittePartie(gameControler.players[0].GetComponent<Joueur>().getPseudo());
        }
        //Application.Quit();
        SceneManager.LoadScene(2);
    }

    public int getScene()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }
}
