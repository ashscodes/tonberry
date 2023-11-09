using Tonberry.Core.Model;

namespace Tonberry.Core;

public static class TaskExtensions
{
    public static TonberryResult Invoke(this TonberrySingleTask task) => task.Run();

    public static TonberryResultCollection Invoke(this TonberryMultiTask task) => task.Run();
}