using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Sockets;
using System.Timers;


var client = new UdpClient();
var ep = new IPEndPoint(IPAddress.Loopback, 27001);
var Timer = new System.Timers.Timer();
var path = "Image.png";
_ = Task.Run(() =>
{
    Timer.Interval = 3000;
    Timer.Elapsed += ScreenShot;
    Timer.AutoReset = true;
    Timer.Enabled = true;
    Timer.Start();
});
void ScreenShot(object? source, ElapsedEventArgs? e)
{
    using (Bitmap bitmap = new Bitmap(1920, 1080))
    {
        using Graphics g = Graphics.FromImage(bitmap);
        g.CopyFromScreen(Point.Empty, Point.Empty, new Size(1920, 1080));
        bitmap.Save(path, ImageFormat.Png);
       
    }
}

while (true)
{
    try
    {
        if (!File.Exists(path))
            continue;
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        int len = 22;
        var bytes = new byte[len];
        var fileSize = fs.Length;
        do
        {
            len = fs.Read(bytes, 0, len);
            client.Send(bytes, ep);
        } while (len > 0);
        File.Delete(path);
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }
}
