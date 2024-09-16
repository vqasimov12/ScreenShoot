using System.Net.Sockets;
using System.Net;
using System.Windows;
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using System.Text;

namespace Server;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    string path = "image.png";
    UdpClient client = new(27001);
    IPEndPoint ep = new(IPAddress.Any, 0);
    private string imagePath;
    public string ImagePath { get => imagePath; set { imagePath = value; OnPropertyChanged(); } }
    DispatcherTimer _timer;
    public MainWindow()
    {

        InitializeComponent();
        DataContext = this;
        var segment = new List<byte>();
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(0.5)
        };
        _timer.Tick += ChangeImage;
        _timer.Start();
        _ = Task.Run(() =>
        {
            byte endMarker = 0x00;
            try
            {
                //var prevPath = path;
                //path = $"{Guid.NewGuid()}.png";
                //if (File.Exists(prevPath))
                //    File.Delete(prevPath);

                while (true)
                {
                    var bytes = client.Receive(ref ep);
                    if (bytes.Length == 1 && bytes[0] == endMarker)
                    {
                        using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                        fileStream.Write(segment.ToArray(), 0, segment.Count);
                        segment.Clear();

                    }
                    else
                        segment.AddRange(bytes);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        });

    }

    private void ChangeImage(object? sender, EventArgs e)
    {
        //ImagePath = path;
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
            var client = new UdpClient();
            var connectEp = new IPEndPoint(IPAddress.Loopback, 27000);
            var bytes = Encoding.UTF8.GetBytes("Start");
            client.Send(bytes, bytes.Length, connectEp);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}