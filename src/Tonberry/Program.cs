using System.Threading.Tasks;

namespace Tonberry;

internal class Program
{
    internal static async Task<int> Main(string[] args)
    {
        return await Task.FromResult(args.Length);
    }
}