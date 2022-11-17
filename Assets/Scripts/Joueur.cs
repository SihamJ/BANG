using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Joueur : MonoBehaviour
{
    [SerializeField] public Text pseudoText;
    [SerializeField] public Text roleText;
    [SerializeField] public Text vieText;
    [SerializeField] public Text porteeText;
    [SerializeField] public Image rang;

	public GameObject PVLabel;
	public GameObject PorteLabel;
	public GameObject RoleLabel;
	public GameObject ScoreLabel;

    public Image myImage;
    public Image background;
	public GameObject bouton;
    private int numero;
	private int carteActive;

    private string pseudo;
    private string role;
    
    private int score;
    private int idbdd;
    private string arme;
    private Personnage personnage;
    private int vie = 0;
    private int portee = 1;
    private GameControler gameControler;

    public bool devoile = false;
    public List<Carte> main = new List<Carte>();
    public List<Carte> plateau = new List<Carte>();
    public GameObject plateau_de_jeu;
    public GameObject handzone;
    public GameObject defausseZone;
    public GameObject Card_Prefab;
    private List<GameObject> UICards = new List<GameObject>();

    public bool isAndroid;

    private int id;
    public int armesprite = 71;
	private Sprite oldsprite;
    void Start()
    {
		gameControler = GameObject.Find("Game Controler - Lancer Partie").GetComponent<GameControler>();
		this.bouton.SetActive(false);
    }


    // TO DO: set image pour les cartes
	public void Update() {
		this.bouton.SetActive((gameControler.getTour() > 0) && !this.est_mort());
		if(this.main.Count != this.UICards.Count)
		{
			Debug.LogFormat("Count in main is: {0}, Count in UICards is: {1}", this.main.Count, this.UICards.Count);
			foreach(GameObject carte in this.UICards)
			{
				Destroy(carte);
			}
			this.UICards.Clear();
			for(int i = 0; i < this.main.Count; i++)
			{
				GameObject curr_carte = Instantiate(Card_Prefab, new Vector3(0,0,0), Quaternion.identity); 
				curr_carte.transform.SetParent(handzone.transform, false);
				// curr_carte.GetComponent<DragDrop>().SetText(main[i].getNomCarte());
				curr_carte.GetComponent<DragDrop>().number = i;
				curr_carte.GetComponent<DragDrop>().SetImage(gameControler.cards[main[i].getSprite()]);
				this.UICards.Add(curr_carte);
			}
		}
		foreach(GameObject carte in this.UICards)
		{
            // changed
			carte.GetComponent<DragDrop>().setDraggable( !(gameControler.getTour() > 0) && (gameControler.indiceJoueurCourant == 0) && this.estVivant() );
		}
	}

    public void init(string pseudo, int id, int idbdd, bool isAndroid)
    {
        this.pseudo = pseudo;
        this.isAndroid = isAndroid;
        this.role = "Role Dissimulé";
        this.arme = "Colt .45";
        //this.personnage = gameObject.AddComponent<Personnage>() as Personnage;
        this.portee = 1;
        this.id = id;
        this.idbdd = idbdd;
        pseudoText.text = pseudo;
        roleText.text = "Dissimulé";
        porteeText.text = this.portee.ToString();
        if(this.isAndroid == false || Application.platform == RuntimePlatform.Android)
        this.score = bdd.elo(idbdd);
        else
            this.score = 0;
        this.personnage = new Personnage("TEMPO1", 0, "TEMPO2",0);
        this.vie = personnage.getVie();
        vieText.text = this.vie.ToString();
    }

    public void init(string pseudo, int id, int idbdd)
    {
        this.pseudo = pseudo;
        this.isAndroid = false;
        this.role = "Role Dissimulé";
        this.arme = "Colt .45";
        //this.personnage = gameObject.AddComponent<Personnage>() as Personnage;
        this.portee = 1;
        this.id = id;
        this.idbdd = idbdd;
        pseudoText.text = pseudo;
        roleText.text = "Dissimulé";
        porteeText.text = this.portee.ToString();
        this.score = 0;
        this.personnage = new Personnage("TEMPO1", 0, "TEMPO2",0);
        this.vie = personnage.getVie();
        vieText.text = this.vie.ToString();
    }



	public void button_clicked()
	{
        int index = 0;
        Debug.Log("pseudo : " + this.pseudo);
        while (!gameControler.players[index].GetComponent<Joueur>().getPseudo().Equals(this.pseudo))
        {
            index = index + 1;
        }
        gameControler.set_j2(index);
        gameControler.action_joueur();
        stop_buttonHover();
    }
	public void start_buttonHover()
	{
		this.PVLabel.SetActive(false);
		this.PorteLabel.SetActive(false);
		this.RoleLabel.SetActive(false);
		this.ScoreLabel.SetActive(false);
		oldsprite = myImage.sprite;
		this.set_color(gameControler.couleur[6]);
	}
	public void stop_buttonHover()
	{
		this.PVLabel.SetActive(true);
		this.PorteLabel.SetActive(true);
		this.RoleLabel.SetActive(true);
		this.ScoreLabel.SetActive(true);
		this.set_color(oldsprite);
	}
    public int getId()
    {
        return this.id;
    }

    public bool getIsAndroid(){
        return this.isAndroid;
    }

    public int getNumero()
    {
        return this.numero;
    }

    public void setElo(Sprite rang)
    {
        this.rang.sprite = rang;
    }

	public void setNumero(int numero)
	{
		this.numero = numero;
	}
    public void setPseudo(string pseudo)
    {
        this.pseudo = pseudo;
        pseudoText.text = pseudo;
    }
    public string getPseudo()
    {
        return this.pseudo;
    }

    public string getRole()
    {
        return this.role;
    }

    public void setRole(string role)
    {
        this.role = role;
    }

    public int getIdbdd()
    {
        return this.idbdd;
    }

    public int getScorePartie()
    {
        return this.score;
    }

    public void setScorePartie(int score)
    {
        this.score = score;
    }

    public void setScorePartie(int idbdd, int score)
    {
        this.score = score;
        bdd.elo(idbdd, score);
    }

    public int getVie()
    {
        return this.vie;
    }

    public int getVieMax()
    {
        if (Equals(this.role, "Shérif"))
            return personnage.getVie() + 1;
        return personnage.getVie();
    }

    public void setVie(int vie)
    {
        this.vie = vie;
        vieText.text = vie.ToString();
    }

    public void setArme(string arme, int sprite)
    {
        this.arme = arme;
		this.armesprite = sprite;
    }


    public int getPortee()
    {
        return this.portee;
    }

    public void setPortee(int portee)
    {
        this.portee = portee;
        porteeText.text = portee.ToString();
    }

    public Personnage getPersonnage()
    {
        return this.personnage;
    }

    public void setPersonnage(Personnage p)
    {
        this.personnage = p;
    }

    public void Afficher()
    {
        pseudoText.text = pseudo;
        roleText.text = role;
        vieText.text = this.vie.ToString();
        porteeText.text = this.portee.ToString();
    }

    public void setCarteActive(int index)
    {
        this.carteActive = index;
    }

    public int getCarteActive()
    {
        return carteActive;
    }

    public int indexCarteActive()
    {
        return 0;
    }

    public bool estVivant()
    {
        if (this.vie > 0)
            return true;
        return false;
    }

    public void piocher(ref List<Carte> pioche, ref List<Carte> defausse)
    {
        if (pioche.Count == 0)
        {
            int n = defausse.Count;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, n + 1);
                Carte value = defausse[k];
                defausse[k] = defausse[n];
                defausse[n] = value;
            }
            for (int i = 0; i < defausse.Count; i++)
            {
                pioche.Add(defausse[i]);
                defausse.Remove(defausse[i]);
            }
        }
        this.main.Add(pioche[0]);
        pioche.RemoveAt(0);
    }

    public bool possedeArme()
    {
        foreach (Carte carte in this.plateau)
        {
            if (Equals(carte.getTypeCarte(), "Arme"))
                return true;
        }
        return false;
    }

    public bool possede(string name)
    {
        foreach (Carte carte in this.main)
        {
            if (Equals(carte.getNomCarte(), name))
                return true;
        }
        return false;
    }

    public bool possedePlateau(string name)
    {
        foreach (Carte carte in this.plateau)
        {
            if (Equals(carte.getNomCarte(), name))
                return true;
        }
        return false;
    }

    public int indexCarte(string name)
    {
        for (int i = 0; i < this.main.Count; i++)
        {
            if (Equals(this.main[i].getNomCarte(), name))
                return i;
        }
        return -1;
    }

    public int indexCartePlateau(string name)
    {
        for (int i = 0; i < this.main.Count; i++)
        {
            if (Equals(this.plateau[i].getNomCarte(), name))
                return i;
        }
        return -1;
    }

    public bool est_mort()
    {
        return (this.getVie() <= 0);
    }

    public void set_bg(Sprite s) {
        background.sprite = s;
    }

    public void set_color(Sprite s) {
        myImage.sprite = s;
    }

    public void devoiler_role()
    {
          devoile = true;
    }

    // Si le joueur pioche une carte coeur, il esquive le tire. Sinon, planque raté.
    public bool degainer(ref List<Carte> pioche, ref List<Carte> defausse, List<int> tabint, string figure)
    {
        if (pioche.Count == 0)
        {
            int n = defausse.Count;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, n + 1);
                Carte value = defausse[k];
                defausse[k] = defausse[n];
                defausse[n] = value;
            }
            for (int i = 0; i < defausse.Count; i++)
            {
                pioche.Add(defausse[i]);
                defausse.Remove(defausse[i]);
            }
        }
        bool ret = true;
        Carte degaine = pioche[0];
        if (!tabint.Contains(degaine.getNombre()))
            ret = false;
        if (degaine.getFigure() != figure)
            ret = false;
        defausse.Add(degaine);
        pioche.Remove(degaine);

        return ret;
    }

    public void afficher_plateau()
    {
        this.plateau_de_jeu.SetActive(true);
    }

    public void cacher_plateau(GameObject plateau)
    {
        this.plateau_de_jeu.SetActive(false);
        plateau.SetActive(false);
    }

    public void setPlateau(ref List<GameObject> plateau, int i)
    {
        this.plateau_de_jeu = plateau[i];
    }
    public void init_plateau(string nom_joueur_local)
    {
        plateau_de_jeu.GetComponent<Plateau>().pseudoText.text = pseudo;
        plateau_de_jeu.GetComponent<Plateau>().vieText.text = vie.ToString();
        //plateau_de_jeu.GetComponent<Plateau>().porteeText.text = portee.ToString();
      //  plateau_de_jeu.GetComponent<Plateau>().nomPersonnage.text = personnage.getNomPersonnage();
      //  plateau_de_jeu.GetComponent<Plateau>().descriptionPersonnage.text = personnage.getDescription();
      //  plateau_de_jeu.GetComponent<Plateau>().arme.text = this.arme;
      /*  if (devoile == true || Equals(nom_joueur_local, pseudo))
        {
            plateau_de_jeu.GetComponent<Plateau>().roleText.text = role;

            if (this.role.Equals("Shérif"))
            {
                plateau_de_jeu.GetComponent<Plateau>().descriptionRole.text = "Le Shérif doit éliminer tous les Hors-la-loi et le Renégat pour faire régner la loi et l’ordre.";
            }
            else if (this.role.Equals("Adjoint"))
            {
                plateau_de_jeu.GetComponent<Plateau>().descriptionRole.text = "Les Adjoints aident et protègent le Shérif. Ils ont le même but que lui et ils l’accompliront coûte que coûte !";
            }
            else if (this.role.Equals("HLL"))
            {
                plateau_de_jeu.GetComponent<Plateau>().descriptionRole.text = "Les Hors-la-loi ont envie de tuer le Shérif, mais ils n’ont aucun scrupule à s’entretuer pour ramasser les récompenses!";
            }
            else
            {
                plateau_de_jeu.GetComponent<Plateau>().descriptionRole.text = "Le Renégat veut prendre la place du Shérif : son but est d’être le dernier personnage en jeu.";
            }

            roleText.text = role;
        }*/

        plateau_de_jeu.GetComponent<Plateau>().init(plateau);

    }



    //_______________________________Mise a jour de la Main du Joueur, pour retourner la carte du joueur dans sa main_________________________________//

    public void Mise_a_jour_carte()
    {
        Debug.LogFormat("Count in main is: {0}, Count in UICards is: {1}", this.main.Count, this.UICards.Count);
        foreach (GameObject carte in this.UICards)
        {
            Destroy(carte);
        }
        this.UICards.Clear();
        for (int i = 0; i < this.main.Count; i++)
        {
            GameObject curr_carte = Instantiate(Card_Prefab, new Vector3(0, 0, 0), Quaternion.identity);
            curr_carte.transform.SetParent(handzone.transform, false);
            // curr_carte.GetComponent<DragDrop>().SetText(main[i].getNomCarte());
            curr_carte.GetComponent<DragDrop>().number = i;
			curr_carte.GetComponent<DragDrop>().SetImage(gameControler.cards[main[i].getSprite()]);
            this.UICards.Add(curr_carte);
        }
    }
}
