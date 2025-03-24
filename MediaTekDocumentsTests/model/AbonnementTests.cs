using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model.Tests
{
    [TestClass()]
    public class AbonnementTests
    {        
        [TestMethod()]
        public void ParutionDansAbonnement_DateDansIntervalle_RetourneVrai()
        {
            Abonnement abonnement = new Abonnement("T0001", new DateTime(2024, 1, 1), 49, new DateTime(2024, 12, 31), "IdT01");
            DateTime dateParution = new DateTime(2024, 6, 15);
            bool result = abonnement.ParutionDansAbonnement(abonnement.DateCommande, abonnement.DateFinAbonnement, dateParution);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void ParutionDansAbonnement_DateAvantCommande_RetourneFaux()
        {
            Abonnement abonnement = new Abonnement("T0001", new DateTime(2024, 1, 1), 49, new DateTime(2024, 12, 31), "IdT01");
            DateTime dateParution = new DateTime(2023, 12, 31);

            bool result = abonnement.ParutionDansAbonnement(abonnement.DateCommande, abonnement.DateFinAbonnement, dateParution);

            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void ParutionDansAbonnement_DateApresFinCommande_RetourneFaux()
        {
            Abonnement abonnement = new Abonnement("T0001", new DateTime(2024, 1, 1), 49, new DateTime(2024, 12, 31), "IdT01");
            DateTime dateParution = new DateTime(2025, 1, 1);

            bool result = abonnement.ParutionDansAbonnement(abonnement.DateCommande, abonnement.DateFinAbonnement, dateParution);

            Assert.IsFalse(result);
        }
    }
}