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
    const int WIDTH = 160;
    const int HEIGHT = 108;
    const int FRAME_SIZE = 1490;

    System.Timers.Timer timer = new System.Timers.Timer();

    public Color[,] Pixels = new Color[WIDTH, HEIGHT];

    Random rand = new Random();

    public GUI()
    {
      InitializeComponent();

      BetsyPictureBox.Width = WIDTH;
      BetsyPictureBox.Height = HEIGHT;
      BetsyPictureBox.Image = new System.Drawing.Bitmap(WIDTH, HEIGHT);

      timer.Interval = 100;
      timer.Elapsed += Timer_Tick;
      timer.Enabled = true;
    }

    Stopwatch watch = new Stopwatch();

    private void Timer_Tick(object sender, System.Timers.ElapsedEventArgs e)
    {
      for (int x = 0; x < WIDTH; x++)
      {
        for (int y = 0; y < HEIGHT; y++)
        {
          Pixels[x, y] = Color.FromArgb((byte)rand.Next(255),
                                   (byte)rand.Next(255),
                                   (byte)rand.Next(255));
        }
      }


      watch.Restart();
      Display(Pixels);
      Transmit(Pixels);

      this.Invoke(new Action(() =>
      {
        TimerLabel.Text = "Elapsed: " + watch.ElapsedMilliseconds;
      }));

      watch.Stop();
    }

    private void Display(Color[,] Pixels)
    {
      Bitmap bmp = (Bitmap)BetsyPictureBox.Image;
      //var g = Graphics.FromImage(bmp);

      for (int x = 0; x < WIDTH; x++)
      {
        for (int y = 0; y < HEIGHT; y++)
        {
          bmp.SetPixel(x, y, Pixels[x, y]);
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

    private void Transmit(Color[,] Pixels)
    {
      byte[] rgb_bytes = Flatten(Pixels);
      byte[] packet = new byte[1500];

      int num_bytes = rgb_bytes.Length;
      int num_packets = num_bytes / FRAME_SIZE;

      int offset = 0;

      for (int packet_num = 1; packet_num <= num_packets; packet_num++)
      {
        packet[0] = 0x9C;   // TMP2.NET packet start
        packet[1] = 0xDA;   // Data
        packet[2] = Utilities.GetHighByte(FRAME_SIZE);
        packet[3] = Utilities.GetLowByte(FRAME_SIZE);
        packet[4] = (byte)packet_num;
        packet[5] = (byte)num_packets;

        // User data
        for (int i = 0; i < FRAME_SIZE; i++)
        {
          packet[6 + i] = rgb_bytes[offset++];
        }

        SendUdp(12345, "127.0.0.1", 65506, packet);
      }
    }

    void SendUdp(int srcPort, string dstIp, int dstPort, byte[] data)
    {
      using (UdpClient c = new UdpClient(srcPort))
      {
        c.Send(data, data.Length, dstIp, dstPort);
      }
    }

    private byte[] Flatten(Color[,] Pixels)
    {
      byte[] flatrgb = new byte[Pixels.Length * 3];
      int offset = 0;

      for (int x = 0; x < WIDTH; x++)
      {
        for (int y = 0; y < HEIGHT; y++)
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