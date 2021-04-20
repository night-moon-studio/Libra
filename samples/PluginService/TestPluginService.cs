using System;

namespace PluginService
{
    public class TestPluginService
    {
        public string Get(string name)
        {
            return "Hello " + name;
        }
        public void Show()
        {
            Console.WriteLine("a");
        }
    }
}
