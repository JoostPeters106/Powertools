using System.Windows;
using DataverseToolbox.Services;
using Microsoft.PowerPlatform.Dataverse.Client;

namespace DataverseToolbox;

public partial class AddConnectionDialog : Window
{
    private readonly ConnectionManager _connectionManager;

    public AddConnectionDialog(ConnectionManager connectionManager)
    {
        InitializeComponent();
        _connectionManager = connectionManager;
    }

    private async void Connect_Click(object sender, RoutedEventArgs e)
    {
        var urlText = UrlTextBox.Text.Trim();

        if (!Uri.TryCreate(urlText, UriKind.Absolute, out var uriResult)
            || (uriResult.Scheme != Uri.UriSchemeHttps && uriResult.Scheme != Uri.UriSchemeHttp))
        {
            MessageBox.Show("Please enter a valid Dataverse URL.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            ConnectButton.IsEnabled = false;
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            var service = await ConnectToDataverseAsync(uriResult);
            if (service is null)
            {
                return;
            }

            var orgName = string.IsNullOrWhiteSpace(service.ConnectedOrgFriendlyName)
                ? uriResult.Host
                : service.ConnectedOrgFriendlyName;

            _connectionManager.SaveConnection(uriResult.ToString(), orgName, service.ConnectedOrgId.ToString());

            MessageBox.Show($"Connected to {orgName}.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            ConnectButton.IsEnabled = true;
            Mouse.OverrideCursor = null;
        }
    }

    private static async Task<ServiceClient?> ConnectToDataverseAsync(Uri dataverseUri)
    {
        return await Task.Run(() =>
        {
            var service = new ServiceClient(instanceUrl: dataverseUri, useUniqueInstance: true);
            if (service.IsReady)
            {
                return service;
            }

            MessageBox.Show($"Connection failed: {service.LastError}", "Connection Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            return null;
        });
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
