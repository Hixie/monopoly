// -*- Mode: Java -*-

using System;
using System.IO;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Forms;

public class MonopolyBoard : Control {

    // Dimensions
    protected float squareHeight = 0.125F;
    public float SquareHeight {
        get { return squareHeight; }
        set { squareHeight = value; UpdateCache(); }
    }

    protected float colorHeight = 0.2F;
    public float ColorHeight {
        get { return colorHeight; }
        set { colorHeight = value; UpdateCache(); }
    }

    protected float padding = 0.05F;
    public float Padding {
        get { return padding; }
        set { padding = value; UpdateCache(); }
    }

    public enum Angle { d0, d90, d180, d270 }
    protected Angle rotation = Angle.d0;
    public Angle Rotation {
        get { return rotation; }
        set { rotation = value; UpdateCache(); }
    }

    protected uint pot = 0;
    public uint Pot {
        get { return pot; }
        set { pot = value; UpdateCache(); }
    }

    protected int die1 = 0;
    protected int die2 = 0;
    protected int diceFrames = 0;
    public void SetDice(int die1, int die2) {
        this.die1 = die1;
        this.die2 = die2;
        if (AnimationEnabled) {
            diceFrames = 5;
            isAnimating = true;
            animationTimer.Start();
        } else
            Invalidate();
    }

    protected HybridDictionary pieces;
    protected class Piece {
        public int ID = 0;
        public Image Image = null;
        public int LastSquare;
        public bool LastInJail;
        public int LastPosition;
        public int CurrentSquare;
        public bool CurrentInJail;
        public int CurrentPosition; 
        public float Position;
        public float Distance;
        public Region Area;
        public class Pending { public int square; public bool jail; }
        public Queue Queue;
    }
    public void AddPiece(int id, Image image, int square, bool inJail) {
        if (pieces[id] != null) return; // already added
        Piece piece = new Piece();
        piece.ID = id;
        piece.Image = image;
        piece.CurrentSquare = square;
        piece.CurrentInJail = inJail;
        piece.CurrentPosition = GetFreePosition(square, inJail);
        piece.Position = 1.0F;
        piece.Queue = new Queue();
        pieces.Add(id, piece);
        Invalidate();
    }
    public void RemovePiece(int id) {
        if (pieces[id] == null) return; // no such piece
        pieces.Remove(id);
        Invalidate();
    }
    public void RenumberPiece(int oldID, int newID) {
        if (pieces[oldID] == null) return; // no such piece
        if (pieces[newID] != null) return; // already added
        Piece p = (Piece)(pieces[oldID]);
        pieces.Remove(p.ID);
        p.ID = newID;
        pieces.Add(newID, p);
    }
    public void MovePiece(int id, int square, bool inJail) {
        if (pieces[id] == null) return; // no such piece
        Piece p = (Piece)(pieces[id]);
        if (p.CurrentSquare == square && p.CurrentInJail == inJail)
            return;
        if (p.Position == 1.0F) {
            int pos = GetFreePosition(square, inJail);
            p.LastSquare = p.CurrentSquare;
            p.LastInJail = p.CurrentInJail;
            p.LastPosition = p.CurrentPosition;
            p.Position = 0.0F;
            p.Distance = 1.0F;
            p.CurrentSquare = square;
            p.CurrentInJail = inJail;
            p.CurrentPosition = pos;
            isAnimating = true;
            animationTimer.Start();
            Invalidate();
        } else {
            Piece.Pending nextMove = new Piece.Pending();
            nextMove.square = square;
            nextMove.jail = inJail;
            p.Queue.Enqueue(nextMove);
        }
    }
    public void MovePieceImmediately(int id, int square, bool inJail) {
        if (pieces[id] == null) return; // no such piece
        Piece p = (Piece)(pieces[id]);
        p.Queue.Clear();
        if (p.CurrentSquare == square && p.CurrentInJail == inJail)
            return;
        int pos = GetFreePosition(square, inJail);
        p.Position = 1.0F;
        p.CurrentSquare = square;
        p.CurrentInJail = inJail;
        p.CurrentPosition = pos;
        if (!animationTimer.Enabled)
            Animate(null, new EventArgs());
    }

    public int GetFreePosition(int square, bool inJail) {
        if (square < 0 || square >= SquareNames.Length)
            throw new ArgumentOutOfRangeException();
        int position = 0; // current tentative position
        int count = 0; // the number of pieces that have been checked
        IEnumerator i = pieces.Values.GetEnumerator();
        while (count < pieces.Count) {
            if (!i.MoveNext()) {
                // reached end of loop
                i.Reset();
                i.MoveNext();
            }
            Piece p = (Piece)(i.Current);
            if ((p.CurrentSquare == square &&
                 p.CurrentInJail == inJail &&
                 p.CurrentPosition == position) ||
                (p.Position < 1.0F &&
                 p.LastSquare == square &&
                 p.LastInJail == inJail &&
                 p.LastPosition == position)) {
                ++position;
                count = 0;
            }
            else
                ++count;
        }
        return position;
    }

    [Flags]
    public enum SquareState {
        WindowOpen = 0x01,
        Mortgaged = 0x02,
        Highlighted = 0x04,
    }

    protected SquareState[] squareStates;
    public void SetState(int square, SquareState state, bool enable) {
        if (enable)
            squareStates[square] |= state;
        else
            squareStates[square] &=~ state;
        if (state == SquareState.Highlighted)
            Invalidate();
        else
            UpdateCache();
    }

    protected int[,] squareHouses;
    public void SetHouses(int square, int houses, int hotels) {
        squareHouses[square, 0] = houses;
        squareHouses[square, 1] = hotels;
        UpdateCache();
    }

    protected Image[] squareIcons;
    protected float[] squareIconSizes;
    public void SetIcon(int square, Image icon) {
        squareIcons[square] = icon;
        if (AnimationEnabled) {
            squareIconSizes[square] = 4.0F;
            isAnimating = true;
            animationTimer.Start();
            Invalidate();
        } else {
            squareIconSizes[square] = 1.0F;
            UpdateCache();
        }
    }

    protected Card card;
    public Card Card {
        get { return card; }
        set {
            card = value;
            UpdateCache();
        }
    }

    protected Region[] squareAreas;

    protected int currentSquareHover = -1;
    protected int currentSquareClick = -1;

    protected int focussedSquare = 0;
    public int FocussedSquare { get { return focussedSquare; } }

    protected MouseButtons lastButton;

    protected override void OnMouseMove(MouseEventArgs e) {
        base.OnMouseMove(e);
        if (squareAreas != null) {
            for (int i = 0; i < squareAreas.Length; ++i) {
                if (squareAreas[i].IsVisible(new Point(e.X, e.Y))) {
                    // this is it
                    if (currentSquareHover != i) {
                        currentSquareHover = i;
                        OnSquareHover(new SquareEventArgs(i));
                    }
                    return;
                }
            }
            if (currentSquareHover != -1) {
                currentSquareHover = -1;
                OnSquareHover(new SquareEventArgs(-1));
            }
        }
    }

    protected override void OnMouseDown(MouseEventArgs e) {
        currentSquareClick = -1;
        // XXX handle clicking on pieces
        if (squareAreas != null) {
            for (int i = 0; i < squareAreas.Length; ++i) {
                if (squareAreas[i].IsVisible(new Point(e.X, e.Y))) {
                    // this is it
                    currentSquareClick = i;
                    if (i != focussedSquare) {
                        focussedSquare = i;
                        Invalidate();
                    }
                    break;
                }
            }
        }
        lastButton = e.Button;
        base.OnMouseDown(e);
    }

    protected override void OnClick(EventArgs e) {
        base.OnClick(e);
        Focus();
        if (currentSquareClick != -1 && lastButton == MouseButtons.Left)
            OnSquareClick(new SquareEventArgs(currentSquareClick));
    }

    protected override void OnDoubleClick(EventArgs e) {
        base.OnDoubleClick(e);
        if (currentSquareClick != -1 && lastButton == MouseButtons.Left)
            OnSquareDoubleClick(new SquareEventArgs(currentSquareClick));
    }

    protected override void OnMouseLeave(EventArgs e) {
        base.OnMouseLeave(e);
        if (currentSquareHover != -1) {
            currentSquareHover = -1;
            OnSquareHover(new SquareEventArgs(-1));
        }
    }

    public delegate void SquareEventHandler(Object sender, SquareEventArgs e);
    public class SquareEventArgs : EventArgs {
        private readonly int square;
        public int Square { get { return square; } }
        public SquareEventArgs(int aSquare) : base() {
            square = aSquare;
        }
    }

    public event SquareEventHandler SquareHover;
    protected virtual void OnSquareHover(SquareEventArgs e) {
        if (SquareHover != null)
            SquareHover(this, e);
    }

    public event SquareEventHandler SquareClick;
    protected virtual void OnSquareClick(SquareEventArgs e) {
        if (SquareClick != null)
            SquareClick(this, e);
    }

    public event SquareEventHandler SquareDoubleClick;
    protected virtual void OnSquareDoubleClick(SquareEventArgs e) {
        if (SquareDoubleClick != null)
            SquareDoubleClick(this, e);
    }

    protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData) {
        // XXX handle focussing pieces
        Keys left, up, right, down;
        switch (rotation) {
        default:
        case Angle.d0:
            left = Keys.Left;
            up = Keys.Up;
            right = Keys.Right;
            down = Keys.Down;
            break;
        case Angle.d90:
            left = Keys.Up;
            up = Keys.Right;
            right = Keys.Down;
            down = Keys.Left;
            break;
        case Angle.d180:
            left = Keys.Right;
            up = Keys.Down;
            right = Keys.Left;
            down = Keys.Up;
            break;
        case Angle.d270:
            left = Keys.Down;
            up = Keys.Left;
            right = Keys.Up;
            down = Keys.Right;
            break;
        }
        if (keyData == left) {
            if (focussedSquare < 10) {
                ++focussedSquare;
            } else if (focussedSquare == 10) {
                focussedSquare = 0;
            } else if (focussedSquare < 21) {
                focussedSquare = 50 - focussedSquare;
            } else if (focussedSquare < 31) {
                --focussedSquare;
            } else {
                focussedSquare = 50 - focussedSquare;
            }
        } else if (keyData == up) {
            if (focussedSquare == 0) {
                focussedSquare = 39;
            } else if (focussedSquare < 10) {
                focussedSquare = 30 - focussedSquare;
            } else if (focussedSquare < 20) {
                ++focussedSquare;
            } else if (focussedSquare < 31) {
                focussedSquare = 30 - focussedSquare;
            } else {
                --focussedSquare;
            }
        } else if (keyData == right) {
            if (focussedSquare == 0) {
                focussedSquare = 10;
            } else if (focussedSquare < 11) {
                --focussedSquare;
            } else if (focussedSquare < 20) {
                focussedSquare = 50 - focussedSquare;
            } else if (focussedSquare < 30) {
                ++focussedSquare;
            } else {
                focussedSquare = 50 - focussedSquare;
            }
        } else if (keyData == down) {
            if (focussedSquare < 11) {
                focussedSquare = 30 - focussedSquare;
            } else if (focussedSquare < 21) {
                --focussedSquare;
            } else if (focussedSquare < 30) {
                focussedSquare = 30 - focussedSquare;
            } else if (focussedSquare < 39) {
                ++focussedSquare;
            } else {
                focussedSquare = 0;
            }
        } else {
            switch (keyData) {
            case Keys.Return:
                OnSquareDoubleClick(new SquareEventArgs(focussedSquare));
                return true;
            case Keys.Space:
                OnSquareClick(new SquareEventArgs(focussedSquare));
                return true;
            default:
                return base.ProcessCmdKey(ref msg, keyData);
            }
        }
        // if we get here, focus was changed
        OnSquareHover(new SquareEventArgs(focussedSquare));
        Invalidate();
        return true;
    }

    protected override void OnGotFocus(EventArgs e) {
        base.OnGotFocus(e);
        Invalidate();
    }

    protected override void OnLostFocus(EventArgs e) {
        base.OnLostFocus(e);
        Invalidate();
    }

    public void Clear() {
        animationTimer.Stop();
        Pot = 0;
        pieces.Clear();
        animatedText.Clear();
        for (int i = 0; i < SquareNames.Length; ++i) {
            squareStates[i] = 0;
            squareHouses[i, 0] = 0;
            squareHouses[i, 1] = 0;
            squareIcons[i] = null;
            squareIconSizes[i] = 0.0F;
        }
        card = null;
        AnimationEnabled = true;
        UpdateCache();
    }

    protected float animatedHeight = 0.1F;
    public float AnimatedHeight {
        get { return animatedHeight; }
        set { animatedHeight = value; Invalidate(); }
    }

    protected float animatedCircleRadius = 0.33F;
    public float AnimatedCircleRadius {
        get { return animatedCircleRadius; }
        set { animatedCircleRadius = value; Invalidate(); }
    }

    protected Queue animatedText;
    protected class AnimatedText {
        public readonly String text;
        public readonly Color color;
        public readonly PointF center;
        public float position;
        public AnimatedText(String text, Color color, PointF center) {
            this.text = text;
            this.color = color;
            this.center = center;
            this.position = 0.0F;
        }
        public void Paint(Graphics g, RectangleF r, Font font, Angle rotation,
                          float animatedHeight, float animatedCircleRadius) {
            Brush brush = new SolidBrush(Color.FromArgb(Convert.ToInt32((1-position) * 255),
                                                        color.R, color.G, color.B));
            PointF point;
            switch (rotation) {
            default:
            case Angle.d0:
                point = new PointF(center.X * r.Width,
                                   (center.Y - position * animatedHeight) * r.Height);
                break;
            case Angle.d90:
                point = new PointF((1 - center.Y) * r.Width,
                                   (center.X - position * animatedHeight) * r.Height);
                break;
            case Angle.d180:
                point = new PointF((1 - center.X) * r.Width,
                                   ((1 - center.Y) - position * animatedHeight) * r.Height);
                break;
            case Angle.d270:
                point = new PointF(center.Y * r.Width,
                                   ((1 - center.X) - position * animatedHeight) * r.Height);
                break;
            }
            if (position < 0.33F) {
                Pen pen = new Pen(Color.FromArgb(Convert.ToInt32((1-position * 3) * 255),
                                                        color.R, color.G, color.B), 5);
                float d = Math.Min(r.Width * animatedCircleRadius,
                                   r.Height * animatedCircleRadius) * (1 - position * 3);
                g.DrawEllipse(pen, point.X - d, point.Y - d, d*2, d*2);
            }
            StringFormat format = new StringFormat(StringFormatFlags.NoClip | StringFormatFlags.NoWrap);
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            format.Trimming = StringTrimming.None;
            g.DrawString(text, font, brush, point, format);
        }
    }
    public void AddAnimatedTextSquare(String text, Color color, int square) {
        // work out where the square is at the moment
        RectangleF squareRect = squareAreas[square].GetBounds(CreateGraphics());
        RectangleF boardRect = new RectangleF(0, 0,
                                              ClientRectangle.Width-1,
                                              ClientRectangle.Height-1);
        PointF center = new PointF((squareRect.Left + squareRect.Width / 2) / boardRect.Width,
                                   (squareRect.Top + squareRect.Height / 2) / boardRect.Height);
        // work out where it would be without rotation
        switch (rotation) {
        default:
        case Angle.d0:
            break;
        case Angle.d90:
            center = new PointF(center.Y, 1 - center.X);
            break;
        case Angle.d180:
            center = new PointF(1 - center.X, 1 - center.Y);
            break;
        case Angle.d270:
            center = new PointF(1 - center.Y, center.X);
            break;
        }
        // add animation to list
        animatedText.Enqueue(new AnimatedText(text, color, center));
        animationTimer.Start();
        Invalidate();
    }
    public void AddAnimatedTextCard(String text, Color color) {
        animatedText.Enqueue(new AnimatedText(text, color, new PointF(0.25F, 0.25F)));
        animationTimer.Start();
        Invalidate();
    }
    public void AddAnimatedMessage(String text, Color color) {
        animatedText.Enqueue(new AnimatedText(text, color, new PointF(0.5F, 0.5F)));
        isAnimating = true;
        animationTimer.Start();
        Invalidate();
    }

    public static String[] SquareNames = {
        "Go",
        "Mediterranean Avenue",
        "Community Chest",
        "Baltic Avenue",
        "Income Tax",
        "Reading Railroad",
        "Oriental Avenue",
        "Chance",
        "Vermont Avenue",
        "Connecticut Avenue",
        "Jail",
        "St. Charles Place",
        "Electric Company",
        "States Avenue",
        "Virginia Avenue",
        "Pennsylvania Railroad",
        "St. James Place",
        "Community Chest",
        "Tennessee Avenue",
        "New York Avenue",
        "Free Parking",
        "Kentucky Avenue",
        "Chance",
        "Indiana Avenue",
        "Illinois Avenue",
        "B & O Railroad",
        "Atlantic Avenue",
        "Ventnor Avenue",
        "Water Works",
        "Marvin Gardens",
        "Go To Jail",
        "Pacific Avenue",
        "North Carolina Avenue",
        "Community Chest",
        "Pennsylvania Avenue",
        "Short Line",
        "Chance",
        "Park Place",
        "Luxury Tax",
        "Boardwalk",
    };

    public String GetSquareName(int square) {
        if (square < 0 || square >= SquareNames.Length)
            return "";
        return SquareNames[square];
    }

    protected static Color[,] groupColors = {
        { Color.FromArgb(255, 115, 26, 142),  Color.White },
        { Color.FromArgb(255, 188, 212, 238), Color.Black },
        { Color.FromArgb(255, 206, 0, 146),   Color.Black },
        { Color.FromArgb(255, 255, 190, 52),  Color.Black },
        { Color.FromArgb(255, 255, 0, 0),     Color.Black },
        { Color.FromArgb(255, 255, 255, 0),   Color.Black },
        { Color.FromArgb(255, 58, 190, 16),   Color.Black },
        { Color.FromArgb(255, 99, 122, 189),  Color.White },
    };

    public Color GetGroupColor(int group) {
        if (group < 0 || group >= groupColors.GetLength(0))
            return Color.White;
        return groupColors[group, 0];
    }

    public Color GetGroupTextColor(int group) {
        if (group < 0 || group >= groupColors.GetLength(0))
            return Color.Black;
        return groupColors[group, 1];
    }

    protected StringFormat topFormat;
    protected StringFormat topLeftFormat;
    protected StringFormat centerFormat;
    protected StringFormat bottomFormat;
    protected Brush textBrush;
    protected Brush goBrush;
    protected Brush chanceBrush;
    protected Brush jailBrush;
    protected Brush windowOpenBrush;
    protected Brush mortgagedBrush;
    protected Brush highlightBrush;
    protected Bitmap trainImage;
    protected Bitmap jailImage;
    protected Bitmap bulbImage;
    protected Bitmap carImage;
    protected Bitmap waterImage;
    protected Bitmap policeImage;
    protected Pen focusPen;
    ImageAttributes iconAttributes;

    public MonopolyBoard() : base() {
        SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        SetStyle(ControlStyles.ContainerControl, false); 
        SetStyle(ControlStyles.DoubleBuffer, true);
        SetStyle(ControlStyles.Opaque, true);
        SetStyle(ControlStyles.ResizeRedraw, true);
        SetStyle(ControlStyles.Selectable, true);
        SetStyle(ControlStyles.StandardClick, true);
        SetStyle(ControlStyles.StandardDoubleClick, true);
        SetStyle(ControlStyles.UserPaint, true); 

        // precache string formats...
        topFormat = new StringFormat();
        topFormat.Alignment = StringAlignment.Center;
        topFormat.LineAlignment = StringAlignment.Near;
        topFormat.HotkeyPrefix = HotkeyPrefix.None;
        topFormat.Trimming = StringTrimming.EllipsisCharacter;
        centerFormat = new StringFormat(topFormat);
        centerFormat.LineAlignment = StringAlignment.Center;
        topLeftFormat = new StringFormat(topFormat);
        topLeftFormat.Alignment = StringAlignment.Near;
        bottomFormat = new StringFormat(topFormat);
        bottomFormat.LineAlignment = StringAlignment.Far;

        // ...and brushes...
        textBrush = new SolidBrush(Color.Black);
        goBrush = new SolidBrush(Color.Red);
        chanceBrush = new SolidBrush(Color.FromArgb(255, 206, 0, 146));
        jailBrush = new SolidBrush(Color.FromArgb(255, 255, 150, 52));
        windowOpenBrush = new SolidBrush(Color.FromArgb(64, 0, 255, 0));
        mortgagedBrush = new HatchBrush(HatchStyle.DiagonalCross, Color.FromArgb(128, 0, 0, 0), Color.FromArgb(64, 204, 204, 204));
        highlightBrush = new SolidBrush(Color.FromArgb(128, 255, 255, 0));

        // ...and images
        trainImage = new Bitmap(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("train.gif"));
        jailImage = new Bitmap(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("jail.jpg"));
        bulbImage = new Bitmap(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("bulb.png"));
        carImage = new Bitmap(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("car.png"));
        waterImage = new Bitmap(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("water.gif"));
        policeImage = new Bitmap(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("police.jpg"));
        policeImage.MakeTransparent(Color.White);

        // a pen, too
        focusPen = new Pen(Color.Black, 1);
        focusPen.DashStyle = DashStyle.Dot;

        // and an icon color matrix
        iconAttributes = new ImageAttributes();
        iconAttributes.SetColorMatrix(new ColorMatrix(new float[][] {
            new float[] {  0.50F,  0.00F,  0.00F,  0.00F,  0.00F },
            new float[] {  0.00F,  0.50F,  0.00F,  0.00F,  0.00F },
            new float[] {  0.00F,  0.00F,  0.50F,  0.00F,  0.00F },
            new float[] {  0.00F,  0.00F,  0.00F,  0.50F,  0.00F },
            new float[] {  0.00F,  0.00F,  0.00F,  0.00F,  1.00F },
        }));

        // some defaults
        BackColor = Color.FromArgb(255, 188, 224, 193);
        ForeColor = Color.Black;
        Text = "Monopoly";

        // the bits that move
        pieces = new HybridDictionary(8);
        squareStates = new SquareState[SquareNames.Length];
        squareHouses = new int[SquareNames.Length, 2];
        squareIcons = new Image[SquareNames.Length];
        squareIconSizes = new float[SquareNames.Length];
        animatedText = new Queue(4);
        animationTimer = new Timer();
        animationTimer.Interval = 50;
        animationTimer.Tick += new EventHandler(Animate);

    }

    override protected void OnTextChanged(EventArgs e) {
        UpdateCache();
    }

    private void PaintHouse(Graphics g, RectangleF r, Brush b, Pen p) {
        PointF[] points = new PointF[] {
            new PointF(r.Left + r.Width / 2, r.Top),
            new PointF(r.Right - 1, r.Top + r.Height / 2),
            new PointF(r.Right - 1, r.Bottom),
            new PointF(r.Left + 1, r.Bottom),
            new PointF(r.Left + 1, r.Top + r.Height / 2),
        };
        g.FillPolygon(b, points);
        g.DrawPolygon(p, points);
    }

/*
    private Icon GetIcon(int width, int height, Brush brush, String name) {
        Bitmap bitmap = new Bitmap(16, 16);
        Graphics g = Graphics.FromImage(bitmap);
        // Enable High Quality Smoothing
        g.SmoothingMode = SmoothingMode.HighQuality;
        g.TextRenderingHint = TextRenderingHint.AntiAlias;
          // ClearTypeGridFit is not supported on Win2k, and
          // AntiAliasGridFit doesn't seem to work on rotated text.
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        // g.PixelOffsetMode, g.CompositingMode, g.CompositingQuality XXX
        RectangleF r = new RectangleF((16 - width) / 2,
                                      (16 - height) / 2, width, height);
        r.Inflate(-1, -1);
        PaintHouse(g, r, brush, Pens.Black);
        bitmap.Save(name, ImageFormat.Png);
        return null;
    }

    public Icon GetHouseIcon(int w) {
        return GetIcon(Convert.ToInt32(w / 1.6F), Convert.ToInt32(w / 1.6F), Brushes.Green, "house.png");
    }

    public Icon GetHotelIcon(int w) {
        return GetIcon(w, Convert.ToInt32(w / 1.6F), Brushes.Maroon, "hotel.png");
    }
*/

    private void PaintHouses(Graphics g, RectangleF r, int houses) {
        r.Inflate(-1, -1);
        if (houses < 5) {
            r.Width /= 4;
            for (int i = 0; i < houses; ++i) {
                PaintHouse(g, r, Brushes.Green, Pens.Black);
                r.Offset(r.Width, 0);
            }
        } else {
            r.Inflate(Convert.ToInt32(-r.Width * 0.3F), 0);
            PaintHouse(g, r, Brushes.Maroon, Pens.Black);
        }
    }

    private void PaintProperty(Graphics g, Font font1, Font font2,
                               StringFormat format1, StringFormat format2,
                               Brush brush, Pen pen, RectangleF r,
                               int squareID, int group, int price) {
        squareAreas[squareID] = new Region(r);
        squareAreas[squareID].Transform(g.Transform);

        g.FillRectangle(new SolidBrush(GetGroupColor(group)),
                        new RectangleF(r.X, r.Y, r.Width, r.Height * colorHeight));
        g.DrawLine(pen, r.X + 1, r.Y + r.Height * colorHeight, 
                   r.X + r.Width - 1, r.Y + r.Height * colorHeight);

        PaintHouses(g, new RectangleF(r.X + pen.Width, r.Y + pen.Width,
                                      r.Width - pen.Width * 2,
                                      r.Height * colorHeight - pen.Width * 2),
                    squareHouses[squareID, 0] + 5 * squareHouses[squareID, 1]);

        r.Inflate(-pen.Width, -pen.Width);

        g.DrawString(GetSquareName(squareID), font1, brush,
                     new RectangleF(r.X, r.Y + r.Height * (colorHeight + padding) + pen.Width,
                                    r.Width, r.Height - r.Height * colorHeight - pen.Width),
                     format1);

        g.DrawString("$" + price, font2, brush,
                     new RectangleF(r.X, r.Y + r.Height / 2,
                                    r.Width, r.Height / 2 - r.Height * padding),
                     format2);
    }

    private void PaintCommunityChest(Graphics g, Font font1, Font font2,
                                     StringFormat format1, StringFormat format2,
                                     Brush brush, Pen pen, RectangleF r,
                                     int squareID) {
        squareAreas[squareID] = new Region(r);
        squareAreas[squareID].Transform(g.Transform);

        r.Inflate(-pen.Width, -pen.Width);

        g.DrawString("\u2743", font1, brush,
                     new RectangleF(r.X, r.Y, r.Width, r.Height / 1.5F),
                     format2);

        g.DrawString("Community\nChest", font2, brush,
                     new RectangleF(r.X, r.Y + r.Height / 2,
                                    r.Width, r.Height / 2),
                     format2);
    }

    private void PaintTax(Graphics g, Font font1, Font font2, Font font3,
                          StringFormat format1, StringFormat format2,
                          StringFormat format3, Brush brush, Pen pen,
                          RectangleF r, int squareID, String name,
                          String comment) {
        squareAreas[squareID] = new Region(r);
        squareAreas[squareID].Transform(g.Transform);

        r.Inflate(-pen.Width, -pen.Width);

        g.DrawString(name, font1, brush,
                     new RectangleF(r.X, r.Y + r.Height * padding,
                                    r.Width, r.Height / 2),
                     format1);

        g.DrawString("\u2756", font2, brush,
                     new RectangleF(r.X, r.Y, r.Width, r.Height),
                     format2);

        g.DrawString(comment, font3, brush,
                     new RectangleF(r.X, r.Y + r.Height / 2,
                                    r.Width, r.Height / 2 - r.Height * padding),
                     format3);
    }

    private void PaintPicturePropertySquare(Graphics g, Font font1, Font font2,
                                            StringFormat format1,
                                            StringFormat format2,
                                            Brush brush, Pen pen, Image image,
                                            RectangleF r, int squareID,
                                            int price) {
        squareAreas[squareID] = new Region(r);
        squareAreas[squareID].Transform(g.Transform);

        r.Inflate(-pen.Width, -pen.Width);

        String name = GetSquareName(squareID);

        g.DrawString(name, font1, brush,
                     new RectangleF(r.X, r.Y + r.Height * padding,
                                    r.Width, r.Height),
                     format1);

        SizeF size1 = g.MeasureString(name, font1, r.Size, format1);
        float ratio = (float)image.Height / (float)image.Width;
        float ratioHeight = r.Width * ratio;

        float topOffset = r.Height * padding + size1.Height;
        float bottomOffset = r.Height * padding + font2.Size;
        float maxHeight = r.Height - topOffset - bottomOffset;

        float height = Math.Min(ratioHeight, maxHeight);
        float width = height / ratio;

        g.DrawImage(image, r.X + r.Width / 2 - width / 2,
                    r.Y + topOffset + maxHeight / 2 - height / 2,
                    width, height);

        g.DrawString("$" + price, font2, brush,
                     new RectangleF(r.X, r.Y + r.Height / 2,
                                    r.Width, r.Height / 2 - r.Height * padding),
                     format2);
    }

    private void PaintChance(Graphics g, Font font1, Font font2,
                             StringFormat format1, StringFormat format2,
                             Brush brush1, Brush brush2, Pen pen1, Pen pen2,
                             RectangleF r, int squareID) {
        squareAreas[squareID] = new Region(r);
        squareAreas[squareID].Transform(g.Transform);

        r.Inflate(-pen1.Width, -pen1.Width);

        g.DrawString("Chance", font1, brush1,
                     new RectangleF(r.X, r.Y + r.Height * padding,
                                    r.Width, r.Height - r.Height * padding),
                     format1);

        SizeF size = g.MeasureString("?", font2);
        GraphicsPath path = new GraphicsPath();
        path.AddString("?", font2.FontFamily, (int)font2.Style, font2.Size, 
                       new RectangleF(r.X, r.Y + font1.Size + r.Height * padding * 2,
                                      r.Width, r.Height - font1.Size - r.Height * padding * 3),
                       format2);
        g.FillPath(brush2, path);
        g.DrawPath(pen2, path);
    }

    private void PaintPictureSquare(Graphics g, Font font, StringFormat format1,
                                    StringFormat format2, Brush brush, Pen pen,
                                    RectangleF r, Image image, String text1,
                                    String text2) {

        r.Inflate(-pen.Width, -pen.Width);
        g.DrawString(text1, font, brush, r, format1);
        g.DrawString(text2, font, brush, r, format2);

        float ratio = (float)image.Height / (float)image.Width;
        float ratioHeight = r.Width * ratio;
        float maxHeight = r.Height - font.Size * 2.4F;
        float height = Math.Min(maxHeight, ratioHeight);
        float width = height / ratio;
        g.DrawImage(image,
                    r.X + r.Width / 2 - width / 2,
                    r.Y + r.Height / 2 - height / 2,
                    width, height);
    }

    private void PaintSquareWalls(Graphics g, float top, float bottom, float left,
                                  float width, int count, Pen pen) {
        for (int i = 1; i < count; ++i) {
            left += width;
            g.DrawLine(pen, left, top + 1, left, bottom);
        }
    }

    private Region PaintPiece(Graphics g, RectangleF r, Image image) {
        Region area = new Region(r);
        area.Transform(g.Transform);
        g.DrawImage(image, r);
        return area;
    }
    
    private RectangleF GetPiecePosition(RectangleF rect, int index,
                                        bool jailSquare, bool inJail,
                                        float dimension, Graphics g) {
        if (jailSquare) {
            // if |jail| is true, limit ourselves to the left/bottom
            // edges (rotated. as required); otherwise, use the rest.
            // (use left for even indexes, bottom for odd)
            // XXX there has GOT to be an easier way of doing this
            if (inJail) {
                switch (rotation) {
                default:
                case Angle.d0:
                    rect = new RectangleF(rect.Left + dimension, rect.Top,
                                          rect.Width - dimension,
                                          rect.Height - dimension);
                    break;
                case Angle.d90:
                    rect = new RectangleF(rect.Left + dimension,
                                          rect.Top + dimension,
                                          rect.Width - dimension,
                                          rect.Height - dimension);
                    break;
                case Angle.d180:
                    rect = new RectangleF(rect.Left, rect.Top + dimension,
                                          rect.Width - dimension,
                                          rect.Height - dimension);
                    break;
                case Angle.d270:
                    rect = new RectangleF(rect.Left, rect.Top,
                                          rect.Width - dimension,
                                          rect.Height - dimension);
                    break;
                }
            } else {
                if (index == 1) { // the second place
                    switch (rotation) {
                    default:
                    case Angle.d0:
                        rect = new RectangleF(rect.Left,
                                              rect.Top + rect.Height - dimension,
                                              dimension, dimension);
                        break;
                    case Angle.d90:
                        rect = new RectangleF(rect.Left, rect.Top,
                                              dimension, dimension);
                        break;
                    case Angle.d180:
                        rect = new RectangleF(rect.Left + rect.Width - dimension,
                                              rect.Top,
                                              dimension, dimension);
                        break;
                    case Angle.d270:
                        rect = new RectangleF(rect.Left + rect.Width - dimension,
                                               rect.Top + rect.Height - dimension,
                                              dimension, dimension);
                        break;
                    }
                    index = 0; // so that the algorithm below picks the spot here
                } else if (index % 2 == 0) { // even numbers
                    switch (rotation) {
                    default:
                    case Angle.d0:
                        rect = new RectangleF(rect.Left, rect.Top,
                                              dimension, rect.Height - dimension);
                        break;
                    case Angle.d90:
                        rect = new RectangleF(rect.Left + dimension, rect.Top,
                                              rect.Width - dimension, dimension);
                        break;
                    case Angle.d180:
                        rect = new RectangleF(rect.Left + rect.Width - dimension,
                                              rect.Top + dimension,
                                              dimension, rect.Height - dimension);
                        break;
                    case Angle.d270:
                        rect = new RectangleF(rect.Left,
                                              rect.Top + rect.Height - dimension,
                                              rect.Width - dimension, dimension);
                        break;
                    }
                    index /= 2; // so that the algorithm below picks the spots continuously
                } else { // odd numbers from 3 up
                    switch (rotation) {
                    default:
                    case Angle.d0:
                        rect = new RectangleF(rect.Left + dimension,
                                              rect.Top + rect.Height - dimension,
                                              rect.Width - dimension, dimension);
                        break;
                    case Angle.d90:
                        rect = new RectangleF(rect.Left, rect.Top + dimension,
                                              dimension, rect.Height - dimension);
                        break;
                    case Angle.d180:
                        rect = new RectangleF(rect.Left, rect.Top,
                                              rect.Width - dimension, dimension);
                        break;
                    case Angle.d270:
                        rect = new RectangleF(rect.Left + rect.Width - dimension,
                                              rect.Top,
                                              dimension, rect.Height - dimension);
                        break;
                    }
                    index = (index - 3) / 2; // so that the algorithm below picks the spots continuously
                }
            }
        }
        rect = Rectangle.Ceiling(rect);
        // find spot |index| in |rect|
        // the conversion to Single before Flooring is needed because otherwise
        // we run into problems rounding 3 to 2 and stuff like that. Trust me.
        int nx = (int)rect.Width / (int)dimension;
        int ny = (int)rect.Height / (int)dimension;
        float remainderx = rect.Width / nx - dimension;
        float remaindery = rect.Height / ny - dimension;
        index %= nx * ny;
        int indexx = index % nx;
        int indexy = index / nx;
        rect = new RectangleF(rect.Left + remainderx / 2 +
                                indexx * (remainderx + dimension),
                              rect.Top + remaindery / 2 +
                                indexy * (remaindery + dimension),
                              dimension, dimension);
        return rect;
    }

    private RectangleF GetTransitionPosition(RectangleF from, RectangleF to,
                                             float position, out float distance) {
        float dX = to.X - from.X;
        float dY = to.Y - from.Y;
        float dW = to.Width - from.Width;
        float dH = to.Height - from.Height;
        distance = Math.Max(Math.Abs(dX * 10F) / ClientSize.Width,
                            Math.Abs(dY * 10F) / ClientSize.Height);
        return new RectangleF(from.X + dX * position,
                              from.Y + dY * position,
                              from.Width + dW * position,
                              from.Height + dH * position);
    }

    private void ResetTransform(Graphics g, RectangleF r) {
        g.ResetTransform();
        switch (rotation) {
        case Angle.d90:
            g.TranslateTransform(r.Width, 0);
            g.RotateTransform(90);
            break;
        case Angle.d180:
            g.TranslateTransform(r.Width, r.Height);
            g.RotateTransform(180);
            break;
        case Angle.d270:
            g.TranslateTransform(0, r.Height);
            g.RotateTransform(270);
            break;
        }
    }

    protected Bitmap boardCache;

    protected void UpdateCache() {
        boardCache = null;
        Invalidate();
    }

    // some key points on the board
    float squareHeightY;
    float squareHeightX;
    float squareWidthY;
    float squareWidthX;
    float squareDimension;
    float d;
    float iconDimension;
    float diceSize;
    float edgePenWidth;

    protected void PaintBoard(Graphics paintGraphics) {
        boardCache = new Bitmap(ClientSize.Width, ClientSize.Height, paintGraphics);
        Graphics g = Graphics.FromImage(boardCache);

        // The font constructor doesn't accept Display units (don't
        // ask me why). The page units have to be the same as the font
        // units so that the paths work right. Therefore the page
        // units can't be Display units. In DoubleBuffer mode the
        // default unit is Display. So here we set it to Pixel.
        g.PageUnit = GraphicsUnit.Pixel;

        squareAreas = new Region[SquareNames.Length];

        // background
        g.FillRectangle(new SolidBrush(BackColor), ClientRectangle);

        // the rect as floats for higher precision
        RectangleF r;
        switch (rotation) {
        default:
        case Angle.d0:
            r = new RectangleF(0, 0, ClientRectangle.Width-1, ClientRectangle.Height-1);
            break;
        case Angle.d90:
            r = new RectangleF(0, 0, ClientRectangle.Height-1, ClientRectangle.Width-1);
            break;
        case Angle.d180:
            r = new RectangleF(0, 0, ClientRectangle.Width-1, ClientRectangle.Height-1);
            break;
        case Angle.d270:
            r = new RectangleF(0, 0, ClientRectangle.Height-1, ClientRectangle.Width-1);
            break;
        }

        // some dimensions
        d = Math.Min(r.Width, r.Height); // representative dimension

        // some key points on the board
        squareHeightY = r.Height * squareHeight;
        squareHeightX = r.Width * squareHeight;
        squareWidthY = (r.Height - squareHeightY * 2) / 9;
        squareWidthX = (r.Width - squareHeightX * 2) / 9;
        squareDimension = Math.Min(squareHeightX, squareHeightY);

        // set up the fonts we'll use... (in size order)
        FontFamily fontFamily = new FontFamily("Tahoma");
        Font titleFont = new Font(fontFamily, d * 0.125F, FontStyle.Bold, g.PageUnit);
        Font chanceFont = new Font("Comic Sans MS", d * 0.1F, FontStyle.Bold, g.PageUnit);
        Font arrowFont = new Font(fontFamily, d * 0.075F, g.PageUnit);
        Font goFont = new Font(fontFamily, d * 0.045F, FontStyle.Bold, g.PageUnit);
        Font decorativeFont = new Font(fontFamily, d * 0.03F, g.PageUnit);
        Font cardTextFont = new Font(fontFamily, Math.Max(d * 0.017F, Font.Size), g.PageUnit);
        Font bigSquareFont = new Font(fontFamily, d * 0.015F, FontStyle.Bold, g.PageUnit);
        Font labelFont = new Font(fontFamily, d * 0.015F, g.PageUnit);
        Font nameFont = new Font(fontFamily, d * 0.01F, g.PageUnit);

        Font cardTitleFont = new Font(fontFamily, cardTextFont.Size * 1.8F/1.4F, FontStyle.Bold, g.PageUnit);

        // ...and various pens
        Pen linePen = new Pen(ForeColor, d * 0.0035F);
        linePen.SetLineCap(LineCap.Square, LineCap.Square, DashCap.Flat);
        Pen lineShadowPen = new Pen(Color.FromArgb(128, 0, 0, 0), d * 0.005F);
        linePen.SetLineCap(LineCap.Square, LineCap.Square, DashCap.Flat);
        Pen outlinePen = new Pen(ForeColor, d * 0.002F);
        outlinePen.Alignment = PenAlignment.Outset;
        outlinePen.SetLineCap(LineCap.Square, LineCap.Square, DashCap.Flat);

        // Enable High Quality Smoothing
        g.SmoothingMode = SmoothingMode.HighQuality;
        g.TextRenderingHint = TextRenderingHint.AntiAlias;
          // ClearTypeGridFit is not supported on Win2k, and
          // AntiAliasGridFit doesn't seem to work on rotated text.
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        // g.PixelOffsetMode, g.CompositingMode, g.CompositingQuality XXX

        // AND NOW THE DRAWING COD

        int square = 0;
        int group = 0;

        // bottom right corner
        ResetTransform(g, ClientRectangle);
        g.TranslateTransform(r.Width - squareHeightX / 2,
                             r.Height - squareHeightY / 2);
        g.RotateTransform(-45);
        g.TranslateTransform(-(r.Width - squareHeightX / 2),
                             -(r.Height - squareHeightY / 2));
        // 00 Go
        // "collect $200 as you pass"
        RectangleF subRect = new RectangleF(r.Width - squareHeightX + (squareHeightX - squareDimension) / 2,
                                            r.Height - squareHeightY + (squareHeightY - squareDimension) / 2,
                                            squareDimension, squareDimension);
        subRect.Offset(0, -3.2F * labelFont.Size);
        g.DrawString("Collect", labelFont, textBrush, subRect, centerFormat);
        subRect.Offset(0, labelFont.Size);
        g.DrawString("$200 Salary", labelFont, textBrush, subRect, centerFormat);
        subRect.Offset(0, labelFont.Size);
        g.DrawString("As You Pass", labelFont, textBrush, subRect, centerFormat);
        // "GO"
        subRect.Offset(0, labelFont.Size * 2F);
        subRect = new RectangleF(r.Width - squareHeightX + (squareHeightX - squareDimension) / 2F,
                                 subRect.Y, squareDimension, squareDimension);
        SizeF size = g.MeasureString("GO", goFont);
        GraphicsPath path = new GraphicsPath();
        path.AddString("GO", goFont.FontFamily, (int)goFont.Style, goFont.Size, 
                         new PointF(subRect.X + (subRect.Width - size.Width) / 2,
                                    subRect.Y + (subRect.Height - size.Height) / 2),
                         StringFormat.GenericDefault);
        g.FillPath(goBrush, path);
        g.DrawPath(outlinePen, path);
        // arrow
        ResetTransform(g, ClientRectangle);
        path.Reset();
        size = g.MeasureString("\u21A2", arrowFont);
        path.AddString("\u21A2", arrowFont.FontFamily,
                         (int)arrowFont.Style, arrowFont.Size,
                         new PointF(0,0), StringFormat.GenericDefault);
        Matrix stretchMatrix = new Matrix();
        float scale = 2.0F * (squareHeightX / squareDimension);
        stretchMatrix.Translate(r.Width - squareHeightX / 2 - scale * size.Width / 2 ,
                                r.Height - size.Height * 0.4F);
        stretchMatrix.Scale(scale, 0.5F);
        path.Transform(stretchMatrix);
        g.FillPath(goBrush, path);
        g.DrawPath(outlinePen, path);
        // hit region testing
        squareAreas[square] = new Region(new RectangleF(r.Width - squareHeightX,
                                                        r.Height - squareHeightY,
                                                        squareHeightX, squareHeightY));
        squareAreas[square].Transform(g.Transform);

        // bottom row
        // 01 Mediterranean Avenue
        PaintProperty(g, nameFont, labelFont, topFormat, bottomFormat,
                      textBrush, linePen,
                      new RectangleF(squareHeightX + 8 * squareWidthX,
                                     r.Height - squareHeightY,
                                     squareWidthX, squareHeightY),
                      ++square, group, 60);
        // 02 Community Chest
        PaintCommunityChest(g, decorativeFont, nameFont,
                            topFormat, centerFormat, textBrush, linePen,
                            new RectangleF(squareHeightX + 7 * squareWidthX,
                                           r.Height - squareHeightY,
                                           squareWidthX, squareHeightY),
                            ++square);
        // 03 Baltic Avenue
        PaintProperty(g, nameFont, labelFont, topFormat, bottomFormat,
                      textBrush, linePen, 
                      new RectangleF(squareHeightX + 6 * squareWidthX,
                                     r.Height - squareHeightY,
                                     squareWidthX, squareHeightY),
                      ++square, group, 60);
        // 04 Income Tax
        PaintTax(g, bigSquareFont, decorativeFont, labelFont,
                 topFormat, centerFormat, bottomFormat,
                 textBrush, linePen,
                 new RectangleF(squareHeightX + 5 * squareWidthX,
                                r.Height - squareHeightY,
                                squareWidthX, squareHeightY),
                 ++square, "INCOME TAX", "Pay 10%\nor $200");
        // 05 Reading Railroad
        PaintPicturePropertySquare(g, nameFont, labelFont, topFormat, bottomFormat,
                                   textBrush, linePen, trainImage,
                                   new RectangleF(squareHeightX + 4 * squareWidthX,
                                                  r.Height - squareHeightY,
                                                  squareWidthX, squareHeightY),
                                   ++square, 200);
        // 06 Oriental Avenue
        PaintProperty(g, nameFont, labelFont, topFormat, bottomFormat,
                      textBrush, linePen,
                      new RectangleF(squareHeightX + 3 * squareWidthX,
                                     r.Height - squareHeightY,
                                     squareWidthX, squareHeightY),
                      ++square, ++group, 100);
        // 07 Chance
        PaintChance(g, nameFont, chanceFont, topFormat, centerFormat,
                    textBrush, chanceBrush,  linePen, outlinePen,
                    new RectangleF(squareHeightX + 2 * squareWidthX,
                                   r.Height - squareHeightY,
                                   squareWidthX, squareHeightY),
                    ++square);
        // 08 Vermont Avenue
        PaintProperty(g, nameFont, labelFont, topFormat, bottomFormat,
                      textBrush, linePen,
                      new RectangleF(squareHeightX + squareWidthX,
                                     r.Height - squareHeightY,
                                     squareWidthX, squareHeightY),
                      ++square, group, 100);
        // 09 Connecticut Avenue
        PaintProperty(g, nameFont, labelFont, topFormat, bottomFormat,
                      textBrush, linePen,
                      new RectangleF(squareHeightX,
                                     r.Height - squareHeightY,
                                     squareWidthX, squareHeightY),
                      ++square, group, 120);
        // walls
        PaintSquareWalls(g, r.Height - squareHeightY, r.Height,
                         squareHeightX, squareWidthX, 9, linePen);

        // bottom left
        // 10 Jail
        squareAreas[++square] = new Region(new RectangleF(0, r.Height - squareHeightY,
                                                          squareHeightX, squareHeightY));
        squareAreas[square].Transform(g.Transform);
        // "VISITING"
        g.DrawString("VISITING", labelFont, textBrush,
                     new RectangleF(squareHeightX * colorHeight,
                                    r.Height - squareHeightY * colorHeight,
                                    squareHeightX * (1 - colorHeight),
                                    squareHeightY * colorHeight), centerFormat);
        // "JUST"
        ResetTransform(g, ClientRectangle);
        g.TranslateTransform(squareHeightX * colorHeight, r.Height - squareHeightY);
        g.RotateTransform(90);
        g.DrawString("JUST", labelFont, textBrush,
                     new RectangleF(0, 0,
                                    squareHeightY * (1 - colorHeight),
                                    squareHeightX * colorHeight),
                     centerFormat);
        // The Jail
        ResetTransform(g, ClientRectangle);
        g.FillRectangle(jailBrush,
                        new RectangleF(squareHeightX * colorHeight, 
                                       r.Height - squareHeightY,
                                       squareHeightX * (1 - colorHeight),
                                       squareHeightY * (1 - colorHeight)));
        
        // "In Jail"
        ResetTransform(g, ClientRectangle);
        g.TranslateTransform((colorHeight + 1) * squareHeightX / 2,
                             r.Height - (colorHeight + 1) * squareHeightY / 2);
        g.RotateTransform(45);
        scale = squareDimension * (1 - colorHeight);
        PaintPictureSquare(g, labelFont, topFormat, bottomFormat, textBrush, linePen,
                           new RectangleF(-scale/2, -scale/2, scale, scale),
                           jailImage, "IN", "JAIL");
        // walls
        ResetTransform(g, ClientRectangle);
        g.DrawLine(linePen,
                   squareHeightX * colorHeight, r.Height - squareHeightY,
                   squareHeightX * colorHeight, r.Height - squareHeightY * colorHeight);
        g.DrawLine(linePen,
                   squareHeightX * colorHeight, r.Height - squareHeightY * colorHeight,
                   squareHeightX, r.Height - squareHeightY * colorHeight);
        
        // left row
        subRect = new RectangleF(0, 0, squareWidthY, squareHeightX);
        // 11 St. Charles Place
        g.TranslateTransform(squareHeightX, r.Height - squareHeightY - squareWidthY);
        g.RotateTransform(90);
        PaintProperty(g, nameFont, labelFont, topFormat, bottomFormat,
                      textBrush, linePen, subRect,
                      ++square, ++group, 140);
        // 12 Electric Company
        g.TranslateTransform(-squareWidthY, 0);
        PaintPicturePropertySquare(g, nameFont, labelFont, topFormat, bottomFormat,
                                   textBrush, linePen, bulbImage, subRect,
                                   ++square, 150);
        // 13 States Avenue
        g.TranslateTransform(-squareWidthY, 0);
        PaintProperty(g, nameFont, labelFont, topFormat, bottomFormat,
                      textBrush, linePen, subRect,
                      ++square, group, 140);
        // 14 Virginia Avenue
        g.TranslateTransform(-squareWidthY, 0);
        PaintProperty(g, nameFont, labelFont, topFormat, bottomFormat,
                      textBrush, linePen, subRect,
                      ++square, group, 160);
        // 15 Pennsylvania Railroad
        g.TranslateTransform(-squareWidthY, 0);
        PaintPicturePropertySquare(g, nameFont, labelFont, topFormat, bottomFormat,
                                   textBrush, linePen, trainImage, subRect,
                                   ++square, 200);
        // 16 St. James Place
        g.TranslateTransform(-squareWidthY, 0);
        PaintProperty(g, nameFont, labelFont, topFormat, bottomFormat,
                      textBrush, linePen, subRect,
                      ++square, ++group, 180);
        // 17 Community Chest
        g.TranslateTransform(-squareWidthY, 0);
        PaintCommunityChest(g, decorativeFont, nameFont,
                            topFormat, centerFormat, textBrush, linePen, subRect,
                            ++square);
        // 18 Tennessee Avenue
        g.TranslateTransform(-squareWidthY, 0);
        PaintProperty(g, nameFont, labelFont, topFormat, bottomFormat,
                      textBrush, linePen, subRect,
                      ++square, group, 180);
        // 19 New York Avenue
        g.TranslateTransform(-squareWidthY, 0);
        PaintProperty(g, nameFont, labelFont, topFormat, bottomFormat,
                      textBrush, linePen, subRect,
                      ++square, group, 200);
        // walls
        PaintSquareWalls(g, 0, squareHeightX, 0, squareWidthY, 9, linePen);

        // top left
        // 20 Free Parking
        ResetTransform(g, ClientRectangle);
        squareAreas[++square] = new Region(new RectangleF(0, 0, squareHeightX, squareHeightY));
        squareAreas[square].Transform(g.Transform);
        g.TranslateTransform(squareHeightX / 2 + linePen.Width,
                             squareHeightY / 2 + linePen.Width);
        g.RotateTransform(135);
        subRect = new RectangleF(-squareDimension / 2, -squareDimension / 2,
                                 squareDimension, squareDimension);
        subRect.Inflate(-squareDimension * padding, -squareDimension * padding);
        PaintPictureSquare(g, labelFont, topFormat, bottomFormat, textBrush,
                           linePen, subRect, carImage, "FREE", "PARKING");

        // top row
        ResetTransform(g, ClientRectangle);
        subRect = new RectangleF(0, 0, squareWidthX, squareHeightY);
        g.TranslateTransform(squareHeightX + squareWidthX, squareHeightY);
        g.RotateTransform(180);
        // 21 Kentucky Avenue
        PaintProperty(g, nameFont, labelFont, topFormat, bottomFormat,
                      textBrush, linePen,subRect,
                      ++square, ++group, 220);
        // 22 Chance
        g.TranslateTransform(-squareWidthX, 0);
        PaintChance(g, nameFont, chanceFont, topFormat, centerFormat,
                    textBrush, chanceBrush, linePen, outlinePen, subRect,
                    ++square);
        // 23 Indiana Avenue
        g.TranslateTransform(-squareWidthX, 0);
        PaintProperty(g, nameFont, labelFont, topFormat, bottomFormat,
                      textBrush, linePen, subRect,
                      ++square, group, 220);
        // 24 Illinois Avenue
        g.TranslateTransform(-squareWidthX, 0);
        PaintProperty(g, nameFont, labelFont, topFormat, bottomFormat,
                      textBrush, linePen, subRect,
                      ++square, group, 240);
        // 25 B & O Railroad
        g.TranslateTransform(-squareWidthX, 0);
        PaintPicturePropertySquare(g, nameFont, labelFont, topFormat, bottomFormat,
                                   textBrush, linePen, trainImage, subRect,
                                   ++square, 200);
        // 25 Atlantic Avenue
        g.TranslateTransform(-squareWidthX, 0);
        PaintProperty(g, nameFont, labelFont, topFormat, bottomFormat,
                      textBrush, linePen, subRect,
                      ++square, ++group, 260);
        // 27 Ventnor Avenue
        g.TranslateTransform(-squareWidthX, 0);
        PaintProperty(g, nameFont, labelFont, topFormat, bottomFormat,
                      textBrush, linePen, subRect,
                      ++square, group, 260);
        // 28 Water Works
        g.TranslateTransform(-squareWidthX, 0);
        PaintPicturePropertySquare(g, nameFont, labelFont, topFormat, bottomFormat,
                                   textBrush, linePen, waterImage, subRect,
                                   ++square, 150);
        // 29 Marvin Gardens
        g.TranslateTransform(-squareWidthX, 0);
        PaintProperty(g, nameFont, labelFont, topFormat, bottomFormat,
                      textBrush, linePen, subRect,
                      ++square, group, 280);
        // walls
        PaintSquareWalls(g, 0, squareHeightY, 0, squareWidthX, 9, linePen);

        // top right
        // 30 Go To Jail
        ResetTransform(g, ClientRectangle);
        squareAreas[++square] = new Region(new RectangleF(r.Width - squareHeightX, 0, squareHeightX, squareHeightY));
        squareAreas[square].Transform(g.Transform);
        g.TranslateTransform(r.Width - squareHeightX / 2 - linePen.Width,
                             squareHeightY / 2 + linePen.Width);
        g.RotateTransform(225);
        subRect = new RectangleF(-squareDimension / 2, -squareDimension / 2,
                                 squareDimension, squareDimension);
        subRect.Inflate(-squareDimension * padding, -squareDimension * padding);
        PaintPictureSquare(g, labelFont, topFormat, bottomFormat, textBrush,
                           linePen, subRect, policeImage, "GO TO", "JAIL");

        // right row
        ResetTransform(g, ClientRectangle);
        subRect = new RectangleF(0, 0, squareWidthY, squareHeightX);
        g.TranslateTransform(r.Width - squareHeightX,
                             squareHeightY + squareWidthY);
        g.RotateTransform(270);
        // 31 Pacific Avenue
        PaintProperty(g, nameFont, labelFont, topFormat, bottomFormat,
                      textBrush, linePen, subRect,
                      ++square, ++group, 300);
        // 32 North Carolina Avenue
        g.TranslateTransform(-squareWidthY, 0);
        PaintProperty(g, nameFont, labelFont, topFormat, bottomFormat,
                      textBrush, linePen, subRect,
                      ++square, group, 300);
        // 33 Community Chest
        g.TranslateTransform(-squareWidthY, 0);
        PaintCommunityChest(g, decorativeFont, nameFont,
                            topFormat, centerFormat, textBrush, linePen, subRect,
                            ++square);
        // 34 Pennsylvania Avenue
        g.TranslateTransform(-squareWidthY, 0);
        PaintProperty(g, nameFont, labelFont, topFormat, bottomFormat,
                      textBrush, linePen, subRect,
                      ++square, group, 320);
        // 35 Short Line
        g.TranslateTransform(-squareWidthY, 0);
        PaintPicturePropertySquare(g, nameFont, labelFont, topFormat, bottomFormat,
                                   textBrush, linePen, trainImage, subRect,
                                   ++square, 200);
        // 36 Chance
        g.TranslateTransform(-squareWidthY, 0);
        PaintChance(g, nameFont, chanceFont, topFormat, centerFormat,
                    textBrush, chanceBrush, linePen, outlinePen, subRect,
                    ++square);
        // 37 Park Place
        g.TranslateTransform(-squareWidthY, 0);
        PaintProperty(g, nameFont, labelFont, topFormat, bottomFormat,
                      textBrush, linePen, subRect,
                      ++square, ++group, 350);
        // 38 Luxury Tax
        g.TranslateTransform(-squareWidthY, 0);
        PaintTax(g, bigSquareFont, decorativeFont, labelFont,
                 topFormat, centerFormat, bottomFormat,
                 textBrush, linePen, subRect,
                 ++square, "LUXURY TAX", "Pay $75");
        // 39 Boardwalk
        g.TranslateTransform(-squareWidthY, 0);
        PaintProperty(g, nameFont, labelFont, topFormat, bottomFormat,
                      textBrush, linePen, subRect,
                      ++square, group, 400);
        // walls
        PaintSquareWalls(g, 0, squareHeightX, 0, squareWidthY, 9, linePen);

        // draw the top lines
        ResetTransform(g, ClientRectangle);
        g.DrawLine(linePen, 0.0F, r.Height - squareHeightY, r.Width, r.Height - squareHeightY);
        g.DrawLine(linePen, squareHeightX, 0.0F, squareHeightX, r.Height);
        g.DrawLine(linePen, 0.0F, squareHeightY, r.Width, squareHeightY);
        g.DrawLine(linePen, r.Width - squareHeightX, 0.0F, r.Width - squareHeightX, r.Height);
        
        // text in the middle
        ResetTransform(g, ClientRectangle);
        g.TranslateTransform(r.Width * 0.5F, r.Height * 0.5F);
        g.RotateTransform(-45);
        g.TranslateTransform(-r.Width * 0.5F, -r.Height * 0.5F);
        path.Reset();
        path.AddString(Text, titleFont.FontFamily,
                       (int)titleFont.Style, titleFont.Size,
                       r, centerFormat);
        g.DrawPath(lineShadowPen, path);
        g.TranslateTransform(-linePen.Width / 2, -linePen.Width / 2);
        g.DrawPath(linePen, path);

        // now the bits that don't rotate
        g.ResetTransform();

        // construction lines
        Pen edgePen = new Pen(ForeColor, d * 0.005F);
        edgePen.SetLineCap(LineCap.Square, LineCap.Square, DashCap.Flat);
        edgePenWidth = edgePen.Width;
        g.DrawPolygon(edgePen, new PointF[] {
            // the -0.5F is to slightly reduce the weight of the top
            // and left lines, so that they look better when placed
            // inside an inset edge.
            new PointF(-0.5F, -0.5F),
            new PointF(ClientRectangle.Width, -0.5F),
            new PointF(ClientRectangle.Width, ClientRectangle.Height),
            new PointF(-0.5F, ClientRectangle.Height),
            new PointF(-0.5F, -0.5F),
        });

        // for this we reset |r| and the square heights to be the real dimensions, not the rotated ones
        r = new RectangleF(0, 0, ClientRectangle.Width-1, ClientRectangle.Height-1);
        squareHeightY = r.Height * squareHeight;
        squareHeightX = r.Width * squareHeight;

        // Dice
        diceSize = Math.Min(squareHeightX, squareHeightY) * 0.5F;
        g.DrawString("Dice:", labelFont, textBrush,
                     new RectangleF(r.Width - squareHeightX - diceSize * 2.8F,
                                    r.Height - squareHeightY - diceSize * 1.5F
                                             - labelFont.Size * 1.3F,
                                    diceSize*2.3F, labelFont.Height),
                     topLeftFormat);        
        g.FillClosedCurve(Brushes.Maroon, new PointF[] {
            new PointF(r.Width - squareHeightX - diceSize * 1.8F,
                       r.Height - squareHeightY - diceSize * 0.5F),
            new PointF(r.Width - squareHeightX - diceSize * 2.8F,
                       r.Height - squareHeightY - diceSize * 0.5F),
            new PointF(r.Width - squareHeightX - diceSize * 2.8F,
                       r.Height - squareHeightY - diceSize * 1.5F),
            new PointF(r.Width - squareHeightX - diceSize * 1.8F,
                       r.Height - squareHeightY - diceSize * 1.5F),
        }, FillMode.Winding, 0.1F);
        g.FillClosedCurve(Brushes.Maroon, new PointF[] {
            new PointF(r.Width - squareHeightX - diceSize * 0.5F,
                       r.Height - squareHeightY - diceSize * 0.5F),
            new PointF(r.Width - squareHeightX - diceSize * 1.5F,
                       r.Height - squareHeightY - diceSize * 0.5F),
            new PointF(r.Width - squareHeightX - diceSize * 1.5F,
                       r.Height - squareHeightY - diceSize * 1.5F),
            new PointF(r.Width - squareHeightX - diceSize * 0.5F,
                       r.Height - squareHeightY - diceSize * 1.5F),
        }, FillMode.Winding, 0.1F);

        // draw icons and states other than highlight
        iconDimension = squareDimension / 5;
        for (square = 0; square < SquareNames.Length; ++square) {
            // icon
            if (squareIcons[square] != null && squareIconSizes[square] == 0.0F) {
                subRect = squareAreas[square].GetBounds(g);
                g.DrawImage(squareIcons[square], new PointF[] {
                    new PointF(subRect.Right - iconDimension, subRect.Bottom - iconDimension),
                    new PointF(subRect.Right, subRect.Bottom - iconDimension),
                    new PointF(subRect.Right - iconDimension, subRect.Bottom),
                }, new RectangleF(new PointF(0, 0), squareIcons[square].Size),
                            GraphicsUnit.Pixel, iconAttributes);
            }
            // states
            if ((squareStates[square] & SquareState.Mortgaged) != 0)
                g.FillRegion(mortgagedBrush, squareAreas[square]);
            if ((squareStates[square] & SquareState.WindowOpen) != 0)
                g.FillRegion(windowOpenBrush, squareAreas[square]);
        }

        // The Chance or Community Chest card
        if (card != null)
            card.Paint(g,
                       new RectangleF(squareHeightX * 1.4F, squareHeightY * 1.4F,
                                      Math.Min(cardTextFont.Size * 43F * 0.6F,
                                               r.Width - squareHeightX * 2.8F),
                                      Math.Min(cardTextFont.Size * 26F * 0.6F,
                                               r.Height - squareHeightY * 2.8F - diceSize * 2.0F)),
                       labelFont, cardTitleFont, cardTextFont, ForeColor);
        
        // The pot amount
        if (pot > 0) {
            subRect = new RectangleF(squareHeightX, squareHeightY,
                                     r.Width - squareHeightX * 2.0F,
                                     r.Height - squareHeightY * 2.0F);
            subRect.Inflate(-squareHeightX * padding, -squareHeightY * padding);
            StringFormat potFormat = new StringFormat();
            potFormat.HotkeyPrefix = HotkeyPrefix.None;
            potFormat.Trimming = StringTrimming.EllipsisCharacter;
            switch (rotation) {
            default:
            case Angle.d0:
                potFormat.Alignment = StringAlignment.Near;
                potFormat.LineAlignment = StringAlignment.Near;
                break;
            case Angle.d90:
                potFormat.Alignment = StringAlignment.Far;
                potFormat.LineAlignment = StringAlignment.Near;
                break;
            case Angle.d180:
                potFormat.Alignment = StringAlignment.Far;
                potFormat.LineAlignment = StringAlignment.Far;
                break;
            case Angle.d270:
                potFormat.Alignment = StringAlignment.Near;
                potFormat.LineAlignment = StringAlignment.Far;
                break;
            }
            g.DrawString(String.Format("Pot: ${0}", pot), labelFont, textBrush,
                         subRect, potFormat);
        }

        g.Dispose();

    }

    private Random random = new Random();

    protected override void OnPaint(PaintEventArgs e) {

        // more convenient access to the graphics object
        Graphics g = e.Graphics;

        try {

            // The font constructor doesn't accept Display units (don't
            // ask me why). The page units have to be the same as the font
            // units so that the paths work right. Therefore the page
            // units can't be Display units. In DoubleBuffer mode the
            // default unit is Display. So here we set it to Pixel.
            g.PageUnit = GraphicsUnit.Pixel;

            if (boardCache != null && boardCache.Size != ClientSize)
                boardCache = null;
            if (boardCache == null)
                PaintBoard(g);

            g.DrawImageUnscaled(boardCache, 0, 0);

            // Enable High Quality Smoothing
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
              // ClearTypeGridFit is not supported on Win2k, and
              // AntiAliasGridFit doesn't seem to work on rotated text.
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            // g.PixelOffsetMode, g.CompositingMode, g.CompositingQuality XXX

            // dice
            Font diceFont = new Font("Tahoma", diceSize, FontStyle.Bold, g.PageUnit);
            // die1
            String die1Text = diceFrames > 0 ? random.Next(1, 7).ToString()
                                             : die1.ToString();
            g.DrawString(die1Text, diceFont, Brushes.White,
                         new RectangleF(ClientRectangle.Width - squareHeightX - diceSize * 2.8F,
                                        ClientRectangle.Height - squareHeightY - diceSize * 1.5F,
                                        diceSize, diceSize), centerFormat);

            // die2
            String die2Text = diceFrames > 0 ? random.Next(1, 7).ToString()
                                             : die2.ToString();

            g.DrawString(die2Text, diceFont, Brushes.White,
                         new RectangleF(ClientRectangle.Width - squareHeightX - diceSize * 1.5F,
                                        ClientRectangle.Height - squareHeightY - diceSize * 1.5F,
                                        diceSize, diceSize), centerFormat);

            // currently animated icons and highlight (but not other square states)
            for (int square = 0; square < SquareNames.Length; ++square) {
                if ((squareStates[square] & SquareState.Highlighted) != 0)
                    g.FillRegion(highlightBrush, squareAreas[square]);
                if (squareIcons[square] != null && squareIconSizes[square] != 0.0F) {
                    float z = iconDimension * squareIconSizes[square];
                    RectangleF subRect = squareAreas[square].GetBounds(g);
                    g.DrawImage(squareIcons[square], new PointF[] {
                        new PointF(subRect.Right - z, subRect.Bottom - z),
                        new PointF(subRect.Right, subRect.Bottom - z),
                        new PointF(subRect.Right - z, subRect.Bottom),
                    }, new RectangleF(new PointF(0, 0), squareIcons[square].Size),
                                GraphicsUnit.Pixel, iconAttributes);
                }
            }

            // draw pieces
            float pieceDimension = squareDimension / 3;
            foreach (Piece p in pieces.Values) {
                RectangleF subRect = squareAreas[p.LastSquare].GetBounds(g);
                subRect = GetPiecePosition(squareAreas[p.CurrentSquare].GetBounds(g), p.CurrentPosition, p.CurrentSquare == 10, p.CurrentInJail, pieceDimension, g);
                if (p.Position < 1.0) {
                    RectangleF last = GetPiecePosition(squareAreas[p.LastSquare].GetBounds(g), p.LastPosition, p.LastSquare == 10, p.LastInJail, pieceDimension, g);
                    subRect = GetTransitionPosition(last, subRect, p.Position, out p.Distance);
                }
                p.Area = PaintPiece(g, subRect, p.Image);
            }

            // Animated Text
            Font animatedTextFont = new Font("Tahoma", d * 0.03F, FontStyle.Bold, g.PageUnit);
            foreach (AnimatedText a in animatedText)
                a.Paint(g, ClientRectangle, animatedTextFont, rotation, animatedHeight, animatedCircleRadius);

            // focus outlines
            if (Focused) {
                if (focussedSquare >= 0 && focussedSquare < squareAreas.Length) {
                    RectangleF subRect = squareAreas[focussedSquare].GetBounds(g);
                    subRect.Inflate(-edgePenWidth, -edgePenWidth);
                    g.DrawPolygon(focusPen, new PointF[] {
                        new PointF(subRect.Left, subRect.Top),
                        new PointF(subRect.Left + subRect.Width, subRect.Top),
                        new PointF(subRect.Left + subRect.Width, subRect.Top + subRect.Height),
                        new PointF(subRect.Left, subRect.Top + subRect.Height),
                        new PointF(subRect.Left, subRect.Top),
                    });
                }
            }

        } catch (Exception E) {
            g.FillRectangle(Brushes.White, ClientRectangle);
            g.DrawString(E.ToString(), Font, Brushes.Black, ClientRectangle);
            boardCache = null;
        }

    }

    protected Timer animationTimer;

    public float WalkSpeed = 0.25F;
    public float TextSpeed = 0.02F;
    public float IconSpeed = 0.10F;
    public float FrameRate {
        get { return 1000.0F / animationTimer.Interval; }
        set { animationTimer.Interval = Convert.ToInt32(1000.0F / value); }
    }

    public bool AnimationEnabled = true;

    protected void Animate(Object sender, EventArgs e) {
        bool moved = false;
        bool done = true;
        bool animatingImportant = false;
        // pieces
        foreach (Piece p in pieces.Values) {
            if (p.Position < 1.0F) {
                p.Position += WalkSpeed / p.Distance;
                moved = true;
                if (p.Position >= 1.0F) {
                    p.Position = 1.0F;
                    if (p.Queue.Count > 0) {
                        Piece.Pending next = (Piece.Pending)(p.Queue.Dequeue());
                        MovePiece(p.ID, next.square, next.jail);
                        done = false;
                        animatingImportant = true;
                    }
                } else {
                    done = false;
                    animatingImportant = true;
                }
            }
        }
        // text
        if (animatedText.Count > 0) {
            moved = true;
            foreach (AnimatedText a in animatedText)
                a.position += TextSpeed;
            while (animatedText.Count > 0 && (animatedText.Peek() as AnimatedText).position >= 1.0F)
                animatedText.Dequeue();
        }
        // animate icons
        for (int square = 0; square < SquareNames.Length; ++square) {
            if (squareIcons[square] != null && squareIconSizes[square] > 1.0F) {
                moved = true;
                animatingImportant = true;
                squareIconSizes[square] -= IconSpeed;
                if (squareIconSizes[square] < 1.0F)
                    squareIconSizes[square] = 1.0F;
            }
        }
        // dice
        if (diceFrames > 0) {
            --diceFrames;
            moved = true;
            animatingImportant = true;
        }
        // update user
        if (moved || sender == null) {
            Invalidate();
        } else if (done) {
            animationTimer.Stop();
            bool repaint = false;
            for (int square = 0; square < SquareNames.Length; ++square)
                if (squareIcons[square] != null &&
                    squareIconSizes[square] != 0.0F) {
                    squareIconSizes[square] = 0.0F;
                    repaint = true;
                }
            if (repaint)
                UpdateCache();
        }
        if (animatingImportant != isAnimating) {
            if (isAnimating)
                OnAnimationsDone(new EventArgs());
            isAnimating = animatingImportant;
        }
    }

    public event EventHandler AnimationsDone;
    protected virtual void OnAnimationsDone(EventArgs e) {
        if (AnimationsDone != null)
            AnimationsDone(this, e);
    }
    
    protected bool isAnimating = false;
    public bool IsAnimating { get { return isAnimating; } }

}
