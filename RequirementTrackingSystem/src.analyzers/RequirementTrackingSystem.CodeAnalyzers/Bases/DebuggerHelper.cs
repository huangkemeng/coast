using System.Diagnostics;

namespace RequirementTrackingSystem.CodeAnalyzer.Bases;

public static class DebuggerHelper
{
    public static bool HasStart;

    public static void DebuggerAttach()
    {
        if (!HasStart && !Debugger.IsAttached)
        {
            Debugger.Launch();
            HasStart = true;
        }
    }
}