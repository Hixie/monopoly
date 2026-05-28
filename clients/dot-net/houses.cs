// -*- Mode: Java -*-

using System;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Forms;

public class Houses : Control {

    protected int houses = 0;

    public Houses() : base() {
        SetStyle(ControlStyles.ContainerControl, false); 
        SetStyle(ControlStyles.ResizeRedraw, true);
        SetStyle(ControlStyles.UserPaint, true); 
    }

    private void PaintHouse(Graphics g, Rectangle r, Brush b) {
        g.FillPolygon(b, new Point[] {
            new Point(r.Left + r.Width / 2, r.Top),
            new Point(r.Right - 1, r.Top + r.Height / 2),
            new Point(r.Right - 1, r.Bottom),
            new Point(r.Left + 1, r.Bottom),
            new Point(r.Left + 1, r.Top + r.Height / 2),
        });
    }

    protected override void OnPaint(PaintEventArgs e) {
        base.OnPaint(e);
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.HighQuality;
        Rectangle r = ClientRectangle;
        r.Inflate(-1, -1);
        if (houses < 5) {
            r.Width /= 4;
            for (int i = 0; i < houses; ++i) {
                PaintHouse(g, r, Brushes.Green);
                r.Offset(r.Width, 0);
            }
        } else {
            r.Inflate(Convert.ToInt32(-r.Width * 0.3F), 0);
            PaintHouse(g, r, Brushes.Maroon);
        }
    }

    public void SetHouses(int a, int b) {
        if (a + b >= 0 && a + b <= 5) {
            houses = a + b;
            Invalidate();
        }
    }

    public int Value {
        get { return houses; }
        set {
            if (value >= 0 && value <= 5) {
                houses = value;
                Invalidate();
            }
        }
    }

}
