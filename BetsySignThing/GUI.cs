using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BetsySignThing
{
  public partial class GUI : Form
  {
    const int WIDTH = 162;
    const int HEIGHT = 108;
    const int UDP_FRAME_SIZE = 1490;
    const int IMAGE_SIZE = WIDTH * HEIGHT;

    System.Timers.Timer timer = new System.Timers.Timer();

    UdpClient c = new UdpClient(12345);

    public Color[,] Pixels = new Color[WIDTH, HEIGHT];

    Random rand = new Random();

    Ball red = new Ball(5, 50, Color.Red);
    Ball green = new Ball(25, 10, Color.FromArgb(0,255,0));
    Ball blue = new Ball(125, 100, Color.FromArgb(0, 0, 255));

    List<Ball> Balls = new List<Ball>();

    public GUI()
    {
      InitializeComponent();

      BetsyPictureBox.Width = WIDTH;
      BetsyPictureBox.Height = HEIGHT;
      BetsyPictureBox.Image = new System.Drawing.Bitmap(WIDTH, HEIGHT);

      Balls.Add(red);
      Balls.Add(green);
      Balls.Add(blue);

      for (int i = 0; i < 10; i++ )
      {
        Ball temp = new Ball(rand.Next(WIDTH), rand.Next(HEIGHT),
                             Color.FromArgb(rand.Next(255), rand.Next(255), rand.Next(255)));
        Balls.Add(temp);
      }


        timer.Interval = 20;
      timer.Elapsed += Timer_Tick;
      timer.Enabled = true;
    }

    Stopwatch watch = new Stopwatch();

    private void Timer_Tick(object sender, System.Timers.ElapsedEventArgs e)
    {
      AnimateBall();
      FadePixels();

      /*
      for (int x = 0; x < WIDTH; x++)
      {
        for (int y = 0; y < HEIGHT; y++)
        {
          Pixels[x, y] = Color.FromArgb((byte)rand.Next(255),
                                   (byte)rand.Next(255),
                                   (byte)rand.Next(255));
        }
      }
       * */

      //Display(Pixels);
      watch.Restart();
      Transmit(Pixels);
      //Transmit(new Bitmap(BetsyPictureBox.Image));

      try
      {
        this.Invoke(new Action(() =>
        {
          TimerLabel.Text = "Elapsed: " + watch.ElapsedMilliseconds;
        }));
      }
      catch (Exception ex)
      {
        // Ignore silently
      }

      watch.Stop();
    }

    private void FadePixels()
    {
      for (int x = 0; x < WIDTH; x++)
      {
        for (int y = 0; y < HEIGHT; y++)
        {
          int R = (int)(Pixels[x, y].R * 0.9);
          int G = (int)(Pixels[x, y].G * 0.9);
          int B = (int)(Pixels[x, y].B * 0.9);

          Pixels[x, y] = Color.FromArgb(R, G, B);
        }
      }
    }

    private void AnimateBall()
    {
      foreach (Ball b in Balls)
      {
        b.Move(WIDTH, HEIGHT);
        Pixels[b.Location.X, b.Location.Y] = b.myColor;
      }
    }

    private void Display(Color[,] Pixels)
    {
      Bitmap bmp = (Bitmap)BetsyPictureBox.Image;
      //var g = Graphics.FromImage(bmp);

      lock (bmp)
      {
        for (int x = 0; x < WIDTH; x++)
        {
          for (int y = 0; y < HEIGHT; y++)
          {
            bmp.SetPixel(x, y, Pixels[x, y]);
          }
        }
      }

      try
      {
        this.Invoke(new Action(() =>
        {
          BetsyPictureBox.Refresh();
        }));
      }
      catch (Exception e)
      {
          // Ignore silently
      }
    }

    [Obsolete]
    private void Transmit_TooFancy(Color[,] Pixels)
    {
      byte[] rgb_bytes = Flatten(Pixels);
      byte[] packet = new byte[1500];

      int num_bytes = rgb_bytes.Length;
      int num_packets = num_bytes / UDP_FRAME_SIZE;

      int offset = 0;

      for (int packet_num = 1; packet_num <= num_packets; packet_num++)
      {
        packet[0] = 0x9C;   // TMP2.NET packet start
        packet[1] = 0xDA;   // Data
        packet[2] = Utilities.GetHighByte(UDP_FRAME_SIZE);
        packet[3] = Utilities.GetLowByte(UDP_FRAME_SIZE);
        packet[4] = (byte)packet_num;
        packet[5] = (byte)num_packets;

        // User data
        for (int i = 0; i < UDP_FRAME_SIZE; i++)
        {
          packet[6 + i] = rgb_bytes[offset++];
        }

        SendUdp("192.168.111.69", 65506, packet);
      }
    }

    private void Transmit(Bitmap bmp)
    {
      for (int x = 0; x < WIDTH; x++)
      {
        for (int y = 0; y < HEIGHT; y++)
        {
          //Pixels[x, y] = Color.FromArgb(0, 0, 100); // bmp.GetPixel(x, y);
          Pixels[x, y] = bmp.GetPixel(x, y);
        }
      }

      Transmit(Pixels);
    }

    private void Transmit(Color[,] Pixels)
    {
      byte[] rgb_bytes = Flatten(Pixels);
      int num_bytes = rgb_bytes.Length;

      byte[] packet = new byte[num_bytes+7];      

      int offset = 0;

      packet[0] = 0x9C;   // TMP2.NET packet start
      packet[1] = 0xDA;   // Data
      packet[2] = Utilities.GetHighByte(num_bytes);
      packet[3] = Utilities.GetLowByte(num_bytes);
      packet[4] = 0;
      packet[5] = 0;

      // User data
      for (int i = 0; i < num_bytes; i++)
      {
        packet[6 + i] = rgb_bytes[offset++];
      }
     // packet[num_bytes] = 0x36;

      SendUdp("192.168.111.69", 65506, packet);
    }

    void SendUdp(string dstIp, int dstPort, byte[] data)
    {
        c.Send(data, data.Length, dstIp, dstPort);
    }

    private byte[] Flatten(Color[,] Pixels)
    {
      byte[] flatrgb = new byte[Pixels.Length * 3];
      int offset = 0;

      for (int y = 0; y < HEIGHT; y++)
      {
        for (int x = 0; x < WIDTH; x++)
        { 
          flatrgb[offset++] = Pixels[x, y].R;
          flatrgb[offset++] = Pixels[x, y].G;
          flatrgb[offset++] = Pixels[x, y].B;
        }
      }

      return flatrgb;
    }
  }
}