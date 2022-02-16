using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Drawing;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Mime;
using Microsoft.AspNetCore.WebUtilities;

namespace DesktopClient
{
    public partial class MainForm : Form
    {
        private HubConnection hubConnection;
        private string token = string.Empty;

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitConnection()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl($"http://localhost:57785/messages?token={token}")
                .WithAutomaticReconnect()
                .Build();

            hubConnection.On<NewMessage>("Send", message =>
            {
                AppendTextToTextBox(message.Sender, message.Text, Color.Black);
            });

            hubConnection.Closed += error =>
            {
                MessageBox.Show($"Connection closed. {error?.Message}");
                return Task.CompletedTask;
            };

            hubConnection.Reconnected += id =>
            {
                MessageBox.Show($"Connection reconnected with id: {id}");
                return Task.CompletedTask;
            };

            hubConnection.Reconnecting += error =>
            {
                MessageBox.Show($"Connection reconnecting. {error?.Message}");
                return Task.CompletedTask;
            };
        }

        private async void connectButton_Click(object sender, EventArgs e)
        {
            if (hubConnection == null)
                InitConnection();

            if (hubConnection.State == HubConnectionState.Disconnected)
            {
                try
                {
                    await hubConnection.StartAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                if (hubConnection.State == HubConnectionState.Connected)
                {
                    connectButton.Text = "Disconnect";
                    stateLabelValue.ForeColor = Color.Green;
                    stateLabelValue.Text = "Connected";
                }
            }
            else if (hubConnection.State == HubConnectionState.Connected)
            {
                await hubConnection.StopAsync();
                connectButton.Text = "Connect";
                stateLabelValue.ForeColor = Color.Red;
                stateLabelValue.Text = "Disconnected";
            }
        }

        private async void sendButton_Click(object sender, EventArgs e)
        {
            if (hubConnection.State == HubConnectionState.Connected)
            {
                var message = new Message
                {
                    Text = messageTextBox.Text
                };

                try
                {
                    await hubConnection.InvokeAsync("SendToOthers", message);
                    AppendTextToTextBox("Me", message.Text, Color.Green);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    messageTextBox.Clear();
                }
            }
        }

        private void AppendTextToTextBox(string sender, string text, Color color)
        {
            chatTextBox.SelectionStart = chatTextBox.TextLength;
            chatTextBox.SelectionLength = 0;
            chatTextBox.SelectionColor = color;
            chatTextBox.AppendText(string.Format("Author: {0}{2}Text: {1}{2}{2}", sender, text, Environment.NewLine));
            chatTextBox.SelectionColor = chatTextBox.ForeColor;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var token = await GetToken();

            if (!string.IsNullOrEmpty(token))
            {
                this.token = token;
                InitConnection();

                var tokenParts = token.Split('.');
                var decodedToken = new StringBuilder();
                for (int i = 0; i < 2; i++)
                {
                    var tokenBytes = WebEncoders.Base64UrlDecode(tokenParts[i]);
                    var decodedPart = Encoding.UTF8.GetString(tokenBytes);
                    decodedToken.AppendLine(decodedPart.PrettifyJsonString());
                }
                decodedToken.AppendLine(tokenParts[2]);

                MessageBox.Show(decodedToken.ToString());
            }
        }

        private async Task<string> GetToken()
        {
            using var httpClient = new HttpClient();

            var authModel = new { Login = nameTextBox.Text, Password = passwordTextBox.Text };

            var json = JsonSerializer.Serialize(authModel);

            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

            var response = await httpClient.PostAsync("http://localhost:57785/api/auth/token", content);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                MessageBox.Show(response.StatusCode.ToString());
                return string.Empty;
            }
        }
    }
}