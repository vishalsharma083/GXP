using GXP.Core.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using GXP.Core.Interfaces;
using GXP.Core.DNNEntities;
using System.Collections.Generic;
using System.Linq;
using GXP.Core;
using System.Web;
using System.Collections.Specialized;
using GXP.Dep;
using System.Configuration;
namespace GXP.Library.Tests
{
    
    
    /// <summary>
    ///This is a test class for PagePublisherTest and is intended
    ///to contain all PagePublisherTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PagePublisherTest
    {

        private Mock<ICachingService> _cachingService = null;
        private Mock<IDBService> _dbService = null;

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
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
           
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
            _cachingService = new Mock<ICachingService>();
            DependencyManager.CachingService = _cachingService.Object;

            _dbService = new Mock<IDBService>();
            DependencyManager.DBService = _dbService.Object;
        }
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Publish
        ///</summary>
        [TestMethod()]
        public void PublishTest()
        {
            PagePublisher target = new PagePublisher();
            PagePublisherInput input_ = new PagePublisherInput();
            var mockContext = new Mock<HttpContextBase>();
            var mockRequest = new Mock<HttpRequestBase>();

            input_.CurrentContext = mockContext.Object;

            mockContext.Setup(x => x.Response).Returns(new Mock<HttpResponseBase>().Object);
            mockContext.Setup(x => x.ApplicationInstance).Returns(new Mock<HttpApplication>().Object);
            mockContext.Setup(x => x.Request).Returns(mockRequest.Object);
            mockRequest.Setup(x => x.PhysicalApplicationPath).Returns(ConfigurationManager.AppSettings["PortalFolderPath"]);

            NameValueCollection coll = new NameValueCollection();
            coll.Add("CityCode","NYC");
            coll.Add("@PortalID","2");
            mockRequest.Setup(x => x.QueryString).Returns(coll);

            input_.ActiveTab = new Tabs() { SkinSrc = "Skins/General/seo-destinations-opt.ascx", TabID = 347, PortalID = 2 };

            List<TabModules> tabModules = new List<TabModules>();
            using (DNNEntities context = new DNNEntities())
            {
                tabModules = context.TabModules.Where(x => x.TabID == input_.ActiveTab.TabID).ToList<TabModules>();
            }

            _dbService.Setup(x => x.GetAllTabModules(input_.ActiveTab.TabID)).Returns(tabModules);
            _dbService.Setup(x => x.LoadBaseData()).Returns(new SQLHelper().LoadBaseData());

            PagePublisherResult actual;
            actual = target.Publish(input_);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
