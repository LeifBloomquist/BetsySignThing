using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetsySignThing
{
  class Ball
  {
    public Point Location = new Point(0, 0);

    private int dx = 1;
    private int dy = 1;

    public Color myColor { get; private set; }

    public Ball(int startx, int starty, Color c)
    {
      Location = new Point(startx, starty);
      myColor = c;
    }

    public void Move(int bound_width, int bound_height)
    {
      Location.X += dx;
      Location.Y += dy;

      if ((Location.X >= (bound_width - dx))  || (Location.X == 0)) { dx = -dx; }
      if ((Location.Y >= (bound_height - dy)) || (Location.Y == 0)) { dy = -dy; }
    }
  }
}
