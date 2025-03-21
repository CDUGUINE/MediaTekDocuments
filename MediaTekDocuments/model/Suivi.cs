namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier Suivi (étape de la commande) hérite de Categorie
    /// </summary>
    public class Suivi : Categorie
    {
        public Suivi(string id, string libelle) : base(id, libelle)
        {
        }

    }
}
