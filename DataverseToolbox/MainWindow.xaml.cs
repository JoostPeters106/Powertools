using System.Windows;
using DataverseToolbox.Models;
using DataverseToolbox.Services;
using Microsoft.PowerPlatform.Dataverse.Client;

namespace DataverseToolbox;

public partial class MainWindow : Window
{
    private readonly ConnectionManager _connectionManager = new();

    public MainWindow()
    {
        InitializeComponent();
        RefreshConnections();
    }

    private void RefreshConnections()
    {
        ConnectionsList.ItemsSource = _connectionManager.LoadConnections()
            .OrderByDescending(c => c.LastUsed)
            .ToList();
    }

    private void AddConnection_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new AddConnectionDialog(_connectionManager)
        {
            Owner = this
        };

        var result = dialog.ShowDialog();
        if (result == true)
        {
            RefreshConnections();
        }
    }

    private async void ConnectSelected_Click(object sender, RoutedEventArgs e)
    {
        await ConnectSelectedAsync();
    }

    private async void ConnectionsList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        await ConnectSelectedAsync();
    }

    private async Task ConnectSelectedAsync()
    {
        if (ConnectionsList.SelectedItem is not DataverseConnection selectedConnection)
        {
            MessageBox.Show("Please select a connection first.", "No Connection Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        try
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            var service = await Task.Run(() => new ServiceClient(
                instanceUrl: new Uri(selectedConnection.Url),
                useUniqueInstance: true));

            if (!service.IsReady)
            {
                MessageBox.Show($"Connection failed: {service.LastError}", "Connection Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _connectionManager.SaveConnection(selectedConnection.Url, service.ConnectedOrgFriendlyName, service.ConnectedOrgId.ToString());
            RefreshConnections();

            var toolsWindow = new ToolsPage(service.ConnectedOrgFriendlyName, selectedConnection.Url)
            {
                Owner = this
            };
            toolsWindow.ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            Mouse.OverrideCursor = null;
        }
    }
}
