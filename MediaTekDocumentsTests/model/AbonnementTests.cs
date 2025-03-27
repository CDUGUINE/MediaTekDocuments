using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MediaTekDocuments.model.Tests
{
    [TestClass()]
    public class AbonnementTests
    {
        [TestMethod()]
        public void ParutionDansAbonnement_DateDansIntervalle_RetourneVrai()
        {
            Abonnement abonnement = new Abonnement("T0001", DateTime.Now, 49, DateTime.Now.AddYears(1), "IdT01");
            DateTime dateParution = DateTime.Now.AddDays(1);
            bool result = abonnement.ParutionDansAbonnement(abonnement.DateCommande, abonnement.DateFinAbonnement, dateParution);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void ParutionDansAbonnement_DateAvantCommande_RetourneFaux()
        {
            Abonnement abonnement = new Abonnement("T0001", DateTime.Now.AddDays(1), 49, DateTime.Now.AddYears(1), "IdT01");
            DateTime dateParution = DateTime.Now;

            bool result = abonnement.ParutionDansAbonnement(abonnement.DateCommande, abonnement.DateFinAbonnement, dateParution);

            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void ParutionDansAbonnement_DateApresFinCommande_RetourneFaux()
        {
            Abonnement abonnement = new Abonnement("T0001", DateTime.Now, 49, DateTime.Now.AddYears(1), "IdT01");
            DateTime dateParution = DateTime.Now.AddYears(2);

            bool result = abonnement.ParutionDansAbonnement(abonnement.DateCommande, abonnement.DateFinAbonnement, dateParution);

            Assert.IsFalse(result);
        }
    }
}