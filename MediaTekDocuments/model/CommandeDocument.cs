using System;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier CommandeDocument hérite de Commande
    /// </summary>
    public class CommandeDocument : Commande
    {
        public int NbExemplaire { get; set; }
        public int IdSuivi { get; set; }
        public string Libelle { get; set; }
        public string IdLivreDVD { get; set; }

        public CommandeDocument(string id, DateTime dateCommande, decimal montant, int idSuivi, string libelle, string idLivreDVD, int nbExemplaire)
            : base(id, dateCommande, montant)
        {
            this.NbExemplaire = nbExemplaire;
            this.IdSuivi = idSuivi;
            this.Libelle = libelle;
            this.IdLivreDVD = idLivreDVD;
        }

    }
}
