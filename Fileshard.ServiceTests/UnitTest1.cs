using Fileshard.Service.Python;

namespace Fileshard.ServiceTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            var teract = new PythonInteractor();
            await teract.Initialize();

            Console.WriteLine(teract.Execute());

            Assert.AreEqual(1, 1);
        }
    }
}