using MediaTekDocuments.controller;
using MediaTekDocuments.model;
using System;
using System.Collections.Generic;
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
                else if (utilisateur.Pwd != password)
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
