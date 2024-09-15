using System.Net.Sockets;
using System.Net;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Media.Imaging;
using System.Text;

namespace Server;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    string path = "image.png";
    UdpClient client = new(27001);
    IPEndPoint ep = new(IPAddress.Any, 0);
    private string imagePath;

    public string ImagePath { get => imagePath; set { imagePath = value; OnPropertyChanged(); } }
    public MainWindow()
    {

        InitializeComponent();
        DataContext = this;

        var segment = new List<byte>();
        _ = Task.Run(() =>
        {
            while (true)
            {
                try
                {
                    byte endMarker = 0x00;
                    var bytes = client.Receive(ref ep);
                    if (bytes.Length == 1 && bytes[0] == endMarker)
                    {
                        File.WriteAllBytes(path, segment.ToArray());
                        segment.Clear();
                    }
                    else
                        segment.AddRange(bytes);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        });
    }

    private void ChageImage(object? sender, System.Timers.ElapsedEventArgs e)
    {
        ImagePath = null;
        ImagePath = "image.png";
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        try
        {

            var UdpSender = new UdpClient(27000);
            var ep = new IPEndPoint(IPAddress.Loopback, 27000);
            var bytes = Encoding.UTF8.GetBytes("Start");
            UdpSender.Send(bytes, bytes.Length, ep);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}