using System.Threading.Tasks;

namespace WebCallerClient.Controller
{
    public interface IStudent1
    {
        string GetName(string a);
        ValueTask<bool> GetBool(int a);
        Task<string> GetString(int a, string b);
    }
}
