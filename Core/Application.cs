#if GODOT4_0_OR_GREATER
namespace Cutulu.Core;

using Godot;

public static partial class Application
{
    /// <summary>
    /// Define new title for application window
    /// </summary>
    public static void SetWindowTitle(string newTitle, int windowId = 0) => DisplayServer.WindowSetTitle(newTitle, windowId);

    /// <summary>
    /// Set or Get value of Clipboard
    /// </summary>
    public static string Clipboard
    {
        set => DisplayServer.ClipboardSet(value);
        get => DisplayServer.ClipboardGet();
    }

    /// <summary>
    /// Returns true if running in headless mode
    /// </summary>
    public static bool IsHeadless
    {
        get => DisplayServer.GetName() == "headless";
    }

    /// <summary>
    /// Returns true if any --script-args were provided
    /// <para>Schema: "command-to-start-application" --script-args "arg1 arg2 arg3"</para>
    /// </summary>
    public static bool TryGetCmdLineArgs(out string[] _args)
    {
        // Get all command line arguments
        return (_args = OS.GetCmdlineArgs()).NotEmpty();
    }

    /// <summary>
    /// Returns any --script-args if provided
    /// <para>Schema: "command-to-start-application" --script-args "arg1 arg2 arg3"</para>
    /// </summary>
    public static string[] GetCmdLineArgs() => TryGetCmdLineArgs(out var args) ? args ?? [] : [];
}
#endif