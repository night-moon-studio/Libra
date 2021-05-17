using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace WebServiceProvider.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PluginController : ControllerBase
    {
        private static readonly string PLUGIN_PATH;
        static PluginController()
        {
            PLUGIN_PATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugin", "PluginService.dll");
        }
        [HttpGet("add")]
        public void AddPlugin()
        {
            LibraDomainManagement.LoadPlugin(PLUGIN_PATH);
        }

        [HttpGet("dispose")]
        public bool DeletePlugin()
        {
            return LibraDomainManagement.UnloadPlugin(PLUGIN_PATH);
        }
    }
}
