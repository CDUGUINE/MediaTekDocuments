using MediaTekDocuments.controller;
using MediaTekDocuments.model;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace MediaTekDocuments.view
{
    /// <summary>
    /// Classe d'affichage
    /// </summary>
    public partial class FrmAuthentification : Form
    {
        private readonly FrmMediatekController controller;
        private readonly List<Utilisateur> lesUtilisateurs;

        internal FrmAuthentification()
        {
            InitializeComponent();
            controller = new FrmMediatekController();
            lesUtilisateurs = controller.GetAllUtilisateurs();
        }

        /// <summary>
        /// Hache le mot de passe saisie
        /// </summary>
        /// <param name="input">mot de passe saisi</param>
        /// <returns>mot de passe crypté</returns>
        private string HashSHA256(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Valide la saisie des identifiants et compare à la base de données
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnValider_Click(object sender, EventArgs e)
        {
            string login = txbLogin.Text;
            string password = txbPassword.Text;
            if (login != "" && password != "")
            {
                Utilisateur utilisateur = lesUtilisateurs.Find(x => x.Login.Equals(login));
                if (utilisateur == null)
                {
                    MessageBox.Show("Utilisateur inconnu", "Erreur");
                }
                else if (utilisateur.Pwd != HashSHA256(password))
                {
                    MessageBox.Show("Mot de passe incorrect", "Erreur");
                }
                else
                {
                    controller.LancerApplication(this, new FrmMediatek(utilisateur.IdService));
                }
            }
            else
            {
                MessageBox.Show("Vous devez compléter les deux champs", "Erreur d'authentification");
            }
        }
    }
}
