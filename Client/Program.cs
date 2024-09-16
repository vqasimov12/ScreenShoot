using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;


var client = new UdpClient();
var ep = new IPEndPoint(IPAddress.Loopback, 27001);
var Timer = new System.Timers.Timer();
var path = "Image.png";

void ScreenShot(object? source, ElapsedEventArgs? e)
{
    object a = new();
    lock (a)
    {
        try
        {
            using (Bitmap bitmap = new Bitmap(1920, 1080))
            {
                using Graphics g = Graphics.FromImage(bitmap);
                g.CopyFromScreen(Point.Empty, Point.Empty, new Size(1920, 1080));
                bitmap.Save(path, ImageFormat.Png);
            }

            byte[] bytes;
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                int bufferSize = 1024;
                byte[] buffer = new byte[bufferSize];
                int bytesRead;

                while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    client.Send(buffer, bytesRead, ep);
                }

                byte endMarker = 0x00;
                client.Send(new byte[] { endMarker }, 1, ep);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}


_ = Task.Run(() =>
{
    var endPoint = new IPEndPoint(IPAddress.Any, 0);
    var reciever = new UdpClient(27000);
    while (true)
    {
        try
        {
            var bytes = reciever.Receive(ref endPoint);
            var str = Encoding.UTF8.GetString(bytes);
            if (str == "Start")
            {
                Timer.Stop();
                Timer.Elapsed -= ScreenShot;
                Timer.Elapsed += ScreenShot;
                Timer.Interval = 3000;
                Timer.AutoReset = true;
                Timer.Start();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
});
Console.ReadKey();
