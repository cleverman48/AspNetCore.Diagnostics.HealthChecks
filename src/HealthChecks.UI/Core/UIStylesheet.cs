using HealthChecks.UI.Configuration;

namespace HealthChecks.UI.Core;

public class UIStylesheet
{
    private const string STYLESHEETS_PATH = "css";

    public string FileName { get; }
    public byte[] Content { get; }
    public string ResourcePath { get; }

    private UIStylesheet(Options options, string filePath)
    {
        FileName = Path.GetFileName(filePath);
        Content = File.ReadAllBytes(filePath);
        ResourcePath = $"{options.ResourcesPath}/{STYLESHEETS_PATH}/{FileName}";
    }

    public static UIStylesheet Create(Options options, string filePath)
    {
        return new UIStylesheet(options, filePath);
    }
}
