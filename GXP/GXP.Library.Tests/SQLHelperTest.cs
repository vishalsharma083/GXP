using GXP.Dep;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GXP.Core.DNNEntities;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
namespace GXP.Library.Tests
{
    
    
    /// <summary>
    ///This is a test class for SQLHelperTest and is intended
    ///to contain all SQLHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SQLHelperTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            GXP.Core.DependencyManager.CachingService = new ASPNetCachingService();
        }
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for GetAllTabModules
        ///</summary>
        [TestMethod()]
        public void GetAllTabModulesTest()
        {
            SQLHelper target = new SQLHelper(); 
            int tabId_ = 347;
            List<TabModules> actual;
            actual = target.GetAllTabModules(tabId_);

            var groupData = actual.GroupBy(x => x.PaneName).ToDictionary(x => x.Key, y => y);

            foreach (var item in groupData)
            {
                foreach (var item2 in item.Value)
                {
                    Console.Out.WriteLine(item2.TabModuleID + " : " + item2.PaneName);    
                }
            }
            
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
