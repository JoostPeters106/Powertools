using System.Windows;

namespace DataverseToolbox;

public partial class ToolsPage : Window
{
    public ToolsPage(string orgName, string url)
    {
        InitializeComponent();
        ConnectedToText.Text = $"Connected to: {orgName}";
        ConnectedUrlText.Text = url;
    }
}
