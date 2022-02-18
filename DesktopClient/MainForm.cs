using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net.WebSockets;
using MyWebSocketProtocol.Client;

namespace DesktopClient
{
    public partial class MainForm : Form
    {
        private readonly MyWebSocketClient myConnection;

        public MainForm()
        {
            InitializeComponent();

            myConnection = new MyWebSocketClient();

            myConnection.On<Message>("Send", message =>
            {
                this.Invoke((Action)(() =>
                {
                    AppendTextToTextBox("Anonymous", message.Text, Color.Black);
                }));
            });
        }

        private async void connectButton_Click(object sender, EventArgs e)
        {
            if (myConnection.State == WebSocketState.None)
            {
                try
                {
                    await myConnection.ConnectAsync("ws://localhost:57785/messages");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                if (myConnection.State == WebSocketState.Open)
                {
                    connectButton.Text = "Disconnect";
                    stateLabelValue.ForeColor = Color.Green;
                    stateLabelValue.Text = "Connected";
                }
            }
            else if (myConnection.State == WebSocketState.Open)
            {
                await myConnection.StopAsync();
                connectButton.Text = "Connect";
                stateLabelValue.ForeColor = Color.Red;
                stateLabelValue.Text = "Disconnected";
            }
        }

        private async void sendButton_Click(object sender, EventArgs e)
        {
            if (myConnection.State == WebSocketState.Open)
            {
                var message = new Message
                {
                    Text = messageTextBox.Text
                };

                try
                {
                    await myConnection.SendAsync("SendToOthers", message);
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
    }
}