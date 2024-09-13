using System.Net.Sockets;
using System.Net;
using System.Windows;
using System.IO;

namespace Server;

public partial class MainWindow : Window
{
    string path = "image.png";
    UdpClient client = new();
    IPEndPoint ep = new(IPAddress.Any, 0);

    public MainWindow()
    {
        InitializeComponent();

        _ = Task.Run(() =>
        {

            while (true)
            {
                try
                {
                    var btyes = client.Receive(ref ep);
                   File.WriteAllBytes(path, btyes);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        });
    }




}