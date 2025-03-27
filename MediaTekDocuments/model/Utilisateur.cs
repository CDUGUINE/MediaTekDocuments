/// <summary>
/// Classe métier Utilisateur
/// </summary>
namespace MediaTekDocuments.model
{
    public class Utilisateur
    {
        public string Login { get; set; }
        public string Pwd { get; set; }
        public int IdService { get; set; }

        public Utilisateur(string login, string pwd, int idService)
        {
            this.Login = login;
            this.Pwd = pwd;
            this.IdService = idService;
        }
    }
}
