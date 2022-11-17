using UnityEngine;
using Mono.Data.Sqlite;
using System;
using System.IO;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

public class bdd : MonoBehaviour
{

    /* public bdd()
     {
         Initialize();
     }
    */

    public static int inscription(string nu, string mdp)
    {
        //Création/ouverture de la db
        MySqlConnection dbcon = new MySqlConnection("SERVER= 35.180.152.210; DATABASE=mydatabse; UID=user2; PASSWORD=user2;");
        dbcon.Open();


        Regex rgx_nu = new Regex(@"^([a-zA-Z0-9]{2,20})$");
        if (!rgx_nu.IsMatch(nu))
        {
            //Debug.Log("nom d'utilisateur entre 2 et 20 caractères");
            return -2;
        }


        Regex rgx_mdp = new Regex(@"^(?=.*\d)(?=.*[a-zA-Z]).{6,20}$");
        if (!rgx_mdp.IsMatch(mdp))
        {
            //Debug.Log("mot de passe 6 caractères minimum dont 1 chiffre minimum, 20 caractères max");
            return -3;
        }

        MySqlCommand dbcmd = new MySqlCommand("INSERT INTO UTILISATEUR (nomUtilisateur, motDePasse) VALUES ('" + nu + "','" + ComputeSha256Hash(mdp) + "')", dbcon);



        try
        {
            dbcmd.ExecuteNonQuery();
            dbcon.Close();
            //Debug.Log("Vous êtes inscrit");
            return 0;
        }
        catch (MySqlException)
        {
            dbcon.Close();
            //Debug.Log("Il existe déjà cet utilisateur");
            return -1;
        }

    }



    // Retourne -1 si probleme de connexion, sinon retourne l'id de l'utilisateur
    public static int connexion(string nu, string mdp)
    {
        //Création/ouverture de la db
        MySqlConnection dbcon = new MySqlConnection("SERVER= 35.180.152.210; DATABASE=mydatabse; UID=user2; PASSWORD=user2;");
        dbcon.Open();


        // verification d'un mot de passe
        MySqlCommand dbcmd = new MySqlCommand("SELECT idUtilisateur, motDePasse FROM UTILISATEUR where nomUtilisateur = '" + nu + "'", dbcon);
        MySqlDataReader reader;
        reader = dbcmd.ExecuteReader();

        reader.Read();

        if (reader[1].ToString() == ComputeSha256Hash(mdp))
        {
            int ret = Convert.ToInt32(reader[0].ToString());
            dbcon.Close();
            return ret;
        }
        else
        {
            dbcon.Close();
            return -1;
        }
    }

    // Créé une partie et retourne l'id de la partie (-1 en cas d'erreur)
    public static int partie()
    {
        //Création/ouverture de la db
        MySqlConnection dbcon = new MySqlConnection("SERVER= 35.180.152.210; DATABASE=mydatabse; UID=user2; PASSWORD=user2;");
        dbcon.Open();


        MySqlCommand dbcmd = new MySqlCommand("INSERT INTO PARTIE (date) VALUES ('" + DateTimeOffset.Now.ToUnixTimeSeconds() + "')", dbcon);

        string cmdtext2 = "SELECT last_insert_id()";
        try
        {
            dbcmd.ExecuteNonQuery();
        }
        catch (MySqlException)
        {
            dbcon.Close();
            return -1;
        }

        dbcmd.CommandText = cmdtext2;
        int ret = Convert.ToInt32(dbcmd.ExecuteScalar());
        dbcon.Close();
        return ret;
    }

    // insère un resultat, renvoie -1 si erreur et renvoie 0 si tout se passe bien
    public static int resultat(int idUtilisateur, int idPartie, string resultat)
    {
        //Création/ouverture de la db
        MySqlConnection dbcon = new MySqlConnection("SERVER= 35.180.152.210; DATABASE=mydatabse; UID=user2; PASSWORD=user2;");
        dbcon.Open();

        // insertion d'un resultat

        MySqlCommand dbcmd = new MySqlCommand("INSERT INTO RESULTAT (idUtilisateur, idPartie, resultat) VALUES ('" + idUtilisateur + "' ,'" + idPartie + "', '" + resultat + "')", dbcon);

        try
        {
            dbcmd.ExecuteNonQuery();
        }
        catch (MySqlException)
        {
            //Debug.Log("resultat erreur : " + MyException.Message);
            dbcon.Close();
            return -1;
        }

        return 0;
    }

    // retourne le nombre de victoire d'un utilisateur
    public static int score(int idUtilisateur)
    {
        //Création/ouverture de la db
        MySqlConnection dbcon = new MySqlConnection("SERVER= 35.180.152.210; DATABASE=mydatabse; UID=user2; PASSWORD=user2;");
        dbcon.Open();


        MySqlCommand dbcmd = new MySqlCommand("SELECT count(*) FROM RESULTAT where idUtilisateur = '" + idUtilisateur + "' AND resultat = 'WIN'", dbcon);
        MySqlDataReader reader;
        reader = dbcmd.ExecuteReader();
        reader.Read();

        int ret = Convert.ToInt32(reader[0].ToString());

        dbcon.Close();
        return ret;
    }

    // retourne le nombre de parties en tout
    public static int nombre_parties()
    {
        //Création/ouverture de la db
        MySqlConnection dbcon = new MySqlConnection("SERVER= 35.180.152.210; DATABASE=mydatabse; UID=user2; PASSWORD=user2;");
        dbcon.Open(); ;


        MySqlCommand dbcmd = new MySqlCommand("SELECT count(*) FROM RESULTAT", dbcon);
        MySqlDataReader reader;
        reader = dbcmd.ExecuteReader();

        reader.Read();
        int ret = Convert.ToInt32(reader[0].ToString());

        dbcon.Close();
        return ret;
    }

    // retourne le nombre de parties d'un joueur
    public static int nombre_parties(int idUtilisateur)
    {
        //Création/ouverture de la db
        MySqlConnection dbcon = new MySqlConnection("SERVER= 35.180.152.210; DATABASE=mydatabse; UID=user2; PASSWORD=user2;");
        dbcon.Open();


        MySqlCommand dbcmd = new MySqlCommand("SELECT count(*) FROM RESULTAT where idUtilisateur = '" + idUtilisateur + "'", dbcon);
        MySqlDataReader reader;
        reader = dbcmd.ExecuteReader();

        reader.Read();
        int ret = Convert.ToInt32(reader[0].ToString());

        dbcon.Close();
        return ret;
    }

    // retourne l'historique d'un utilisateur dans une liste de string, DateTimeOffset (WIN ou LOSE) + Date de la partie
    public static List<Tuple<string, DateTimeOffset>> historique(int idUtilisateur)
    {
        //Création/ouverture de la db
        MySqlConnection dbcon = new MySqlConnection("SERVER= 35.180.152.210; DATABASE=mydatabse; UID=user2; PASSWORD=user2;");
        dbcon.Open();

        List<Tuple<string, DateTimeOffset>> ret = new List<Tuple<string, DateTimeOffset>>();


        MySqlCommand dbcmd = new MySqlCommand("SELECT resultat, date from PARTIE natural join RESULTAT where idUtilisateur = '" + idUtilisateur + "'", dbcon);
        MySqlDataReader reader;
        reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            ret.Add(new Tuple<string, DateTimeOffset>(reader[0].ToString(), DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt32(reader[1].ToString()))));
        }

        dbcon.Close();
        return ret;
    }

    // Créé les trois tables dans la base de données si elles ne sont pas déjà crées
    public static void creation_tables()
    {
        //Création/ouverture de la db
        MySqlConnection dbcon = new MySqlConnection("SERVER= 35.180.152.210; DATABASE=mydatabse; UID=user2; PASSWORD=user2;");
        dbcon.Open();

        //Création de la table UTILISATEUR
        //Création de la table UTILISATEUR
        MySqlCommand dbcmd = new MySqlCommand("CREATE TABLE IF NOT EXISTS UTILISATEUR (idUtilisateur INTEGER PRIMARY KEY AUTO_INCREMENT, nomUtilisateur VARCHAR(256) UNIQUE NOT NULL, motDePasse VARCHAR(256) NOT NULL, elo INTEGER DEFAULT 0 )", dbcon);
        dbcmd.ExecuteNonQuery();

        //Création de la table PARTIE

        //, CHECK (date <= strftime('%s','now'))
        dbcmd = new MySqlCommand("CREATE TABLE IF NOT EXISTS PARTIE (idPartie INTEGER PRIMARY KEY AUTO_INCREMENT, date INTEGER )", dbcon);
        dbcmd.ExecuteNonQuery();

        /*
                dbcmd = dbcon.CreateCommand();
                dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS PARTIE (idPartie INTEGER PRIMARY KEY, date INTEGER, CHECK (date <= strftime('%s','now')) )";
                dbcmd.ExecuteReader();
        */

        //CHECK(resultat IN('WIN', 'LOSE'))
        //Création de la table RESULTAT
        dbcmd = new MySqlCommand("CREATE TABLE IF NOT EXISTS RESULTAT(idUtilisateur INTEGER, idPartie INTEGER, resultat VARCHAR(256) , PRIMARY KEY (idUtilisateur, idPartie), FOREIGN KEY(idUtilisateur) REFERENCES UTILISATEUR(idUtilisateur), FOREIGN KEY(idPartie) REFERENCES PARTIE(idPartie))", dbcon);
        dbcmd.ExecuteNonQuery();

        dbcon.Close();
    }

    // Supprime la table utilisateur de la base de données
    public static void suppression_table_utilisateur()
    {
        //Création/ouverture de la db
        MySqlConnection dbcon = new MySqlConnection("SERVER= 35.180.152.210; DATABASE=mydatabse; UID=user2; PASSWORD=user2;");
        dbcon.Open();


        //Suppression de la table UTILISATEUR
        MySqlCommand dbcmd_u = new MySqlCommand("DROP TABLE IF EXISTS UTILISATEUR", dbcon);
        dbcmd_u.ExecuteNonQuery();

        dbcon.Close();
    }

    // Supprime la table partie de la base de données
    public static void suppression_table_partie()
    {
        //Création/ouverture de la db
        MySqlConnection dbcon = new MySqlConnection("SERVER= 35.180.152.210; DATABASE=mydatabse; UID=user2; PASSWORD=user2;");
        dbcon.Open();

        //Suppression de la table PARTIE
        MySqlCommand dbcmd_p = new MySqlCommand("DROP TABLE IF EXISTS PARTIE", dbcon);
        dbcmd_p.ExecuteNonQuery();

        dbcon.Close();
    }

    // Supprime la table resultat de la base de données
    public static void suppression_table_resultat()
    {
        //Création/ouverture de la db
        MySqlConnection dbcon = new MySqlConnection("SERVER= 35.180.152.210; DATABASE=mydatabse; UID=user2; PASSWORD=user2;");
        dbcon.Open();

        //Suppression de la table RESULTAT
        MySqlCommand dbcmd_s = new MySqlCommand("DROP TABLE IF EXISTS RESULTAT", dbcon);
        dbcmd_s.ExecuteNonQuery();


        dbcon.Close();
    }

    // Retourne la chaine passé en argument crypté
    static string ComputeSha256Hash(string rawData)
    {
        // Create a SHA256   
        using (SHA256 sha256Hash = SHA256.Create())
        {
            // ComputeHash - returns byte array  
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            // Convert byte array to a string   
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

    public static void elo(int idUtilisateur, int ajout_elo)
    {
        //Création/ouverture de la db
        MySqlConnection dbcon = new MySqlConnection("SERVER= 35.180.152.210; DATABASE=mydatabse; UID=user2; PASSWORD=user2;");
        dbcon.Open();
        if (ajout_elo < 0)
            ajout_elo = 0;


        MySqlCommand dbcmd_s = new MySqlCommand("UPDATE UTILISATEUR set elo = " + ajout_elo + " WHERE idUtilisateur = '" + idUtilisateur + "'", dbcon);
        dbcmd_s.ExecuteNonQuery();

        dbcon.Close();
    }

    // Renvoie l'elo d'un joueur
    public static int elo(int idUtilisateur)
    {
        //Création/ouverture de la db
        MySqlConnection dbcon = new MySqlConnection("SERVER= 35.180.152.210; DATABASE=mydatabse; UID=user2; PASSWORD=user2;");
        dbcon.Open();

        MySqlCommand dbcmd = new MySqlCommand("SELECT elo FROM UTILISATEUR where idUtilisateur = '" + idUtilisateur + "'", dbcon);
        MySqlDataReader reader;
        reader = dbcmd.ExecuteReader();


        reader.Read();
        int elo = Convert.ToInt32(reader[0].ToString());

        dbcon.Close();

        return elo;
    }


}

