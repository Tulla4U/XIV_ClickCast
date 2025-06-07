using System.IO;
using ClickCast.Windows;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace ClickCast;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService]
    internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;

    [PluginService]
    internal static ICommandManager CommandManager { get; private set; } = null!;

    [PluginService]
    internal static IPluginLog Log { get; private set; } = null!;

    [PluginService]
    internal static IClientState ClientState { get; private set; } = null!;

    [PluginService]
    internal static IPartyList PartyList { get; private set; } = null!;

    [PluginService]
    public static IObjectTable ObjectTable { get; private set; } = null!;

    private const string CommandName = "/cc";
    private const string StalkerWindow = "/stalk";
    private const string ActionAssignement = "/ccas";
    private const string Config = "/cccfg";

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("SamplePlugin");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }
    private ClickCastWindow ClickCastWindow { get; init; }
    private ActionAssignmentWindow ActionAssignmentWindow { get; init; }

    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        // you might normally want to embed resources and load them from the manifest stream
        // var goatImagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");

        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this);
        ClickCastWindow = new ClickCastWindow(this);
        ActionAssignmentWindow = new(this);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);
        WindowSystem.AddWindow(ClickCastWindow);
        WindowSystem.AddWindow(ActionAssignmentWindow);

        ClickCastWindow.ActionAssigmentWindowToggle += ToggleActionAssignementUi;

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Show ClickCast window"
        });

        CommandManager.AddHandler(StalkerWindow, new CommandInfo(OnCommand)
        {
            HelpMessage = "Stalker Window"
        });
        CommandManager.AddHandler(ActionAssignement, new CommandInfo(OnCommand)
        {
            HelpMessage = "Configure assigned Actions for Click Casting"
        });        
        CommandManager.AddHandler(Config, new CommandInfo(OnCommand)
        {
            HelpMessage = "Configure assigned Actions for Click Casting"
        });

        PluginInterface.UiBuilder.Draw += DrawUi;

        // This adds a button to the plugin installer entry of this plugin which allows
        // to toggle the display status of the configuration ui
        PluginInterface.UiBuilder.OpenConfigUi += ToggleActionAssignementUi;

        // Adds another button that is doing the same but for the main ui of the plugin
        PluginInterface.UiBuilder.OpenMainUi += ToggleClickCastUi;

        // Add a simple message to the log with level set to information
        // Use /xllog to open the log window in-game
        // Example Output: 00:57:54.959 | INF | [SamplePlugin] ===A cool log message from Sample Plugin===
        Log.Information($"===A cool log message from {PluginInterface.Manifest.Name}===");
        
    }
    

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();
        ClickCastWindow.Dispose();
        ActionAssignmentWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);
        CommandManager.RemoveHandler(StalkerWindow);
    }

    private void OnCommand(string command, string args)
    {
        // in response to the slash command, just toggle the display status of our main ui
        switch (command)
        {
            case StalkerWindow:
                ToggleStalkerUi();
                break;
            case CommandName:
                ToggleClickCastUi();
                break;
            case ActionAssignement:
                ToggleActionAssignementUi();
                break;
            case  Config:
                ToggleConfigUi();
                break;
        }
    }

    private void DrawUi() => WindowSystem.Draw();

    public void ToggleConfigUi() => ConfigWindow.Toggle();
    public void ToggleStalkerUi() => MainWindow.Toggle();
    public void ToggleClickCastUi() => ClickCastWindow.Toggle();
    public void ToggleActionAssignementUi() => ActionAssignmentWindow.Toggle();
}
