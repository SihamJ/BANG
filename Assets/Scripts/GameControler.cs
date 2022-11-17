using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class GameControler : NetworkBehaviour
{
    public GameObject player;
    public GameObject plateau; // Plateau des joueurs qu'on peut afficher/dissimuler dans la même scène
    public GameObject rate;
    public GameObject voler;
    public GameObject bouton_defausse;
    public GameObject password;
    public GameObject startmenu;  // Menu du Lobby
    public GameObject scene_de_jeu; // Plateau du jeu qui est caché avant le démarrage de la partie
    public GameObject menu; // Menu: règles de jeu
    public GameObject regles; // Menu règles du jeu
    public GameObject resultats;  // Menu résultats fin partie
    public GameObject historique; // Menu de l'historique
    public GameObject chat_pannel;
    public Button button_chat;
   

    public AudioSource son; //Son du jeu
    public Slider slider; // Slider du son
    public Text name_bouton_son; // Nom du bouton son

    public InputField login;
    public InputField pass;
    
    public GameObject historique_zone;
    public GameObject historique_menu;
    public GameObject drop_zone;
    public GameObject carteJouee;
    public GameObject handzone;
    public GameObject defausseZone;
    public GameObject chat_ui;
    public SceneLoader sceneLoader; // GameObject pour changer de scène en fin de partie


    [SerializeField] Text instruction; // Instruction au niveau des inputs dans le lobby
    [SerializeField] Text bienvenue; // "bienvenue au Far West #pseudo"
    [SerializeField] Text carte_defausse; // Première carte dans la défausse qu'on peut piocher
    [SerializeField] Text scene; // Au centre de la scène les actions, erreurs, ...etc
    [SerializeField] Text instructions_jeu; // Au centre de la scène les instructions de jeu
    [SerializeField] Text chat_text;
    [SerializeField] InputField chat_input;
    Text historique_text; // Le texte affiché dans l'historique
    Text historique_menu_text;
    public Text warning;
    [SerializeField] Text newsDefausse; // Affiche la dernière action effectué dans la défausse: "Joueur J a défaussé carte Bang!"
    [SerializeField] Text resultats_finPartie; // Résultats fin partie
    public List<Text> plateau_texte = new List<Text>(); // Pour afficher le plateau des joueurs. Temporaire avant d'améliorer l'IHM 

    public Dropdown nbJoueurs; // Menu déroulant pour choisir le nombre de joueur au menu de démarrage
    //public Dropdown mon_plateau; // menu déroulant plateau joueur 0
    //public Dropdown indexJ2_dropdown; // Pour choisir le joueur 2 de l'action multiple

    public List<Sprite> couleur = new List<Sprite>(); // Image joueurs: 0-caché 1-shérif 2-adjoint 3-renegat 4-hors la loi 5-mort 6-ciblé
    public List<Sprite> bg = new List<Sprite>(); // Image joueurs: 0-noir 1-vert 2-gris 
	public List<Sprite> cards = new List<Sprite>();
    public List<Sprite> spritepers = new List<Sprite>();
	public List<Sprite> spriterole = new List<Sprite>();

    public List<Carte> pioche = new List<Carte>();
    public List<Carte> defausse = new List<Carte>();
    public List<int> id_joueurs = new List<int>();
    public List<int> id_bdd = new List<int>();

    public List<bool> is_android = new List<bool>();
    public List<Joueur> morts = new List<Joueur>(); //list des joueurs morts
    public Transform[] positions; // Tableau de coordonnées des joueurs
    public List<GameObject> plateaux; // Liste des plateaux
    public List<String> pseudos_joueurs = new List<String>();
    public List<GameObject> players = new List<GameObject>(); // Liste des joueurs
    public List<Sprite> Rangs = new List<Sprite>();


    int indexCarte; //index de La carte à Joueur
    public int indexJoueur2; // Index du joueur 2 de l'action
    private int tour = 0; // Numéro du tour
    private bool deja_bang = false; // Le joueur courant a-t-il déjà utilisée la carte Bang! ?
    private bool action_en_cours = false; // Attendre la réponse du joueur 2 après action multi
    private int passage = 0; // Pour ne pas repiocher au début du premier tour

    public bool musique = true;

    // _____________________PLUS BESOIN ?_______________________________
    int nb = 0; // Nombre de joueurs dans la partie, initialisé à 0 pour test.
    int count; // Nombre de joueurs restant qui doivent rejoindre la partie
    // int val_duel = 0; // Pour faire le duel 
    // bool J2action = false; // Si le deuxième joueur doit répondre de ses actes 
    // bool joueur_pret = false;
    // int indexJ2 = -1; // Valeur du joueur 2 qui doit répondre de l'attaque


    // _____________________VARIABLES RESEAU____________________________
    int nb_joueurs_prets = 0;
    bool partie_lancee = false;
    public int numero_joueur = -1;
    public int indiceJoueurCourant; // Index du joueur dont c'est le tour
    int tour_pioche_init = 0;
    bool main_deja_init = false;
    bool on_veut_init = false;
    public GameObject validernbjoueur;
    public GameObject start;
    bool doit_piocher = false;
    bool nb_joueurs_set = false;
    bool carte_indien = false;
    bool carte_gatling = false;

    public bool est_mort = false; //donne l'information si leur joueur est mort 
    bool deja_appuyer_rejouer = false;


    public int nb_rejouer = 0;    

    //________________________Variable Drag Drop___________________________________

    public List<Personnage> pers = new List<Personnage>();

    public void Update()
    {
        ///Initialisation du jeu pour tous
        if (!partie_lancee && nb_joueurs_prets == 4 && isServer)
        {
            if (on_veut_init == false)
            {
                RpcDonnePseudoId(pseudos_joueurs, id_joueurs, id_bdd, is_android);
                RpcLobbyToGame(); //Puis InstantiateJoueurs
                RpcPositionnePseudos(pseudos_joueurs);
                DefinirRole(); //Puis CmdDonnerRole et RpcDefinirSherif 
                DefinirPersonnages();
                InitPioche();
                RpcDonnerNb(nb);
                RpcOnVeutInit(); // Pour passer à l'initialisation des mains des joueurs
               
            }
        }

        //Initialisation des mains des joueurs
        if (on_veut_init && (!main_deja_init) && tour_pioche_init == numero_joueur)
        {
            Debug.Log("Initialisation de la liste ");
            InitMain();
            main_deja_init = true;
            CmdPiocherCarteMulti(players[0].GetComponent<Joueur>().getVie(), numero_joueur);
            //Debug.Log("nombre dans la pioche est : " + pioche.Count);
            IncrementTourPiocheInit();
        }

        // Lancement du jeu 
        if (tour_pioche_init == 4 && numero_joueur == 0)
        {
            CmdLancerJeu();
            tour_pioche_init++;
        }

        // Le joueur doit piocher 
        if (doit_piocher && est_mort == false)
        {
            loop_partie();
        }


        
        if(nb_rejouer == 4 && partie_lancee)
        {
            Debug.Log("Rejouer Partie");
            partie_lancee = false;
            on_veut_init = false;
            nb_rejouer = 0;
            deja_appuyer_rejouer = false;
            resultats.SetActive(false);
        }
        

    }
    public void set_indexcarte(int i)
    {
        this.indexCarte = i;
        Debug.LogFormat("Setting indexCarte at value {0}", indexCarte);
    }

	public void set_j2(int i)
	{
		this.indexJoueur2 = i;
	}

    [ClientRpc]
    public void RpcDonnerNb(int nombre)
    {
        nb = nombre;
    }

    [Command(ignoreAuthority = true)]
    public void CmdLancerJeu()
    {
        RpcLancerJeu();
    }

    [ClientRpc]
    public void RpcLancerJeu()
    {
        //indexJ2_Dropdown_init();
        init_carte_defausse();
        update_plateau();

       // scene.color = Color.black;
        //instructions_jeu.color = Color.black;
        instructions_jeu.text = "";
        scene.text = players[indiceJoueurCourant].GetComponent<Joueur>().getPseudo() + " à toi de jouer!";
        instructions_jeu.text = "";
        players[indiceJoueurCourant].GetComponent<Joueur>().set_bg(bg[1]);
    }

    [ClientRpc]
    public void RpcOnVeutInit()
    {
        on_veut_init = true;
    }

    [Command(ignoreAuthority = true)]
    public void IncrementTourPiocheInit()
    {
        IncrementTourInit();
    }

    [ClientRpc]
    public void IncrementTourInit()
    {
        tour_pioche_init = tour_pioche_init + 1;
        //Debug.Log("valeur de tpi : " + tour_pioche_init + "le nbr j pret est " + nb_joueurs_prets);
    }

    //Démarrage du Lobby
    void Start()
    {
        validernbjoueur = GameObject.Find("ValiderNbJoueurs");
        start = GameObject.Find("LancerPartie");
        InitPersonnages();
        if (Application.platform != RuntimePlatform.Android)
        {
        bdd.creation_tables();
        }
        int scene = sceneLoader.getScene();
        if (scene == 1)
        {
            menu.SetActive(false);
            rate.SetActive(false);
            scene_de_jeu.SetActive(false);
            regles.SetActive(false);
            resultats.SetActive(false);
            password.SetActive(false);
            //voler.SetActive(false);
            //bouton_defausse.SetActive(false);
            rate.SetActive(false);
            historique.SetActive(false);
            if (!isServer)
            {
                validernbjoueur.SetActive(false);
                instruction.text = "Entrez vos informations :";
                password.SetActive(true);
                nbJoueurs.gameObject.SetActive(false);
                login.text = "";
                pass.text = "";
            }
            else
            {
                instruction.text = "Choisir le nombre de joueurs:";
                login.text = "";
                pass.text = "";
            }
        }
    }

    public void ValiderNbJoueurs()
    {
        //nb_joueurs_set = true;
        nb = int.Parse(nbJoueurs.options[nbJoueurs.value].text);
        count = nb;
        validernbjoueur.SetActive(false);
        instruction.text = "Entrez vos informations :";
        password.SetActive(true);
        nbJoueurs.gameObject.SetActive(false);
        login.text = "";
        pass.text = "";
    }

    public void ValiderConnexion()
    {
        int idutilisateur;
        if (Application.platform != RuntimePlatform.Android)
        {
            idutilisateur = bdd.inscription(login.text, pass.text);
        }
        else
        {
            idutilisateur = 12;
        }
        // REGEX fausse
        if (idutilisateur < -1)
        {
            if (idutilisateur == -2)
            {
                instruction.color = Color.red;
                instruction.text = "Veuillez entrer un login correct";
            }
            else if (idutilisateur == -3)
            {
                instruction.color = Color.red;
                instruction.text = "Veuillez entrer un mot de passe correct";
            }
        }
        else
        {
            if (Application.platform != RuntimePlatform.Android)
            {
            idutilisateur = bdd.connexion(login.text, pass.text);
            }
            else
            {
                idutilisateur = 12;
            }

            // Login/Mdp incorrects
            if (idutilisateur == -1)
            {
                instruction.color = Color.red;
                instruction.text = "Nom d'utilisateur ou mot de passe incorrect !";
            }

            else
            {
                numero_joueur = nb_joueurs_prets;
                string pseudo = login.text;
                bienvenue.color = Color.green;
                bienvenue.text = "Bienvenu au Far West " + pseudo + "! Tu es le joueur " + numero_joueur;
                login.text = "";
                pass.text = "";
                bool tmp;
                if(Application.platform == RuntimePlatform.Android)
                    tmp = true;
                else 
                    tmp = false;
                CmdInitjoueurReso(pseudo, numero_joueur, idutilisateur, tmp);
                start.SetActive(false);
                CmdIncrementJoueursPrets();
            }
        }
    }

    [Command(ignoreAuthority = true)]
    void CmdIncrementJoueursPrets()
    {
        RpcIncrementJoueursPrets();
    }

    [ClientRpc]
    void RpcIncrementJoueursPrets()
    {
        nb_joueurs_prets++;
        CmdPrintJoueursPrets();
    }

    [Command(ignoreAuthority = true)]
    void CmdPrintJoueursPrets()
    {
        //Debug.Log("Joueurs prets : " + nb_joueurs_prets);
    }

    [ClientRpc]
    void RpcLobbyToGame()
    {
        resultats.SetActive(false);
        startmenu.SetActive(false);
        scene_de_jeu.SetActive(true);
        partie_lancee = true;
        InstantiateJoueurs();
    }

    public void InstantiateJoueurs()
    {
        //Debug.Log("Debut boucle instantiation");
        historique_text = historique_zone.GetComponent<Text>();
        historique_menu_text = historique_menu.GetComponent<Text>();
        for (int i = 0; i < nb_joueurs_prets; i++)
        {
            //Debug.Log("boucle instantiation passage : " + i + "instantiate");
            players.Add(Instantiate(player, positions[i + 1].transform.position, Quaternion.Euler(0, 0, 0)) as GameObject);
            //Debug.Log("boucle instantiation passage : " + i + "setparent");
            players[i].transform.SetParent(scene_de_jeu.transform);
            //Debug.Log("boucle instantiation passage : " + i + "localscale");
            players[i].transform.localScale = positions[i + 1].transform.localScale;
            //Debug.Log("boucle instantiation passage : " + i + "init pseudo id");
            if (Application.platform != RuntimePlatform.Android)
            {
                players[i].GetComponent<Joueur>().init(pseudos_joueurs[i], id_joueurs[i], id_bdd[i], is_android[i]);
            }
            else
            {
            players[i].GetComponent<Joueur>().init(pseudos_joueurs[i], id_joueurs[i], id_bdd[i]);
            }
            players[i].GetComponent<Joueur>().handzone = handzone;
            players[i].GetComponent<Joueur>().defausseZone = defausseZone;
			players[i].GetComponent<Joueur>().setNumero(i);
            if (Application.platform != RuntimePlatform.Android)
            {
                if( players[i].GetComponent<Joueur>().getIsAndroid() == false)
                {
                    Debug.Log("NON ANDROID");
            int elo = bdd.elo(players[i].GetComponent<Joueur>().getIdbdd());
            players[i].GetComponent<Joueur>().setElo((elo < 2000) ? Rangs[0] : (elo < 4000) ? Rangs[1] : (elo < 6000) ? Rangs[2] : (elo < 8000) ? Rangs[3] : (elo < 10000) ? Rangs[4] : (elo < 12000) ? Rangs[5] : (elo < 14000) ? Rangs[6] : (elo < 16000) ? Rangs[7] : (elo < 18000) ? Rangs[8] : (elo < 25000) ? Rangs[9] : Rangs[10]);
        }
                else{
                    Debug.Log("ANDROID");
                    players[i].GetComponent<Joueur>().setElo(Rangs[0]);
                }

            }
            else{
                players[i].GetComponent<Joueur>().setElo(Rangs[10]);
            }
        }
        this.chat_pannel.transform.SetAsLastSibling();
        this.button_chat.onClick.AddListener(delegate{onSendChat(players[0].GetComponent<Joueur>().getPseudo());});
        //Debug.Log("Instanciation plateaux");
        for (int i = 0; i < nb_joueurs_prets; i++)
        {
            plateaux.Add(Instantiate(plateau, positions[0].transform.position, Quaternion.Euler(0, 0, 0)) as GameObject);
            plateaux[i].transform.localScale = positions[0].transform.localScale;
            plateaux[i].transform.SetParent(positions[0]);
            players[i].GetComponent<Joueur>().setPlateau(ref plateaux, i);
            players[i].GetComponent<Joueur>().init_plateau(players[0].GetComponent<Joueur>().getPseudo());
            plateaux[i].SetActive(false);
        }
    }

    [Command(ignoreAuthority = true)]
    public void CmdInitjoueurReso(String pseudo, int id, int getidbdd, bool isAndroid)
    {
        if (isServer)
        {
            //Debug.Log("Je suis le serveur, le joueur : " + pseudo + "a l'ID : " + id);
        }

        //Debug.Log("Appelle de NouveauJoueur avec : " + pseudo + id);
        //Debug.Log("Nouveau joueur appelée avec : " + pseudo + id);
        id_bdd.Add(getidbdd);
        is_android.Add(isAndroid);
        id_joueurs.Add(id);
        pseudos_joueurs.Add(pseudo);
    }

    [ClientRpc]
    public void RpcDonnePseudoId(List<String> pseudolist, List<int> idlist, List<int> idbddlist, List<bool> isAndroidlist)
    {
        //ajout attention enlever si marche pas
        pseudos_joueurs.Clear();
        id_joueurs.Clear();
        id_bdd.Clear();
        is_android.Clear();

        int c = pseudolist.Count;

        for (int i = 0; i < c; i++)
        {
            //Debug.Log("Pseudo " + i + ": " + pseudolist[i] + "ID : " + idlist[i]);
        }

        is_android.AddRange(isAndroidlist);
        id_bdd.AddRange(idbddlist);
        pseudos_joueurs.AddRange(pseudolist);
        id_joueurs.AddRange(idlist);
    }

    [ClientRpc] //Lance la scène pour tout les joueurs
    public void RpcPositionnePseudos(List<string> noms)
    {
        int ind = numero_joueur;
        int i;

        //Debug.Log("Le Numero de joueur est: " + ind);

        List<string> List = new List<string>();
        int c = noms.Count;

        //Découpage de la liste pour l'organisation 
        for (i = 0; i < c; i++)
        {
            //Debug.Log("Num est : " + (ind + i) % 4);
            List.Add(noms[(ind + i) % nb_joueurs_prets]);
        }

        for (i = 0; i < c; i++)
        {
            players[i].GetComponent<Joueur>().setPseudo(List[i]);
            plateaux[i].GetComponent<Plateau>().pseudoText.text = List[i];
        }
    }

    public int DefinirRole()
    {
        List<string> roles = new List<string>();
        roles.Add("Shérif");
        roles.Add("Adjoint");
        roles.Add("Renégat");
        roles.Add("HLL");
        if (nb_joueurs_prets > 4)
        {
            roles.Add("HLL");
            if (nb_joueurs_prets > 6)
            {
                roles.Add("Adjoint");
                if (nb_joueurs_prets > 7)
                {
                    roles.Add("HLL");
                }
            }
        }

        int n = roles.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            string value = roles[k];
            roles[k] = roles[n];
            roles[n] = value;
        }

        for (int i = 0; i < nb_joueurs_prets; i++)
        {
            CmdDonnerRole(roles[i], pseudos_joueurs[i]);
        }

        Debug.Log("Role");

        RpcDefinirSherif();
        return 0;
    }

    [Command(ignoreAuthority = true)]
    public void CmdDonnerRole(string role, string nomJoueur)
    {
        RpcJoueurGetsRole(role, nomJoueur);
    }

    [ClientRpc]
    public void RpcJoueurGetsRole(string role, string nomJoueur)
    {
        Joueur joueur;

        for (int i = 0; i < nb_joueurs_prets; i++)
        {
            joueur = players[i].GetComponent<Joueur>();
            if (Equals(joueur.getPseudo(), nomJoueur))
            {
                if (Equals(players[0].GetComponent<Joueur>().getPseudo(), nomJoueur))
                {
                    players[i].GetComponent<Joueur>().roleText.text = role;
                }
                joueur.setRole(role);
                joueur.setPortee(1);

                if (Equals(role, "Shérif"))
                {
                    joueur.setVie(joueur.getVie() + 1);
                    joueur.set_color(couleur[1]);
                    joueur.set_bg(bg[0]);
                    joueur.roleText.text = "Shérif";
                }
                else{
                    joueur.set_color(couleur[0]);
                    joueur.set_bg(bg[0]);
                }
                return;
            }
        }
    }

    [ClientRpc]
    public void RpcDefinirSherif()
    {
        indiceJoueurCourant = IndiceScherif();

        int j = 0;
        while (pseudos_joueurs[indiceJoueurCourant] != players[j].GetComponent<Joueur>().getPseudo())
        {
            j++;
        }

        indiceJoueurCourant = j;

        players[indiceJoueurCourant].GetComponent<Joueur>().devoiler_role();

        if (indiceJoueurCourant == -1)
        {
            //Debug.Log(" !!! ERREUR INDEX SHERIF NON TROUVE  !!! ");
        }
    }

    public int IndiceScherif()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (Equals(players[i].GetComponent<Joueur>().getRole(), "Shérif"))
            {
                for (int j = 0; j < pseudos_joueurs.Count; j++)
                {
                    if (Equals(pseudos_joueurs[j], players[i].GetComponent<Joueur>().getPseudo()))
                        return j;
                }
            }
        }
        return -1;
    }

    public void InitPersonnages()
    {
        // pers.Add(new Personnage("KIT CARLSON", 4, "blablabla"));
        // pers.Add(new Personnage("BART CASSIDY", 4, "blablabla"));
        // pers.Add(new Personnage("EL GRINGO", 3, "blablabla"));
        // pers.Add(new Personnage("SAM LE VAUTOUR", 4, "blablabla"));  //Faire ce perso ? 
                pers.Add(new Personnage("BLACK JACK", 4, "Durant la phase de pioche de son tour, si la deuxième carte piochée est Coeur ou Carreau, il pioche une 3ème carte",0));
                pers.Add(new Personnage("PAUL REGRET", 3, "Tous les autres joueurs le voient à une distance augmentée de 1",1));

        pers.Add(new Personnage("WILLY LE KID", 4, "Peut jouer autant de BANG! qu'il veut durant son tour",2));
        //pers.Add(new Personnage("PEDRO RAMIREZ", 4, "blablabla"));   //Faire ce perso ? 

        // pers.Add(new Personnage("LUCKY DUKE", 4, "blablabla"));
        // pers.Add(new Personnage("SLAB LE FLINGUEUR", 4, "blablabla"));
        // pers.Add(new Personnage("JOURDONNAIS", 4, "blablabla"));
        // pers.Add(new Personnage("JESSE JONES", 4, "blablabla"));
        // pers.Add(new Personnage("SUZY LAFAYETTE", 4, "blablabla"));
        pers.Add(new Personnage("ROSE DOOLAN", 4, "Elle voit tous les autres joueurs à une distance réduite de 1",3));
        // pers.Add(new Personnage("CALAMITY JANET", 4, "blablabla"));
        // pers.Add(new Personnage("SID KETCHUM", 4, "blablabla")); 

        if (!isServer)
            CmdDebug(pers.Count);
        else
        {
            Debug.Log(pers.Count);
        }
    }

    [Command(ignoreAuthority = true)]
    public void CmdDebug(int nbpers)
    {
        Debug.Log("nbpers : " + nbpers);
    }

    public void DefinirPersonnages()
    {
        int k;
        int i = 0;

        List<int> used_nums = new List<int>();
        List<string> persos = new List<string>();

        for (i = 0; i < pers.Count; i++)
        {
            Debug.Log("Perso numéro " + i + " : " + pers[i].getNomPersonnage());
        }

        Debug.Log("----------Rempli list perso partie");
        //remplissage de la liste des persos de la partie
        i = 0;
        while (i < nb_joueurs_prets)
        {
            Debug.Log("i = " + i);
            //chercher aléatoirement un perso non deja selectionné
            k = UnityEngine.Random.Range(0, pers.Count);
            if (!used_nums.Contains(k))
            {
                Debug.Log("On a trouvé un num inutilisé");
                used_nums.Add(k); //ajouter son indice à la liste des rand nums
                Debug.Log("On a ajouté " + k);
                persos.Add(pers[k].getNomPersonnage()); //ajouter le perso à la liste des persos de la partie
                Debug.Log("i++");
                i++;
                Debug.Log("i = " + i);
            }
        }
        Debug.Log("----------Distri perso");
        //distribution personnages
        for (i = 0; i < nb_joueurs_prets; i++)
        {
            Debug.Log("distri perso, i = " + i);
            Debug.Log("" + pseudos_joueurs[i] + "a le personnage : " + persos[i]);
            CmdDonnerPersonnage(persos[i], pseudos_joueurs[i]);
        }
    }

    [Command(ignoreAuthority = true)]
    public void CmdDonnerPersonnage(string perso, string nomJoueur)
    {
        RpcJoueurGetsPersonnage(perso, nomJoueur);
    }

    [ClientRpc]
    public void RpcJoueurGetsPersonnage(string perso, string nomJoueur)
    {
        Joueur joueur;

        for (int i = 0; i < nb_joueurs_prets; i++)
        {
            joueur = players[i].GetComponent<Joueur>();
            if (Equals(joueur.getPseudo(), nomJoueur))
            {
                for (int j = 0; j < pers.Count; j++)
                {
                    if (Equals(pers[j].getNomPersonnage(), perso))
                    {
                        joueur.setPersonnage(pers[j]);
                      //  joueur.plateau_de_jeu.GetComponent<Plateau>().nomPersonnage.text = joueur.getPersonnage().getNomPersonnage();
                       // joueur.plateau_de_jeu.GetComponent<Plateau>().descriptionPersonnage.text = joueur.getPersonnage().getDescription();
                        joueur.plateau_de_jeu.GetComponent<Plateau>().imagePerso.sprite = spritepers[pers[j].getSprite()];
                    }
                    Debug.Log("Vie avant setVie : " + joueur.getVie() + " |On ajoute" + joueur.getPersonnage().getVie());

                    //obligé pour esquiver un bug lié au clientrpc
                    if (joueur.getVie() < 2)
                    {
                        joueur.setVie(joueur.getVie() + joueur.getPersonnage().getVie());
                        Debug.Log("Vie après setVie : " + joueur.getVie());
                        joueur.plateau_de_jeu.GetComponent<Plateau>().vieText.text = joueur.getVie().ToString();
                    }

                    if (Equals(joueur.getPersonnage().getNomPersonnage(), "ROSE DOOLAN"))
                        joueur.setPortee(2);
                }



                // if (Equals(role, "Shérif"))
                // {
                //     joueur.setVie(joueur.getVie() + 1);
                //     joueur.set_color(couleur[1]);
                //     joueur.roleText.text = "Shérif"; 
                // }
                return;
            }
        }
    }

    // Le serveur initialise la pioche aléatoirement puis la partage
    public void InitPioche()
    {
        List<Carte> pioche1 = new List<Carte>();

        // Actions
        pioche1.Add(new Bang(1, "Pique",0));
        pioche1.Add(new Bang(9, "Carreau",1));
        pioche1.Add(new Bang(3, "Trefle",2));
        pioche1.Add(new Bang(7, "Trefle",3));
        pioche1.Add(new Bang(13, "Carreau",4));
        pioche1.Add(new Bang(7, "Carreau",5));
        pioche1.Add(new Bang(1, "Carreau",6));
        pioche1.Add(new Bang(9, "Trefle",7));
        pioche1.Add(new Bang(4, "Carreau",8));
        pioche1.Add(new Bang(6, "Trefle",9));
        pioche1.Add(new Bang(13, "Coeur",10));
        pioche1.Add(new Bang(1, "Coeur",11));
        pioche1.Add(new Bang(5, "Carreau",12));
        pioche1.Add(new Bang(8, "Carreau",13));
        pioche1.Add(new Bang(8, "Trefle",14));
        pioche1.Add(new Bang(2, "Trefle",15));
        pioche1.Add(new Bang(10, "Carreau",16));
        pioche1.Add(new Bang(12, "Carreau",17));
        pioche1.Add(new Bang(6, "Carreau",18));
        pioche1.Add(new Bang(2, "Carreau",19));
        pioche1.Add(new Bang(4, "Trefle",20));
        pioche1.Add(new Bang(5, "Trefle",21));
        pioche1.Add(new Bang(12, "Coeur",22));
        pioche1.Add(new Bang(11, "Carreau",23));
        pioche1.Add(new Bang(3, "Carreau",24));

        pioche1.Add(new Rate(5, "Pique",25));
        pioche1.Add(new Rate(4, "Pique",26));
        pioche1.Add(new Rate(10, "Trefle",27));
        pioche1.Add(new Rate(11, "Trefle",28));
        pioche1.Add(new Rate(13, "Trefle",29));
        pioche1.Add(new Rate(6, "Pique",30));

        /*
        pioche1.Add(new Rate(7, "Pique"));
        pioche1.Add(new Rate(2, "Pique"));
        pioche1.Add(new Rate(8, "Pique"));
        pioche1.Add(new Rate(1, "Trefle"));
        pioche1.Add(new Rate(3, "Pique"));
        pioche1.Add(new Rate(12, "Trefle"));
        */

        pioche1.Add(new Biere(6, "Coeur",31));
        pioche1.Add(new Biere(7, "Coeur",32));
        pioche1.Add(new Biere(8, "Coeur",33));

        /*
        pioche1.Add(new Biere(9, "Coeur"));
        pioche1.Add(new Biere(10, "Coeur"));
        pioche1.Add(new Biere(11, "Coeur"));
        */

        pioche1.Add(new Convoi(9, "Pique",34));
        pioche1.Add(new Convoi(9, "Pique",35));

        pioche1.Add(new Diligence(3, "Coeur",36));

        pioche1.Add(new Saloon(5, "Coeur",37));

        pioche1.Add(new Gatling(10, "Coeur",38));
        pioche1.Add(new Gatling(10, "Coeur",39));
        pioche1.Add(new Gatling(10, "Coeur",40));

        pioche1.Add(new Indiens(13, "Carreau",41));
        pioche1.Add(new Indiens(1, "Carreau",42));
        pioche1.Add(new Indiens(13, "Carreau",43));
        pioche1.Add(new Indiens(13, "Carreau",44));

        pioche1.Add(new Duel(11, "Pique",45));
        pioche1.Add(new Duel(8, "Trefle",46));
        pioche1.Add(new Duel(12, "Carreau",47));

        /*   pioche1.Add(new Magasin(9, "Trefle"));
           pioche1.Add(new Magasin(12, "Pique"));*/

        pioche1.Add(new Braquage(11, "Coeur",48));
        pioche1.Add(new Braquage(12, "Coeur",49));

        /*
        pioche1.Add(new Braquage(8, "Carreau"));
        pioche1.Add(new Braquage(1, "Coeur"));
        */

        pioche1.Add(new CoupDeFoudre(9, "Carreau",50));
        pioche1.Add(new CoupDeFoudre(11, "Carreau",51));

        /*
        pioche1.Add(new CoupDeFoudre(10, "Carreau"));
        pioche1.Add(new CoupDeFoudre(13, "Carreau"));
        */

        // Objets
        pioche1.Add(new Mustang(8, "Coeur",52));
        pioche1.Add(new Mustang(9, "Coeur",53));

        pioche1.Add(new Lunette(1, "Pique",54));

        pioche1.Add(new Dynamite(2, "Coeur",55));
        pioche1.Add(new Dynamite(3, "Pique",56));
        pioche1.Add(new Dynamite(4, "Trefle",57));

        pioche1.Add(new Prison(4, "Coeur",58));
        pioche1.Add(new Prison(10, "Pique",59));
        pioche1.Add(new Prison(11, "Pique",60));

        pioche1.Add(new Planque(13, "Pique",61));
        pioche1.Add(new Planque(12, "Pique",62));

        // Armes
        pioche1.Add(new Volcanic(10, "Trefle",63));
        pioche1.Add(new Volcanic(10, "Pique",64));

        pioche1.Add(new Schofield(13, "Pique",65));
        pioche1.Add(new Schofield(11, "Trefle",66));
        pioche1.Add(new Schofield(12, "Trefle",67));

        pioche1.Add(new Remington(13, "Trefle",68));

        pioche1.Add(new Carabine(1, "Trefle",69));

        pioche1.Add(new Winchester(8, "Pique",70));

        // Melange
        int n = pioche1.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            Carte value = pioche1[k];
            pioche1[k] = pioche1[n];
            pioche1[n] = value;
        }

        // Partage la pioche
        for (int j = 0; j < pioche1.Count; j++)
        {
            ////Debug.Log("INIT NEW LIST  "+ j + "taille :" +  pioche1.Count);
            CmdInitNewList(pioche1[j].getNombre(), pioche1[j].GetType().Name, pioche1[j].getFigure(), pioche1[j].getSprite());
        }
    }

    [Command(ignoreAuthority = true)]
    public void CmdInitNewList(int nombre, string nom, string figure, int sprite)
    {
        RpcInitNewList(nombre, nom, figure, sprite);
    }

    //Utiliser pour créer une liste et pour la mettre à jour surement le remplacer avec "SUPP" élément 
    [ClientRpc]
    public void RpcInitNewList(int nombre, string nom, string figure, int sprite)
    {
        Carte newCarte = (Carte)Activator.CreateInstance(Type.GetType(nom), nombre, figure, sprite); //On instancie la carte trouvé 

        pioche.Add(newCarte); //On ajoute élément par élément dans la liste vide  

    }

    public void InitMain()
    {
        int vie = players[0].GetComponent<Joueur>().getVie();

        int shef = 0;

        if(players[0].GetComponent<Joueur>().getRole().Equals("Shérif"))
        {
            shef = 2;
        }

        for (int j = 0; j < players[0].GetComponent<Joueur>().getVie()+shef; j++)
        {
            players[0].GetComponent<Joueur>().piocher(ref pioche, ref defausse);
        }



       //delete      // delete cartes_init();
    }

    [Command(ignoreAuthority = true)]
    public void CmdPiocherCarteMulti(int numfois, int numjour)
    {
        RpcSuppCarte(numfois, numjour);
    }

    [ClientRpc]
    public void RpcSuppCarte(int numfois, int numjour)
    {
        if (numero_joueur == numjour)
            return;

        for (int i = 0; i < numfois; i++)
            pioche.RemoveAt(0);
    }

    //A mettre dans le loop de piocher carte dans le 
    public void loop_partie()
    {
        //A rajouter dans le jeu 
        int index;

        doit_piocher = false;
        deja_bang = false;

        if (players[indiceJoueurCourant].GetComponent<Joueur>().possedePlateau("Dynamite"))
        {
            if (players[indiceJoueurCourant].GetComponent<Joueur>().degainer(ref pioche, ref defausse, new List<int>() { 2, 3, 4, 5, 6, 7, 8, 9 }, "Pique"))
            {
                Debug.Log("Coucou je dynamite 1");
                Carte carte = players[indiceJoueurCourant].GetComponent<Joueur>().main[players[indiceJoueurCourant].GetComponent<Joueur>().indexCartePlateau("Dynamite")];
                index = players[indiceJoueurCourant].GetComponent<Joueur>().indexCartePlateau("Dynamite");
                Debug.Log("D1 Index est : " + index);
                CmdEffect(0, carte.getNombre(), carte.GetType().Name, carte.getFigure(),carte.getSprite(), index);

            }
            else
            {
                Debug.Log("Coucou je dynamite 2");
                Carte carte = players[indiceJoueurCourant].GetComponent<Joueur>().main[players[indiceJoueurCourant].GetComponent<Joueur>().indexCartePlateau("Dynamite")];
                index = players[indiceJoueurCourant].GetComponent<Joueur>().indexCartePlateau("Dynamite");
                Debug.Log("D2 Index est : " + index);
                CmdEffect(1, carte.getNombre(), carte.GetType().Name, carte.getFigure(),carte.getSprite(), index);
            }
        }
        if (players[indiceJoueurCourant].GetComponent<Joueur>().possedePlateau("Prison"))
        {
            if (players[indiceJoueurCourant].GetComponent<Joueur>().degainer(ref pioche, ref defausse, new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 }, "Coeur"))
            {

                Debug.Log("Coucou je prison 1");

                index = players[indiceJoueurCourant].GetComponent<Joueur>().indexCartePlateau("Prison");

                CmdEffect(2, 0, "", "",0, index);

            }
            else
            {
                Debug.Log("Coucou je prison 2");

                index = players[indiceJoueurCourant].GetComponent<Joueur>().indexCartePlateau("Prison");

                CmdEffect(3, 0, "", "",0, index);
                doit_piocher = false;
                terminer_tour();
                return;

            }
        }
        else
        {
            int bj = 0;
            bool piocher_vide = false;
            for (int i = 0; i < 2; i++)
            {
                if (pioche.Count == 0)
                    piocher_vide = true;            

                players[indiceJoueurCourant].GetComponent<Joueur>().piocher(ref pioche, ref defausse);
                if (Equals(players[indiceJoueurCourant].GetComponent<Joueur>().getPersonnage().getNomPersonnage(), "BLACK JACK") && i == 1)
                {
                    Black_Jack(indiceJoueurCourant);
                    bj = 1;
                }

            }
            update_plateau();

            if (piocher_vide == false)
            {
                CmdPiocherCarteMulti(2 + bj, numero_joueur);
            }else
            {
                CmdViderList(this.numero_joueur);
            }
 
            doit_piocher = false;
        }
    }

    [Command(ignoreAuthority = true)]
    public void CmdEffect(int value, int num, string nom, string figure,int sprite, int indextrue)
    {
        RpcEffectOn(value, num, nom, figure,sprite, indextrue);
    }

    [ClientRpc]
    public void RpcEffectOn(int values, int num, string nom, string figure,int sprite, int indextrue)
    {
        Carte newCarte;

        if (values == 0)
        {

            newCarte = (Carte)Activator.CreateInstance(Type.GetType(nom), num, figure, sprite);

            //ClientRpc
            instructions_jeu.text = players[indiceJoueurCourant].GetComponent<Joueur>().getPseudo() + " vient d'exploser la dynamite ! Il perd 3 points de vie";
            historique_text.text = historique_text.text + "\n\n-" + instructions_jeu;
            players[indiceJoueurCourant].GetComponent<Joueur>().setVie(players[indiceJoueurCourant].GetComponent<Joueur>().getVie() - 3);

            defausse.Add(newCarte);

            //Debug.Log("Bug 0 ? pour : " + ((indiceJoueurCourant + 1) % players.Count));

            players[indiceJoueurCourant].GetComponent<Joueur>().plateau.RemoveAt(indextrue);

            if (players[indiceJoueurCourant].GetComponent<Joueur>().est_mort())
            {
                int taille = players[0].GetComponent<Joueur>().main.Count;
                for (int i = 0; i < taille; i++)
                {
                    players[0].GetComponent<Joueur>().main.RemoveAt(0);
                }

                terminer_tour();
            }



            //Debug.Log("Bug 1 ? pour : " + ((indiceJoueurCourant + 1) % players.Count));

           // delete      // delete cartes_init();
            update_plateau();
            init_carte_defausse();
            if (verifiePartie() != 0)
            {
                finPartie();
            }
        }

        if (values == 1)
        {
            newCarte = (Carte)Activator.CreateInstance(Type.GetType(nom), num, figure);
            instructions_jeu.text = players[indiceJoueurCourant].GetComponent<Joueur>().getPseudo() + "  l'a echappé belle ! Il passe la dynamite à " + players[(indiceJoueurCourant + 1) % players.Count].GetComponent<Joueur>().getPseudo();
            historique_text.text = historique_text.text + "\n\n-" + instructions_jeu;

            //Debug.Log(" " + instructions_jeu.text.ToString());

            Debug.Log("Bug 0 ? pour : " + indextrue);

            //c'est là le problème
            players[(indiceJoueurCourant + 1) % players.Count].GetComponent<Joueur>().plateau.Add(players[indiceJoueurCourant].GetComponent<Joueur>().plateau[indextrue]);

            int index = players[indiceJoueurCourant].GetComponent<Joueur>().indexCartePlateau("Dynamite");

            //Debug.Log("Bug 1 ? pour : " + ((indiceJoueurCourant + 1) % players.Count) + "à l'index" + index);

            players[indiceJoueurCourant].GetComponent<Joueur>().plateau.RemoveAt(indextrue);

            //Debug.Log("Bug 1 ?  fin ? ");

           // delete      // delete cartes_init();
            update_plateau();

        }

        if (values == 2)
        {
            instructions_jeu.text = players[indiceJoueurCourant].GetComponent<Joueur>().getPseudo() + " sort de Prison ! Il a dégainé la carte suivante : " + defausse[0].getNomCarte() + " " + defausse[defausse.Count - 1].getNombre() + defausse[defausse.Count - 1].getFigure();
            historique_text.text = historique_text.text + "\n\n-" + instructions_jeu;
            int index = players[indiceJoueurCourant].GetComponent<Joueur>().indexCartePlateau("Prison");
            Debug.Log("Index est :" + index);
            defausse.Add(players[indiceJoueurCourant].GetComponent<Joueur>().plateau[indextrue]);
            players[indiceJoueurCourant].GetComponent<Joueur>().plateau.RemoveAt(indextrue);

            update_plateau();

        }

        if (values == 3)
        {
            instructions_jeu.text = players[indiceJoueurCourant].GetComponent<Joueur>().getPseudo() + " : Ne sort pas de Prison. Il a dégainer la carte suivante : " + defausse[0].getNomCarte() + " " + defausse[defausse.Count - 1].getNombre() + defausse[defausse.Count - 1].getFigure();
			historique_text.text = historique_text.text + "\n\n-" + instructions_jeu;
            if (indiceJoueurCourant == 0 && est_mort == false)
            {
                terminer_tour();
            }
        }
    }

    public void terminer_tour()
    {
        if (indiceJoueurCourant == 0 && action_en_cours == false)
        {
            if (est_mort == true)
                return;

            if (players[0].GetComponent<Joueur>().getVie() < players[0].GetComponent<Joueur>().main.Count)
            {
                instructions_jeu.text = "Vous avez trop de carte pour terminer le tour";
            }
            else
            {
                if (est_mort == false)
                    CmdUpdateJoueurCourant(1);
                else
                    CmdUpdateJoueurCourant(0);

                    //scene.text = players[indiceJoueurCourant].GetComponent<Joueur>().getPseudo() + " à toi de jouer!";
                    deja_bang = false;
                    action_en_cours = false;
                    indexJoueur2 = 0;
                    passage++;
                    //loop_partie();
            }
        }
        else
        {
            if (action_en_cours == true)
                warning.text = "Terminez l'action en cours!";
            else
                warning.text = "Ce n'est pas votre tour !";
            



            if (est_mort == false)  //Met à jour les cartes quand le joueur 
            {
                Debug.Log("Faire revenir carte");
                players[0].GetComponent<Joueur>().Mise_a_jour_carte();
            }
        }
    }

    [Command(ignoreAuthority = true)]
    public void CmdUpdateJoueurCourant(int i)
    {
        RpcUpdateJoueurCourant(i);
    }

    // Modifie l'index du joueur courant, remet la couleur de l'ancien index en noir, et met le nouveau en vert
    [ClientRpc]
    public void RpcUpdateJoueurCourant(int i)
    {
        players[indiceJoueurCourant].GetComponent<Joueur>().set_bg(bg[0]);
        indiceJoueurCourant = (indiceJoueurCourant + i) % nb;
        players[indiceJoueurCourant].GetComponent<Joueur>().set_bg(bg[1]);
        instructions_jeu.text = "";
        scene.text = players[indiceJoueurCourant].GetComponent<Joueur>().getPseudo() + " à toi de jouer!";

        //Update pour prendre deux cartes 
        if(indiceJoueurCourant == 0 && est_mort == false)
        {
            doit_piocher = true;
        }
    }

    // Affiche les cartes de chaque joueur dans le menus déroulant correspondant. 
    // Les menus déroulants des autres joueurs seront supprimés (i.e pas de boucle et i=0) après intégration avec la TEAM RESEAU
  /*  [ClientRpc]
    public void plateau_init()
    {
        mon_plateau.ClearOptions();
        foreach (var carte in players[0].GetComponent<Joueur>().plateau)
            mon_plateau.options.Add(new Dropdown.OptionData() { text = carte.getNomCarte() });
    }

    public void indexJ2_Dropdown_init()
    {
        indexJ2_dropdown.ClearOptions();
        foreach (var joueur in players)
            indexJ2_dropdown.options.Add(new Dropdown.OptionData() { text = joueur.GetComponent<Joueur>().getPseudo() });
    }

    public void indexJ2Selected()
    {
        indexJoueur2 = indexJ2_dropdown.value;
    }
  */
    // #TO_DO TEAM IHM -INCOMPLETE
    // On-Click() sur le bouton "Jouer" sur la carte: effectue l'action de la carte si conditions vraies.
    public void action_joueur()
    {
        if(indiceJoueurCourant != 0)
        {
            players[0].GetComponent<Joueur>().Mise_a_jour_carte();
            scene.text = "Ce n'est pas votre Tour";
            return;
        }

        if (action_en_cours == false)
        {
            if (tour == 0){
                //indexCarte = players[indiceJoueurCourant].GetComponent<Joueur>().cartes.GetComponent<Dropdown>().value;
            }
            //on regarde si la carte demande du multi 
            if (players[indiceJoueurCourant].GetComponent<Joueur>().main[indexCarte].getMulti())
            {
                //players[indiceJoueurCourant].GetComponent<Joueur>().cacher_dropdown();

                //deux joueur doivent communiquer et ensuite mise à jour en fonction du joueur 
                faireActionMulti();
            }
            else
            {
                tour = 0;
                if (players[indiceJoueurCourant].GetComponent<Joueur>().main[indexCarte].getNomCarte().Equals("Magasin"))
                {
                    List<Carte> magasin = new List<Carte> { pioche[0], pioche[1], pioche[2], pioche[3] };
                    for (int i = 0; i < 4; i++)
                    {
                        //Les deux en ClientRpc
                        defausse.Add(pioche[0]);

                        //ClientRpc
                        pioche.RemoveAt(0);
                    }
                    
                    //Je sais pas encore à 100/100 ce que ça fait 
                    players[indiceJoueurCourant].GetComponent<Joueur>().main[indexCarte - 1].action(indiceJoueurCourant, indexJoueur2, ref defausse, ref pioche, ref players, ref instructions_jeu, ref historique_text);
                    update_plateau();
                    for (int i = 0; i < magasin.Count; i++)
                    {
                        // #TO_DO afficher les cartes : (i + 1).ToString() + magasin[i]);
                    }
                    instructions_jeu.text = "Quelles carte veux-tu récupérer ?";
                    // #TO_DO: action magasin
                }
                else
                {
                    if (players[indiceJoueurCourant].GetComponent<Joueur>().main[indexCarte].getNomCarte().Equals("Gatling"))
                    {
                        players[indiceJoueurCourant].GetComponent<Joueur>().setScorePartie(players[indiceJoueurCourant].GetComponent<Joueur>().getScorePartie()+5);
                        action_en_cours = true;

                        players[indiceJoueurCourant].GetComponent<Joueur>().main[indexCarte].action(indiceJoueurCourant, indexJoueur2, ref defausse, ref pioche, ref players, ref instructions_jeu, ref historique_text);

                        CmdCarteChange(0);
                        CmdSceneText(players[0].GetComponent<Joueur>().getPseudo() + " utilise la Gatling !");
                        historique_text.text += "\n\n" + players[0].GetComponent<Joueur>().getPseudo() + " utilise la Gatling ! !";
                        CmdUtiliserRateMulti(players[1].GetComponent<Joueur>().getPseudo());


                        // delete     // delete cartes_init();
                        update_plateau();
                        // TO DO reponse_rate_multiple();
                    }
                    else if (players[indiceJoueurCourant].GetComponent<Joueur>().main[indexCarte].getNomCarte().Equals("Indiens"))
                    {
                        players[indiceJoueurCourant].GetComponent<Joueur>().setScorePartie(players[indiceJoueurCourant].GetComponent<Joueur>().getScorePartie()+5);
                        action_en_cours = true;
                        CmdCarteChange(1);
                        //Ajouter carte dans defausse
                        CmdUtiliserRateMulti(players[1].GetComponent<Joueur>().getPseudo());
                        players[indiceJoueurCourant].GetComponent<Joueur>().main[indexCarte].action(indiceJoueurCourant, indexJoueur2, ref defausse, ref pioche, ref players, ref instructions_jeu, ref historique_text);
                        CmdSceneText(players[0].GetComponent<Joueur>().getPseudo() + " appelle les indiens !");
                        historique_text.text += "\n\n-" + players[0].GetComponent<Joueur>().getPseudo() + " appelle les indiens !";
                        // delete     // delete cartes_init();
                        update_plateau();
                        // #TO_DO: utiliser BANG ?
                    }
                    else
                    {
                        //Je stocke la carte pour donner les infos à la fonction réseau  
                        Carte newCarte = players[indiceJoueurCourant].GetComponent<Joueur>().main[indexCarte];

                        //string nom2 = players[indexJ2].GetComponent<Joueur>().getPseudo();  //A utiliser pour les actions mutli simple

                        CmdActionSimple(newCarte.getNombre(), newCarte.GetType().Name, newCarte.getFigure(),newCarte.getSprite(), indexJoueur2);

                        //.action(indiceJoueurCourant, indexJoueur2, ref defausse, ref pioche, ref players, ref instructions_jeu);

                        // delete     // delete cartes_init();
                        update_plateau();
                        CmdVerifPartie();
                    }
                }
            }
        }
        else
        {
            warning.text = "Terminez l'action en cours!";

            if (est_mort == false)  //Met à jour les cartes quand le joueur 
            {
                players[0].GetComponent<Joueur>().Mise_a_jour_carte();
            }
        }
    }

    [Command(ignoreAuthority = true)]
    public void CmdActionSimple(int num, string nom, string figure,int sprite, int indexJoueur2)
    {
        RpcActionSimple(num, nom, figure,sprite, indexJoueur2);
    }

    [ClientRpc]
    public void RpcActionSimple(int num, string nom, string figure,int sprite, int indexJoueur2)
    {
        Carte newCarte = (Carte)Activator.CreateInstance(Type.GetType(nom), num, figure,sprite);

        if (Equals(newCarte.getTypeCarte(), "Objet") || Equals(newCarte.getTypeCarte(), "Arme"))
        {
            pioche.Add(newCarte); //La carte est ajouté pour pouvoir l'utiliser le temps de la fonction 
        }

        //Debug.Log("Vous avez choisi carte");

        newCarte.action(indiceJoueurCourant, indexJoueur2, ref defausse, ref pioche, ref players, ref instructions_jeu, ref historique_text);
       // delete     // delete cartes_init();
        update_plateau();
    }

    public void faireActionMulti()
    {
        // Premier passage: choix de joueur 2 ça en local 
        if (action_en_cours == false)
        {
            if (tour == 0)
            {
                if (players[indiceJoueurCourant].GetComponent<Joueur>().main[indexCarte].getNomCarte().Equals("Bang!")
                    && deja_bang
                    && !players[indiceJoueurCourant].GetComponent<Joueur>().possedePlateau("Volcanic")
                    && !Equals(players[indiceJoueurCourant].GetComponent<Joueur>().getPersonnage().getNomPersonnage(), "WILLY LE KID"))
                { 
                    players[0].GetComponent<Joueur>().Mise_a_jour_carte();
                    instructions_jeu.text = "Vous ne pouvez Bang! qu'une seule fois par tour.";
                }
                else
                {
                    instructions_jeu.text = "Quel Cow-Boy vise-tu ? Clique sur ta cible !";
                    tour++;
                }
                // #TO_DO: attendre le choix du j2 par clic souris et recupérer son index. variable 'tour' temporaire pour tester.
            }
            // Deuxième passage: faire l'action ça en local 
            else
            {
                if (indexJoueur2 == indiceJoueurCourant)
                {
                    instructions_jeu.text = "Tu ne peux pas effectuer d'action sur toi même. Quel Cow-Boy vises-tu ? ";
                }
                else
                {
                    instructions_jeu.text = " ";
                    //CARTE BANG
                    if (players[indiceJoueurCourant].GetComponent<Joueur>().main[indexCarte].getNomCarte().Equals("Bang!"))
                    {
                        if (deja_bang
                            && !players[indiceJoueurCourant].GetComponent<Joueur>().possedePlateau("Volcanic")
                            && !Equals(players[indiceJoueurCourant].GetComponent<Joueur>().getPersonnage().getNomPersonnage(), "WILLY LE KID"))

                            instructions_jeu.text = "Vous ne pouvez Bang! qu'une seule fois par tour.";
                        else if (distance(indiceJoueurCourant, indexJoueur2) > players[indiceJoueurCourant].GetComponent<Joueur>().getPortee())
                            instructions_jeu.text = "Vous n'avez pas la portée suffisante pour tirer sur " + players[indexJoueur2].GetComponent<Joueur>().getPseudo() + "Tu as une portée de " + players[indiceJoueurCourant].GetComponent<Joueur>().getPortee().ToString() + " et il est à une distance de " + distance(indiceJoueurCourant, indexJoueur2).ToString();
                        else
                        {
                            if (players[indexJoueur2].GetComponent<Joueur>().possedePlateau("Planque"))
                            {
                                if (players[indexJoueur2].GetComponent<Joueur>().degainer(ref pioche, ref defausse, new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 }, "Coeur"))
                                {
                                    String text = players[indexJoueur2].GetComponent<Joueur>().getPseudo() + " s'est planqué! Il a esquivé le bang.";
                                    CmdSceneText(text);

                                    deja_bang = true;
                                    tour = 0;

                                    carte_utilisee(indexCarte, indiceJoueurCourant);
                                }
                                else
                                {
                                    String text = players[indexJoueur2].GetComponent<Joueur>().getPseudo() + " a tenté de se planquer! Mais cela a échoué!";
                                    CmdSceneText(text);

                                    deja_bang = true;
                                    action_en_cours = true;
                                    tour = 0;

                                    CmdUtiliserRateMulti(players[indexJoueur2].GetComponent<Joueur>().getPseudo());
                                    carte_utilisee(indexCarte, indiceJoueurCourant);
                                }
                            }
                            else
                            {
                                deja_bang = true;
                                action_en_cours = true;
                                tour = 0;

                                String text = players[0].GetComponent<Joueur>().getPseudo() + " attaque avec un bang " + players[indexJoueur2].GetComponent<Joueur>().getPseudo();
                                CmdSceneText(text);


                                CmdUtiliserRateMulti(players[indexJoueur2].GetComponent<Joueur>().getPseudo());
                                carte_utilisee(indexCarte, indiceJoueurCourant);
                            }
                        }
                    }
                    // CARTE BRAQUAGE
                    //Implémentation de ces cartes 

                    //La prochaine étape à faire  
                    else if (players[indiceJoueurCourant].GetComponent<Joueur>().main[indexCarte].getNomCarte().Equals("Braquage"))
                    {
                        //local 
                        if (distance(indiceJoueurCourant, indexJoueur2) > 1)
                            instructions_jeu.text = players[indiceJoueurCourant].GetComponent<Joueur>().getPseudo() + " : Ta cible est à une distance supérieure à 1, tu ne peux pas la braquer !\r\n Quel Cow-Boy vises-tu ?";
                        else
                        {
                            players[indiceJoueurCourant].GetComponent<Joueur>().main[indexCarte].action(indiceJoueurCourant, indexJoueur2, ref defausse, ref pioche, ref players, ref instructions_jeu, ref historique_text);
                            //rajouter la défausse pour cette carte
                            //On charge les cartes du joueur2

                            int index = players[0].GetComponent<Joueur>().indexCarte("Braquage");
                            Carte newCarte = players[0].GetComponent<Joueur>().main[index];
                            players[0].GetComponent<Joueur>().main.RemoveAt(index);

                            CmdChargerCarteJ2(players[indexJoueur2].GetComponent<Joueur>().getPseudo());
                            //voler.SetActive(true);
                            // delete cartes_nit_J2();
                           
                            
                            //action_braquage();
                            tour = 0;
                            //action_en_cours = false;
                        }
                    }
                    // CARTE COUP DE FOUDRE

                    //Encore à faire 
                    else if (players[indiceJoueurCourant].GetComponent<Joueur>().main[indexCarte].getNomCarte().Equals("CoupDeFoudre"))
                    {
                        players[indiceJoueurCourant].GetComponent<Joueur>().main[indexCarte].action(indiceJoueurCourant, indexJoueur2, ref defausse, ref pioche, ref players, ref instructions_jeu, ref historique_text);

                        //Rajouter la défausse pour cette carte
                        int index = players[0].GetComponent<Joueur>().indexCarte("CoupDeFoudre");
                        Carte newCarte = players[0].GetComponent<Joueur>().main[index];
                        players[0].GetComponent<Joueur>().main.RemoveAt(index);

                        RpcEnleverCarteJ2(0, newCarte.GetType().Name, newCarte.getFigure(), players[indexJoueur2].GetComponent<Joueur>().getPseudo(),0);
                        
                        //bouton_defausse.SetActive(true);
                        //action_coup_de_foudre();
                        // delete cartes_nit_J2();
                        tour = 0;
                        //action_en_cours = true;
                    }
                    // AUTRE CARTE
                    else
                    {
                        if (players[indiceJoueurCourant].GetComponent<Joueur>().main[indexCarte].getNomCarte().Equals("Duel"))
                        {
                            //Début du duel
                            CmdFaireDuel(players[indexJoueur2].GetComponent<Joueur>().getPseudo());

                            players[indiceJoueurCourant].GetComponent<Joueur>().main.RemoveAt(indexCarte);

                            tour = 0;
                        }
                        else
                        {
                            tour = 0;
                            //Les autres cartes pour l'instants seul duel est spécial


                            //players[indiceJoueurCourant].GetComponent<Joueur>().main[indexCarte].action(indiceJoueurCourant, indexJoueur2, ref defausse, ref pioche, ref players, ref instructions_jeu);

                            Carte newCarte = players[indiceJoueurCourant].GetComponent<Joueur>().main[indexCarte];

                            CmdActionMultiMulti(newCarte.getNombre(), newCarte.GetType().Name, newCarte.getFigure(), players[indexJoueur2].GetComponent<Joueur>().getPseudo(), newCarte.getSprite());
                        }

                       //delete      // delete cartes_init();
                        update_plateau();

                        CmdVerifPartie();

                        update_plateau();
                        tour = 0;
                    }
                }
            }
        }
        else
        {
            warning.text = "Terminez l'action en cours!";

            if (est_mort == false)  //Met à jour les cartes quand le joueur 
            {
                players[0].GetComponent<Joueur>().Mise_a_jour_carte();
            }
        }
    }

    [Command(ignoreAuthority = true)]
    public void CmdFaireDuel(string nom)
    {
        RpcDuelPret(nom);
    }

    [ClientRpc]
    public void RpcDuelPret(string nom)
    {
        //Debug.Log("Debut du duel");

        int i = 0;
        int j2;
        while (!Equals(players[i].GetComponent<Joueur>().getPseudo(), nom))
        {
            i++;
        }
        j2 = i;
        scene.text = players[indiceJoueurCourant].GetComponent<Joueur>().getPseudo() + " à provoqué " + players[j2].GetComponent<Joueur>().getPseudo() + " en Duel !";

        //si vous êtes le joueur 2 alors regarder si vous avez la bonne carte 
        if (j2 == 0 && est_mort == false)
        {
            //Debug.Log("Je suis joueur 2");

            if (players[j2].GetComponent<Joueur>().possede("Bang!"))
            {
                players[j2].GetComponent<Joueur>().main.RemoveAt(players[j2].GetComponent<Joueur>().indexCarte("Bang!"));
                CmdModifDuel(0, players[j2].GetComponent<Joueur>().getPseudo());
            }
            else
            {
                CmdFinBoucle(1, players[j2].GetComponent<Joueur>().getPseudo());
            }
        }
    }

    //Début de la boucle 
    [Command(ignoreAuthority = true)]
    public void CmdModifDuel(int val, string nom)
    {
        RpcBoucleDuel(val, nom);
    }

    [ClientRpc]
    public void RpcBoucleDuel(int val, string nom)
    {
        int i = 0;
        int j2;
        while (!Equals(players[i].GetComponent<Joueur>().getPseudo(), nom))
        {
            i++;
        }
        j2 = i;

        if (val == 0)
        {
            //Debug.Log("Attaque joueur 1");
            scene.text += players[j2].GetComponent<Joueur>().getPseudo() + " BANG! " + players[indiceJoueurCourant].GetComponent<Joueur>().getPseudo();
        }
        else
        {
            //Debug.Log("Attaque joueur 2");
            scene.text += players[indiceJoueurCourant].GetComponent<Joueur>().getPseudo() + " BANG! " + players[j2].GetComponent<Joueur>().getPseudo();
        }

        //Pour le joueur J1 ou le joueur indice courant
        if (val == 0 && indiceJoueurCourant == 0 && est_mort == false)
        {
            if (players[indiceJoueurCourant].GetComponent<Joueur>().possede("Bang!"))
            {
                players[indiceJoueurCourant].GetComponent<Joueur>().main.RemoveAt(players[indiceJoueurCourant].GetComponent<Joueur>().indexCarte("Bang!"));
                CmdModifDuel(1, nom);
            }
            else
            {
                //Le joueur 1 perd
                CmdFinBoucle(0, nom);
            }
        }

        //Pour le joueur J2 attaquer
        if (val == 1 && Equals(nom, players[0].GetComponent<Joueur>().getPseudo()))
        {
            if (players[0].GetComponent<Joueur>().possede("Bang!"))
            {
                players[0].GetComponent<Joueur>().main.RemoveAt(players[0].GetComponent<Joueur>().indexCarte("Bang!"));
                CmdModifDuel(0, nom);
            }
            else
            {
                CmdFinBoucle(1, nom);
            }
        }
    }

    // On ressort de la boucle
    [Command(ignoreAuthority = true)]
    public void CmdFinBoucle(int win, string nom)
    {
        RpcFinDuel(win, nom);
    }

    [ClientRpc]
    public void RpcFinDuel(int win, string nom)
    {
        //Debug.Log("Fin du duel");

        int j2;
        int j = 0;

        // On recherche le nom du joueur 2 chez le joueur actuel
        while (!Equals(nom, players[j].GetComponent<Joueur>().getPseudo()))
        {
            j++;
        }

        j2 = j;

        // Le joueur 2 à perdu et le joueur indiceJoueurCourant à gagné
        if (win == 1 )//&& est_mort == false)
        {
            players[j2].GetComponent<Joueur>().setVie(players[j2].GetComponent<Joueur>().getVie() - 1);
            scene.text = players[j2].GetComponent<Joueur>().getPseudo() + " a perdu le Duel. Il n'a plus que " + players[j2].GetComponent<Joueur>().getVie() + " points de vie(s)";
            players[indiceJoueurCourant].GetComponent<Joueur>().setScorePartie(players[indiceJoueurCourant].GetComponent<Joueur>().getScorePartie()+10);
            players[j2].GetComponent<Joueur>().setScorePartie(players[j2].GetComponent<Joueur>().getScorePartie()-10);
        }

        //Le joueur 1 à perdu et le joueur 2 à gagné
        if (win == 0) //&& est_mort == false)
        {
            players[indiceJoueurCourant].GetComponent<Joueur>().setVie(players[indiceJoueurCourant].GetComponent<Joueur>().getVie() - 1);
            scene.text = players[indiceJoueurCourant].GetComponent<Joueur>().getPseudo() + " a perdu le Duel. Il n'a plus que " + players[indiceJoueurCourant].GetComponent<Joueur>().getVie() + " points de vie(s)";
            players[indiceJoueurCourant].GetComponent<Joueur>().setScorePartie(players[indiceJoueurCourant].GetComponent<Joueur>().getScorePartie()-10);
            players[j2].GetComponent<Joueur>().setScorePartie(players[j2].GetComponent<Joueur>().getScorePartie()+10);
        }

        // delete     // delete cartes_init();
        update_plateau();

        if (verifiePartie() != 0)
        {
            finPartie();
        }
        return;
    }

    [Command(ignoreAuthority = true)]
    // Nom représente le nom du joueur qui va se faire attaquer pour le faire apparaîter 
    public void CmdUtiliserRateMulti(string nom)
    {
        RpcVousEtesJoueur2(nom);
    }

    [ClientRpc]
    // Affiche chez le joueur  2 les bouton rate 
    public void RpcVousEtesJoueur2(string nom)
    {
        //Debug.Log("Bonjour je teste ce :" + nom);

        // Si c'est le joueur qui a envoyé la demande alors j'attends 
        if (Equals(players[0].GetComponent<Joueur>().getPseudo(), nom) && est_mort == false)
        {
            if (0 == indiceJoueurCourant && (carte_indien == true || carte_gatling == true) && est_mort == false)
            {
                action_en_cours = false;
                tour = 0;

                if (carte_indien){
                    CmdCarteChange(1);
                }

                if (carte_gatling){
                    CmdCarteChange(0);
                }
                return;
            }

            rate.SetActive(true);

            if (carte_indien == true){
                instructions_jeu.text = nom + "veux-tu utiliser un Bang !";
            }
            else
            {
                instructions_jeu.text = nom + ", veux-tu utiliser un Raté! ?";
            }
        }
    }

    [Command(ignoreAuthority = true)]
    public void CmdActionFinie()
    {
        RpcActionFinie();
    }

    [ClientRpc]
    public void RpcActionFinie()
    {
        Debug.Log("Vérification de la fin d'une action");

        action_en_cours = false;
        tour = 0;
        deja_bang = true;
    }

    //C'est local
    public void utiliserRater()
    {
        rate.SetActive(false);

        // Stocke le text final
        string text;

        int index;

        string nom_joueur_suivant = players[1].GetComponent<Joueur>().getPseudo();

        if (carte_indien == true)
        {
            text = players[0].GetComponent<Joueur>().getPseudo() + " veut tu défausser un Bang!";
            if (players[0].GetComponent<Joueur>().possede("Bang!") == false)
            {
                text += "\r\n Mais il n'a pas de Bang! Mauvais Bluff!\r\n";
                CmdInstructionJeu(text);

                CmdJoueurPerdVie(players[0].GetComponent<Joueur>().getPseudo());
               // delete     // delete cartes_init();
                update_plateau();
                CmdVerifPartie();
            }
            else
            {
                index = players[0].GetComponent<Joueur>().indexCarte("Bang!");
                Carte newCarte = players[0].GetComponent<Joueur>().main[index];
                CmdChangerDefausse(2, newCarte.getNombre(), newCarte.GetType().Name, newCarte.getFigure(), newCarte.getSprite(), 0);
                players[0].GetComponent<Joueur>().main.RemoveAt(index);

                int i = players[0].GetComponent<Joueur>().indexCarte("Raté!");
                text = players[0].GetComponent<Joueur>().getPseudo() + " vient d'utiliser un Bang !";
                players[indiceJoueurCourant].GetComponent<Joueur>().setScorePartie(players[indiceJoueurCourant].GetComponent<Joueur>().getScorePartie()+2);

                //Affichage dans le jeu
                CmdInstructionJeu(text);

                //Mettre ce truc en Réseau

                //Peut être pas mis en Réseau par forcement besoin puisque on l'a mis avant
                //carte_utilisee(i, indexJoueur2);

                //On change le message du warning même si on peut  mettre un message plus sympa
                CmdChangerWarning("");
               // delete     // delete cartes_init();
                update_plateau();

                Debug.Log("Coucou je cherche à comprendre pourquoi ça bug");
            }
            tour = 0;

            //On donne la main au joueur suivant
            CmdUtiliserRateMulti(nom_joueur_suivant);
            update_plateau();
            //players[indiceJoueurCourant].GetComponent<Joueur>().afficher_dropdown();
            return;
        }


        text = players[0].GetComponent<Joueur>().getPseudo() + " veux utiliser un Raté!";

        if (carte_gatling == true)
        {
            if (players[0].GetComponent<Joueur>().possede("Raté!") == false)
            {
                text += "\nMais il n'a pas de Raté! Mauvais Bluff!";


                CmdInstructionJeu(text);

                CmdJoueurPerdVie(players[0].GetComponent<Joueur>().getPseudo());
               //delete     // delete cartes_init();
                update_plateau();
                CmdVerifPartie();
            }
            else
            {
                index = players[0].GetComponent<Joueur>().indexCarte("Raté!");
                Carte newCarte = players[0].GetComponent<Joueur>().main[index];
                CmdChangerDefausse(2, newCarte.getNombre(), newCarte.GetType().Name, newCarte.getFigure(), newCarte.getSprite(), 0);

                //Mettre ce truc en Réseau

                carte_utilisee(index, indexJoueur2);

                text = players[0].GetComponent<Joueur>().getPseudo() + " vient d'utiliser un Raté!";
                players[indiceJoueurCourant].GetComponent<Joueur>().setScorePartie(players[indiceJoueurCourant].GetComponent<Joueur>().getScorePartie()+2);


                CmdInstructionJeu(text);

                //On change le message du warning même si on peut  mettre un message plus sympa
                CmdChangerWarning("");
                // delete     // delete cartes_init();
                update_plateau();
            }
            tour = 0;

            CmdUtiliserRateMulti(nom_joueur_suivant);
            update_plateau();

            update_plateau();

            return;
        }



        //Code pour le Bang dans le code 

        if (players[0].GetComponent<Joueur>().possede("Raté!") == false)
        {
            text += "\nMais il n'a pas de Raté! Mauvais Bluff!";


            CmdInstructionJeu(text);

            Carte newCarte = new Bang(5, "Pique", 0);

            CmdActionMultiMulti(newCarte.getNombre(), newCarte.GetType().Name, newCarte.getFigure(), players[0].GetComponent<Joueur>().getPseudo(), newCarte.getSprite());

           //delete     // delete cartes_init();
            update_plateau();

            CmdVerifPartie();
        }
        else
        {
            int i = players[0].GetComponent<Joueur>().indexCarte("Raté!");
            text = players[0].GetComponent<Joueur>().getPseudo() + "vient d'utiliser un Raté!";
            players[indiceJoueurCourant].GetComponent<Joueur>().setScorePartie(players[indiceJoueurCourant].GetComponent<Joueur>().getScorePartie()+2);

            CmdInstructionJeu(text);
            //Mettre ce truc en Réseau
            carte_utilisee(i, indexJoueur2);

            //On change le message du warning même si on peut  mettre un message plus sympa
            CmdChangerWarning("");
            // delete     // delete cartes_init();
            update_plateau();
        }
        tour = 0;

        CmdActionFinie();

        update_plateau();
    }

    //Utiliser pour toute les cartes multijoueur comprenant deux intervenants 
    [Command(ignoreAuthority = true)]
    public void CmdActionMultiMulti(int num, string nom, string figure, string nom2, int sprite)
    {
        RpcFaireActionMulti(num, nom, figure, nom2,sprite);
    }

    [ClientRpc]
    public void RpcFaireActionMulti(int num, string nom, string figure, string nom2, int sprite)
    {
        Carte newCarte = (Carte)Activator.CreateInstance(Type.GetType(nom), num, figure,sprite);

        int i = 0;
        int J2;

        //Debug.Log("Bonjour vous utiliser la carte :" + nom + " pour le joueur : " + nom2);

        pioche.Add(newCarte);

        while (!Equals(players[i].GetComponent<Joueur>().getPseudo(), nom2))
        {
            i++;
        }
        J2 = i;

        //Debug.Log("Debug1");

        newCarte.action(indiceJoueurCourant, i, ref defausse, ref pioche, ref players, ref instructions_jeu, ref historique_text);

        //Debug.Log("Debug2");

            // delete cartes_init();
        update_plateau();
    }

    public void pas_rate()
    {
        rate.SetActive(false);

        if (carte_indien == true)
        {
            CmdInstructionJeu(players[0].GetComponent<Joueur>().getPseudo() + " n'utilisera pas de Bang!");

            CmdJoueurPerdVie(players[0].GetComponent<Joueur>().getPseudo());
                // delete cartes_init();
            update_plateau();
            CmdVerifPartie();

            CmdChangerWarning("");

            tour = 0;




            //On donne la main au joueur suivant
            CmdUtiliserRateMulti(players[1].GetComponent<Joueur>().getPseudo());
            update_plateau();

            return;
        }


        if (carte_gatling == true)
        {
            CmdInstructionJeu(players[0].GetComponent<Joueur>().getPseudo() + " n'utilisera pas de Raté!");



            CmdJoueurPerdVie(players[0].GetComponent<Joueur>().getPseudo());
                // delete cartes_init();
            update_plateau();
            CmdVerifPartie();

            CmdChangerWarning("");

            tour = 0;

            CmdUtiliserRateMulti(players[1].GetComponent<Joueur>().getPseudo());
            update_plateau();


            return;

        }

        CmdInstructionJeu(players[0].GetComponent<Joueur>().getPseudo() + " n'utilisera pas de Raté!");

        Carte newCarte = new Bang(5, "Pique",0);

        CmdActionMultiMulti(newCarte.getNombre(), newCarte.GetType().Name, newCarte.getFigure(), players[0].GetComponent<Joueur>().getPseudo(),newCarte.getSprite());

            // delete cartes_init();
        update_plateau();

        //Debug.Log("Tu arrives ici ?");

        CmdVerifPartie();

        //Debug.Log("Jusque là ?");
        tour = 0;
        CmdChangerWarning("");
        CmdActionFinie();
        update_plateau();
    }

    //Gros bordel au vue
    public void action_braquage()
    {
        // TO_DO IHM amélioration: affichage des cartes + trigger on_click avec carte game object
        //voler.SetActive(false);

        //Je charge les cartes du joueur 2
        // delete players[indexJoueur2].GetComponent<Joueur>().    // delete cartes_init();

        int joueur = indiceJoueurCourant;
        indiceJoueurCourant = indexJoueur2;

        //mettre un client 

        //joueur 2 on prends la carte du joueur 2 
        int index =  UnityEngine.Random.Range(0,players[indiceJoueurCourant].GetComponent<Joueur>().main.Count);
        players[joueur].GetComponent<Joueur>().main.Add(players[indiceJoueurCourant].GetComponent<Joueur>().main[index]);
        string nomCarte = players[indiceJoueurCourant].GetComponent<Joueur>().main[index].getNomCarte();

        //Debug.Log("Vous avez choisit la carte :" + nomCarte);

        //indexJoueur2 va être enlevé par le joueur
        //envoyer la carte que le joueur 2 doit enlever 

        //players[indiceJoueurCourant].GetComponent<Joueur>().main.RemoveAt(indexCarte);

        Carte carte = players[indiceJoueurCourant].GetComponent<Joueur>().main[index];

        RpcEnleverCarteJ2(carte.getNombre(), carte.GetType().Name, carte.getFigure(), players[indiceJoueurCourant].GetComponent<Joueur>().getPseudo(), carte.getSprite());

        //joueur 1

        CmdInstructionJeu(players[joueur].GetComponent<Joueur>().getPseudo() + " a volé la carte " + nomCarte + " de " + players[indiceJoueurCourant].GetComponent<Joueur>().getPseudo());

        players[indiceJoueurCourant].GetComponent<Joueur>().main.Clear(); //on vide la main du joueur 2

        instructions_jeu.text = players[joueur].GetComponent<Joueur>().getPseudo() + " a volé la carte " + nomCarte + " de " + players[indiceJoueurCourant].GetComponent<Joueur>().getPseudo();

        indexJoueur2 = indiceJoueurCourant;
        indiceJoueurCourant = joueur;
        // delete cartes_init();
        update_plateau();
        //action_en_cours = false;
        warning.text = "";
        tour = 0;
    }

    [Command(ignoreAuthority = true)]
    public void CmdChargerCarteJ2(string nom)
    {
        RpcChargerJ2(nom);
    }

    [ClientRpc]
    public void RpcChargerJ2(string nom)
    {
        //Carte du joueur 2

        if (Equals(players[0].GetComponent<Joueur>().getPseudo(), nom))
        {
            //Aléatoire nombre 
            int numCarte = players[0].GetComponent<Joueur>().main.Count;

            if (numCarte == 0)
            {
               CmdInstrucText(players[0].GetComponent<Joueur>().getPseudo() + " n'a plus de carte");                        //Le joueur n'a pas de carte dans sa main 
               return;
            }

            int i = UnityEngine.Random.Range(0, numCarte-1);

            CmdDonnerCarte(players[0].GetComponent<Joueur>().main[i].getNombre(), players[0].GetComponent<Joueur>().main[i].GetType().Name, players[0].GetComponent<Joueur>().main[i].getFigure(), players[0].GetComponent<Joueur>().main[i].getSprite());
            int index = players[0].GetComponent<Joueur>().indexCarte(players[0].GetComponent<Joueur>().main[i].getNomCarte());
            players[0].GetComponent<Joueur>().main.RemoveAt(index);
            players[0].GetComponent<Joueur>().Mise_a_jour_carte();
        }
    }

    [Command(ignoreAuthority = true)]
    public void CmdDonnerCarte(int num, string nom, string figure, int sprite)
    {
        RpcDonnerCarteJoueur2(num, nom, figure, sprite);
    }

    [ClientRpc]
    public void RpcDonnerCarteJoueur2(int num, string nom, string figure, int sprite)
    {
        Carte newCarte = (Carte)Activator.CreateInstance(Type.GetType(nom), num, figure, sprite); //On instancie la carte trouvé

        //Debug.Log("On donne la carte dans Joueur 2 pour: " + newCarte.getNomCarte());


        if (indiceJoueurCourant == 0 && est_mort == false)
        {
            //Debug.Log("Je suis le joueur indiceJoueur Courant pour ajout carte : " + newCarte.getNomCarte() + "et" + players[indexJoueur2].GetComponent<Joueur>().getPseudo());

            players[indexJoueur2].GetComponent<Joueur>().main.Add(newCarte);

           // delete cartes_nit_J2();
        }
    }

    [Command(ignoreAuthority = true)]
    public void RpcEnleverCarteJ2(int num, string nom, string figure, string nom2, int sprite)
    {
        RpcEnleverCarte(num, nom, figure, nom2, sprite);
    }

    [ClientRpc]
    public void RpcEnleverCarte(int num, string nom, string figure, string nom2, int sprite)
    {
        Carte newCarte = (Carte)Activator.CreateInstance(Type.GetType(nom), num, figure, sprite); //On instancie la carte à enlever
        int index;

        Debug.Log("Enlever une carte de joueur");

       if (Equals(players[0].GetComponent<Joueur>().getPseudo(), nom2))
       {
           int numCarte = players[0].GetComponent<Joueur>().main.Count;

            if (numCarte == 0)
            {
                CmdInstrucText(players[0].GetComponent<Joueur>().getPseudo() + " n'a plus de carte");                        //Le joueur n'a pas de carte dans sa main 
                return;
            }

            Debug.Log("il y a " + numCarte);

            CmdInstrucText(players[0].GetComponent<Joueur>().getPseudo() + "à perdu une carte");
            int i = UnityEngine.Random.Range(0, numCarte-1);

            CmdInstrucText("la valeur de i : " + i);

            if (numCarte == 1)
                i = 0;

            index = players[0].GetComponent<Joueur>().indexCarte(players[0].GetComponent<Joueur>().main[i].getNomCarte());
            newCarte = players[0].GetComponent<Joueur>().main[i];

            if (index != -1 && newCarte == null)
            {
                CmdChangerDefausse(2, newCarte.getNombre(), newCarte.getNomCarte(), newCarte.getFigure(), newCarte.getSprite(), 0);
            }

            players[0].GetComponent<Joueur>().main.RemoveAt(index);

            players[0].GetComponent<Joueur>().Mise_a_jour_carte();
        } 
    }

   /*  public void cartes_nit_J2() //delete
    {
        //Debug.Log("Bonjour voici carte 2 pour joueur : ");
       // players[indexJoueur2].GetComponent<Joueur>().delete cartes_init();
    }*/

    public void action_coup_de_foudre()
    {
        // TO_DO IHM amélioration: affichage des cartes + trigger on_click avec carte game object
        bouton_defausse.SetActive(false);
        int joueur = indiceJoueurCourant;
        indiceJoueurCourant = indexJoueur2;
        int indexCarte = UnityEngine.Random.Range(0, players[indiceJoueurCourant].GetComponent<Joueur>().main.Count);
        string nomCarte = players[indiceJoueurCourant].GetComponent<Joueur>().main[indexCarte].getNomCarte();

        //defausse en client Rpc
        //defausse.Add(players[indiceJoueurCourant].GetComponent<Joueur>().main[indexCarte]);

        //players[indiceJoueurCourant].GetComponent<Joueur>().main.RemoveAt(indexCarte);
        Carte carte = players[indiceJoueurCourant].GetComponent<Joueur>().main[indexCarte];

        CmdChangerDefausse(2, carte.getNombre(), carte.GetType().Name, carte.getFigure(), carte.getSprite(), 0);

        RpcEnleverCarteJ2(carte.getNombre(), carte.GetType().Name, carte.getFigure(), players[indiceJoueurCourant].GetComponent<Joueur>().getPseudo(),carte.getSprite());

        instructions_jeu.text = players[joueur].GetComponent<Joueur>().getPseudo() + " a détruit la carte " + nomCarte + " du joueur " + players[indiceJoueurCourant].GetComponent<Joueur>().getPseudo();
        historique_text.text = historique_text.text + "\n\n-" + instructions_jeu.text;
        indexJoueur2 = indiceJoueurCourant;
        indiceJoueurCourant = joueur;

            // delete cartes_init();
        update_plateau();
        action_en_cours = false;
        warning.text = "";
        tour = 0;
    }

    public void annuler_action()
    {
        if (tour == 1 && action_en_cours == false)
        {

            action_en_cours = false;
            tour = 0;
            CmdInstrucText("Action Annulée!");
            warning.text = "";

            players[0].GetComponent<Joueur>().Mise_a_jour_carte();            
            
        }
        else if (tour == 1 && action_en_cours == true)
        {
            warning.text = "Trop tard pour annuler cette action!";
        }
    }

    //Distance entre deux joueurs
    public int distance(int j1, int j2)
    {
        if (j1 == j2)
            return 0;

        int res1 = Math.Abs(j1 - j2); // Sens 1
        int res2 = players.Count - res1; // Sens 2

        if (players[j2].GetComponent<Joueur>().possedePlateau("Mustang"))
        {
            res1++;
            res2++;
        }

        if (Equals(players[j2].GetComponent<Joueur>().getPersonnage().getNomPersonnage(), "PAUL REGRET"))
        {
            res1++;
            res2++;
        }

        if (players[j1].GetComponent<Joueur>().possedePlateau("Lunette"))
        {
            if (res1 > 1)
                res1--;
            if (res2 > 1)
                res2--;
        }

        // Return le min
        if (res1 < res2)
            return res1;
        else return res2;
    }
    
    public void pointerEnterDrop(){
    	drop_zone.GetComponent<Image>().color = new Color32(46,36,21,81);
        defausseZone.GetComponent<Image>().color = new Color32(46,36,21,81);
    }
    
    
    public void pointerExitDrop(){
    	drop_zone.GetComponent<Image>().color = new Color32(46,36,21,0);
        defausseZone.GetComponent<Image>().color = new Color32(46,36,21,81);
    }


    [Command(ignoreAuthority = true)]
    public void CmdVerifPartie()
    {
        RpcVerifPartie();
    }

    [ClientRpc]
    public void RpcVerifPartie()
    {
        //Debug.Log("Coucou je teste la vérification de partie");
        if (verifiePartie() != 0)
        {
            finPartie();
        }
    }

    public int verifiePartie()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetComponent<Joueur>().est_mort())
            {

                players[i].GetComponent<Joueur>().devoiler_role();

                if (i == 0 && est_mort == false)
                {
                    est_mort = true;
                    for(int v = 0; v < players[0].GetComponent<Joueur>().main.Count; v++)
                    {
                        //a mettre en tom 
                       //CmdChangerDefausse(2, players[0].GetComponent<Joueur>().main[v].getNombre(), players[0].GetComponent<Joueur>().main[v].getNomCarte(), players[0].GetComponent<Joueur>().main[v].getFigure(),0);
                    }
                    players[0].GetComponent<Joueur>().main.Clear();
                    players[0].GetComponent<Joueur>().Mise_a_jour_carte();
                }

                players[i].GetComponent<Joueur>().set_color(couleur[5]);
                if (!morts.Contains(players[i].GetComponent<Joueur>()))
                {
                    instruction.text = players[i].GetComponent<Joueur>().getPseudo() + " est mort. Il était " + players[i].GetComponent<Joueur>().getRole() + ".";
                    if (players[i].GetComponent<Joueur>().getRole().Equals("Shérif"))
                    {
                        resultats_finPartie.text = "Le Shérif est mort !";
                        Debug.Log("Le sherif est mort");
                        if (players.Count > 1)
                        {
                            resultats_finPartie.text += " Les hors la loi ont gagné!";
                            Debug.Log("Les hors la loi ont gagné");
                            if (Application.platform != RuntimePlatform.Android)
                            {   
                            int partie = bdd.partie();
                            for (int j = 0; j < morts.Count; j++)
                            {
                                if (Equals(morts[j].getRole(), "HLL"))
                                {
                                        if(morts[j].getIsAndroid() == false)
                                        {
                                    morts[j].setScorePartie(morts[j].getIdbdd(), morts[j].getScorePartie() + 600);
                                    bdd.resultat(morts[j].getIdbdd(), partie, "WIN");
                                }
                                    }
                                else
                                {
                                        if(morts[j].getIsAndroid() == false)
                                        {
                                    morts[j].setScorePartie(morts[j].getIdbdd(), morts[j].getScorePartie() - 500);
                                    bdd.resultat(morts[j].getIdbdd(), partie, "LOSE");
                                }
                            }
                                }
                            for (int j = 0; j < players.Count; j++)
                            {
                                if (Equals(players[j].GetComponent<Joueur>().getRole(), "HLL"))
                                {
                                        if( players[j].GetComponent<Joueur>().getIsAndroid() == false)
                                        {
                                    players[j].GetComponent<Joueur>().setScorePartie(players[j].GetComponent<Joueur>().getIdbdd(), players[j].GetComponent<Joueur>().getScorePartie()+600);
                                    bdd.resultat(players[j].GetComponent<Joueur>().getIdbdd(), partie, "WIN");
                                }
                                    }
                                else
                                {
                                        if( players[j].GetComponent<Joueur>().getIsAndroid() == false)
                                        {
                                    players[j].GetComponent<Joueur>().setScorePartie(players[j].GetComponent<Joueur>().getIdbdd(), players[j].GetComponent<Joueur>().getScorePartie() - 500);
                                    bdd.resultat(players[j].GetComponent<Joueur>().getIdbdd(), partie, "LOSE");
                                }
                            }
                                }
                            }
                            return 2;
                        }
                        else
                        {
                            resultats_finPartie.text += "Le renégat a remporté la partié!";
                            Debug.Log("Le renégat a remporté la partié");
                            if (Application.platform != RuntimePlatform.Android)
                            {
                            int partie = bdd.partie();
                            for (int j = 0; j < morts.Count; j++)
                            {
                                if (Equals(morts[j].getRole(), "Renégat"))
                                {
                                        if(morts[j].getIsAndroid() == false)
                                        {
                                    morts[j].setScorePartie(morts[j].getIdbdd(), morts[j].getScorePartie() + 750);
                                    bdd.resultat(morts[j].getIdbdd(), partie, "WIN");
                                }
                                    }
                                else
                                {
                                        if(morts[j].getIsAndroid() == false)
                                        {
                                    morts[j].setScorePartie(morts[j].getIdbdd(), morts[j].getScorePartie() - 500);
                                    bdd.resultat(morts[j].getIdbdd(), partie, "LOSE");
                                }
                            }
                                }
                            for (int j = 0; j < players.Count; j++)
                            {
                                if (Equals(players[j].GetComponent<Joueur>().getRole(), "Renégat"))
                                {
                                        if( players[j].GetComponent<Joueur>().getIsAndroid() == false)
                                        {
                                    players[j].GetComponent<Joueur>().setScorePartie(players[j].GetComponent<Joueur>().getIdbdd(), players[j].GetComponent<Joueur>().getScorePartie() + 750);
                                    bdd.resultat(players[j].GetComponent<Joueur>().getIdbdd(), partie, "WIN");
                                }
                                    }
                                else
                                {
                                        if( players[j].GetComponent<Joueur>().getIsAndroid() == false)
                                        {
                                    players[j].GetComponent<Joueur>().setScorePartie(players[j].GetComponent<Joueur>().getIdbdd(), players[j].GetComponent<Joueur>().getScorePartie() - 500);
                                    bdd.resultat(players[j].GetComponent<Joueur>().getIdbdd(), partie, "LOSE");
                                }
                            }
                                }
                            }
                            return 3;
                        }
                    }
                    else if (players[i].GetComponent<Joueur>().getRole().Equals("Renégat"))
                    {
                        morts.Add(players[i].GetComponent<Joueur>());
                        players.Remove(players[i]);
                        nb--;
                        if (i < indiceJoueurCourant)
                            indiceJoueurCourant--;
                        int tmp = 0;
                        foreach (var joueur in players)
                        {
                            if (joueur.GetComponent<Joueur>().getRole().Equals("HLL"))
                                tmp = 1;
                        }
                        if (tmp == 0)
                        {
                            Debug.Log("le shérif et adjoint on gagné 0");

                            resultats_finPartie.text = "Le Renégat est mort! Le Shérif et ses adjoints ont remporté la partie!";
                            if(numero_joueur == 0)
                            {
                                if (Application.platform != RuntimePlatform.Android)
                                {
                                int partie = bdd.partie();
                                for (int j = 0; j < morts.Count; j++)
                                {
                                    if (Equals(morts[j].GetComponent<Joueur>().getRole(), "Shérif") || Equals(morts[j].GetComponent<Joueur>().getRole(), "Adjoint"))
                                    {
                                        if (Equals(morts[j].GetComponent<Joueur>().getRole(), "Shérif"))
                                        {
                                                if(morts[j].getIsAndroid() == false)
                                            morts[j].setScorePartie(morts[j].getIdbdd(), morts[j].getScorePartie() + 500);
                                        }
                                        if (Equals(morts[j].GetComponent<Joueur>().getRole(), "Adjoint"))
                                        {
                                                if(morts[j].getIsAndroid() == false)
                                            morts[j].setScorePartie(morts[j].getIdbdd(), morts[j].getScorePartie() + 400);
                                        }
                                            if(morts[j].getIsAndroid() == false)
                                        bdd.resultat(morts[j].getIdbdd(), partie, "WIN");
                                    }
                                    else
                                    {
                                            if(morts[j].getIsAndroid() == false)
                                            {
                                        morts[j].setScorePartie(morts[j].getIdbdd(), morts[j].getScorePartie() - 500);
                                        bdd.resultat(morts[j].getIdbdd(), partie, "LOSE");
                                    }
                                }
                                    }
                                for (int j = 0; j < players.Count; j++)
                                {
                                    if (Equals(players[j].GetComponent<Joueur>().getRole(), "Shérif") || Equals(players[j].GetComponent<Joueur>().getRole(), "Adjoint"))
                                    {
                                        if (Equals(players[j].GetComponent<Joueur>().getRole(), "Shérif"))
                                        {
                                                if( players[j].GetComponent<Joueur>().getIsAndroid() == false)
                                            players[j].GetComponent<Joueur>().setScorePartie(players[j].GetComponent<Joueur>().getIdbdd(), players[j].GetComponent<Joueur>().getScorePartie() + 500);
                                        }
                                        if (Equals(players[j].GetComponent<Joueur>().getRole(), "Adjoint"))
                                        {
                                                if( players[j].GetComponent<Joueur>().getIsAndroid() == false)
                                            players[j].GetComponent<Joueur>().setScorePartie(players[j].GetComponent<Joueur>().getIdbdd(), players[j].GetComponent<Joueur>().getScorePartie() + 400);
                                        }
                                            if( players[j].GetComponent<Joueur>().getIsAndroid() == false)
                                        bdd.resultat(players[j].GetComponent<Joueur>().getIdbdd(), partie, "WIN");
                                    }
                                    else
                                    {
                                            if( players[j].GetComponent<Joueur>().getIsAndroid() == false)
                                            {
                                        players[j].GetComponent<Joueur>().setScorePartie(players[j].GetComponent<Joueur>().getIdbdd(), players[j].GetComponent<Joueur>().getScorePartie() - 500);
                                        bdd.resultat(players[j].GetComponent<Joueur>().getIdbdd(), partie, "LOSE");
                                    }
                                }
                            }
                                }
                            }
                            return 1;
                        }
                    }
                    else if (players[i].GetComponent<Joueur>().getRole().Equals("HLL"))
                    {
                        morts.Add(players[i].GetComponent<Joueur>());
                        players.Remove(players[i]);
                        nb--;
                        if (i < indiceJoueurCourant)
                            indiceJoueurCourant--;
                        int tmp = 0;
                        foreach (var joueur in players)
                        {
                            if (joueur.GetComponent<Joueur>().getRole().Equals("Renégat"))
                                tmp = 1;
                            else if (joueur.GetComponent<Joueur>().getRole().Equals("HLL"))
                                tmp = 1;
                        }
                        if (tmp == 0)
                        {
                            resultats_finPartie.text = "Les hors-la-loi et le renégat sont morts! Le Shérif et ses adjoints ont remporté la partie!";
                            if(numero_joueur == 0)
                            {
                                if (Application.platform != RuntimePlatform.Android)
                                {
                                int partie = bdd.partie();
                                for (int j = 0; j < morts.Count; j++)
                                {
                                    if (Equals(morts[j].GetComponent<Joueur>().getRole(), "Shérif") || Equals(morts[j].GetComponent<Joueur>().getRole(), "Adjoint"))
                                    {
                                        if (Equals(morts[j].GetComponent<Joueur>().getRole(), "Shérif"))
                                        {
                                                if(morts[j].getIsAndroid() == false)
                                            morts[j].setScorePartie(morts[j].getIdbdd(), morts[j].getScorePartie() + 500);
                                        }
                                        if (Equals(morts[j].GetComponent<Joueur>().getRole(), "Adjoint"))
                                        {
                                                if(morts[j].getIsAndroid() == false)
                                            morts[j].setScorePartie(morts[j].getIdbdd(), morts[j].getScorePartie() + 400);
                                        }
                                            if(morts[j].getIsAndroid() == false)
                                        bdd.resultat(morts[j].getIdbdd(), partie, "WIN");
                                    }
                                    else
                                    {
                                            if(morts[j].getIsAndroid() == false)
                                            {
                                        morts[j].setScorePartie(morts[j].getIdbdd(), morts[j].getScorePartie() - 500);
                                        bdd.resultat(morts[j].getIdbdd(), partie, "LOSE");
                                    }
                                }
                                    }
                                for (int j = 0; j < players.Count; j++)
                                {
                                    if (Equals(players[j].GetComponent<Joueur>().getRole(), "Shérif") || Equals(players[j].GetComponent<Joueur>().getRole(), "Adjoint"))
                                    {
                                        if (Equals(players[j].GetComponent<Joueur>().getRole(), "Shérif"))
                                        {
                                                if( players[j].GetComponent<Joueur>().getIsAndroid() == false)
                                            players[j].GetComponent<Joueur>().setScorePartie(players[j].GetComponent<Joueur>().getIdbdd(), players[j].GetComponent<Joueur>().getScorePartie() + 500);
                                        }
                                        if (Equals(players[j].GetComponent<Joueur>().getRole(), "Adjoint"))
                                        {
                                                if( players[j].GetComponent<Joueur>().getIsAndroid() == false)
                                            players[j].GetComponent<Joueur>().setScorePartie(players[j].GetComponent<Joueur>().getIdbdd(), players[j].GetComponent<Joueur>().getScorePartie() + 400);
                                        }
                                            if( players[j].GetComponent<Joueur>().getIsAndroid() == false)
                                        bdd.resultat(players[j].GetComponent<Joueur>().getIdbdd(), partie, "WIN");
                                    }
                                    else
                                    {
                                            if( players[j].GetComponent<Joueur>().getIsAndroid() == false)
                                            {
                                        players[j].GetComponent<Joueur>().setScorePartie(players[j].GetComponent<Joueur>().getIdbdd(), players[j].GetComponent<Joueur>().getScorePartie() - 500);
                                        bdd.resultat(players[j].GetComponent<Joueur>().getIdbdd(), partie, "LOSE");
                                    }
                                }
                            }
                                }
                            }
                            return 1;
                        }
                    }
                    else
                    {
                        morts.Add(players[i].GetComponent<Joueur>());
                        players.Remove(players[i]);
                        nb--;
                        if (i < indiceJoueurCourant)
                            indiceJoueurCourant--;
                    }
                }
            }
        }
        return 0;
    }



    public void finPartie()
    {
        Debug.Log("Fin de la partie");
        resultats.SetActive(true);
        menu.SetActive(false);
        scene_de_jeu.SetActive(false);
    }

    // On-Click() sur le bouton "Défausser": Défausser la carte selectionnée dans le menu déroulant
    public void defausser()
    {
        Carte newCarte;

        Debug.Log("action = " + action_en_cours + " et tour : " + tour);

        if(indiceJoueurCourant != 0)
        {
            players[0].GetComponent<Joueur>().Mise_a_jour_carte();
            scene.text = "Ce n'est pas votre Tour";
            return;
        }

        else if (action_en_cours == false && tour == 0)
        {
            if (players[indiceJoueurCourant].GetComponent<Joueur>().main.Count == 0)
                instructions_jeu.text = "Vous n'avez pas de cartes!";
            else
            {
               // delete int indexCarte = players[indiceJoueurCourant].GetComponent<Joueur>().cartes.GetComponent<Dropdown>().value;

                newCarte = players[indiceJoueurCourant].GetComponent<Joueur>().main[indexCarte];


                CmdChangerDefausse(2, newCarte.getNombre(), newCarte.GetType().Name, newCarte.getFigure(), newCarte.getSprite(), 0);

                // defausse.Add(players[indiceJoueurCourant].GetComponent<Joueur>().main[indexCarte]);
                players[indiceJoueurCourant].GetComponent<Joueur>().main.RemoveAt(indexCarte);
                // delete cartes_init();

                // carte_defausse.text = newCarte.getNomCarte();

                newsDefausse.text = players[indiceJoueurCourant].GetComponent<Joueur>().getPseudo() + " a défaussé la carte " + newCarte.GetType().Name;
               // CmdMAJDefausse(carte_defausse.text);
            }
        }
        else
        {
            //En local ou pour tout le monde
            warning.text = "Terminez l'action en cours!";
            if (est_mort == false)  //Met à jour les cartes quand le joueur 
            {
                players[0].GetComponent<Joueur>().Mise_a_jour_carte();
            }
        }

    }

    [Command(ignoreAuthority = true)]
    public void CmdMAJDefausse(string lanews)
    {
        RpcMAJDefausse(lanews);
    }

    [ClientRpc]
    public void RpcMAJDefausse(string lanews)
    {
        newsDefausse.text = players[indiceJoueurCourant].GetComponent<Joueur>().getPseudo() + " a défaussé la carte " + lanews;

    }

    // Mettre la dernière carte utilisée en dernière position dans la défausse, s'effectue au sein de la fonction "action_joueur()"
    public void carte_utilisee(int indexCarte, int indexJoueur)
    {
        //defausse.Insert(defausse.Count, players[indexJoueur].GetComponent<Joueur>().main[indexCarte]);

        Carte newCarte = players[indexJoueur].GetComponent<Joueur>().main[indexCarte];

        CmdChangerDefausse(3, newCarte.getNombre(), newCarte.GetType().Name, newCarte.getFigure(), newCarte.getSprite(), defausse.Count);

        players[0].GetComponent<Joueur>().main.RemoveAt(indexCarte);
            // delete cartes_init();
    }

    // récupérer la première carte dans la défausse
    public void piocher_la_defausse()
    {
        if (action_en_cours == false && tour == 0)
        {
            if (players[indiceJoueurCourant].GetComponent<Joueur>().getPersonnage().Equals("Pedro Ramirez"))
            {
                if (defausse.Count != 0)
                {
                    players[indiceJoueurCourant].GetComponent<Joueur>().main.Add(defausse[0]);

                    CmdChangerDefausse(0, 0, "", "",0, 0);
                    // defausse.RemoveAt(0);
                        // delete cartes_init();
                    //init_carte_defausse();
                    RpcInitCarteDefausse();
                }
                else
                {
                    //En local
                    instructions_jeu.text = " La défausse est vide!";
                }
            }
            else
            {
                instructions_jeu.text = players[indiceJoueurCourant].GetComponent<Joueur>().getPseudo() + ", tu te prends pour Pedro Ramirez ?";
            }
        }
        else
        {
            Debug.Log("valeur de tour : " + tour);
            //En local
            warning.text = "Terminez l'action en cours!";
        }
    }

    // Affiche la première carte dans la défausse mettre ceci en clientRpc ou une copie en clientRPC
    [Command(ignoreAuthority = true)]
    public void CmdDefausseInit()
    {
        RpcInitCarteDefausse();
    }

    [ClientRpc]
    public void RpcInitCarteDefausse()
    {
        /*if (defausse.Count == 0)
            carte_defausse.text = "vide";
        else
            carte_defausse.text = defausse[defausse.Count - 1].getNomCarte();*/
    }

    public void init_carte_defausse()
    {
        /*if (defausse.Count == 0)
            carte_defausse.text = "vide";
        else
            carte_defausse.text = defausse[defausse.Count - 1].getNomCarte();*/
    }

    [Command(ignoreAuthority = true)]
    public void CmdDevoilerJoueur()
    {
        RpcDevoilerJoueur();
    }

    [ClientRpc]
    public void RpcDevoilerJoueur()
    {
        devoiler_role();
    }

    // On-Click() sur "devoiler role" , permet au joueur courant de dévoiler son role.
    public void devoiler_role()
    {
        if (action_en_cours == false && tour == 0)
        {
            if (players[indiceJoueurCourant].GetComponent<Joueur>().devoile == false)
            {
                // Couleur du joueur selon son rôle
                if (players[indiceJoueurCourant].GetComponent<Joueur>().getRole().Equals("Adjoint"))
                {
                    players[indiceJoueurCourant].GetComponent<Joueur>().set_color(couleur[2]);
                    
                }
                else if (players[indiceJoueurCourant].GetComponent<Joueur>().getRole().Equals("Renégat"))
                {
                    players[indiceJoueurCourant].GetComponent<Joueur>().set_color(couleur[3]);
                }
                else
                {
                    players[indiceJoueurCourant].GetComponent<Joueur>().set_color(couleur[4]);
                }
                players[indiceJoueurCourant].GetComponent<Joueur>().devoiler_role();
                instructions_jeu.text = players[indiceJoueurCourant].GetComponent<Joueur>().getPseudo() + " a dévoilé son role! Il prétend être " + players[indiceJoueurCourant].GetComponent<Joueur>().getRole() + "!";
                historique_text.text = historique_text.text + "\n\n-" + instructions_jeu.text;
                update_plateau();
            }
        }
        else
        {
            warning.text = "Terminez l'actions en cours";
        }
    }

    // On_Click() sur le bouton élargir, pour afficher le plateau du joueur
    public void afficher_plateau_joueur()
    {
        Debug.Log("je test l'accès au plateau");
        menu.SetActive(false);
        scene_de_jeu.SetActive(false);
    }

    // On-Click() sur le bouton "menu": Affiche un menu général
    public void afficher_menu()
    {
        menu.SetActive(true);
        regles.SetActive(false);
        scene_de_jeu.SetActive(false);
    }

    // On-Click() sur le bouton "historique": Affiche un menu de l'historique
    public void afficher_historique()
    {
        historique.SetActive(true);
        scene_de_jeu.SetActive(false);
        historique_menu_text.text = historique_text.text;
    }

    // Affiche un menu avec les règles du jeu (On-click() sur le bouton "règles du jeu" du menu général)
    public void afficher_regles()
    {

        menu.SetActive(false);
        scene_de_jeu.SetActive(false);
        regles.SetActive(true);

    }

    // On-Click() sur le bouton Retour, Revenir sur le plateau de jeu
    public void afficher_plateau_de_jeu()
    {
        foreach (var plateau in plateaux)
        plateau.SetActive(false);
        scene_de_jeu.SetActive(true);
        menu.SetActive(false);
        regles.SetActive(false);
        historique.SetActive(false); 
    }
    
    // On-Click() sur le bouton Activer/Désactiver, active ou désactive le son
    public void activer_desactiver_son()
    {
        if(musique == true && son.mute == false
        || musique == false && son.mute == false){
            son.mute = true;
            musique = false;
            son.volume = 0f;
            slider.value = 0f;
            name_bouton_son.text = "ACTIVER";
        }
        else{
            son.mute = false;
            musique = true;
            son.volume = 0.5f;
            slider.value = 0.5f;
            name_bouton_son.text = "DESACTIVER";
        }
    }


    // Mise à jour du plateau des joueurs. Appel après une action.
    [ClientRpc]
    public void update_plateau()
    {
        foreach (var player in players)
        {
            player.GetComponent<Joueur>().init_plateau(players[0].GetComponent<Joueur>().getPseudo());
            player.GetComponent<Joueur>().plateau_de_jeu.GetComponent<Plateau>().imageArme.sprite = cards[player.GetComponent<Joueur>().armesprite];
        }
    }

    /* delete public void cartes_init()
    {
        players[0].GetComponent<Joueur>().    // delete cartes_init();
    }*/

    [Command(ignoreAuthority = true)]
    public void CmdChangerWarning(string def)
    {
        RpcChangerWarning(def);
    }

    [ClientRpc]
    public void RpcChangerWarning(string def)
    {
        warning.text = "";
    }

    [Command(ignoreAuthority = true)]
    public void CmdInstructionJeu(string def)
    {
        RpcInstructionJeu(def);
    }

    [ClientRpc]
    public void RpcInstructionJeu(string def)
    {
        instructions_jeu.text = def;
    }


    //_____________ Martin qui test pour défausse et pioche changement de mort ____________________

    //Evènement d'ajout et enlever élément de la défausse
    [Command(ignoreAuthority = true)]
    public void CmdChangerDefausse(int d, int num, string nom, string figure, int sprite, int num_insert)
    {

        RpcChangerDefausse(d, num, nom, figure, sprite, num_insert);

    }

    [ClientRpc]
    public void RpcChangerDefausse(int d, int num, string nom, string figure, int sprite, int num_insert)
    {
        //Alors on enlève une carte dans la défauuse
        if (d == 0)
        {
            defausse.RemoveAt(0);
            return;
        }

        Carte newCarte = (Carte)Activator.CreateInstance(Type.GetType(nom), num, figure, sprite); //On instancie la carte trouvé
        //Debug.Log("Bonjour nous ajouton sune carte : " +  newCarte.getNomCarte());

        if (d == 1)
        {
            defausse.RemoveAt(num_insert);
            return;
        }

        //On ajoute la carte à la défausse
        if (d == 2)
        {
            Debug.Log("Nous ajoutons la carte" + newCarte.getNomCarte());
            defausse.Add(newCarte);
            return;
        }

        if (d == 3)
        {
            //Debug.Log("Carte ajouté à " + newCarte.getNomCarte() + " au num " + num_insert);
            //à enlever quand le num de defausse sera le bon
            num_insert = defausse.Count;
            defausse.Insert(num_insert, newCarte);
            //Debug.Log("Réussi");
        }
    }

    //On remplis la nouvelle liste pour avoir la bonne 
    [Command(ignoreAuthority = true)]
    public void CmdMAJCartePioche()
    {
        RpcAjouterPioche();
    }

    [Command(ignoreAuthority = true)]
    public void CmdMAJCarteDefausse()
    {
        RpcAjouterDefausse();
    }

    [ClientRpc]
    public void RpcAjouterDefausse() { }

    [ClientRpc]
    public void RpcAjouterPioche() { }

    //Je vide chaque liste pour les remetttre à jour sur la nouvelle bonne liste 
    [Command(ignoreAuthority = true)]
    public void CmdViderList(int numero)
    {
        RpcViderList(numero);
    }

    [ClientRpc]
    public void RpcViderList(int numero)
        {
        Debug.Log("Réintialiser la liste");

        if(numero_joueur != numero)
        {
        pioche.Clear();
        defausse.Clear();
        }
        else
        {
            List<Carte> pioche1 = new List<Carte>();
            pioche1 = pioche;
            this.pioche.Clear();

            for (int j = 0; j < pioche1.Count; j++)
            {
                ////Debug.Log("INIT NEW LIST  "+ j + "taille :" +  pioche1.Count);
                CmdInitNewList(pioche1[j].getNombre(), pioche1[j].GetType().Name, pioche1[j].getFigure(), pioche1[j].getSprite());
            }

        }
        
    }


    // _____________________IMPLEMENTATION NILS PERSONNAGES____________________________

    public void Black_Jack(int i)
    {
        int last_card = (players[i].GetComponent<Joueur>().main.Count) - 1;

        if (Equals(players[i].GetComponent<Joueur>().main[last_card].getFigure(), "Carreau")
           || Equals(players[i].GetComponent<Joueur>().main[last_card].getFigure(), "Coeur"))
        {
            players[i].GetComponent<Joueur>().piocher(ref pioche, ref defausse);
            CmdInstructionJeu("A pioché une carte supplémentaire!");
        }
    }

    //Vérifie si le joueur i à deux Bang ou plus dans sa main.
    //--> Pour perso : SLAB LE FLINGUEUR
    public bool verifDoubleRate(int i)
    {
        int nb_carte = players[i].GetComponent<Joueur>().main.Count;
        int result = 0;
        for (int j = 0; j < nb_carte; ++j)
        {
            if (Equals(players[i].GetComponent<Joueur>().main[j].getNomCarte(), "Raté!"))
                result++;

        }

        return result >= 2 ? true : false;
    }

    //Pré : le joueur i a le personnage SAM LE VAUTOUR
    //      && le joueur j vient de tomber à 0 pdv
    //Le joueur i prend l'ancienne main du joueur j ainsi que ses cartes en jeu.
    //--> Perso : SAM LE VAUTOUR
    public void Sam_Le_Vautour(int i, int j)
    {
        players[i].GetComponent<Joueur>().main = players[j].GetComponent<Joueur>().main;
        players[i].GetComponent<Joueur>().plateau = players[j].GetComponent<Joueur>().plateau;
    }


    // _____________________IMPLEMENTATION MARTIN CARTES____________________________

    [Command(ignoreAuthority = true)]
    public void CmdCarteChange(int values)
    {
        RpcCarteChange(values);
    }

    [ClientRpc]
    public void RpcCarteChange(int values)
    {

        Debug.Log("Nous testons gatling et indiens");

        //0 représente la carte gatling
        if (values == 0)
        {
            if (carte_gatling == false)
            {
                carte_gatling = true;
            }
            else
            {
                scene.text = "Fin de l'attaque";
                carte_gatling = false;
            }
            return;
        }

        //1 représente la cartes indiens
        if (values == 1){
            if (carte_indien == false){  
                carte_indien = true;
            }
            else
            {
                scene.text = "Fin de l'attaque";
                carte_indien = false;
            }
        }
    }

    [Command(ignoreAuthority = true)]
    public void CmdJoueurPerdVie(string nom)
    {
        RpcJoueurPerdVie(nom);
    }

    [ClientRpc]
    public void RpcJoueurPerdVie(string nom)
    {
        int i = 0;
        while (!Equals(players[i].GetComponent<Joueur>().getPseudo(), nom))
        {
            i++;
        }
        players[i].GetComponent<Joueur>().setVie(players[i].GetComponent<Joueur>().getVie() - 1);

        update_plateau();
        //On met directement la vie du joueur en question 
    }


    //__________________MISE A JOUR TEXTE_____________________//

    [Command(ignoreAuthority = true)]
    public void CmdSceneText(string text)
    {
        RpcSceneText(text);
    }

    [ClientRpc]
    public void RpcSceneText(string text)
    {
        scene.text = text;
    }


    [Command(ignoreAuthority = true)]
    public void CmdChatText(string text, string pseudo)
    {
        RpcChatText(text, pseudo);
    }

    [ClientRpc]
    public void RpcChatText(string text, string pseudo)
    {
        chat_text.text += "\n" + pseudo + ": " + text + "\n";
        chat_input.text = "";
    }

    public void onSendChat(string pseudo){
        CmdChatText(chat_input.text,pseudo);
    }


    [Command(ignoreAuthority = true)]
    public void CmdInstrucText(string text)
    {
        RpcInstrucText(text);
    }


    [ClientRpc]
    public void RpcInstrucText(string text)
    {
        instruction.text = text;
    }



    [Command(ignoreAuthority = true)]

    public void CmdHistoriqueText(string text)
    {
        RpcHistoriqueText(text);

    }


    [ClientRpc]
    public void RpcHistoriqueText(string text)
    {
        
    }





    //___________________________________ JOUEUR QUITTE UNE PARTIE____________________________________//

    [Command(ignoreAuthority = true)]

    public void CmdJoueurQuittePartie(string nom)
    {
        RpcJoueurQuittePartie(nom); 
    }


    [ClientRpc]
    public void RpcJoueurQuittePartie(string nom)
    {
        int i = 0;
        while(!players[i].GetComponent<Joueur>().getPseudo().Equals(nom)) {
            i++;
        }

        players[i].GetComponent<Joueur>().setVie(0);

        Debug.Log("Quelqu'un est mort");


        if (verifiePartie() != 0)
        {
            Debug.Log("fin de partie ");
            finPartie();
        }
    }



    //_________________________POUR REJOUER UNE PARTIE_______________________//

    [Command(ignoreAuthority = true)]
    public void CmdRejouer_Partie()
    {
        RpcRejouer_Partie(this.numero_joueur);
    }


    [ClientRpc]
    public void RpcRejouer_Partie(int n)
    {
        if (deja_appuyer_rejouer == false )
        {
            Debug.Log("Rejouer une Partie");
            pioche.Clear();
            defausse.Clear();
            players[0].GetComponent<Joueur>().main.Clear();
            players[0].GetComponent<Joueur>().Mise_a_jour_carte();

            foreach(GameObject Joueur in players)
            {
                Destroy(Joueur);
            }

            foreach(Joueur Joueur in morts)
            {
                Destroy(Joueur);
            }


            players.Clear();
            morts.Clear();
            plateaux.Clear();
            est_mort = false;
            tour_pioche_init = 0;
            main_deja_init = false;
            on_veut_init = false;
            doit_piocher = false;
            //nb_joueurs_set = false;
            carte_indien = false;
            carte_gatling = false;
            deja_appuyer_rejouer = true;
        }

        if(n == numero_joueur)
        {
            nb_rejouer = nb_rejouer + 1;
        }

        Debug.Log("On refait une partie");
    }

    [Command(ignoreAuthority = true)]
    public void CmdNewPartie()
    {
        RpcNewPartie();
    }

    [ClientRpc]
    public void RpcNewPartie()
    {
        Debug.Log("Ca marche pas ? ");
        finPartie(); 
    }
	public int getTour()
	{
		return this.tour;
	}

}
