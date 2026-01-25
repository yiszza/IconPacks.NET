using System.Text;
using CliWrap;
using CliWrap.Buffered;
using IconPacksGenerator;
using IconPacksGenerator.Generators;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Icons initializing...");
        await InitIcons();

        Console.WriteLine();
        Console.WriteLine("Icons updating...");

        await UpdateIcons();

        Console.WriteLine();
        Console.WriteLine("Generator running...");

        await RunIconsGenerator();

        Console.WriteLine();
        Console.WriteLine("Packs building...");

        await BuildIconPacks();

        Console.WriteLine();
        Console.WriteLine("Done!");
    }

    private static async Task InitIcons()
    {
        await InitIcons(
            Paths.FeatherIconPath,
            "https://github.com/feathericons/feather.git",
            "main",
            "icons/"
        );
        await InitIcons(
            Paths.FontAwesomeIconPath,
            "https://github.com/FortAwesome/Font-Awesome.git",
            "7.x",
            "svgs-full/"
        );
        await InitIcons(
            Paths.IonicIconPath,
            "https://github.com/ionic-team/ionicons.git",
            "main",
            "src/svg/"
        );
        await InitIcons(
            Paths.MaterialIconPath,
            "https://github.com/google/material-design-icons.git",
            "master",
            "src/"
        );
        await InitIcons(
            Paths.MaterialCommunityIconPath,
            "https://github.com/Templarian/MaterialDesign.git",
            "master",
            "svg/"
        );
        await InitIcons(
            Paths.TablerIconPath,
            "https://github.com/tabler/tabler-icons.git",
            "main",
            "icons/"
        );
        await InitIcons(
            Paths.FluentIconPath,
            "https://github.com/microsoft/fluentui-system-icons.git",
            "main",
            "assets/"
        );
        await InitIcons(
            Paths.RemixIconPath,
            "https://github.com/Remix-Design/RemixIcon.git",
            "master",
            "icons/"
        );
        await InitIcons(
            Paths.HeroIconPath,
            "https://github.com/tailwindlabs/heroicons.git",
            "master",
            "src/24/"
        );
        await InitIcons(
            Paths.LucideIconPath,
            "https://github.com/lucide-icons/lucide.git",
            "main",
            "icons/"
        );
    }

    private static async Task InitIcons(
        string workPath,
        string remote,
        string branch,
        string sparseCheckout
    )
    {
        if (!Directory.Exists(workPath))
        {
            Directory.CreateDirectory(workPath);
        }
        if (!Directory.Exists(Path.Combine(workPath, ".git")))
        {
            await Cli.Wrap("git")
                .WithWorkingDirectory(workPath)
                .WithArguments("init")
                .ExecuteBufferedAsync();
            await Cli.Wrap("git")
                .WithWorkingDirectory(workPath)
                .WithArguments($"remote add remote {remote}")
                .WithStandardOutputPipe(PipeTarget.ToDelegate(Console.WriteLine, Encoding.UTF8))
                .ExecuteBufferedAsync();
            await Cli.Wrap("git")
                .WithWorkingDirectory(workPath)
                .WithArguments("config core.sparsecheckout true")
                .WithStandardOutputPipe(PipeTarget.ToDelegate(Console.WriteLine, Encoding.UTF8))
                .ExecuteBufferedAsync();
            File.WriteAllText(Path.Combine(workPath, ".git/info/sparse-checkout"), sparseCheckout);
            await Cli.Wrap("git")
                .WithWorkingDirectory(workPath)
                .WithArguments($"pull remote {branch}:master")
                .WithStandardOutputPipe(PipeTarget.ToDelegate(Console.WriteLine, Encoding.UTF8))
                .ExecuteBufferedAsync();
        }
    }

    private static async Task UpdateIcons()
    {
        await UpdateIcons(Paths.FeatherIconPath, "main", "icons/");
        await UpdateIcons(Paths.FontAwesomeIconPath, "7.x", "svgs-full/");
        await UpdateIcons(Paths.IonicIconPath, "main", "src/svg/");
        await UpdateIcons(Paths.MaterialIconPath, "master", "src/");
        await UpdateIcons(Paths.MaterialCommunityIconPath, "master", "svg/");
        await UpdateIcons(Paths.TablerIconPath, "main", "icons/");
        await UpdateIcons(Paths.FluentIconPath, "main", "assets/");
        await UpdateIcons(Paths.RemixIconPath, "master", "icons/");
        await UpdateIcons(Paths.HeroIconPath, "master", "src/24/");
        await UpdateIcons(Paths.LucideIconPath, "main", "icons/");
    }

    private static async Task UpdateIcons(string workPath, string branch, string sparseCheckout)
    {
        if (Directory.Exists(Path.Combine(workPath, ".git")))
        {
            await Cli.Wrap("git")
                .WithWorkingDirectory(workPath)
                .WithArguments("config core.sparsecheckout true")
                .WithStandardOutputPipe(PipeTarget.ToDelegate(Console.WriteLine, Encoding.UTF8))
                .ExecuteBufferedAsync();
            File.WriteAllText(Path.Combine(workPath, ".git/info/sparse-checkout"), sparseCheckout);
            await Cli.Wrap("git")
                .WithWorkingDirectory(workPath)
                .WithArguments($"pull remote {branch}:master")
                .WithStandardOutputPipe(PipeTarget.ToDelegate(Console.WriteLine, Encoding.UTF8))
                .ExecuteBufferedAsync();
        }
    }

    private static async Task RunIconsGenerator()
    {
        await FeatherGenerator.RunAsync();
        await FontAwesomeGenerator.RunAsync();
        await IonicGenerator.RunAsync();
        await MaterialGenerator.RunAsync();
        await MaterialCommunityGenerator.RunAsync();
        await TablerGenerator.RunAsync();
        await FluentGenerator.RunAsync();
        await RemixGenerator.RunAsync();
        await HeroGenerator.RunAsync();
        await LucideGenerator.RunAsync();
    }

    private static async Task BuildIconPacks()
    {
        if (!Directory.Exists(Paths.PacksPath))
            Directory.CreateDirectory(Paths.PacksPath);

        foreach (var pack in Directory.EnumerateFiles(Paths.PacksPath, "*.nupkg"))
        {
            File.Delete(pack);
        }

        await Cli.Wrap("dotnet")
            .WithWorkingDirectory(Paths.RootPath)
            .WithArguments("pack ./IconPacks.Feather -c release -o ./packs")
            .WithStandardOutputPipe(PipeTarget.ToDelegate(Console.WriteLine, Encoding.UTF8))
            .ExecuteBufferedAsync();
        await Cli.Wrap("dotnet")
            .WithWorkingDirectory(Paths.RootPath)
            .WithArguments("pack ./IconPacks.FontAwesome -c release -o ./packs")
            .WithStandardOutputPipe(PipeTarget.ToDelegate(Console.WriteLine, Encoding.UTF8))
            .ExecuteBufferedAsync();
        await Cli.Wrap("dotnet")
            .WithWorkingDirectory(Paths.RootPath)
            .WithArguments("pack ./IconPacks.Ionic -c release -o ./packs")
            .WithStandardOutputPipe(PipeTarget.ToDelegate(Console.WriteLine, Encoding.UTF8))
            .ExecuteBufferedAsync();
        await Cli.Wrap("dotnet")
            .WithWorkingDirectory(Paths.RootPath)
            .WithArguments("pack ./IconPacks.Material -c release -o ./packs")
            .WithStandardOutputPipe(PipeTarget.ToDelegate(Console.WriteLine, Encoding.UTF8))
            .ExecuteBufferedAsync();
        await Cli.Wrap("dotnet")
            .WithWorkingDirectory(Paths.RootPath)
            .WithArguments("pack ./IconPacks.MaterialCommunity -c release -o ./packs")
            .WithStandardOutputPipe(PipeTarget.ToDelegate(Console.WriteLine, Encoding.UTF8))
            .ExecuteBufferedAsync();
        await Cli.Wrap("dotnet")
            .WithWorkingDirectory(Paths.RootPath)
            .WithArguments("pack ./IconPacks.Tabler -c release -o ./packs")
            .WithStandardOutputPipe(PipeTarget.ToDelegate(Console.WriteLine, Encoding.UTF8))
            .ExecuteBufferedAsync();
        await Cli.Wrap("dotnet")
            .WithWorkingDirectory(Paths.RootPath)
            .WithArguments("pack ./IconPacks.Fluent -c release -o ./packs")
            .WithStandardOutputPipe(PipeTarget.ToDelegate(Console.WriteLine, Encoding.UTF8))
            .ExecuteBufferedAsync();
        await Cli.Wrap("dotnet")
            .WithWorkingDirectory(Paths.RootPath)
            .WithArguments("pack ./IconPacks.Hero -c release -o ./packs")
            .WithStandardOutputPipe(PipeTarget.ToDelegate(Console.WriteLine, Encoding.UTF8))
            .ExecuteBufferedAsync();
        await Cli.Wrap("dotnet")
            .WithWorkingDirectory(Paths.RootPath)
            .WithArguments("pack ./IconPacks.Lucide -c release -o ./packs")
            .WithStandardOutputPipe(PipeTarget.ToDelegate(Console.WriteLine, Encoding.UTF8))
            .ExecuteBufferedAsync();
        await Cli.Wrap("dotnet")
            .WithWorkingDirectory(Paths.RootPath)
            .WithArguments("pack ./IconPacks.Remix -c release -o ./packs")
            .WithStandardOutputPipe(PipeTarget.ToDelegate(Console.WriteLine, Encoding.UTF8))
            .ExecuteBufferedAsync();

        //var newNupkgs = Directory.EnumerateFiles(
        //    Paths.RootPath,
        //    "*.nupkg",
        //    SearchOption.AllDirectories
        //);

        //foreach (var nupkg in newNupkgs)
        //{
        //    await Cli.Wrap("dotnet")
        //        .WithStandardOutputPipe(PipeTarget.ToDelegate(Console.WriteLine, Encoding.UTF8))
        //        .WithArguments(
        //            $"nuget push {nupkg} --api-key {apiKey} --source https://api.nuget.org/v3/index.json"
        //        )
        //        .ExecuteBufferedAsync();
        //}
    }
}
