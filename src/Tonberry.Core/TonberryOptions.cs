using Tonberry.Core.Model;

namespace Tonberry.Core
{
    public static class TonberryOptions
    {
        // If your commits have a different format to the usual semantic commit, you can define your own parser here.
        public static ICommitParser CommitParser { get; set; }

        // If your tags have a different format to the usual tag format, you can define your own parser here.
        public static ITagParser TagParser { get; set; }
    }
}