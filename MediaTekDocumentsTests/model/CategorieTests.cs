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
    public class CategorieTests
    {
        [TestMethod()]
        public void ToStringTest()
        {
            Categorie categorie = new Categorie("10020", "Horreur");
            string result = categorie.ToString();
            Assert.AreEqual("Horreur", result);
        }
    }
}