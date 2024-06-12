using Python.Included;
using Python.Runtime;

namespace Fileshard.Service.Python
{
    public class PythonInteractor
    {
        public async Task Initialize()
        {
            await Installer.SetupPython();
        }

        public string Execute()
        {
            PythonEngine.Initialize();

            using (Py.GIL())
            {
                using var scope = Py.CreateScope();
                scope.Exec("def test(a):\r\n\treturn { \"retdata\": \"Hmm, \" + a }");

                dynamic test = scope.Get("test");
                dynamic ret = test(".NET is kind of funky sometimes");

                var netDict = new Dictionary<string, string>();

                foreach (var item in ret.items())
                {
                    string key = item[0];
                    netDict[key] = item[1];
                }

                return netDict["retdata"];
            }
        }
    }
}
