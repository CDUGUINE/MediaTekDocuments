﻿using MediaTekDocuments.dal;
using MediaTekDocuments.model;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MediaTekDocuments.controller
{
    /// <summary>
    /// Classe contrôleur
    /// </summary>
    internal class NamespaceDoc
    {

    }

    /// <summary>
    /// Contrôleur lié à FrmMediatek
    /// </summary>
    class FrmMediatekController
    {
        /// <summary>
        /// Objet d'accès aux données
        /// </summary>
        private readonly Access access;

        /// <summary>
        /// Récupération de l'instance unique d'accès aux données
        /// </summary>
        public FrmMediatekController()
        {
            access = Access.GetInstance();
        }

        /// <summary>
        /// getter sur la liste des genres
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            return access.GetAllGenres();
        }

        /// <summary>
        /// getter sur la liste des livres
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            return access.GetAllLivres();
        }

        /// <summary>
        /// getter sur la liste des Dvd
        /// </summary>
        /// <returns>Liste d'objets dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            return access.GetAllDvd();
        }

        /// <summary>
        /// getter sur la liste des revues
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            return access.GetAllRevues();
        }

        /// <summary>
        /// getter sur les rayons
        /// </summary>
        /// <returns>Liste d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            return access.GetAllRayons();
        }

        /// <summary>
        /// getter sur les publics
        /// </summary>
        /// <returns>Liste d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            return access.GetAllPublics();
        }

        /// <summary>
        /// getter sur les commandes document
        /// </summary>
        /// <returns>Liste d'objets CommandeDocument</returns>
        public List<CommandeDocument> GetAllCommandeDocument()
        {
            return access.GetAllCommandeDocument();
        }

        /// <summary>
        /// getter sur les suivis
        /// </summary>
        /// <returns>Liste d'objets Suivi</returns>
        public List<Categorie> GetAllSuivis()
        {
            return access.GetAllSuivis();
        }

        /// <summary>
        /// setter sur un suivi
        /// </summary>
        public bool SetOneSuivi(CommandeDocument commandeDocument)
        {
            return access.SetOneSuivi(commandeDocument);
        }

        /// <summary>
        /// récupère les exemplaires d'une revue
        /// </summary>
        /// <param name="idDocument">id de la revue concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesRevue(string idDocument)
        {
            return access.GetExemplairesRevue(idDocument);
        }

        /// <summary>
        /// Crée un exemplaire d'une revue dans la bdd
        /// </summary>
        /// <param name="exemplaire">L'objet Exemplaire concerné</param>
        /// <returns>True si la création a pu se faire</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            return access.CreerExemplaire(exemplaire);
        }

        /// <summary>
        /// getter sur les abonnements
        /// </summary>
        /// <returns>Liste d'objets CommandeDocument</returns>
        public List<Abonnement> GetAllAbonnements()
        {
            return access.GetAllAbonnements();
        }

        /// <summary>
        /// getter sur les utilisateurs
        /// </summary>
        /// <returns>Liste d'objets Utilisateurs</returns>
        public List<Utilisateur> GetAllUtilisateurs()
        {
            return access.GetAllUtilisateurs();
        }

        /// <summary>
        /// Crée un commandeDocument
        /// </summary>
        /// <param name="commandeDocument">Le commandeDocument concerné</param>
        /// <returns>true si un commandeDocument a été créé, false sinon</returns>
        public bool CreerCommandeDocument(CommandeDocument commandeDocument)
        {
            return access.CreerCommandeDocument(commandeDocument);
        }

        /// <summary>
        /// Supprime un commandeDocument
        /// </summary>
        /// <param name="commandeDocument">Le commandeDocument concerné</param>
        public void SupprCommandeDocument(CommandeDocument commandeDocument)
        {
            access.SupprCommandeDocument(commandeDocument);
        }

        /// <summary>
        /// Crée un abonnement
        /// </summary>
        /// <param name="abonnement">L'abonnement concerné</param>
        /// <returns>true si un abonnement a été créé, false sinon</returns>
        public bool CreerAbonnement(Abonnement abonnement)
        {
            return access.CreerAbonnement(abonnement);
        }

        /// <summary>
        /// Supprime un abonnement
        /// </summary>
        /// <param name="abonnement">L'abonnement concerné</param>
        /// <returns>true si un abonnement a été supprimé, false sinon</returns>
        public bool SupprAbonnement(Abonnement abonnement)
        {
            return access.SupprAbonnement(abonnement);
        }

        /// <summary>
        /// Après une authentification réussie, lance la fenêtre principale de l'application
        /// </summary>
        /// <param name="fenetreActuelle">Fenêtre à fermer</param>
        /// <param name="nouvelleFenetre">Fenêtre à ouvrir</param>
        public void LancerApplication(Form fenetreActuelle, Form nouvelleFenetre)
        {
            fenetreActuelle.Hide();
            nouvelleFenetre.ShowDialog();
            fenetreActuelle.Close();
            nouvelleFenetre.FormClosed += (s, args) =>
            {
                if (!fenetreActuelle.IsDisposed)
                    fenetreActuelle.Close();
            };
        }

    }
}
