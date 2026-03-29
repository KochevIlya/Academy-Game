using UnityEditor;
using UnityEditor.Build.Reporting;

public class BuildScript
{
    private static readonly string[] scenes = { "Assets/_Project/Scenes/GameScene.unity" };
    private const string BuildDir = "QwenBuilds";

    [MenuItem("Tools/Build/Windows")]
    public static void BuildWindows()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = $"{BuildDir}/AcademyGame.exe",
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        UnityEngine.Debug.Log($"Build completed: {report.summary.result}");
    }

    [MenuItem("Tools/Build/Linux")]
    public static void BuildLinux()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = $"{BuildDir}/AcademyGame.x86_64",
            target = BuildTarget.StandaloneLinux64,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        UnityEngine.Debug.Log($"Build completed: {report.summary.result}");
    }
}
