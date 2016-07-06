using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using AppLogic.Interfaces;
using AppLogic.Enums;
using AppLogic;
using AppData;
using Windows.ApplicationModel;
using System.IO;
using System.Diagnostics;
using Utils;
using System.Threading.Tasks;

namespace Utils.Tests {
    [TestClass]
    public class SaverTests {
        [ClassInitialize]
        public static void Initialize( TestContext context ) {
            Resources.ConnectionString = Path.Combine(Package.Current.InstalledLocation.Path, Resources.DBFileName);
        }

        [TestMethod]
        public async Task CanSaveSettingToFile() {
            string inputString = "test";
            await SettingSaver.SaveSettingToFile("TestSetting", inputString);
            string outputString = await SettingSaver.GetSettingFromFile("TestSetting");

            Assert.IsNotNull(outputString, "out == null");
            Assert.AreEqual(inputString, outputString, "in != out");
        }

        [TestMethod]
        public void CanSaveSettingToContainer() {
            string inputString = "test";
            SettingSaver.SaveSetting("TestSetting", inputString);
            string outputString = SettingSaver.GetSetting("TestSetting");

            Assert.IsNotNull(outputString, "out == null");
            Assert.AreEqual(inputString, outputString, "in != out");
        }
    }
}
