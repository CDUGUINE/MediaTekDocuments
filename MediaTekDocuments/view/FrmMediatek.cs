using System;
using System.Windows.Forms;
using MediaTekDocuments.model;
using MediaTekDocuments.controller;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;

namespace MediaTekDocuments.view

{
    /// <summary>
    /// Classe d'affichage
    /// </summary>
    public partial class FrmMediatek : Form
    {
        #region Commun
        private readonly FrmMediatekController controller;
        private readonly BindingSource bdgGenres = new BindingSource();
        private readonly BindingSource bdgPublics = new BindingSource();
        private readonly BindingSource bdgRayons = new BindingSource();

        /// <summary>
        /// Constructeur : création du contrôleur lié à ce formulaire
        /// </summary>
        internal FrmMediatek()
        {
            InitializeComponent();
            this.controller = new FrmMediatekController();
        }

        /// <summary>
        /// Affiche un avetissement à l'ouverture de l'application si des abonnements expirent bientôt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMediatek_Load(object sender, EventArgs e)
        {
            lesAbonnements = controller.GetAllAbonnements();
            lesRevues = controller.GetAllRevues();
            Boolean ecrireMessage = false;
            // Trier les abonnements par date de fin d'abonnement
            List<Abonnement> abonnementsTries = lesAbonnements.OrderBy(a => a.DateFinAbonnement).ToList();

            string message = "Abonnements expirants dans moins de 30 jours :\n";
            foreach (Abonnement abonnement in abonnementsTries)
            {
                DateTime dateFinAbonnement = abonnement.DateFinAbonnement;
                Revue revue = lesRevues.Find(x => x.Id.Equals(abonnement.IdRevue));
                string titre = revue?.Titre ?? "Inconnu"; // Evite les erreurs si la revue n'est pas trouvée

                if (!abonnement.AbonnementFinImminente(dateFinAbonnement))
                {
                    ecrireMessage = true;
                    message += $"{titre} expire le {dateFinAbonnement.ToShortDateString()}\n";
                }
            }
            if (ecrireMessage)
            {
                MessageBox.Show(message, "Avertissement");
            }
        }


        /// <summary>
        /// Rempli un des 4 combo (genre, public, rayon, suivi)
        /// </summary>
        /// <param name="lesCategories">liste des objets de type Genre ou Public ou Rayon ou Suivi</param>
        /// <param name="bdg">bindingsource contenant les informations</param>
        /// <param name="cbx">combobox à remplir</param>
        public void RemplirComboCategorie(List<Categorie> lesCategories, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesCategories;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }
        #endregion

        #region Onglet Livres
        private readonly BindingSource bdgLivresListe = new BindingSource();
        private List<Livre> lesLivres = new List<Livre>();

        /// <summary>
        /// Ouverture de l'onglet Livres : 
        /// appel des méthodes pour remplir le datagrid des livres et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabLivres_Enter(object sender, EventArgs e)
        {
            lesLivres = controller.GetAllLivres();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxLivresGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxLivresPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxLivresRayons);
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void RemplirLivresListe(List<Livre> livres)
        {
            bdgLivresListe.DataSource = livres;
            dgvLivresListe.DataSource = bdgLivresListe;
            dgvLivresListe.Columns["isbn"].Visible = false;
            dgvLivresListe.Columns["idRayon"].Visible = false;
            dgvLivresListe.Columns["idGenre"].Visible = false;
            dgvLivresListe.Columns["idPublic"].Visible = false;
            dgvLivresListe.Columns["image"].Visible = false;
            dgvLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLivresListe.Columns["id"].DisplayIndex = 0;
            dgvLivresListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbLivresNumRecherche.Text.Equals(""))
            {
                txbLivresTitreRecherche.Text = "";
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                Livre livre = lesLivres.Find(x => x.Id.Equals(txbLivresNumRecherche.Text));
                if (livre != null)
                {
                    List<Livre> livres = new List<Livre>() { livre };
                    RemplirLivresListe(livres);
                }
                else
                {
                    MessageBox.Show("numéro introuvable", "Information");
                    RemplirLivresListeComplete();
                }
            }
            else
            {
                RemplirLivresListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des livres dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbLivresTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbLivresTitreRecherche.Text.Equals(""))
            {
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                txbLivresNumRecherche.Text = "";
                List<Livre> lesLivresParTitre;
                lesLivresParTitre = lesLivres.FindAll(x => x.Titre.ToLower().Contains(txbLivresTitreRecherche.Text.ToLower()));
                RemplirLivresListe(lesLivresParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxLivresGenres.SelectedIndex < 0 && cbxLivresPublics.SelectedIndex < 0 && cbxLivresRayons.SelectedIndex < 0
                    && txbLivresNumRecherche.Text.Equals(""))
                {
                    RemplirLivresListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre">le livre</param>
        private void AfficheLivresInfos(Livre livre)
        {
            txbLivresAuteur.Text = livre.Auteur;
            txbLivresCollection.Text = livre.Collection;
            txbLivresImage.Text = livre.Image;
            txbLivresIsbn.Text = livre.Isbn;
            txbLivresNumero.Text = livre.Id;
            txbLivresGenre.Text = livre.Genre;
            txbLivresPublic.Text = livre.Public;
            txbLivresRayon.Text = livre.Rayon;
            txbLivresTitre.Text = livre.Titre;
            string image = livre.Image;
            try
            {
                pcbLivresImage.Image = Image.FromFile(@"..\..\images\" + image);
            }
            catch
            {
                pcbLivresImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du livre
        /// </summary>
        private void VideLivresInfos()
        {
            txbLivresAuteur.Text = "";
            txbLivresCollection.Text = "";
            txbLivresImage.Text = "";
            txbLivresIsbn.Text = "";
            txbLivresNumero.Text = "";
            txbLivresGenre.Text = "";
            txbLivresPublic.Text = "";
            txbLivresRayon.Text = "";
            txbLivresTitre.Text = "";
            pcbLivresImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresGenres.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Genre genre = (Genre)cbxLivresGenres.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresPublics.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Public lePublic = (Public)cbxLivresPublics.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresRayons.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxLivresRayons.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirLivresListe(livres);
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivresListe.CurrentCell != null)
            {
                try
                {
                    Livre livre = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
                    AfficheLivresInfos(livre);
                }
                catch
                {
                    VideLivresZones();
                }
            }
            else
            {
                VideLivresInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirLivresListeComplete()
        {
            RemplirLivresListe(lesLivres);
            VideLivresZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideLivresZones()
        {
            cbxLivresGenres.SelectedIndex = -1;
            cbxLivresRayons.SelectedIndex = -1;
            cbxLivresPublics.SelectedIndex = -1;
            txbLivresNumRecherche.Text = "";
            txbLivresTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideLivresZones();
            string titreColonne = dgvLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<Livre> sortedList = new List<Livre>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesLivres.OrderBy(o => o.Titre).ToList();
                    break;
                case "Collection":
                    sortedList = lesLivres.OrderBy(o => o.Collection).ToList();
                    break;
                case "Auteur":
                    sortedList = lesLivres.OrderBy(o => o.Auteur).ToList();
                    break;
                case "Genre":
                    sortedList = lesLivres.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesLivres.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesLivres.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirLivresListe(sortedList);
        }
        #endregion

        #region Onglet Dvd
        private readonly BindingSource bdgDvdListe = new BindingSource();
        private List<Dvd> lesDvd = new List<Dvd>();

        /// <summary>
        /// Ouverture de l'onglet Dvds : 
        /// appel des méthodes pour remplir le datagrid des dvd et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabDvd_Enter(object sender, EventArgs e)
        {
            lesDvd = controller.GetAllDvd();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxDvdGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxDvdPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxDvdRayons);
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="Dvds">liste de dvd</param>
        private void RemplirDvdListe(List<Dvd> Dvds)
        {
            bdgDvdListe.DataSource = Dvds;
            dgvDvdListe.DataSource = bdgDvdListe;
            dgvDvdListe.Columns["idRayon"].Visible = false;
            dgvDvdListe.Columns["idGenre"].Visible = false;
            dgvDvdListe.Columns["idPublic"].Visible = false;
            dgvDvdListe.Columns["image"].Visible = false;
            dgvDvdListe.Columns["synopsis"].Visible = false;
            dgvDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvDvdListe.Columns["id"].DisplayIndex = 0;
            dgvDvdListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du Dvd dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbDvdNumRecherche.Text.Equals(""))
            {
                txbDvdTitreRecherche.Text = "";
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbDvdNumRecherche.Text));
                if (dvd != null)
                {
                    List<Dvd> Dvd = new List<Dvd>() { dvd };
                    RemplirDvdListe(Dvd);
                }
                else
                {
                    MessageBox.Show("numéro introuvable", "Information");
                    RemplirDvdListeComplete();
                }
            }
            else
            {
                RemplirDvdListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des Dvd dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbDvdTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbDvdTitreRecherche.Text.Equals(""))
            {
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                txbDvdNumRecherche.Text = "";
                List<Dvd> lesDvdParTitre;
                lesDvdParTitre = lesDvd.FindAll(x => x.Titre.ToLower().Contains(txbDvdTitreRecherche.Text.ToLower()));
                RemplirDvdListe(lesDvdParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxDvdGenres.SelectedIndex < 0 && cbxDvdPublics.SelectedIndex < 0 && cbxDvdRayons.SelectedIndex < 0
                    && txbDvdNumRecherche.Text.Equals(""))
                {
                    RemplirDvdListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du dvd sélectionné
        /// </summary>
        /// <param name="dvd">le dvd</param>
        private void AfficheDvdInfos(Dvd dvd)
        {
            txbDvdRealisateur.Text = dvd.Realisateur;
            txbDvdSynopsis.Text = dvd.Synopsis;
            txbDvdImage.Text = dvd.Image;
            txbDvdDuree.Text = dvd.Duree.ToString();
            txbDvdNumero.Text = dvd.Id;
            txbDvdGenre.Text = dvd.Genre;
            txbDvdPublic.Text = dvd.Public;
            txbDvdRayon.Text = dvd.Rayon;
            txbDvdTitre.Text = dvd.Titre;
            string image = dvd.Image;
            try
            {
                pcbDvdImage.Image = Image.FromFile(@"..\..\images\" + image);
            }
            catch
            {
                pcbDvdImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du dvd
        /// </summary>
        private void VideDvdInfos()
        {
            txbDvdRealisateur.Text = "";
            txbDvdSynopsis.Text = "";
            txbDvdImage.Text = "";
            txbDvdDuree.Text = "";
            txbDvdNumero.Text = "";
            txbDvdGenre.Text = "";
            txbDvdPublic.Text = "";
            txbDvdRayon.Text = "";
            txbDvdTitre.Text = "";
            pcbDvdImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdGenres.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Genre genre = (Genre)cbxDvdGenres.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdPublics.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Public lePublic = (Public)cbxDvdPublics.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdRayons.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxDvdRayons.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDvdListe.CurrentCell != null)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
                    AfficheDvdInfos(dvd);
                }
                catch
                {
                    VideDvdZones();
                }
            }
            else
            {
                VideDvdInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des Dvd
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirDvdListeComplete()
        {
            RemplirDvdListe(lesDvd);
            VideDvdZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideDvdZones()
        {
            cbxDvdGenres.SelectedIndex = -1;
            cbxDvdRayons.SelectedIndex = -1;
            cbxDvdPublics.SelectedIndex = -1;
            txbDvdNumRecherche.Text = "";
            txbDvdTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideDvdZones();
            string titreColonne = dgvDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<Dvd> sortedList = new List<Dvd>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesDvd.OrderBy(o => o.Titre).ToList();
                    break;
                case "Duree":
                    sortedList = lesDvd.OrderBy(o => o.Duree).ToList();
                    break;
                case "Realisateur":
                    sortedList = lesDvd.OrderBy(o => o.Realisateur).ToList();
                    break;
                case "Genre":
                    sortedList = lesDvd.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesDvd.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesDvd.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirDvdListe(sortedList);
        }
        #endregion

        #region Onglet Revues
        private readonly BindingSource bdgRevuesListe = new BindingSource();
        private List<Revue> lesRevues = new List<Revue>();

        /// <summary>
        /// Ouverture de l'onglet Revues : 
        /// appel des méthodes pour remplir le datagrid des revues et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabRevues_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxRevuesGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxRevuesPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxRevuesRayons);
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="revues"></param>
        private void RemplirRevuesListe(List<Revue> revues)
        {
            bdgRevuesListe.DataSource = revues;
            dgvRevuesListe.DataSource = bdgRevuesListe;
            dgvRevuesListe.Columns["idRayon"].Visible = false;
            dgvRevuesListe.Columns["idGenre"].Visible = false;
            dgvRevuesListe.Columns["idPublic"].Visible = false;
            dgvRevuesListe.Columns["image"].Visible = false;
            dgvRevuesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvRevuesListe.Columns["id"].DisplayIndex = 0;
            dgvRevuesListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage de la revue dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbRevuesNumRecherche.Text.Equals(""))
            {
                txbRevuesTitreRecherche.Text = "";
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbRevuesNumRecherche.Text));
                if (revue != null)
                {
                    List<Revue> revues = new List<Revue>() { revue };
                    RemplirRevuesListe(revues);
                }
                else
                {
                    MessageBox.Show("numéro introuvable", "Information");
                    RemplirRevuesListeComplete();
                }
            }
            else
            {
                RemplirRevuesListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des revues dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbRevuesTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbRevuesTitreRecherche.Text.Equals(""))
            {
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                txbRevuesNumRecherche.Text = "";
                List<Revue> lesRevuesParTitre;
                lesRevuesParTitre = lesRevues.FindAll(x => x.Titre.ToLower().Contains(txbRevuesTitreRecherche.Text.ToLower()));
                RemplirRevuesListe(lesRevuesParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxRevuesGenres.SelectedIndex < 0 && cbxRevuesPublics.SelectedIndex < 0 && cbxRevuesRayons.SelectedIndex < 0
                    && txbRevuesNumRecherche.Text.Equals(""))
                {
                    RemplirRevuesListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionné
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheRevuesInfos(Revue revue)
        {
            txbRevuesPeriodicite.Text = revue.Periodicite;
            txbRevuesImage.Text = revue.Image;
            txbRevuesDateMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbRevuesNumero.Text = revue.Id;
            txbRevuesGenre.Text = revue.Genre;
            txbRevuesPublic.Text = revue.Public;
            txbRevuesRayon.Text = revue.Rayon;
            txbRevuesTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbRevuesImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbRevuesImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations de la reuve
        /// </summary>
        private void VideRevuesInfos()
        {
            txbRevuesPeriodicite.Text = "";
            txbRevuesImage.Text = "";
            txbRevuesDateMiseADispo.Text = "";
            txbRevuesNumero.Text = "";
            txbRevuesGenre.Text = "";
            txbRevuesPublic.Text = "";
            txbRevuesRayon.Text = "";
            txbRevuesTitre.Text = "";
            pcbRevuesImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesGenres.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Genre genre = (Genre)cbxRevuesGenres.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesPublics.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Public lePublic = (Public)cbxRevuesPublics.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesRayons.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxRevuesRayons.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations de la revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRevuesListe.CurrentCell != null)
            {
                try
                {
                    Revue revue = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
                    AfficheRevuesInfos(revue);
                }
                catch
                {
                    VideRevuesZones();
                }
            }
            else
            {
                VideRevuesInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des revues
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirRevuesListeComplete()
        {
            RemplirRevuesListe(lesRevues);
            VideRevuesZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideRevuesZones()
        {
            cbxRevuesGenres.SelectedIndex = -1;
            cbxRevuesRayons.SelectedIndex = -1;
            cbxRevuesPublics.SelectedIndex = -1;
            txbRevuesNumRecherche.Text = "";
            txbRevuesTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideRevuesZones();
            string titreColonne = dgvRevuesListe.Columns[e.ColumnIndex].HeaderText;
            List<Revue> sortedList = new List<Revue>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesRevues.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesRevues.OrderBy(o => o.Titre).ToList();
                    break;
                case "Periodicite":
                    sortedList = lesRevues.OrderBy(o => o.Periodicite).ToList();
                    break;
                case "DelaiMiseADispo":
                    sortedList = lesRevues.OrderBy(o => o.DelaiMiseADispo).ToList();
                    break;
                case "Genre":
                    sortedList = lesRevues.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesRevues.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesRevues.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirRevuesListe(sortedList);
        }
        #endregion

        #region Onglet Paarutions
        private readonly BindingSource bdgExemplairesListe = new BindingSource();
        private List<Exemplaire> lesExemplaires = new List<Exemplaire>();
        const string ETATNEUF = "00001";

        /// <summary>
        /// Ouverture de l'onglet : récupère le revues et vide tous les champs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabReceptionRevue_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            txbReceptionRevueNumero.Text = "";
        }

        /// <summary>
        /// Remplit le dategrid des exemplaires avec la liste reçue en paramètre
        /// </summary>
        /// <param name="exemplaires">liste d'exemplaires</param>
        private void RemplirReceptionExemplairesListe(List<Exemplaire> exemplaires)
        {
            if (exemplaires != null)
            {
                bdgExemplairesListe.DataSource = exemplaires;
                dgvReceptionExemplairesListe.DataSource = bdgExemplairesListe;
                dgvReceptionExemplairesListe.Columns["idEtat"].Visible = false;
                dgvReceptionExemplairesListe.Columns["id"].Visible = false;
                dgvReceptionExemplairesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvReceptionExemplairesListe.Columns["numero"].DisplayIndex = 0;
                dgvReceptionExemplairesListe.Columns["dateAchat"].DisplayIndex = 1;
            }
            else
            {
                bdgExemplairesListe.DataSource = null;
            }
        }

        /// <summary>
        /// Recherche d'un numéro de revue et affiche ses informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionRechercher_Click(object sender, EventArgs e)
        {
            if (!txbReceptionRevueNumero.Text.Equals(""))
            {
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbReceptionRevueNumero.Text));
                if (revue != null)
                {
                    AfficheReceptionRevueInfos(revue);
                }
                else
                {
                    MessageBox.Show("numéro introuvable", "Information");
                }
            }
        }

        /// <summary>
        /// Si le numéro de revue est modifié, la zone de l'exemplaire est vidée et inactive
        /// les informations de la revue son aussi effacées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbReceptionRevueNumero_TextChanged(object sender, EventArgs e)
        {
            txbReceptionRevuePeriodicite.Text = "";
            txbReceptionRevueImage.Text = "";
            txbReceptionRevueDelaiMiseADispo.Text = "";
            txbReceptionRevueGenre.Text = "";
            txbReceptionRevuePublic.Text = "";
            txbReceptionRevueRayon.Text = "";
            txbReceptionRevueTitre.Text = "";
            pcbReceptionRevueImage.Image = null;
            RemplirReceptionExemplairesListe(null);
            AccesReceptionExemplaireGroupBox(false);
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionnée et les exemplaires
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheReceptionRevueInfos(Revue revue)
        {
            // informations sur la revue
            txbReceptionRevuePeriodicite.Text = revue.Periodicite;
            txbReceptionRevueImage.Text = revue.Image;
            txbReceptionRevueDelaiMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbReceptionRevueNumero.Text = revue.Id;
            txbReceptionRevueGenre.Text = revue.Genre;
            txbReceptionRevuePublic.Text = revue.Public;
            txbReceptionRevueRayon.Text = revue.Rayon;
            txbReceptionRevueTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbReceptionRevueImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbReceptionRevueImage.Image = null;
            }
            // affiche la liste des exemplaires de la revue
            AfficheReceptionExemplairesRevue();
        }

        /// <summary>
        /// Récupère et affiche les exemplaires d'une revue
        /// </summary>
        private void AfficheReceptionExemplairesRevue()
        {
            string idDocument = txbReceptionRevueNumero.Text;
            lesExemplaires = controller.GetExemplairesRevue(idDocument);
            RemplirReceptionExemplairesListe(lesExemplaires);
            AccesReceptionExemplaireGroupBox(true);
        }

        /// <summary>
        /// Permet ou interdit l'accès à la gestion de la réception d'un exemplaire
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces">true ou false</param>
        private void AccesReceptionExemplaireGroupBox(bool acces)
        {
            grpReceptionExemplaire.Enabled = acces;
            txbReceptionExemplaireImage.Text = "";
            txbReceptionExemplaireNumero.Text = "";
            pcbReceptionExemplaireImage.Image = null;
            dtpReceptionExemplaireDate.Value = DateTime.Now;
        }

        /// <summary>
        /// Recherche image sur disque (pour l'exemplaire à insérer)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireImage_Click(object sender, EventArgs e)
        {
            string filePath = "";
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                // positionnement à la racine du disque où se trouve le dossier actuel
                InitialDirectory = Path.GetPathRoot(Environment.CurrentDirectory),
                Filter = "Files|*.jpg;*.bmp;*.jpeg;*.png;*.gif"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }
            txbReceptionExemplaireImage.Text = filePath;
            try
            {
                pcbReceptionExemplaireImage.Image = Image.FromFile(filePath);
            }
            catch
            {
                pcbReceptionExemplaireImage.Image = null;
            }
        }

        /// <summary>
        /// Enregistrement du nouvel exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireValider_Click(object sender, EventArgs e)
        {
            if (!txbReceptionExemplaireNumero.Text.Equals("")) 
            {
                try
                {
                    int numero = int.Parse(txbReceptionExemplaireNumero.Text);
                    DateTime dateAchat = dtpReceptionExemplaireDate.Value;
                    string photo = txbReceptionExemplaireImage.Text;
                    string idEtat = ETATNEUF;
                    string idDocument = txbReceptionRevueNumero.Text;
                    Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocument);
                    if (controller.CreerExemplaire(exemplaire)) // à copier ?
                    {
                        AfficheReceptionExemplairesRevue();
                    }
                    else
                    {
                        MessageBox.Show("numéro de publication déjà existant", "Erreur");
                    }
                }
                catch
                {
                    MessageBox.Show("le numéro de parution doit être numérique", "Information");
                    txbReceptionExemplaireNumero.Text = "";
                    txbReceptionExemplaireNumero.Focus();
                }
            }
            else
            {
                MessageBox.Show("numéro de parution obligatoire", "Information");
            }
        }

        /// <summary>
        /// Tri sur une colonne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvExemplairesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvReceptionExemplairesListe.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            switch (titreColonne)
            {
                case "Numero":
                    sortedList = lesExemplaires.OrderBy(o => o.Numero).Reverse().ToList();
                    break;
                case "DateAchat":
                    sortedList = lesExemplaires.OrderBy(o => o.DateAchat).Reverse().ToList();
                    break;
                case "Photo":
                    sortedList = lesExemplaires.OrderBy(o => o.Photo).ToList();
                    break;
            }
            RemplirReceptionExemplairesListe(sortedList);
        }

        /// <summary>
        /// affichage de l'image de l'exemplaire suite à la sélection d'un exemplaire dans la liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvReceptionExemplairesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.CurrentCell != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];
                string image = exemplaire.Photo;
                try
                {
                    pcbReceptionExemplaireRevueImage.Image = Image.FromFile(image);
                }
                catch
                {
                    pcbReceptionExemplaireRevueImage.Image = null;
                }
            }
            else
            {
                pcbReceptionExemplaireRevueImage.Image = null;
            }
        }
        #endregion

        #region Onglet Commande livres
        private readonly BindingSource bdgCommandesListe = new BindingSource();
        private readonly BindingSource bdgSuivis = new BindingSource();
        private List<CommandeDocument> lesCommandesDocument = new List<CommandeDocument>();
        private string derniereColonneTriee = "";
        private bool triCroissant = true;

        /// <summary>
        /// Ouverture de l'onglet Commande Livres : 
        /// appel des méthodes pour remplir le datagrid des commandes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabCommandeLivres_Enter(object sender, EventArgs e)
        {
            lesCommandesDocument = controller.GetAllCommandeDocument();
            VideLivresInfosCom();
            RemplirComboCategorie(controller.GetAllSuivis(), bdgSuivis, cbxSuivi);
            ToolTip aideSuivi = new ToolTip
            {
                AutoPopDelay = 5000,
                InitialDelay = 1000,
                ReshowDelay = 500
            };
            String message1 = "Une commande livrée ou réglée ne peut pas revenir à en cours ou relancée.\n";
            String message2 = "Une commande ne peut pas être réglée si elle n'est pas livrée.";
            aideSuivi.SetToolTip(btnAideSuivi, message1 + message2);
        }

        /// <summary>
        /// Vide les infos du livre et réintialise les autres éléments
        /// </summary>
        private void VideLivresInfosCom()
        {
            txbLivresNumRechercheCom.Text = "";
            txbLivresAuteurCom.Text = "";
            txbLivresCollectionCom.Text = "";
            txbLivresImageCom.Text = "";
            txbLivresIsbnCom.Text = "";
            txbLivresGenreCom.Text = "";
            txbLivresPublicCom.Text = "";
            txbLivresRayonCom.Text = "";
            txbLivresTitreCom.Text = "";
            lblNumDocument.Text = "Document n°";
            pcbLivresImageCom.Image = null;
            AccesNouvelleCommandeLivreGroupBox(false);
            AccesModifCommandeLivreGroupBox(false);
            dgvCommandesListe.DataSource = null;
        }

        /// <summary>
        /// Remplit le datagrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="commandeDocuments">liste de commandes</param>
        private void RemplirCommandesListe(List<CommandeDocument> commandeDocuments)
        {
            bdgCommandesListe.DataSource = commandeDocuments;
            dgvCommandesListe.DataSource = bdgCommandesListe;
            dgvCommandesListe.Columns["id"].Visible = false;
            dgvCommandesListe.Columns["idsuivi"].Visible = false;
            dgvCommandesListe.Columns["idlivredvd"].Visible = false;
            dgvCommandesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvCommandesListe.Columns["dateCommande"].DisplayIndex = 0;
            dgvCommandesListe.Columns["montant"].DisplayIndex = 1;
            dgvCommandesListe.Columns["dateCommande"].HeaderText = "Date de commande";
            dgvCommandesListe.Columns["montant"].HeaderText = "Montant (€)";
            dgvCommandesListe.Columns["nbexemplaire"].HeaderText = "Nombre d'exemplaires";
            dgvCommandesListe.Columns["libelle"].HeaderText = "Suivi commande";
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre">le livre</param>
        private void AfficheLivresInfosCom(Livre livre)
        {
            txbLivresAuteurCom.Text = livre.Auteur;
            txbLivresCollectionCom.Text = livre.Collection;
            txbLivresIsbnCom.Text = livre.Isbn;
            txbLivresGenreCom.Text = livre.Genre;
            txbLivresPublicCom.Text = livre.Public;
            txbLivresRayonCom.Text = livre.Rayon;
            txbLivresTitreCom.Text = livre.Titre;
            string image = livre.Image;
            txbLivresImageCom.Text = image;
            try
            {
                pcbLivresImageCom.Image = Image.FromFile(@"..\..\images\" + image);
            }
            catch
            {
                pcbLivresImageCom.Image = null;
            }
        }

        /// <summary>
        /// Recherche et affichage des commandes du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresNumRechercheCom_Click(object sender, EventArgs e)
        {
            if (!txbLivresNumRechercheCom.Text.Equals(""))
            {
                Livre livre = lesLivres.Find(x => x.Id.Equals(txbLivresNumRechercheCom.Text));
                if (livre != null)
                {
                    AfficheLivresInfosCom(livre);
                    AccesNouvelleCommandeLivreGroupBox(true);
                    AccesModifCommandeLivreGroupBox(true);
                    lblNumDocument.Text = "Document n° " + txbLivresNumRechercheCom.Text;
                    List<CommandeDocument> commandeDocuments = lesCommandesDocument.FindAll(x => x.IdLivreDVD.Equals(txbLivresNumRechercheCom.Text));
                    if (commandeDocuments.Count > 0)
                    {
                        RemplirCommandesListe(commandeDocuments);
                    }
                    else
                    {
                        MessageBox.Show("Aucune commande pour ce livre", "Information");
                        dgvCommandesListe.DataSource = null;
                        AccesModifCommandeLivreGroupBox(false);
                    }
                }
                else
                {
                    MessageBox.Show("numéro introuvable", "Information");
                    VideLivresInfosCom();
                }
            }
            else
            {
                VideLivresInfosCom();
            }
        }

        /// <summary>
        /// Retourne le nouvel indice de commande au format VARCHAR(5)
        /// </summary>
        /// <returns>indice de la nouvelle commande</returns>
        private string NouvelIndex()
        {
            List<CommandeDocument> lesCommandesDocument = controller.GetAllCommandeDocument();
            List<Abonnement> lesAbonnements = controller.GetAllAbonnements();
            if(lesCommandesDocument.Count > 0)
            {
                int intNewIndex = int.Parse(lesCommandesDocument[lesCommandesDocument.Count - 1].Id) + 1;
                if (lesAbonnements.Count > 0)
                {
                    int intNewIndex2 = int.Parse(lesAbonnements[lesAbonnements.Count - 1].Id) + 1;
                    intNewIndex = Math.Max(intNewIndex, intNewIndex2);
                }
                string NewIndex = intNewIndex.ToString();
                while (NewIndex.Length < 5)
                {
                    NewIndex = $"0{NewIndex}";
                }
                return NewIndex;
            }

            else
            {
                return "00001";
            }
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvCommandesListe.Columns[e.ColumnIndex].HeaderText;
            List<CommandeDocument> commandeDocuments = lesCommandesDocument.FindAll(x => x.IdLivreDVD.Equals(txbLivresNumRechercheCom.Text));
            if (derniereColonneTriee == titreColonne)
            {
                triCroissant = !triCroissant;
            }
            else
            {
                triCroissant = true;
                derniereColonneTriee = titreColonne;
            }
            List<CommandeDocument> sortedList = new List<CommandeDocument>();
            switch (titreColonne)
            {
                case "Date de commande":
                    sortedList = triCroissant ?
                        commandeDocuments.OrderBy(o => o.DateCommande).ToList() :
                        commandeDocuments.OrderByDescending(o => o.DateCommande).ToList();
                    break;
                case "Montant (€)":
                    sortedList = triCroissant ?
                        commandeDocuments.OrderBy(o => o.Montant).ToList() :
                        commandeDocuments.OrderByDescending(o => o.Montant).ToList();
                    break;
                case "Nombre d'exemplaires":
                    sortedList = triCroissant ?
                        commandeDocuments.OrderBy(o => o.NbExemplaire).ToList() :
                        commandeDocuments.OrderByDescending(o => o.NbExemplaire).ToList();
                    break;
                case "Suivi commande":
                    sortedList = triCroissant ?
                        commandeDocuments.OrderBy(o => o.Libelle).ToList() :
                        commandeDocuments.OrderByDescending(o => o.Libelle).ToList();
                    break;
            }
            RemplirCommandesListe(sortedList);
        }

        /// <summary>
        /// Mise à jour du grid des commandes en conservant les tris
        /// </summary>
        private void RechargerCommandes()
        {
            lesCommandesDocument = controller.GetAllCommandeDocument();
            List<CommandeDocument> commandeDocuments = lesCommandesDocument.FindAll(x => x.IdLivreDVD.Equals(txbLivresNumRechercheCom.Text));

            if (!string.IsNullOrEmpty(derniereColonneTriee))
            {
                switch (derniereColonneTriee)
                {
                    case "Date de commande":
                        commandeDocuments = triCroissant ?
                                            commandeDocuments.OrderBy(o => o.DateCommande).ToList() :
                                            commandeDocuments.OrderByDescending(o => o.DateCommande).ToList();
                        break;
                    case "Montant (€)":
                        commandeDocuments = triCroissant ?
                                            commandeDocuments.OrderBy(o => o.Montant).ToList() :
                                            commandeDocuments.OrderByDescending(o => o.Montant).ToList();
                        break;
                    case "Nombre d'exemplaires":
                        commandeDocuments = triCroissant ?
                                            commandeDocuments.OrderBy(o => o.NbExemplaire).ToList() :
                                            commandeDocuments.OrderByDescending(o => o.NbExemplaire).ToList();
                        break;
                    case "Suivi commande":
                        commandeDocuments = triCroissant ?
                                            commandeDocuments.OrderBy(o => o.Libelle).ToList() :
                                            commandeDocuments.OrderByDescending(o => o.Libelle).ToList();
                        break;
                }
            }

            RemplirCommandesListe(commandeDocuments);
        }

        /// <summary>
        /// Sélection d'une ligne dans la liste des commandes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCommandesListe.SelectedRows.Count > 0)
            {
                cbxSuivi.SelectedIndex = int.Parse(dgvCommandesListe.SelectedRows[0].Cells[1].Value.ToString()) - 1;
                if (cbxSuivi.SelectedIndex == 1)
                {
                    btnSupprimerCommande.Enabled = false;
                }
                else
                {
                    btnSupprimerCommande.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Modification du suivi d'une commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModifSuivi_Click(object sender, EventArgs e)
        {
            //lesCommandesDocuments = controller.GetAllCommandeDocument();
            CommandeDocument commandeDocument = lesCommandesDocument.Find(x => x.Id.Equals(dgvCommandesListe.SelectedRows[0].Cells[4].Value));
            commandeDocument.IdSuivi = cbxSuivi.SelectedIndex + 1;
            try
            {
                controller.SetOneSuivi(commandeDocument);
                RechargerCommandes();
            }
            catch
            {
                String message1 = "Une commande livrée ou réglée ne peut pas revenir à en cours ou relancée.\n";
                String message2 = "Une commande ne peut pas être réglée si elle n'est pas livrée.";
                MessageBox.Show(message1 + message2, "Information");
            }
        }

        /// <summary>
        /// Suppression d'une commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSupprimerCommande_Click(object sender, EventArgs e)
        {
            CommandeDocument commandeDocument = lesCommandesDocument.Find(x => x.Id.Equals(dgvCommandesListe.SelectedRows[0].Cells[4].Value));
            controller.SupprCommandeDocument(commandeDocument);
            RechargerCommandes();
        }

        /// <summary>
        /// Création d'une nouvelle commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnValider_Click(object sender, EventArgs e)
        {
            if (!txbMontant.Text.Equals("") && nudQuantite.Value > 0)
            {
                try
                {
                    string id = NouvelIndex();
                    DateTime dateCommande = DateTime.Now;
                    decimal montant = Convert.ToDecimal(txbMontant.Text);
                    int idsuivi = 1;
                    string libelle = "en cours";
                    string idLivreDVD = txbLivresNumRechercheCom.Text;
                    int nbExemplaire = Convert.ToInt32(nudQuantite.Value);
                    CommandeDocument commandeDocument = new CommandeDocument(id, dateCommande, montant, idsuivi, libelle, idLivreDVD, nbExemplaire);
                    bool insert = controller.CreerCommandeDocument(commandeDocument);
                    if (insert)
                    {
                        MessageBox.Show("Commande n° " + id + " créée", "Information");
                        txbMontant.Text = "";
                        nudQuantite.Value = 0;
                        RechargerCommandes();
                    }
                }
                catch
                {
                    MessageBox.Show("Le montant doit être un nombre décimal", "Information");
                }
            }
            else
            {
                MessageBox.Show("Vous devez compléter le montant et la quantité", "Information");
            }
        }

        /// <summary>
        /// Permet ou interdit l'accès à la gestion d'une nouvelle commande
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces">true ou false</param>
        private void AccesNouvelleCommandeLivreGroupBox(bool acces)
        {
            grpNouvCommande.Enabled = acces;
            lblNumDocument.Text = "Document n°";
            txbMontant.Text = "";
        }

        /// <summary>
        /// Permet ou interdit l'accès à la gestion de la modification d'une commande
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces">true ou false</param>
        private void AccesModifCommandeLivreGroupBox(bool acces)
        {
            grpModifCommande.Enabled = acces;
        }

        #endregion

        #region Onglet Commande DVD
        private readonly BindingSource bdgCommandesListeDvd = new BindingSource();
        private readonly BindingSource bdgSuivisDvd = new BindingSource();
        private string derniereColonneTrieeDvd = "";
        private bool triCroissantDvd = true;

        /// <summary>
        /// Ouverture de l'onglet Commande DVD : 
        /// appel des méthodes pour remplir le datagrid des commandes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabCommandeDvd_Enter(object sender, EventArgs e)
        {
            lesCommandesDocument = controller.GetAllCommandeDocument();
            VideDvdInfosCom();
            lesDvd = controller.GetAllDvd();
            RemplirComboCategorie(controller.GetAllSuivis(), bdgSuivisDvd, cbxSuiviDvd);
            ToolTip aideSuiviDvd = new ToolTip
            {
                AutoPopDelay = 5000,
                InitialDelay = 1000,
                ReshowDelay = 500
            };
            String message1 = "Une commande livrée ou réglée ne peut pas revenir à en cours ou relancée.\n";
            String message2 = "Une commande ne peut pas être réglée si elle n'est pas livrée.";
            aideSuiviDvd.SetToolTip(btnAideSuiviDvd, message1 + message2);
        }

        /// <summary>
        /// Vide les infos du DVD et réintialise les autres éléments
        /// </summary>
        private void VideDvdInfosCom()
        {
            txbDvdNumRechercheCom.Text = "";
            txbDvdDureeCom.Text = "";
            txbDvdTitreCom.Text = "";
            txbDvdRealisateurCom.Text = "";
            txbDvdSynopsisCom.Text = "";
            txbDvdGenreCom.Text = "";
            txbDvdPublicCom.Text = "";
            txbDvdRayonCom.Text = "";
            txbDvdImageCom.Text = "";
            pcbDvdImageCom.Image = null;
            AccesNouvelleCommandeDvdGroupBox(false);
            AccesModifCommandeDvdGroupBox(false);
            dgvCommandesListeDvd.DataSource = null;
        }

        /// <summary>
        /// Remplit le datagrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="commandeDocuments"></param>
        private void RemplirCommandesDvdListe(List<CommandeDocument> commandeDocuments)
        {
            bdgCommandesListeDvd.DataSource = commandeDocuments;
            dgvCommandesListeDvd.DataSource = bdgCommandesListeDvd;
            dgvCommandesListeDvd.Columns["id"].Visible = false;
            dgvCommandesListeDvd.Columns["idSuivi"].Visible = false;
            dgvCommandesListeDvd.Columns["idLivreDVD"].Visible = false;
            dgvCommandesListeDvd.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvCommandesListeDvd.Columns["DateCommande"].DisplayIndex = 0;
            dgvCommandesListeDvd.Columns["Montant"].DisplayIndex = 1;
            dgvCommandesListeDvd.Columns["dateCommande"].HeaderText = "Date de commande";
            dgvCommandesListeDvd.Columns["montant"].HeaderText = "Montant (€)";
            dgvCommandesListeDvd.Columns["nbexemplaire"].HeaderText = "Nombre d'exemplaires";
            dgvCommandesListeDvd.Columns["libelle"].HeaderText = "Suivi commande";
        }

        /// <summary>
        /// Affichage des informations du DVD sélectionné
        /// </summary>
        /// <param name="dvd"></param>
        private void AfficheDvdInfosCom(Dvd dvd)
        {
            txbDvdDureeCom.Text = dvd.Duree.ToString();
            txbDvdTitreCom.Text = dvd.Titre;
            txbDvdRealisateurCom.Text = dvd.Realisateur;
            txbDvdSynopsisCom.Text = dvd.Synopsis;
            txbDvdGenreCom.Text = dvd.Genre;
            txbDvdPublicCom.Text = dvd.Public;
            txbDvdRayonCom.Text = dvd.Rayon;
            string image = dvd.Image;
            txbDvdImageCom.Text = image;
            try
            {
                pcbDvdImageCom.Image = Image.FromFile(@"..\..\images\" + image);
            }
            catch
            {
                pcbDvdImageCom.Image = null;
            }
        }

        /// <summary>
        /// Recherche et affichage des commandes du DVD dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdNumRechercheCom_Click(object sender, EventArgs e)
        {
            if (!txbDvdNumRechercheCom.Text.Equals(""))
            {
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbDvdNumRechercheCom.Text));
                if (dvd != null)
                {
                    AfficheDvdInfosCom(dvd);
                    AccesNouvelleCommandeDvdGroupBox(true);
                    AccesModifCommandeDvdGroupBox(true);
                    lblNumDvd.Text = "Document n° " + txbDvdNumRechercheCom.Text;
                    List<CommandeDocument> commandeDocuments = lesCommandesDocument.FindAll(x => x.IdLivreDVD.Equals(txbDvdNumRechercheCom.Text));
                    if (commandeDocuments.Count > 0)
                    {
                        RemplirCommandesDvdListe(commandeDocuments);
                    }
                    else
                    {
                        MessageBox.Show("Aucune commande pour ce DVD", "Information");
                        dgvCommandesListeDvd.DataSource = null;
                        AccesModifCommandeDvdGroupBox(false);
                    }
                }
                else
                {
                    MessageBox.Show("numéro introuvable", "Information");
                    VideDvdInfosCom();
                }
            }
            else
            {
                VideDvdInfosCom();
            }
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandesListeDvd_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvCommandesListeDvd.Columns[e.ColumnIndex].HeaderText;
            List<CommandeDocument> commandeDocuments = lesCommandesDocument.FindAll(x => x.IdLivreDVD.Equals(txbDvdNumRechercheCom.Text));
            if (derniereColonneTrieeDvd == titreColonne)
            {
                triCroissantDvd = !triCroissantDvd;
            }
            else
            {
                triCroissantDvd = true;
                derniereColonneTrieeDvd = titreColonne;
            }
            List<CommandeDocument> sortedList = new List<CommandeDocument>();
            switch (titreColonne)
            {
                case "Date de commande":
                    sortedList = triCroissantDvd ?
                        commandeDocuments.OrderBy(o => o.DateCommande).ToList() :
                        commandeDocuments.OrderByDescending(o => o.DateCommande).ToList();
                    break;
                case "Montant (€)":
                    sortedList = triCroissantDvd ?
                        commandeDocuments.OrderBy(o => o.Montant).ToList() :
                        commandeDocuments.OrderByDescending(o => o.Montant).ToList();
                    break;
                case "Nombre d'exemplaires":
                    sortedList = triCroissantDvd ?
                        commandeDocuments.OrderBy(o => o.NbExemplaire).ToList() :
                        commandeDocuments.OrderByDescending(o => o.NbExemplaire).ToList();
                    break;
                case "Suivi commande":
                    sortedList = triCroissantDvd ?
                        commandeDocuments.OrderBy(o => o.Libelle).ToList() :
                        commandeDocuments.OrderByDescending(o => o.Libelle).ToList();
                    break;
            }
            RemplirCommandesDvdListe(sortedList);
        }

        /// <summary>
        /// Mise à jour du grid des commandes en conservant les tris
        /// </summary>
        private void RechargerCommandesDvd()
        {
            lesCommandesDocument = controller.GetAllCommandeDocument();
            List<CommandeDocument> commandeDocuments = lesCommandesDocument.FindAll(x => x.IdLivreDVD.Equals(txbDvdNumRechercheCom.Text));

            if (!string.IsNullOrEmpty(derniereColonneTriee))
            {
                switch (derniereColonneTrieeDvd)
                {
                    case "Date de commande":
                        commandeDocuments = triCroissantDvd ?
                                            commandeDocuments.OrderBy(o => o.DateCommande).ToList() :
                                            commandeDocuments.OrderByDescending(o => o.DateCommande).ToList();
                        break;
                    case "Montant (€)":
                        commandeDocuments = triCroissantDvd ?
                                            commandeDocuments.OrderBy(o => o.Montant).ToList() :
                                            commandeDocuments.OrderByDescending(o => o.Montant).ToList();
                        break;
                    case "Nombre d'exemplaires":
                        commandeDocuments = triCroissantDvd ?
                                            commandeDocuments.OrderBy(o => o.NbExemplaire).ToList() :
                                            commandeDocuments.OrderByDescending(o => o.NbExemplaire).ToList();
                        break;
                    case "Suivi commande":
                        commandeDocuments = triCroissantDvd ?
                                            commandeDocuments.OrderBy(o => o.Libelle).ToList() :
                                            commandeDocuments.OrderByDescending(o => o.Libelle).ToList();
                        break;
                }
            }

            RemplirCommandesDvdListe(commandeDocuments);
        }

        /// <summary>
        /// Sélection d'une ligne dans la liste des commandes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandesListeDvd_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCommandesListeDvd.SelectedRows.Count > 0)
            {
                cbxSuiviDvd.SelectedIndex = int.Parse(dgvCommandesListeDvd.SelectedRows[0].Cells[1].Value.ToString()) - 1;
                if (cbxSuiviDvd.SelectedIndex == 1)
                {
                    btnSupprimerCommandeDvd.Enabled = false;
                }
                else
                {
                    btnSupprimerCommandeDvd.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Modification du suivi d'une commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModifSuiviDvd_Click(object sender, EventArgs e)
        {
            //lesCommandesDocuments = controller.GetAllCommandeDocument();
            CommandeDocument commandeDocument = lesCommandesDocument.Find(x => x.Id.Equals(dgvCommandesListeDvd.SelectedRows[0].Cells[4].Value));
            commandeDocument.IdSuivi = cbxSuiviDvd.SelectedIndex + 1;
            try
            {
                controller.SetOneSuivi(commandeDocument); // idem livre ?
                RechargerCommandesDvd();
            }
            catch
            {
                String message1 = "Une commande livrée ou réglée ne peut pas revenir à en cours ou relancée.\n";
                String message2 = "Une commande ne peut pas être réglée si elle n'est pas livrée.";
                MessageBox.Show(message1 + message2, "Information");
            }
        }

        /// <summary>
        /// Suppression d'une commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSupprimerCommandeDvd_Click(object sender, EventArgs e)
        {
            CommandeDocument commandeDocument = lesCommandesDocument.Find(x => x.Id.Equals(dgvCommandesListeDvd.SelectedRows[0].Cells[4].Value));
            controller.SupprCommandeDocument(commandeDocument);
            RechargerCommandesDvd();
        }

        /// <summary>
        /// Création d'une nouvelle commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnValiderDvd_Click(object sender, EventArgs e)
        {
            if (!txbMontantDvd.Text.Equals("") && nudQuantiteDvd.Value > 0)
            {
                try
                {
                    string id = NouvelIndex();
                    DateTime dateCommande = DateTime.Now;
                    decimal montant = Convert.ToDecimal(txbMontantDvd.Text);
                    int idsuivi = 1;
                    string libelle = "en cours";
                    string idLivreDVD = txbDvdNumRechercheCom.Text;
                    int nbExemplaire = Convert.ToInt32(nudQuantiteDvd.Value);
                    CommandeDocument commandeDocument = new CommandeDocument(id, dateCommande, montant, idsuivi, libelle, idLivreDVD, nbExemplaire);
                    bool insert = controller.CreerCommandeDocument(commandeDocument);
                    if (insert)
                    {
                        MessageBox.Show("Commande n° " + id + " créée", "Information");
                        txbMontantDvd.Text = "";
                        nudQuantiteDvd.Value = 0;
                        RechargerCommandesDvd();
                    }
                }
                catch
                {
                    MessageBox.Show("Le montant doit être un nombre décimal", "Information");
                }
            }
            else
            {
                MessageBox.Show("Vous devez compléter le montant et la quantité", "Information");
            }
        }

        /// <summary>
        /// Permet ou interdit l'accès à la gestion d'une nouvelle commande
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces">true ou false</param>
        private void AccesNouvelleCommandeDvdGroupBox(bool acces)
        {
            grpNouvCommandeDvd.Enabled = acces;
            lblNumDvd.Text = "Document n°";
            txbMontantDvd.Text = "";
        }

        /// <summary>
        /// Permet ou interdit l'accès à la gestion de la modification d'une commande
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces">true ou false</param>
        private void AccesModifCommandeDvdGroupBox(bool acces)
        {
            grpModifCommandeDvd.Enabled = acces;
        }

        #endregion

        #region Onglet Commande revues
        //private List<Revue> lesRevues = new List<Revue>(); pour rappel, défini dans l'onglet revues

        private readonly BindingSource bdgAbonnementsListe = new BindingSource();
        private List<Abonnement> lesAbonnements = new List<Abonnement>();
        private string derniereColonneTrieeRevue = "";
        private bool triCroissantRevue = true;

        /// <summary>
        /// Ouverture de l'onglet Commande revues : 
        /// appel des méthodes pour remplir le datagrid des commandes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabCommandeRevues_Enter(object sender, EventArgs e)
        {
            lesAbonnements = controller.GetAllAbonnements();
            VideRevueInfosCom();
            lesRevues = controller.GetAllRevues();
        }

        /// <summary>
        /// Vide les infos de la revue et réintialise les autres éléments
        /// </summary>
        private void VideRevueInfosCom()
        {
            txbRevuesNumRechercheCom.Text = "";
            txbRevuesTitreCom.Text = "";
            txbRevuesPeriodiciteCom.Text = "";
            txbRevuesDateMiseADispoCom.Text = "";
            txbRevuesGenreCom.Text = "";
            txbRevuesPublicCom.Text = "";
            txbRevuesRayonCom.Text = "";
            txbRevuesImageCom.Text = "";
            pcbRevuesImageCom.Image = null;
            AccesNouvelleCommandeRevueGroupBox(false);
            AccesModifCommandeRevueGroupBox(false);
            dgvCommandesListeRevue.DataSource = null;
        }

        /// <summary>
        /// Remplit le datagrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="commandeDocuments"></param>
        private void RemplirCommandesRevueListe(List<Abonnement> abonnements)
        {
            bdgAbonnementsListe.DataSource = abonnements;
            dgvCommandesListeRevue.DataSource = bdgAbonnementsListe;
            dgvCommandesListeRevue.Columns["id"].Visible = false;
            dgvCommandesListeRevue.Columns["idRevue"].Visible = false;
            dgvCommandesListeRevue.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvCommandesListeRevue.Columns["DateCommande"].DisplayIndex = 0;
            dgvCommandesListeRevue.Columns["Montant"].DisplayIndex = 1;
            dgvCommandesListeRevue.Columns["dateCommande"].HeaderText = "Date de commande";
            dgvCommandesListeRevue.Columns["montant"].HeaderText = "Montant (€)";
            dgvCommandesListeRevue.Columns["dateFinAbonnement"].HeaderText = "Date de fin d'abonnement";
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionnée
        /// </summary>
        /// <param name="dvd"></param>
        private void AfficheRevueInfosCom(Revue revue)
        {
            txbRevuesTitreCom.Text = revue.Titre;
            txbRevuesPeriodiciteCom.Text = revue.Periodicite;
            txbRevuesDateMiseADispoCom.Text = revue.DelaiMiseADispo.ToString();
            txbRevuesGenreCom.Text = revue.Genre;
            txbRevuesPublicCom.Text = revue.Public;
            txbRevuesRayonCom.Text = revue.Rayon;
            string image = revue.Image;
            txbRevuesImageCom.Text = image;
            try
            {
                pcbRevuesImageCom.Image = Image.FromFile(@"..\..\images\" + image);
            }
            catch
            {
                pcbRevuesImageCom.Image = null;
            }
        }

        /// <summary>
        /// Recherche et affichage des commandes de revues dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevueNumRechercheCom_Click(object sender, EventArgs e)
        {
            if (!txbRevuesNumRechercheCom.Text.Equals(""))
            {
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbRevuesNumRechercheCom.Text));
                if (revue != null)
                {
                    AfficheRevueInfosCom(revue);
                    AccesNouvelleCommandeRevueGroupBox(true);
                    AccesModifCommandeRevueGroupBox(true);
                    lblNumRevue.Text = "Document n° " + txbRevuesNumRechercheCom.Text;
                    List<Abonnement> abonnements = lesAbonnements.FindAll(x => x.IdRevue.Equals(txbRevuesNumRechercheCom.Text));
                    if (abonnements.Count > 0)
                    {
                        RemplirCommandesRevueListe(abonnements);
                    }
                    else
                    {
                        MessageBox.Show("Aucune commande pour cette revue", "Information");
                        bdgAbonnementsListe.DataSource = null;
                        AccesModifCommandeRevueGroupBox(false);
                    }
                }
                else
                {
                    MessageBox.Show("numéro introuvable", "Information");
                    VideRevueInfosCom();
                }
            }
            else
            {
                VideRevueInfosCom();
            }
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>            
        private void dgvCommandesListeRevue_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvCommandesListeRevue.Columns[e.ColumnIndex].HeaderText;
            List<Abonnement> abonnements = lesAbonnements.FindAll(x => x.IdRevue.Equals(txbRevuesNumRechercheCom.Text));
            if (derniereColonneTrieeRevue == titreColonne)
            {
                triCroissantRevue = !triCroissantRevue;
            }
            else
            {
                triCroissantRevue = true;
                derniereColonneTrieeRevue = titreColonne;
            }
            List<Abonnement> sortedList = new List<Abonnement>();
            switch (titreColonne)
            {
                case "Date de commande":
                    sortedList = triCroissantRevue ?
                        abonnements.OrderBy(o => o.DateCommande).ToList() :
                        abonnements.OrderByDescending(o => o.DateCommande).ToList();
                    break;
                case "Montant (€)":
                    sortedList = triCroissantRevue ?
                        abonnements.OrderBy(o => o.Montant).ToList() :
                        abonnements.OrderByDescending(o => o.Montant).ToList();
                    break;
                case "Date de fin d'abonnement":
                    sortedList = triCroissantRevue ?
                        abonnements.OrderBy(o => o.DateFinAbonnement).ToList() :
                        abonnements.OrderByDescending(o => o.DateFinAbonnement).ToList();
                    break;
            }
            RemplirCommandesRevueListe(sortedList);
        }

        /// <summary>
        /// Mise à jour du grid des commandes en conservant les tris
        /// </summary>
        private void RechargerAbonnementsRevues()
        {
            lesAbonnements = controller.GetAllAbonnements();
            List<Abonnement> abonnements = lesAbonnements.FindAll(x => x.IdRevue.Equals(txbRevuesNumRechercheCom.Text));

            if (!string.IsNullOrEmpty(derniereColonneTriee))
            {
                switch (derniereColonneTrieeRevue)
                {
                    case "Date de commande":
                        abonnements = triCroissantRevue ?
                                            abonnements.OrderBy(o => o.DateCommande).ToList() :
                                            abonnements.OrderByDescending(o => o.DateCommande).ToList();
                        break;
                    case "Montant (€)":
                        abonnements = triCroissantRevue ?
                                            abonnements.OrderBy(o => o.Montant).ToList() :
                                            abonnements.OrderByDescending(o => o.Montant).ToList();
                        break;
                    case "Date de fin d'abonnement":
                        abonnements = triCroissantRevue ?
                                            abonnements.OrderBy(o => o.DateFinAbonnement).ToList() :
                                            abonnements.OrderByDescending(o => o.DateFinAbonnement).ToList();
                        break;
                }
            }
            RemplirCommandesRevueListe(abonnements);
        }

        /// <summary>
        /// Sélection d'une ligne dans la liste des commandes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandesListeRevue_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCommandesListeRevue.SelectedRows.Count > 0) // à revoir
            {
                AccesModifCommandeRevueGroupBox(true);                
            }
            else
            {
                AccesModifCommandeRevueGroupBox(false);
            }
        }

        /// <summary>
        /// Création d'un nouvel abonnement (ou renouvellement)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnValiderRevue_Click(object sender, EventArgs e)
        {
            if (!txbMontantRevue.Text.Equals("") && dtpFinAbonnementDate.Value > DateTime.Now)
            {
                try
                {
                    string id = NouvelIndex();
                    DateTime dateCommande = DateTime.Now;
                    decimal montant = Convert.ToDecimal(txbMontantRevue.Text);
                    DateTime dateFinAbonnement = dtpFinAbonnementDate.Value;
                    string idRevue = txbRevuesNumRechercheCom.Text;
                    Abonnement abonnement = new Abonnement(id, dateCommande, montant, dateFinAbonnement, idRevue);
                    bool insert = controller.CreerAbonnement(abonnement);
                    if (insert)
                    {
                        MessageBox.Show("Abonnement n° " + id + " créé", "Information");
                        txbMontantRevue.Text = "";
                        RechargerAbonnementsRevues();
                    }
                }
                catch
                {
                    MessageBox.Show("Le montant doit être un nombre décimal", "Information");
                }
            }
            else
            {
                MessageBox.Show("Vous devez compléter le montant et la quantité", "Information");
            }
        }

        /// <summary>
        /// Permet ou interdit l'accès à la gestion d'une nouvelle commande
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces">true ou false</param>
        private void AccesNouvelleCommandeRevueGroupBox(bool acces)
        {
            grpNouvCommandeRevue.Enabled = acces;
            lblNumRevue.Text = "Document n°";
            txbMontantRevue.Text = "";
            dtpFinAbonnementDate.Value = DateTime.Now;
        }

        /// <summary>
        /// Permet ou interdit l'accès à la gestion de la modification d'une commande
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces">true ou false</param>
        private void AccesModifCommandeRevueGroupBox(bool acces)
        {
            grpModifCommandeRevue.Enabled = acces;
        }

        /// <summary>
        /// Suppression d'un abonnement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSupprimerCommandeRevue_Click(object sender, EventArgs e)
        {
            Boolean supprimer = true;
            Abonnement abonnement = lesAbonnements.Find(x => x.Id.Equals(dgvCommandesListeRevue.SelectedRows[0].Cells[2].Value));
            string idDocument = txbRevuesNumRechercheCom.Text;
            lesExemplaires = controller.GetExemplairesRevue(idDocument);
            DateTime dateCommande = abonnement.DateCommande;
            DateTime dateFinAbonnement = abonnement.DateFinAbonnement;
            foreach (Exemplaire exemplaire in lesExemplaires)
            {
                DateTime dateParution = exemplaire.DateAchat;
                if (abonnement.ParutionDansAbonnement(dateCommande, dateFinAbonnement, dateParution))
                {
                    supprimer = false;
                }
            }
            if (supprimer)
                {
                    controller.SupprAbonnement(abonnement);
                    RechargerAbonnementsRevues();
                }
            else
            {
                MessageBox.Show("Un ou plusieurs exemplaires sont encore rattachés à cette revue", "Suppression impossible");
            }              
        }

        #endregion

    }
}
