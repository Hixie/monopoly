// -*- Mode: Java -*-

// This is one big mess of interdependent classes updating themselves
// left right and center. It should be done using events and stuff.

using System;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;

public abstract class Thing {
    public abstract void ShowWindow(MonopolyForm parent);
    public abstract void ToggleWindow(MonopolyForm parent);
}

public class Property : Thing {
    public readonly int id;
    public readonly int type;
    public readonly int price;
    public readonly int houses;
    public readonly int mortgage;
    public readonly int rent0;
    public readonly int rent1;
    public readonly int rent2;
    public readonly int rent3;
    public readonly int rent4;
    public readonly int rent5;
    public readonly int group;
    public readonly int square;
    protected MonopolyBoard board;
    public Property(int id, int type, int price, int houses, int mortgage,
                    int rent0, int rent1, int rent2, int rent3, int rent4,
                    int rent5, int group, MonopolyBoard board, int square) {
        this.id = id;
        this.type = type;
        this.price = price;
        this.houses = houses;
        this.mortgage = mortgage;
        this.rent0 = rent0;
        this.rent1 = rent1;
        this.rent2 = rent2;
        this.rent3 = rent3;
        this.rent4 = rent4;
        this.rent5 = rent5;
        this.group = group;
        this.board = board;
        this.square = square;
    }
    public String Name { get { return board.GetSquareName(square); } }
    public Color ForeColor { get { return board.GetGroupTextColor(group); } }
    public Color BackColor { get { return board.GetGroupColor(group); } }

    protected int currentHouses = 0;
    public int CurrentHouses { get { return currentHouses; } }
    protected int currentHotels = 0;
    public int CurrentHotels { get { return currentHotels; } }
    public void SetHouses(int houses, int hotels) {
        currentHouses = houses;
        currentHotels = hotels;
        board.SetHouses(square, houses, hotels);
        if (window != null)
            window.UpdateWindow();
        if (Owner != null)
            Owner.Update(this);
    }
    protected bool mortgaged = false;
    public bool Mortgaged {
        get { return mortgaged; }
        set {
            if (mortgaged != value) {
                mortgaged = value;
                board.SetState(square, MonopolyBoard.SquareState.Mortgaged, value);
                if (window != null)
                    window.UpdateWindow();
                if (Owner != null)
                    Owner.Update(this);
            }
        }
    }

    public Control QuickView;

    protected Player owner = null;
    public Player Owner {
        get { return owner; }
        set {
            if (owner == value)
                return;
            if (owner != null)
                owner.Remove(this);
            owner = value;
            if (owner != null)
                owner.Add(this);
            board.SetIcon(square, owner == null ? null : owner.Image);
            if (window != null)
                window.UpdateWindow();
            QuickView.Visible = (owner != null) && (owner.IsUs);
        }
    }
    public bool ownerSetDuringState;

    protected PropertyForm window;
    public override void ShowWindow(MonopolyForm parent) {
        if (window == null || window.IsDisposed) {
            window = new PropertyForm(parent, this, board);
        }
        window.Show();
    }
    public override void ToggleWindow(MonopolyForm parent) {
        if (window == null || window.IsDisposed || !window.Visible) {
            ShowWindow(parent);
        } else {
            window.Hide();
        }
    }

    public void Clear() {
        // no need to update the player or the board
        currentHouses = 0;
        currentHotels = 0;
        mortgaged = false;
        owner = null;
        QuickView.Visible = false;
        if (window != null)
            window.UpdateWindow();
    }
}

public class Card : Thing {
    public readonly int id;
    public readonly int type;
    public readonly int pile;
    public readonly int data1;
    public readonly int data2;
    public readonly String text;
    public Card(int id, int type, int pile, int data1, int data2, String text) {
        this.id = id;
        this.type = type;
        this.pile = pile;
        this.data1 = data1;
        this.data2 = data2;
        this.text = text;
    }

    protected Player owner = null;
    public Player Owner {
        get { return owner; }
        set {
            if (owner == value)
                return;
            if (owner != null)
                owner.Remove(this);
            owner = value;
            if (owner != null)
                owner.Add(this);
            if (window != null)
                window.UpdateWindow();
        }
    }
    public bool ownerSetDuringState;

    protected Player picker = null;
    public Player Picker {
        get { return picker; }
        set {
            if (picker == value)
                return;
            picker = value;
            if (window != null)
                window.UpdateWindow();
        }
    }

    public String Title {
        get { return pile == 0 ? "Chance" : "Community Chest"; }
    }

    protected CardForm window;
    public override void ShowWindow(MonopolyForm parent) {
        if (window == null || window.IsDisposed) {
            window = new CardForm(parent, this);
        }
        window.Show();
    }
    public override void ToggleWindow(MonopolyForm parent) {
        if (window == null || window.IsDisposed || !window.Visible) {
            ShowWindow(parent);
        } else {
            window.Hide();
        }
    }

    public void Clear() {
        // no need to update the player or the board
        owner = null;
        if (window != null)
            window.UpdateWindow();
    }

    // OnPaint helper routine
    public void Paint(Graphics g, RectangleF ClientRectangle, Font font1, Font font2, Font font3,
                      Color color) {
        RectangleF r = new RectangleF(ClientRectangle.Location, ClientRectangle.Size);
        r.Inflate(-font1.Size * 2F, -font1.Size * 2F);
        r.Height -= font1.Size * 2F;
        float shadow = ClientRectangle.Width / 50;
        r.Offset(shadow / 2F, shadow / 2F);
        g.FillRectangle(new SolidBrush(Color.FromArgb(128, 0, 0, 0)), r);
        r.Offset(-shadow, -shadow);
        g.FillRectangle(new SolidBrush(Color.White), r);
        Pen pen = new Pen(color, ClientRectangle.Width / 200);
        g.DrawPolygon(pen, new PointF[] {
            new PointF(r.Left, r.Top), 
            new PointF(r.Right, r.Top), 
            new PointF(r.Right, r.Bottom), 
            new PointF(r.Left, r.Bottom), 
            new PointF(r.Left, r.Top), 
        });
        r.Width -= pen.Width;
        r.Height -= pen.Width;

        StringFormat centerFormat = new StringFormat();
        centerFormat.Alignment = StringAlignment.Center;
        centerFormat.LineAlignment = StringAlignment.Center;
        StringFormat leftFormat = new StringFormat();
        leftFormat.Alignment = StringAlignment.Near;
        leftFormat.LineAlignment = StringAlignment.Near;

        RectangleF subRect = new RectangleF(r.Location, r.Size);
        subRect.Inflate(-font3.Size, -font3.Size);
        subRect.Height = font2.Height;
        g.DrawString(Title, font2, Brushes.Black, subRect, centerFormat);

        subRect = new RectangleF(r.Location, r.Size);
        subRect.Inflate(-font3.Size, -font3.Size);
        subRect.Y += font2.Height + font3.Size / 2;
        subRect.Height -= font2.Height + font3.Size / 2;
        g.DrawString(text, font3, Brushes.Black, subRect, leftFormat);

        r = new RectangleF(ClientRectangle.Left, ClientRectangle.Bottom - font1.Size * 4F,
                           ClientRectangle.Width, font1.Size * 4F);
        if (owner != null) {
            g.DrawString(String.Format("Owned by {0}", owner.Name),
                         font1, new SolidBrush(color), r, centerFormat);
        } else if (picker != null) {
            g.DrawString(String.Format("Picked by {0}", picker.Name),
                         font1, new SolidBrush(color), r, centerFormat);
        } else {
            g.DrawString("Unowned",
                         font1, new SolidBrush(color), r, centerFormat);
        }
    }    

}

public class Player : Thing {
    protected int id;
    public int ID { get { return id; } }
    protected String name;
    public String Name { get { return name; } }
    protected int piece;
    public int Piece {
        get { return piece; }
        set { piece = value; }
    }
    protected Image image;
    public Image Image {
        get { return image; }
        set {
            image = value;
            if (window != null)
                window.UpdateWindow(false);
        }
    }
    protected bool playing;
    public bool Playing {
        get { return playing; }
        set { playing = value; }
    }
    protected uint cash;
    public uint Cash {
        get { return cash; }
        set {
            cash = value;
            item.SubItems[1].Text = "$" + value;
            if (window != null)
                window.UpdateWindow(false);
        }
    }

    protected HybridDictionary properties;
    public HybridDictionary Properties { get { return properties; } }
    public void Add(Property property) {
        properties.Add(property.id, property);
        if (HouseBuyingForm != null)
            HouseBuyingForm.UpdateWindow(false);
        if (window != null)
            window.UpdateWindow(true);
    }
    public void Remove(Property property) {
        properties.Remove(property.id);
        if (HouseBuyingForm != null)
            HouseBuyingForm.UpdateWindow(false);
        if (window != null)
            window.UpdateWindow(true);
    }

    protected HybridDictionary cards;
    public HybridDictionary Cards { get { return cards; } }
    public void Add(Card card) {
        cards.Add(card.id, card);
        if (window != null)
            window.UpdateWindow(true);
    }
    public void Remove(Card card) {
        cards.Remove(card.id);
        if (window != null)
            window.UpdateWindow(true);
    }

    protected int square;
    public int Square {
        get { return square; }
        set { square = value; }
    }
    protected bool inJail;
    public bool InJail {
        get { return inJail; }
        set { inJail = value; }
    }
    public void MoveImmediately(int square, bool inJail) {
        this.square = square;
        this.inJail = inJail;
        if (playing)
            board.MovePieceImmediately(id, square, inJail);
    }

    protected bool isUs;
    public bool IsUs { get { return isUs; } }
    protected ListViewItem item;
    protected MonopolyBoard board;
    public ListViewItem Item { get { return item; } }
    public Player(int id, String name, int piece, bool playing,
                  uint cash, int square, bool jail, bool isUs,
                  MonopolyBoard board, Image image) {
        this.id = id;
        this.name = name;
        this.piece = piece;
        this.playing = playing;
        this.properties = new HybridDictionary(5);
        this.cards = new HybridDictionary(2);
        this.cash = cash;
        this.square = square;
        this.inJail = jail;
        this.isUs = isUs;
        item = new ListViewItem(new String[] {name, "$" + cash});
        if (playing)
            item.ImageIndex = piece;
        item.Tag = this;
        this.board = board;
        this.image = image;
    }

    protected PlayerForm window;
    public override void ShowWindow(MonopolyForm parent) {
        if (window == null || window.IsDisposed) {
            window = new PlayerForm(parent, this, board);
        }
        window.Show();
    }
    public override void ToggleWindow(MonopolyForm parent) {
        if (window == null || window.IsDisposed || !window.Visible) {
            ShowWindow(parent);
        } else {
            window.Hide();
        }
    }

    public void Update(Property property) {
        if (HouseBuyingForm != null)
            HouseBuyingForm.UpdateWindow(false);
        if (window != null)
            window.UpdateWindow(false);
    }

    public void Clear() {
        // no need to clear the list view or the board
        if (window != null) {
            window.Dispose();
            window = null;
        }
    }

    // only relevant if isUs
    public HouseBuyingForm HouseBuyingForm;

}
