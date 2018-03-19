using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stroller.Bll;

namespace BllTests
{
    [TestClass]
    public class StrollerSettingsServiceTests
    {
        [TestMethod]
        public async void GetConfig()
        {
            var result = await new StrollerSettingsService().GetSettings();

            Assert.IsNotNull(result);
        }
    }
}
