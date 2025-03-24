using System;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier Abonnement hérite de Commande
    /// </summary>
    public class Abonnement : Commande
    {
        public DateTime DateFinAbonnement { get; set; }
        public String IdRevue { get; set; }

        public Abonnement(string id, DateTime dateCommande, decimal montant, DateTime dateFinAbonnement, string idRevue)
            : base(id, dateCommande, montant)
        {
            this.DateFinAbonnement = dateFinAbonnement;
            this.IdRevue = idRevue;
        }

        /// <summary>
        /// Vérifie si la date de parution est comprise entre la date de commande et la date de fin d'abonnement
        /// </summary>
        /// <param name="dateCommande"></param>
        /// <param name="dateFinAbonnement"></param>
        /// <param name="dateParution"></param>
        /// <returns></returns>
        public bool ParutionDansAbonnement(DateTime dateCommande, DateTime dateFinAbonnement, DateTime dateParution)
        {
            return (dateParution > dateCommande && dateParution < dateFinAbonnement);
        }

        /// <summary>
        /// Vérifie si l'abonnement expire bientôt
        /// </summary>
        /// <param name="dateFinAbonnement"></param>
        /// <returns></returns>
        public bool AbonnementFinImminente(DateTime dateFinAbonnement)
        {
            DateTime limite = DateTime.Now.AddDays(30);
            return (limite < dateFinAbonnement);
        }
    }
}

