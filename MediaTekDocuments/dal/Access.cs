using MediaTekDocuments.manager;
using MediaTekDocuments.model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Serilog;
using System.Configuration;

namespace MediaTekDocuments.dal
{
    /// <summary>
    /// Classe d'accès aux données
    /// </summary>
    internal class NamespaceDoc
    {

    }

    /// <summary>
    /// Classe d'accès aux données
    /// </summary>
    public class Access
    {
        /// <summary>
        /// adresse de l'API
        /// </summary>
        private static readonly string uriApi = "http://otsomediatekdocuments.com/rest_mediatekdocuments/";
        /// <summary>
        /// instance unique de la classe
        /// </summary>
        private static Access instance = null;
        /// <summary>
        /// instance de ApiRest pour envoyer des demandes vers l'api et recevoir la réponse
        /// </summary>
        private readonly ApiRest api = null;
        /// <summary>
        /// méthode HTTP pour select
        /// </summary>
        private const string GET = "GET";
        /// <summary>
        /// méthode HTTP pour insert
        /// </summary>
        private const string POST = "POST";
        /// <summary>
        /// méthode HTTP pour update
        /// </summary>
        private const string UPDATE = "PUT";
        /// <summary>
        /// Méthode HTTP pour delete
        /// </summary>
        private const string SUPR = "DELETE";

        /// <summary>
        /// Méthode privée pour créer un singleton
        /// initialise l'accès à l'API
        /// </summary>
        private Access()
        {
            String authValue = ConfigurationManager.AppSettings["authenticationName"];
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                api = ApiRest.GetInstance(uriApi, authValue);
            }
            catch (Exception e)
            {
                Log.Fatal(e, "erreur d'authentification auprès de l'API");
                Console.WriteLine(e.Message);
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Création et retour de l'instance unique de la classe
        /// </summary>
        /// <returns>instance unique de la classe</returns>
        public static Access GetInstance()
        {
            if (instance == null)
            {
                instance = new Access();
            }
            return instance;
        }

        /// <summary>
        /// Retourne tous les genres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            IEnumerable<Genre> lesGenres = TraitementRecup<Genre>(GET, "genre", null);
            return new List<Categorie>(lesGenres);
        }

        /// <summary>
        /// Retourne tous les rayons à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            IEnumerable<Rayon> lesRayons = TraitementRecup<Rayon>(GET, "rayon", null);
            return new List<Categorie>(lesRayons);
        }

        /// <summary>
        /// Retourne toutes les catégories de public à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            IEnumerable<Public> lesPublics = TraitementRecup<Public>(GET, "public", null);
            return new List<Categorie>(lesPublics);
        }

        /// <summary>
        /// Retourne toutes les livres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            List<Livre> lesLivres = TraitementRecup<Livre>(GET, "livre", null);
            return lesLivres;
        }

        /// <summary>
        /// Retourne toutes les dvd à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            List<Dvd> lesDvd = TraitementRecup<Dvd>(GET, "dvd", null);
            return lesDvd;
        }

        /// <summary>
        /// Retourne toutes les revues à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            List<Revue> lesRevues = TraitementRecup<Revue>(GET, "revue", null);
            return lesRevues;
        }

        /// <summary>
        /// Retourne les exemplaires d'une revue
        /// </summary>
        /// <param name="idDocument">id de la revue concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesRevue(string idDocument)
        {
            String jsonIdDocument = convertToJson("id", idDocument);
            List<Exemplaire> lesExemplaires = TraitementRecup<Exemplaire>(GET, "exemplaire/" + jsonIdDocument, null);
            return lesExemplaires;
        }

        /// <summary>
        /// Retourne toutes les commandes de documents à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets CommandeDocument</returns>
        public List<CommandeDocument> GetAllCommandeDocument()
        {
            List<CommandeDocument> lesCommandesLivres = TraitementRecup<CommandeDocument>(GET, "commandedocument", null);
            return lesCommandesLivres;
        }

        /// <summary>
        /// Retourne tous les abonnements à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Abonnement</returns>
        public List<Abonnement> GetAllAbonnements()
        {
            List<Abonnement> lesAbonnements = TraitementRecup<Abonnement>(GET, "abonnement", null);
            return lesAbonnements;
        }

        /// <summary>
        /// Retourne tous les suivis à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Suivi</returns>
        public List<Categorie> GetAllSuivis()
        {
            List<Categorie> lesSuivis = TraitementRecup<Categorie>(GET, "suivi", null);
            return lesSuivis;
        }

        /// <summary>
        /// Retourne tous les utilisateurs à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Utilisateurs</returns>
        public List<Utilisateur> GetAllUtilisateurs()
        {
            List<Utilisateur> lesUtilisateurs = TraitementRecup<Utilisateur>(GET, "utilisateur", null);
            return lesUtilisateurs;
        }

        /// <summary>
        /// Modifie un suivi dans la BDD
        /// </summary>
        /// <param name="commandeDocument"></param>
        /// <returns></returns>
        public bool SetOneSuivi(CommandeDocument commandeDocument)
        {
            String jsonCommandeDocument = convertToJson("idsuivi", commandeDocument.IdSuivi);
            try
            {
                List<CommandeDocument> liste = TraitementRecup<CommandeDocument>(UPDATE, "commandedocument/" + commandeDocument.Id, "champs=" + jsonCommandeDocument);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Log.Information(ex, "le changement de suivi n'a pas pu être effectué");
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Crée une nouvelle commande dans la BDD
        /// </summary>
        /// <param name="commandeDocument"></param>
        public bool CreerCommandeDocument(CommandeDocument commandeDocument)
        {
            String jsonCommandeDocument = JsonConvert.SerializeObject(commandeDocument, new CustomDateTimeConverter());
            try
            {
                List<CommandeDocument> liste = TraitementRecup<CommandeDocument>(POST, "commandedocument", "champs=" + jsonCommandeDocument);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Log.Information(ex, "La nouvelle commande n'a pas pu être créée");
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Supprime une commande dans la BDD
        /// </summary>
        /// <param name="commandeDocument"></param>
        public void SupprCommandeDocument(CommandeDocument commandeDocument)
        {
            String jsonCommandeDocument = convertToJson("id", commandeDocument.Id);
            TraitementRecup<Object>(SUPR, "commandedocument/" + jsonCommandeDocument, null);
        }

        /// <summary>
        /// ecriture d'un exemplaire en base de données
        /// </summary>
        /// <param name="exemplaire">exemplaire à insérer</param>
        /// <returns>true si l'insertion a pu se faire (retour != null)</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            String jsonExemplaire = JsonConvert.SerializeObject(exemplaire, new CustomDateTimeConverter());
            try
            {
                List<Exemplaire> liste = TraitementRecup<Exemplaire>(POST, "exemplaire", "champs=" + jsonExemplaire);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Log.Information(ex, "Le nouvel exemplaire n'a pas pu être créé");
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Crée un nouvel abonnement dans la BDD
        /// </summary>
        /// <param name="abonnement"></param>
        public bool CreerAbonnement(Abonnement abonnement)
        {
            String jsonAbonnement = JsonConvert.SerializeObject(abonnement, new CustomDateTimeConverter());
            try
            {
                List<Abonnement> liste = TraitementRecup<Abonnement>(POST, "abonnement", "champs=" + jsonAbonnement);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Log.Information(ex, "Le nouvel abonnement n'a pas pu être créé");
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Supprime un abonnement dans la BDD
        /// </summary>
        /// <param name="abonnement"></param>
        /// <returns></returns>
        public bool SupprAbonnement(Abonnement abonnement)
        {
            try
            {
                String jsonAbonnement = convertToJson("id", abonnement.Id);
                TraitementRecup<Object>(SUPR, "abonnement/" + jsonAbonnement, null);
            }
            catch (Exception ex)
            {
                Log.Information(ex, "L'abonnement n'a pas pu être supprimé");
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Traitement de la récupération du retour de l'api, avec conversion du json en liste pour les select (GET)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methode">verbe HTTP (GET, POST, PUT, DELETE)</param>
        /// <param name="message">information envoyée dans l'url</param>
        /// <param name="parametres">paramètres à envoyer dans le body, au format "chp1=val1&chp2=val2&..."</param>
        /// <returns>liste d'objets récupérés (ou liste vide)</returns>
        private List<T> TraitementRecup<T>(String methode, String message, String parametres)
        {
            // trans
            List<T> liste = new List<T>();
            try
            {
                JObject retour = api.RecupDistant(methode, message, parametres);
                // extraction du code retourné
                String code = (String)retour["code"];
                if (code.Equals("200"))
                {
                    // dans le cas du GET (select), récupération de la liste d'objets
                    if (methode.Equals(GET))
                    {
                        String resultString = JsonConvert.SerializeObject(retour["result"]);
                        // construction de la liste d'objets à partir du retour de l'api
                        liste = JsonConvert.DeserializeObject<List<T>>(resultString, new CustomBooleanJsonConverter());
                    }
                }
                else
                {
                    Log.Information((String)retour["message"], "code erreur = " + code);
                    Console.WriteLine("code erreur = " + code + " message = " + (String)retour["message"]);
                }
            }
            catch (Exception e)
            {
                Log.Fatal(e, "erreur lors de l'accès à l'API");
                Console.WriteLine("Erreur lors de l'accès à l'API : " + e.Message);
                Environment.Exit(0);
            }
            return liste;
        }

        /// <summary>
        /// Convertit en json un couple nom/valeur
        /// </summary>
        /// <param name="nom"></param>
        /// <param name="valeur"></param>
        /// <returns>couple au format json</returns>
        private String convertToJson(Object nom, Object valeur)
        {
            Dictionary<Object, Object> dictionary = new Dictionary<Object, Object>();
            dictionary.Add(nom, valeur);
            return JsonConvert.SerializeObject(dictionary);
        }

        /// <summary>
        /// Modification du convertisseur Json pour gérer le format de date
        /// </summary>
        private sealed class CustomDateTimeConverter : IsoDateTimeConverter
        {
            public CustomDateTimeConverter()
            {
                base.DateTimeFormat = "yyyy-MM-dd";
            }
        }

        /// <summary>
        /// Modification du convertisseur Json pour prendre en compte les booléens
        /// classe trouvée sur le site :
        /// https://www.thecodebuzz.com/newtonsoft-jsonreaderexception-could-not-convert-string-to-boolean/
        /// </summary>
        private sealed class CustomBooleanJsonConverter : JsonConverter<bool>
        {
            public override bool ReadJson(JsonReader reader, Type objectType, bool existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                return Convert.ToBoolean(reader.ValueType == typeof(string) ? Convert.ToByte(reader.Value) : reader.Value);
            }

            public override void WriteJson(JsonWriter writer, bool value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }
        }
    }
}
