// -*- Mode: Java -*-

using System;
using System.Net;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;

public class FloatingForm : Form {

    public FloatingForm(Form parent) : base() {
        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        MinimizeBox = false;
        MaximizeBox = false;
        ShowInTaskbar = false;
        parent.AddOwnedForm(this);
    }

    protected override void OnClosing(CancelEventArgs e) {
        if (Visible) {
            // we don't want user triggered closing to dispose us
            e.Cancel = true;
            base.OnClosing(e);
            base.OnClosed(e); // propogate that event too, even though we didn't do it
            Hide();
        } else {
            base.OnClosing(e);
        }
    }

    public void ShrinkWrap(Button button) {
        Label testLabel = new Label();
        testLabel.Text = button.Text;
        testLabel.Font = button.Font;
        button.Width = testLabel.PreferredWidth + 16;
    }

}

public class PropertyForm : FloatingForm {
    
    protected MonopolyBoard board;
    protected Property property;

    public PropertyForm(Form parent, Property property, MonopolyBoard board) : base(parent) {
        this.board = board;
        this.property = property;

        Font = new Font("Tahoma", Font.Size);
        ClientSize = new Size(Convert.ToInt32(Font.Size * 40F),
                              Convert.ToInt32(Font.Size * 49F));

        SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        SetStyle(ControlStyles.DoubleBuffer, true);
        SetStyle(ControlStyles.ResizeRedraw, true);
        SetStyle(ControlStyles.UserPaint, true); 

        Text = board.GetSquareName(property.square);
        UpdateWindow();
    }

    override protected void OnVisibleChanged(EventArgs e) {
        base.OnVisibleChanged(e);
        board.SetState(property.square, MonopolyBoard.SquareState.WindowOpen, Visible);
    }

    public void UpdateWindow() {
        // XXX control to turn the card over?
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e) {

        Graphics g = e.Graphics;

        // keep this consistent with the board
        g.SmoothingMode = SmoothingMode.HighQuality;
        g.TextRenderingHint = TextRenderingHint.AntiAlias;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        // g.PixelOffsetMode XXX

        Brush brush = new SolidBrush(Color.Black);
        Brush red = new SolidBrush(Color.Red);

        Font font0 = new Font(Font.FontFamily, Font.Size * 3.5F, FontStyle.Bold);
        Font font1 = new Font(Font.FontFamily, Font.Size * 1.8F);
        Font font2 = new Font(Font.FontFamily, Font.Size * 1.5F);
        Font font3 = new Font(Font.FontFamily, Font.Size * 0.9F);
        Font font4 = new Font(Font.FontFamily, Font.Size * 1.2F, FontStyle.Bold);

        RectangleF r = new RectangleF(ClientRectangle.Location, ClientRectangle.Size);
        r.Inflate(-font4.Size * 2F, -font4.Size * 2F);
        r.Height -= font4.Size * 2F;
        float shadow = ClientRectangle.Width / 50;
        r.Offset(shadow / 2F, shadow / 2F);
        g.FillRectangle(new SolidBrush(Color.FromArgb(128, 0, 0, 0)), r);
        r.Offset(-shadow, -shadow);
        g.FillRectangle(new SolidBrush(Color.White), r);
        Pen pen = new Pen(ForeColor, ClientRectangle.Width / 200);
        g.DrawPolygon(pen, new PointF[] {
            new PointF(r.Left, r.Top), 
            new PointF(r.Right, r.Top), 
            new PointF(r.Right, r.Bottom), 
            new PointF(r.Left, r.Bottom), 
            new PointF(r.Left, r.Top), 
        });
        r.Width -= pen.Width;
        r.Height -= pen.Width;

        StringFormat bottomFormat = new StringFormat();
        bottomFormat.Alignment = StringAlignment.Center;
        bottomFormat.LineAlignment = StringAlignment.Far;
        StringFormat topFormat = new StringFormat();
        topFormat.Alignment = StringAlignment.Center;
        topFormat.LineAlignment = StringAlignment.Near;
        StringFormat centerFormat = new StringFormat();
        centerFormat.Alignment = StringAlignment.Center;
        centerFormat.LineAlignment = StringAlignment.Center;
        StringFormat leftFormat = new StringFormat();
        leftFormat.Alignment = StringAlignment.Near;
        leftFormat.LineAlignment = StringAlignment.Near;
        StringFormat rightFormat = new StringFormat();
        rightFormat.Alignment = StringAlignment.Far;
        rightFormat.LineAlignment = StringAlignment.Near;

        if (property.Mortgaged) {

            RectangleF subRect = new RectangleF(r.Left, font0.Size, r.Width,
                                                r.Height - font0.Size * 2);
                
            g.DrawString("MORTGAGED", font0, red, subRect, topFormat);

            subRect = new RectangleF(r.Left, r.Top - Font.Size,
                                     r.Width, r.Height - Font.Size * 5);
            String s = board.GetSquareName(property.square).ToUpper();
            CharacterRange[] charRanges = new CharacterRange[] {
                new CharacterRange(0, s.Length),
            };
            centerFormat.SetMeasurableCharacterRanges(charRanges);
            Region[] charRegions = g.MeasureCharacterRanges(s, font1, subRect,
                                                            centerFormat);
            g.DrawString(s, font1, red, subRect, centerFormat);
            RectangleF r2 = charRegions[0].GetBounds(g);
            r2.Inflate(0, Font.Size/3);
            Pen redPen = new Pen(Color.Red);
            g.DrawLine(redPen, r2.Left, r2.Top, r2.Right, r2.Top);
            g.DrawLine(redPen, r2.Left, r2.Bottom, r2.Right, r2.Bottom);

            subRect.Y += Font.Size * 5;
            g.DrawString(String.Format("Mortgaged for ${0}", property.mortgage),
                         font2, red, subRect, centerFormat);

            subRect = new RectangleF(r.Left, r.Top + r.Height - Font.Size * 6F,
                                     r.Width, Font.Size * 6F);
            g.DrawString("CARD MUST BE TURNED THIS SIDE\nUP IF PROPERTY IS MORTGAGED",
                         font3, red, subRect, centerFormat);

        } else {

            switch (property.type) {
            case 0: {

                // TITLE

                RectangleF subRect = new RectangleF(r.Location, r.Size);
                subRect.Inflate(-Font.Size / 2.0F, -Font.Size / 2.0F);
                subRect.Height = Font.Size * 7.0F;
                g.FillRectangle(new SolidBrush(property.BackColor), subRect);
                Brush color = new SolidBrush(property.ForeColor);

                subRect.Inflate(0, -Font.Size);
                g.DrawString("TITLE DEED", font3, color, subRect, topFormat);
                subRect.Y += Font.Size / 4;
                g.DrawString(board.GetSquareName(property.square).ToUpper(), font1,
                             color, subRect, bottomFormat);

                // RENTS

                subRect = new RectangleF(r.Location, r.Size);
                subRect.Inflate(-Font.Size, -Font.Size / 2);
                subRect.Height -= Font.Size * 4.5F;
                subRect.Y += Font.Size * 8F;

                // "Rent --"
                CharacterRange[] charRanges = new CharacterRange[] {
                    new CharacterRange(0, 3),
                    new CharacterRange(0, 4),
                };
                leftFormat.SetMeasurableCharacterRanges(charRanges);
                String s = "RENT\u2014";
                Region[] charRegions = g.MeasureCharacterRanges(s, font2,
                                                                subRect, leftFormat);
                g.DrawString(s, font2, brush, subRect, leftFormat);

                // ditto marks
                s = "\u2033\n\u2033\n\u2033\n\u2033\n\u2033";
                RectangleF r2 = charRegions[0].GetBounds(g);
                r2.Y += r2.Height;
                r2.Height *= 5;
                g.DrawString(s, font2, brush, r2, topFormat);

                // "Site only" and "With" bits
                s = "Site only\nWith";
                r2 = charRegions[1].GetBounds(g);
                subRect.X += r2.Width;
                subRect.Width -= r2.Width;
                charRanges = new CharacterRange[] {
                    new CharacterRange(10, 4),
                };
                leftFormat.SetMeasurableCharacterRanges(charRanges);
                charRegions = g.MeasureCharacterRanges(s, font2, subRect, leftFormat);
                g.DrawString(s, font2, brush, subRect, leftFormat);

                // more ditto marks
                s = "\u2033\n\u2033\n\u2033\n\u2033";
                r2 = charRegions[0].GetBounds(g);
                r2.Y += r2.Height;
                r2.Height *= 4;
                g.DrawString(s, font2, brush, r2, topFormat);

                // "1 house" "2 houses" etc
                s = "1 House\n2 Houses\n3 Houses\n4 Houses\nHOTEL";
                r2 = charRegions[0].GetBounds(g);
                r2.X += r2.Width;
                r2.Width = subRect.Width - r2.Width;
                r2.Height *= 5;
                g.DrawString(s, font2, brush, r2, leftFormat);

                // prices
                String s0 = "$" + property.rent0 + "\n";
                String s1 = "$" + property.rent1 + "\n";
                String s2 = "$" + property.rent2 + "\n";
                String s3 = "$" + property.rent3 + "\n";
                String s4 = "$" + property.rent4 + "\n";
                String s5 = "$" + property.rent5 + "\n";
                s = s0 + s1 + s2 + s3 + s4 + s5;

                if (property.Owner != null) {
                    int i = 0;
                    charRanges = new CharacterRange[] {
                        new CharacterRange(i,              s0.Length - 1),
                        new CharacterRange(i += s0.Length, s1.Length - 1),
                        new CharacterRange(i += s1.Length, s2.Length - 1),
                        new CharacterRange(i += s2.Length, s3.Length - 1),
                        new CharacterRange(i += s3.Length, s4.Length - 1),
                        new CharacterRange(i += s4.Length, s5.Length - 1),
                    };
                    rightFormat.SetMeasurableCharacterRanges(charRanges);
                    charRegions = g.MeasureCharacterRanges(s, font2,
                                                           subRect, rightFormat);
                    
                    RectangleF rh = charRegions[property.CurrentHouses +
                                                5*property.CurrentHotels].GetBounds(g);
                    rh.Inflate(Font.Size * 0.75F, 2F);
                    g.FillRectangle(Brushes.Yellow, rh);
                }

                g.DrawString(s, font2, brush, subRect, rightFormat);

                // bottom part
                // first costs
                subRect = new RectangleF(r.Left + Font.Size,
                                         r2.Bottom + Font.Size * 2,
                                         r.Width - Font.Size * 2,
                                         r.Bottom - r2.Bottom - Font.Size * 3);
                g.DrawString(String.Format("Mortgage Value: ${0}\nHouses cost ${1} each\nHotels, ${1} plus 4 houses", 
                                           property.mortgage, property.houses),
                             font2, brush, subRect, topFormat);

                // then, the "If a player yada yada" small print
                g.DrawString("If a player owns ALL the Sites of any Color-Group, the rent is Doubled on Unimproved Sites in that group.", font3, brush, subRect, bottomFormat);
                break;
            }
            case 1: {

                RectangleF subRect = new RectangleF(r.Location, r.Size);
                subRect.Inflate(-Font.Size / 2.0F, -Font.Size / 2.0F);
                subRect.Height = Font.Size * 7.0F;

                subRect.Inflate(0, -Font.Size);
                g.DrawString(board.GetSquareName(property.square).ToUpper(),
                             font1, brush, subRect, topFormat);
                subRect.Y += Font.Size / 4;
                g.DrawString("Railroad", font3, brush, subRect, bottomFormat);

                subRect = new RectangleF(r.Location, r.Size);
                subRect.Inflate(-Font.Size, -Font.Size / 2.0F);
                subRect.Height -= Font.Size * 4.5F;
                subRect.Y += Font.Size * 9F;

                g.DrawString("Rent\n\nIf 2 Railways are owned\n\nIf 3 Railways are owned\n\nIf 4 Railways are owned\n\n\n\nMortgage Value", font2, brush, subRect, leftFormat);

                // prices
                String s0 = "$" + property.rent0 + "\n\n";
                String s1 = "$" + property.rent1 + "\n\n";
                String s2 = "$" + property.rent2 + "\n\n";
                String s3 = "$" + property.rent3 + "\n\n\n\n";
                String s4 = "$" + property.mortgage;
                String s = s0 + s1 + s2 + s3 + s4;

                if (property.Owner != null) {
                    int i = 0;
                    CharacterRange[] charRanges = new CharacterRange[] {
                        new CharacterRange(i,              s0.Length - 2),
                        new CharacterRange(i += s0.Length, s1.Length - 2),
                        new CharacterRange(i += s1.Length, s2.Length - 2),
                        new CharacterRange(i += s2.Length, s3.Length - 4),
                    };
                    rightFormat.SetMeasurableCharacterRanges(charRanges);
                    Region[] charRegions = g.MeasureCharacterRanges(s, font2, subRect, rightFormat);
                    
                    int count = 0;
                    foreach (Property p in property.Owner.Properties.Values) {
                        if (p.type == property.type && p.Owner == property.Owner)
                            ++count;
                    }
                    
                    RectangleF rh = charRegions[count-1].GetBounds(g);
                    rh.Inflate(Font.Size * 0.75F, 2F);
                    g.FillRectangle(Brushes.Yellow, rh);
                }

                g.DrawString(s, font2, brush, subRect, rightFormat);

                break;
            }
            case 2: {

                RectangleF subRect = new RectangleF(r.Location, r.Size);
                subRect.Inflate(-Font.Size / 2.0F, -Font.Size / 2.0F);
                subRect.Height = Font.Size * 7.0F;
                
                g.DrawString(board.GetSquareName(property.square).ToUpper(),
                             font1, brush, subRect, centerFormat);

                subRect = new RectangleF(r.Location, r.Size);
                subRect.Inflate(-Font.Size, -Font.Size / 2);
                subRect.Height -= Font.Size * 4.5F;
                subRect.Y += Font.Size * 8F;

                String s0 = "If one \"Utility\" is owned rent is 4 times amount shown on dice.\n\n\n";
                String s1 = "If both \"Utilities\" are owned rent is 10 times amount shown on dice.";
                String s = s0 + s1;

                if (property.Owner != null) {
                    int i = 0;
                    CharacterRange[] charRanges = new CharacterRange[] {
                        new CharacterRange(i,              s0.Length - 3),
                        new CharacterRange(i += s0.Length, s1.Length - 0),
                    };
                    leftFormat.SetMeasurableCharacterRanges(charRanges);
                    Region[] charRegions = g.MeasureCharacterRanges(s, font2, subRect, leftFormat);
                    
                    int count = 0;
                    foreach (Property p in property.Owner.Properties.Values) {
                        if (p.type == property.type && p.Owner == property.Owner)
                            ++count;
                    }

                    RectangleF rh = charRegions[count-1].GetBounds(g);
                    rh.X = subRect.Left;
                    rh.Width = subRect.Width;
                    rh.Inflate(Font.Size * 0.5F, Font.Size * 0.5F);
                    g.FillRectangle(Brushes.Yellow, rh);
                }

                g.DrawString(s, font2, brush, subRect, leftFormat);

                subRect.Y += subRect.Height - font2.Size * 5F;
                subRect.Height = font2.Size * 5F;
                g.DrawString("Mortgage Value", font2, brush, subRect, leftFormat);
                g.DrawString(String.Format("${0}", property.mortgage),
                             font2, brush, subRect, rightFormat);
                
                break;
            }
            }
        }
        r = new RectangleF(ClientRectangle.Left, ClientRectangle.Bottom - font4.Size * 4F,
                           ClientRectangle.Width, font4.Size * 4F);
        if (property.Owner == null) {
            g.DrawString("Unowned",
                         font4, new SolidBrush(ForeColor), r, centerFormat);
        } else {
            g.DrawString(String.Format("Owned by {0}", property.Owner.Name),
                         font4, new SolidBrush(ForeColor), r, centerFormat);
        }
    }

}

public class CardForm : FloatingForm {
    
    protected Card card;

    public CardForm(Form parent, Card card) : base(parent) {
        Font = new Font("Tahoma", Font.Size);
        ClientSize = new Size(Convert.ToInt32(Font.Size * 43F),
                              Convert.ToInt32(Font.Size * 26F));
        this.card = card;
        Text = card.Title;
    }

    public void UpdateWindow() {
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e) {
        // keep this consistent with the board
        e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
        e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
        e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        // e.Graphics.PixelOffsetMode XXX
        card.Paint(e.Graphics, ClientRectangle,
                   new Font(Font.FontFamily, Font.Size * 1.2F, FontStyle.Bold),
                   new Font(Font.FontFamily, Font.Size * 1.8F, FontStyle.Bold),
                   new Font(Font.FontFamily, Font.Size * 1.4F),
                   ForeColor);
    }

}

public class PropertySorter : IComparer {
    public int Compare(Object x, Object y) {
        if (x == null)
            return -1;
        if (y == null)
            return 1;
        if (x == y)
            return 0;
        int i = ((x as ListViewItem).Tag as Property).group.CompareTo(((y as ListViewItem).Tag as Property).group);
        if (i == 0)
            i = ((x as ListViewItem).Tag as Property).id.CompareTo(((y as ListViewItem).Tag as Property).id);
        return i;
    }
}

public class PlayerForm : FloatingForm {
    
    protected MonopolyBoard board;
    protected Player player;

    protected PictureBox picture;
    protected Label cash;
    protected ListView properties;
    protected ListView cards;

    protected ContextMenu propertiesContextMenu;
    protected MenuItem propertyOpenMenuItem;
    protected MenuItem propertyClaimRentMenuItem;
    protected MenuItem propertyMortgageMenuItem;
    protected MenuItem propertyUnmortgageMenuItem;
    protected MenuItem propertySellMenuItem;

    protected ContextMenu cardsContextMenu;
    protected MenuItem cardOpenMenuItem;

    public PlayerForm(MonopolyForm parent, Player player, MonopolyBoard board) : base(parent) {
        Width = Convert.ToInt32(Font.Size * 35F);
        this.board = board;
        this.player = player;
        DockPadding.All = 8;

        Label cashLabel = new Label();
        cashLabel.Text = "Cash: $";
        cashLabel.TextAlign = ContentAlignment.MiddleRight;
        picture = new PictureBox();
        picture.Location = new Point(DockPadding.Left, DockPadding.Top);
        picture.SizeMode = PictureBoxSizeMode.StretchImage;
        picture.Height = Math.Max(cashLabel.PreferredHeight, 32);
        picture.Width = picture.Height;
        cashLabel.Location = new Point(picture.Right + 8, picture.Top);
        cashLabel.Height = picture.Height;
        cashLabel.Width = cashLabel.PreferredWidth;
        cash = new Label();
        cash.UseMnemonic = false;
        cash.TextAlign = ContentAlignment.MiddleLeft;
        cash.Location = new Point(cashLabel.Right, cashLabel.Top);
        cash.Width = ClientRectangle.Width - DockPadding.Right - cashLabel.Right;
        cash.Height = cashLabel.Height;
        Label propertiesLabel = new Label();
        propertiesLabel.Text = "&Properties:";
        propertiesLabel.TextAlign = ContentAlignment.MiddleLeft;
        propertiesLabel.Location = new Point(DockPadding.Left,
                                             cashLabel.Bottom + 8);
        propertiesLabel.Width = propertiesLabel.PreferredWidth;
        propertiesLabel.Height = propertiesLabel.PreferredHeight;
        properties = new ListView();
        properties.Columns.Add("Name", Convert.ToInt32(Math.Round(Width * 0.7F - 32)),
                               HorizontalAlignment.Left);
        properties.Columns.Add("\u25a0", -2, HorizontalAlignment.Center);
        properties.Columns.Add("Status", -2, HorizontalAlignment.Left);
        properties.FullRowSelect = true;
        properties.HeaderStyle = ColumnHeaderStyle.Nonclickable;
        properties.View = View.Details;
        properties.ListViewItemSorter = new PropertySorter();
        properties.DoubleClick += new EventHandler(ShowWindowCommand);
        properties.Location = new Point(DockPadding.Left,
                                        propertiesLabel.Bottom + 4);
        properties.Size = new Size(ClientRectangle.Width - DockPadding.Left -
                                   DockPadding.Right, Convert.ToInt32(17 * Font.Size));
        properties.MultiSelect = false;
        propertyOpenMenuItem = new MenuItem("&Open", new EventHandler(ShowWindowCommand));
        propertyOpenMenuItem.DefaultItem = true;
        propertyClaimRentMenuItem = new MenuItem("&Claim Rent", new EventHandler(ClaimRentCommand));
        propertyMortgageMenuItem = new MenuItem("&Mortgage", new EventHandler(MortgageCommand));
        propertyUnmortgageMenuItem = new MenuItem("&Unmortgage", new EventHandler(UnmortgageCommand));
        propertySellMenuItem = new MenuItem("Sell House", new EventHandler(SellCommand));
        propertiesContextMenu = new ContextMenu(new MenuItem[] {
            propertyOpenMenuItem,
            propertyClaimRentMenuItem,
            propertyMortgageMenuItem,
            propertyUnmortgageMenuItem,
            propertySellMenuItem,
        });
        propertiesContextMenu.Popup += new EventHandler(UpdatePropertyContextMenu);
        properties.SelectedIndexChanged += new EventHandler(UpdatePropertyContextMenu);
        properties.ItemDrag += new ItemDragEventHandler(DragItem);
        Label cardsLabel = new Label();
        cardsLabel.Text = "&Cards:";
        cardsLabel.TextAlign = ContentAlignment.MiddleLeft;
        cardsLabel.Location = new Point(DockPadding.Left, properties.Bottom + 8);
        cardsLabel.Width = cardsLabel.PreferredWidth;
        cardsLabel.Height = cardsLabel.PreferredHeight;
        cards = new ListView();
        cards.Columns.Add("Name", -2, HorizontalAlignment.Left);
        cards.FullRowSelect = true;
        cards.HeaderStyle = ColumnHeaderStyle.Nonclickable;
        cards.View = View.Details;
        cards.SelectedIndexChanged += new EventHandler(UpdateCardContextMenu);
        cards.DoubleClick += new EventHandler(ShowWindowCommand);
        cards.Location = new Point(DockPadding.Left, cardsLabel.Bottom + 4);
        cards.Size = new Size(ClientRectangle.Width - DockPadding.Left -
                              DockPadding.Right, Convert.ToInt32(7 * Font.Size));
        cardOpenMenuItem = new MenuItem("&Open", new EventHandler(ShowWindowCommand));
        cardOpenMenuItem.DefaultItem = true;
        cardsContextMenu = new ContextMenu(new MenuItem[] {
            cardOpenMenuItem,
        });
        cards.MultiSelect = false;
        cards.ItemDrag += new ItemDragEventHandler(DragItem);

        Controls.AddRange(new Control[] {
            picture, cashLabel, cash,
            propertiesLabel, properties,
            cardsLabel, cards,
        });
        AllowDrop = true;
        ClientSize = new Size(ClientSize.Width,
                              cards.Bottom + DockPadding.Bottom);
        UpdateWindow(true);
    }

    private String GetStatus(Property property) {
        String status;
        if (property.Mortgaged) {
            status = "Mortgaged";
        } else if (property.CurrentHotels > 0) {
            status = String.Format("{0} Hotel{1}",
                                   property.CurrentHotels, property.CurrentHotels == 1 ? "" : "s");
        } else if (property.CurrentHouses > 0) {
            status = String.Format("{0} House{1}",
                                   property.CurrentHouses, property.CurrentHouses == 1 ? "" : "s");
        } else {
            status = "";
        }
        return status;
    }

    public void UpdateWindow(bool reload) {
        Text = player.Name;
        picture.Image = player.Image;
        cash.Text = player.Cash.ToString();
        if (reload) {
            properties.Items.Clear();
            foreach (DictionaryEntry entry in player.Properties) {
                Property property = (Property)(entry.Value);
                ListViewItem item = new ListViewItem(new String[] {
                    property.Name,
                    (property.BackColor == Color.White ? " " : "\u25a0"),
                    GetStatus(property)
                });
                item.UseItemStyleForSubItems = false;
                item.SubItems[1].ForeColor = property.BackColor;
                item.Tag = property;
                properties.Items.Add(item);
            }
            properties.Sort();
            cards.Items.Clear();
            foreach (DictionaryEntry entry in player.Cards) {
                Card card = (Card)(entry.Value);
                ListViewItem item = new ListViewItem(card.text);
                item.Tag = card;
                cards.Items.Add(item);
            }
            cards.Sort();
        } else {
            foreach (ListViewItem item in properties.Items) {
                Property property = item.Tag as Property;
                item.SubItems[2].Text = GetStatus(property);
            }
        }
        Invalidate();
    }

    public void UpdatePropertyContextMenu(Object sender, EventArgs e) {
        if (properties.SelectedItems.Count == 0) {
            properties.ContextMenu = null;
            return;
        }
        Property property = properties.SelectedItems[0].Tag as Property;
        propertyClaimRentMenuItem.Visible = player.IsUs;
        propertyMortgageMenuItem.Enabled = !property.Mortgaged
            && property.CurrentHouses == 0 && property.CurrentHotels == 0;
        propertyMortgageMenuItem.Visible = player.IsUs && !property.Mortgaged;
        propertyUnmortgageMenuItem.Enabled = property.Mortgaged;
        propertyUnmortgageMenuItem.Visible = player.IsUs && property.Mortgaged;
        propertySellMenuItem.Enabled = (property.CurrentHotels > 0 || property.CurrentHouses > 0);
        propertySellMenuItem.Visible = player.IsUs && property.type == 0;
        properties.ContextMenu = propertiesContextMenu;
    }

    public void ShowWindowCommand(Object sender, EventArgs e) {
        if (sender is MenuItem)
            sender = ((sender as MenuItem).Parent as ContextMenu).SourceControl;
        if ((sender as ListView).SelectedItems.Count == 0) return;
        ((sender as ListView).SelectedItems[0].Tag as Thing).ShowWindow(Owner as MonopolyForm);
    }

    public void UpdateCardContextMenu(Object sender, EventArgs e) {
        if (cards.SelectedItems.Count == 0)
            cards.ContextMenu = null;
        else 
            cards.ContextMenu = cardsContextMenu;
    }

    public void ClaimRentCommand(Object sender, EventArgs e) {
        if (properties.SelectedItems.Count == 0) return;
        Property p = properties.SelectedItems[0].Tag as Property;
        MonopolyForm f = (MonopolyForm)Owner;
        if (f.LatestPlayer != null && f.Network != null)
            f.Network.SendMessage(new PIMP_CLAIM_RENT(Convert.ToByte(f.LatestPlayer.ID), Convert.ToByte(Convert.ToByte(p.id))));
    }

    public void MortgageCommand(Object sender, EventArgs e) {
        if (properties.SelectedItems.Count == 0) return;
        Property p = properties.SelectedItems[0].Tag as Property;
        MonopolyForm f = (MonopolyForm)Owner;
        if (f.Network != null)
            f.Network.SendMessage(new PIMP_MORTGAGE_PROPERTY(Convert.ToByte(Convert.ToByte(p.id))));
    }

    public void UnmortgageCommand(Object sender, EventArgs e) {
        if (properties.SelectedItems.Count == 0) return;
        Property p = properties.SelectedItems[0].Tag as Property;
        MonopolyForm f = (MonopolyForm)Owner;
        if (f.Network != null)
            f.Network.SendMessage(new PIMP_UNMORTGAGE_PROPERTY(Convert.ToByte(Convert.ToByte(p.id))));
    }

    public void SellCommand(Object sender, EventArgs e) {
        if (properties.SelectedItems.Count == 0) return;
        Property p = properties.SelectedItems[0].Tag as Property;
        int a = p.CurrentHotels > 0 ? 0 : 1;
        int b = p.CurrentHotels > 0 ? 1 : 0;
        MonopolyForm f = (MonopolyForm)Owner;
        if (f.Network != null)
            f.Network.SendMessage(new PIMP_PURCHASE_HOUSES(new byte[] { Convert.ToByte(Convert.ToByte(p.id)) },
                                                           new sbyte[] { Convert.ToSByte(-a) },
                                                           new sbyte[] { Convert.ToSByte(-b) }));
    }

    public void DragItem(Object sender, ItemDragEventArgs e) {
        if (e.Button == MouseButtons.Left && player.IsUs) {
            // let's roll
            DoDragDrop((e.Item as ListViewItem).Tag, DragDropEffects.Link);
        }
    }

    protected override void OnDragEnter(DragEventArgs e) {
        base.OnDragEnter(e);
        if ((e.Data.GetDataPresent(typeof(Property)) &&
             (e.Data.GetData(typeof(Property)) as Property).Owner != player) ||
            (e.Data.GetDataPresent(typeof(Card)) &&
             (e.Data.GetData(typeof(Card)) as Card).Owner != player))
            e.Effect = DragDropEffects.Link;
    }

    protected override void OnDragDrop(DragEventArgs e) {
        base.OnDragDrop(e);
        MonopolyForm f = (MonopolyForm)Owner;
        // try to add the thing to the transaction, if there are no transactions
        // with that player, request one
        if (f.AddToPendingTransactionList(player, e.Data.GetDataPresent(typeof(Property)) ? e.Data.GetData(typeof(Property)) as Thing : e.Data.GetData(typeof(Card)) as Thing) && (f.Network != null))
            f.Network.SendMessage(new PIMP_TRANSACTION_REQUEST_TRADE(Convert.ToByte(player.ID)));
    }

}

public class HouseBuyingForm : FloatingForm {

    public Panel RightSplit() {
        Panel panel = new Panel();
        panel.Dock = DockStyle.Right;
        panel.Width = 4;
        return panel;
    }

    ArrayList propertyPanels;
    Panel propertiesPanel;
    Panel nonePanel;
    Label costLabel;

    public HouseBuyingForm(Form parent, Property[] properties) : base(parent) {
        Size = new Size(Convert.ToInt32(Font.Size * 55F),
                        Convert.ToInt32(Font.Size * 30F));
        // XXX for some reason, setting StartPosition to
        // FormStartPosition.CenterParent doesn't work here.
        Text = "Purchase Houses and Hotels";
        DockPadding.All = 8;
        ControlBox = false;

        propertiesPanel = new Panel();
        propertiesPanel.Dock = DockStyle.Fill;
        propertiesPanel.AutoScroll = true;
        propertiesPanel.BorderStyle = BorderStyle.Fixed3D;
        propertiesPanel.AutoScrollMargin = new Size(0, 4);
        // and a button to confirm
        Panel buttonPanel = new Panel();
        buttonPanel.Dock = DockStyle.Bottom;
        buttonPanel.DockPadding.Top = 8;
        Button confirmButton = new Button();
        confirmButton.Text = "&Confirm";
        confirmButton.Dock = DockStyle.Right;
        confirmButton.Click += new EventHandler(ConfirmCommand);
        Panel spacer = new Panel();
        spacer.Width = 8;
        spacer.Dock = DockStyle.Right;
        Button cancelButton = new Button();
        cancelButton.Text = "Cancel";
        cancelButton.Dock = DockStyle.Right;
        cancelButton.Click += new EventHandler(CancelCommand);
        costLabel = new Label();
        costLabel.Text = "$0";
        costLabel.Dock = DockStyle.Fill;
        costLabel.TextAlign = ContentAlignment.MiddleLeft;
        buttonPanel.Height = confirmButton.Height + buttonPanel.DockPadding.Top;
        buttonPanel.Controls.AddRange(new Control[] {
            costLabel, confirmButton, spacer, cancelButton
        });
        Controls.AddRange(new Control[] {
            propertiesPanel, buttonPanel,
        });
        AcceptButton = confirmButton;
        CancelButton = cancelButton;
        // now add the properties
        propertyPanels = new ArrayList(properties.Length);
        foreach (Property p in properties) {
            Panel panel = new Panel();
            panel.Tag = p;
            panel.DockPadding.Top = 4;
            panel.DockPadding.Left = 4;
            panel.DockPadding.Right = 8;
            panel.Width = propertiesPanel.ClientRectangle.Width - 18;
            Label label = new Label();
            label.ForeColor = p.ForeColor;
            label.BackColor = p.BackColor;
            label.Font = new Font("Tahoma", Font.Size);
            label.Text = p.Name;
            label.Dock = DockStyle.Fill;
            label.UseMnemonic = false;
            label.TextAlign = ContentAlignment.MiddleCenter;
            Button view = new Button();
            view.Text = "View";
            view.Click += new EventHandler(ViewClicked);
            view.Dock = DockStyle.Right;
            ShrinkWrap(view);
            Houses houses = new Houses();
            houses.Dock = DockStyle.Right;
            Button less = new Button();
            less.Text = "\u2212"; // negative sign
            less.Dock = DockStyle.Right;
            less.Tag = houses;
            less.Font = new Font("Tahoma", less.Height * 0.8F);
            less.Click += new EventHandler(Decrease);
            less.Width = Convert.ToInt32(Font.Size * 4F);
            Button more = new Button();
            more.Text = "+";
            more.Dock = DockStyle.Right;
            more.Tag = houses;
            more.Font = new Font("Tahoma", more.Height * 0.8F);
            more.Click += new EventHandler(Increase);
            more.Width = Convert.ToInt32(Font.Size * 4F);
            // assume that all the buttons will have the same height
            panel.Height = Math.Max(label.PreferredHeight, view.Height)
                + panel.DockPadding.Top + panel.DockPadding.Bottom;
            panel.Controls.AddRange(new Control[] {
                label, RightSplit(), less, houses, more, RightSplit(), view,
            });
            houses.Width = houses.Height * 4 + 8;
            panel.Visible = false;
            propertiesPanel.Controls.Add(panel);
            propertyPanels.Add(panel);
        }
        nonePanel = new Panel();
        nonePanel.DockPadding.All = 8;
        nonePanel.Dock = DockStyle.Fill;
        nonePanel.Visible = false;
        Label noneLabel = new Label();
        noneLabel.Font = new Font("Tahoma", Font.Size * 2);
        noneLabel.Text = "You do not own any complete color groups.";
        noneLabel.Dock = DockStyle.Fill;
        noneLabel.UseMnemonic = false;
        noneLabel.TextAlign = ContentAlignment.MiddleCenter;
        nonePanel.Controls.Add(noneLabel);
        propertiesPanel.Controls.Add(nonePanel);
    }

    Player player;
    
    public void ShowWindow(Player p) {
        player = p;
        p.HouseBuyingForm = this;
        UpdateWindow(true);
        Show();
    }

    public bool HasGroup(Player player, int group) {
        foreach (Panel p in propertiesPanel.Controls) {
            Property property = p.Tag as Property;
            if (property != null &&
                property.group == group &&
                property.Owner != player)
                return false;
        }
        return true;
    }

    public void UpdateWindow(bool reset) {
        int y = 0;
        Point pos = propertiesPanel.AutoScrollPosition;
        propertiesPanel.AutoScrollPosition = new Point(0, 0);
        foreach (Panel p in propertyPanels) {
            Property property = p.Tag as Property;
            if ((property != null) &&
                (property.Owner == player) &&
                (property.type == 0) &&
                (property.Mortgaged == false) &&
                (HasGroup(player, property.group))) {
                if (!p.Visible || reset) {
                    p.Visible = true;
                    ((Houses)(p.Controls[3])).SetHouses(property.CurrentHouses,
                                                        5 * property.CurrentHotels);
                }
                p.Top = y;
                y += p.Height;
            } else {
                p.Visible = false;
            }
        }
        propertiesPanel.AutoScrollPosition = pos; // XXX This doesn't work, although I can't see why.
        nonePanel.Visible = y == 0;
        Reprice();
    }

    protected void Reprice() {
        int cost = 0;
        foreach (Panel p in propertiesPanel.Controls) {
            if (p.Tag == null || p.Visible == false) continue;
            Property property = p.Tag as Property;
            int v = ((Houses)(p.Controls[3])).Value;
            int a, b;
            if (v == 5) {
                // hotel
                a = 0;
                b = 1;
            } else {
                // houses
                a = v;
                b = 0;
            }
            int oldNumber = property.CurrentHouses + 5 * property.CurrentHotels;
            int newNumber = a + 5 * b;
            if (newNumber > oldNumber)
                cost += (newNumber - oldNumber) * property.houses;
            else if (newNumber < oldNumber)
                cost += (newNumber - oldNumber) * property.houses / 2;
        }
        if (cost > 0)
            costLabel.Text = String.Format("This will cost: ${0}", cost);
        else if (cost < 0)
            costLabel.Text = String.Format("This will get you: ${0}", -cost);
        else 
            costLabel.Text = "$0";
    }

    class Entry {
        public readonly byte id;
        public readonly sbyte houses;
        public readonly sbyte hotels;
        public Entry(int id, int houses, int hotels) {
            this.id = Convert.ToByte(id);
            this.houses = Convert.ToSByte(houses);
            this.hotels = Convert.ToSByte(hotels);
        }
    }

    protected void CancelCommand(Object sender, EventArgs e) {
        Close();
    }

    protected void ConfirmCommand(Object sender, EventArgs e) {
        Stack data = new Stack();
        foreach (Panel p in propertiesPanel.Controls) {
            if (p.Tag == null || p.Visible == false) continue;
            Property property = p.Tag as Property;
            // make a nice long list
            int v = ((Houses)(p.Controls[3])).Value;
            int a, b;
            if (v == 5) {
                // hotel
                a = 0;
                b = 1;
            } else {
                // houses
                a = v;
                b = 0;
            }
            if (a != property.CurrentHouses || b != property.CurrentHotels)
                data.Push(new Entry(property.id,
                                    a + 4 * b - property.CurrentHouses - 4 * property.CurrentHotels,
                                    b - property.CurrentHotels));
        }
        if (data.Count > 0) {
            byte[] ids = new byte[data.Count];
            sbyte[] houses = new sbyte[data.Count];
            sbyte[] hotels = new sbyte[data.Count];
            int i = 0;
            while (data.Count > 0) {
                Entry entry = (Entry)(data.Pop());
                ids[i] = entry.id;
                houses[i] = entry.houses;
                hotels[i] = entry.hotels;
                ++i;
            }
            // and send it back to the parent form somehow 
            OnConfirm(new ConfirmEventArgs(ids, houses, hotels));
        }
        Close();
    }

    public delegate void ConfirmEventHandler(Object sender, ConfirmEventArgs e);
    public class ConfirmEventArgs : EventArgs {
        public readonly byte[] ids;
        public readonly sbyte[] houses;
        public readonly sbyte[] hotels;
        public ConfirmEventArgs(byte[] ids,
                                sbyte[] houses, sbyte[] hotels) : base() {
            this.ids = ids;
            this.houses = houses;
            this.hotels = hotels;
        }
    }
    public event ConfirmEventHandler Confirm;
    protected virtual void OnConfirm(ConfirmEventArgs e) {
        if (Confirm != null)
            Confirm(this, e);
    }

    public void ViewClicked(Object sender, EventArgs e) {
        (((sender as Button).Parent.Tag) as Property).ShowWindow(Owner as MonopolyForm);
    }

    public void Decrease(Object sender, EventArgs e) {
        (((sender as Button).Tag) as Houses).Value -= 1;
        Reprice();
    }

    public void Increase(Object sender, EventArgs e) {
        (((sender as Button).Tag) as Houses).Value += 1;
        Reprice();
    }

    protected override void OnVisibleChanged(EventArgs e) {
        base.OnVisibleChanged(e);
        if (Visible == false)
            player.HouseBuyingForm = null;
    }

}

public class Transaction : FloatingForm {

    public readonly uint id;
    public readonly Player thisPlayer;
    public readonly Player otherPlayer;
    protected HybridDictionary thisPropertiesMap;
    protected HybridDictionary thisCardsMap;
    protected HybridDictionary otherPropertiesMap;
    protected HybridDictionary otherCardsMap;
    
    protected enum State { Editing, Finished, Agreed }
    protected State thisState = State.Editing;
    protected State otherState = State.Editing;

    protected NumericUpDown thisCash;
    protected Label otherCash;
    protected ListView thisProperties;
    protected ListView thisCards;
    protected ListView otherProperties;
    protected ListView otherCards;
    protected Button agreeButton;
    protected Button finishButton;
    protected Label otherStatus;

    protected ContextMenu propertiesContextMenu;
    protected MenuItem propertyMortgageMenuItem;
    protected MenuItem propertyUnmortgageMenuItem;
    protected MenuItem propertySellMenuItem;

    protected ContextMenu cardsContextMenu;
    protected ContextMenu otherContextMenu;

    public Transaction(uint id, String comment, Object reason, uint amount,
                       Player thisPlayer, Player otherPlayer, bool cancellable,
                       MonopolyForm parent, bool visible) : base(parent) {
        this.id = id;
        this.thisPlayer = thisPlayer;
        this.thisPropertiesMap = new HybridDictionary(5);
        this.thisCardsMap = new HybridDictionary(2);
        this.otherPlayer = otherPlayer;
        this.otherPropertiesMap = new HybridDictionary(5);
        this.otherCardsMap = new HybridDictionary(2);

        ControlBox = false;

        Width = Convert.ToInt32(Font.Size * 70F);
        DockPadding.All = 8;
        if (otherPlayer != null)
            Text = "Transaction with " + otherPlayer.Name;
        else
            Text = "Transaction with bank";

        Button viewButton = new Button();
        viewButton.Text = "&View";
        viewButton.Width = Convert.ToInt32(Font.Size * 8);
        viewButton.Location =
            new Point(ClientSize.Width - DockPadding.Right - viewButton.Width,
                      DockPadding.Top + Convert.ToInt32((Font.Size * 5 - viewButton.Height) / 2));
        viewButton.Visible = (reason is Property || reason is Card);
        viewButton.Tag = reason;
        viewButton.Click += new EventHandler(OpenThing);

        Label caption = new Label();
        caption.Location = new Point(DockPadding.Left, DockPadding.Top);
        caption.Text = comment;
        caption.TextAlign = ContentAlignment.TopLeft;
        caption.UseMnemonic = false;
        caption.Size =
            new Size(ClientSize.Width - DockPadding.Left - DockPadding.Right
                     + ((viewButton.Visible) ? - 8 - viewButton.Width : 0),
                     Convert.ToInt32(Font.Size * 5));

        Label thisLabel = new Label();
        thisLabel.Text = "You are offering...";
        thisLabel.UseMnemonic = false;
        thisLabel.TextAlign = ContentAlignment.MiddleLeft;
        thisLabel.Height = thisLabel.PreferredHeight;

        Label thisCashLabel = new Label();
        thisCashLabel.Text = "Cas&h: $";
        thisCashLabel.TextAlign = ContentAlignment.MiddleLeft;
        thisCashLabel.Width = thisCashLabel.PreferredWidth;

        thisCash = new NumericUpDown();
        thisCash.Minimum = UInt32.MinValue;
        thisCash.Maximum = UInt32.MaxValue;
        thisCash.Width = Convert.ToByte(thisCashLabel.Width * 1.5F);
        thisCash.Height = Math.Max(thisCash.Height,
                                   thisCashLabel.PreferredHeight);
        thisCash.Enter += new EventHandler(EnteredCash);
        thisCash.Leave += new EventHandler(LeftCash);
        thisCash.TextChanged += new EventHandler(CashChanged);
        thisCash.ValueChanged += new EventHandler(CashChanged);

        PictureBox thisPicture = new PictureBox();
        thisPicture.Location = new Point(DockPadding.Left, caption.Bottom + 8);
        thisPicture.SizeMode = PictureBoxSizeMode.StretchImage;
        thisPicture.Height = Math.Max(thisCashLabel.PreferredHeight + 8
                                      + thisLabel.PreferredHeight, 32);
        thisPicture.Width = thisPicture.Height;
        thisPicture.Image = thisPlayer.Image;

        Button viewThisButton = new Button();
        viewThisButton.Text = "&Open";
        viewThisButton.Width = Convert.ToInt32(Font.Size * 8);
        viewThisButton.Location = new Point(ClientSize.Width / 2 - 4 - viewThisButton.Width,
                                            thisPicture.Top);
        viewThisButton.Tag = thisPlayer;
        viewThisButton.Click += new EventHandler(OpenThing);

        thisLabel.Location = new Point(thisPicture.Right + 8,
                                       thisPicture.Top);
        thisLabel.Width = viewThisButton.Left - 8 - thisLabel.Left;

        thisCashLabel.Location = new Point(thisPicture.Right + 8,
                                           thisLabel.Bottom + 8);
        thisCashLabel.Height = thisCash.Height;

        thisCash.Location = new Point(thisCashLabel.Right + 4,
                                      thisCashLabel.Top);

        Label thisPropertiesLabel = new Label();
        thisPropertiesLabel.Text = "&Properties:";
        thisPropertiesLabel.TextAlign = ContentAlignment.MiddleLeft;
        thisPropertiesLabel.Location = new Point(DockPadding.Left,
                                                 thisPicture.Bottom + 8);
        thisPropertiesLabel.Width = thisPropertiesLabel.PreferredWidth;
        thisPropertiesLabel.Height = thisPropertiesLabel.PreferredHeight;
        thisProperties = new ListView();
        thisProperties.Columns.Add("Name", -2, HorizontalAlignment.Left);
        thisProperties.FullRowSelect = true;
        thisProperties.HeaderStyle = ColumnHeaderStyle.Nonclickable;
        thisProperties.View = View.Details;
        thisProperties.DoubleClick += new EventHandler(ShowWindowCommand);
        thisProperties.Location = new Point(DockPadding.Left,
                                            thisPropertiesLabel.Bottom + 4);
        thisProperties.Size = new Size(ClientRectangle.Width / 2
                                       - DockPadding.Left - 4, Convert.ToInt32(12 * Font.Size));
        thisProperties.MultiSelect = false;
        MenuItem propertyOpenMenuItem = new MenuItem("&Open", new EventHandler(ShowWindowCommand));
        propertyOpenMenuItem.DefaultItem = true;
        propertyMortgageMenuItem = new MenuItem("&Mortgage", new EventHandler(MortgageCommand));
        propertyUnmortgageMenuItem = new MenuItem("&Unmortgage", new EventHandler(UnmortgageCommand));
        propertySellMenuItem = new MenuItem("Sell House", new EventHandler(SellCommand));
        propertiesContextMenu = new ContextMenu(new MenuItem[] {
            propertyOpenMenuItem,
            propertyMortgageMenuItem,
            propertyUnmortgageMenuItem,
            propertySellMenuItem,
            new MenuItem("-"),
            new MenuItem("&Remove", new EventHandler(RemoveCommand)),
        });
        propertiesContextMenu.Popup += new EventHandler(UpdatePropertyContextMenu);
        thisProperties.SelectedIndexChanged += new EventHandler(UpdatePropertyContextMenu);
        thisProperties.AllowDrop = true;
        thisProperties.DragEnter += new DragEventHandler(DragEnterProperties);
        thisProperties.DragDrop += new DragEventHandler(DragDropProperties);

        Label thisCardsLabel = new Label();
        thisCardsLabel.Text = "&Cards:";
        thisCardsLabel.TextAlign = ContentAlignment.MiddleLeft;
        thisCardsLabel.Location = new Point(DockPadding.Left,
                                            thisProperties.Bottom + 8);
        thisCardsLabel.Width = ClientSize.Width / 2 - 4 - DockPadding.Left;
        thisCardsLabel.Height = thisCardsLabel.PreferredHeight;
        thisCards = new ListView();
        thisCards.Columns.Add("Name", -2, HorizontalAlignment.Left);
        thisCards.FullRowSelect = true;
        thisCards.HeaderStyle = ColumnHeaderStyle.Nonclickable;
        thisCards.View = View.Details;
        thisCards.DoubleClick += new EventHandler(ShowWindowCommand);
        thisCards.Location = new Point(DockPadding.Left,
                                       thisCardsLabel.Bottom + 4);
        thisCards.Size = new Size(ClientRectangle.Width / 2
                                  - DockPadding.Left - 4, Convert.ToInt32(7 * Font.Size));
        thisCards.MultiSelect = false;
        MenuItem cardOpenMenuItem = new MenuItem("&Open", new EventHandler(ShowWindowCommand));
        cardOpenMenuItem.DefaultItem = true;
        cardsContextMenu = new ContextMenu(new MenuItem[] {
            cardOpenMenuItem,
            new MenuItem("-"),
            new MenuItem("&Remove", new EventHandler(RemoveCommand)),
        });
        cardsContextMenu.Popup += new EventHandler(UpdateCardContextMenu);
        thisCards.SelectedIndexChanged += new EventHandler(UpdateCardContextMenu);
        thisCards.AllowDrop = true;
        thisCards.DragEnter += new DragEventHandler(DragEnterCards);
        thisCards.DragDrop += new DragEventHandler(DragDropCards);

        Label otherLabel = new Label();
        if (otherPlayer != null)
            otherLabel.Text = String.Format("{0} is offering...", otherPlayer.Name);
        else
            otherLabel.Text = "Bank is offering...";
        otherLabel.UseMnemonic = false;
        otherLabel.TextAlign = ContentAlignment.MiddleLeft;
        otherLabel.Size = thisLabel.Size;

        Label otherCashLabel = new Label();
        otherCashLabel.Text = "Cash: $";
        otherCashLabel.TextAlign = ContentAlignment.MiddleLeft;
        otherCashLabel.Size = thisCashLabel.Size;

        otherCash = new Label();
        otherCash.Text = "0";
        otherCash.TextAlign = ContentAlignment.MiddleLeft;
        otherCash.Height = thisCash.Height;

        PictureBox otherPicture = new PictureBox();
        otherPicture.Location = new Point(ClientSize.Width / 2 + 4, caption.Bottom + 8);
        otherPicture.SizeMode = PictureBoxSizeMode.StretchImage;
        otherPicture.Size = thisPicture.Size;
        if (otherPlayer != null)
            otherPicture.Image = otherPlayer.Image;
        // XXX else?

        Button viewOtherButton = new Button();
        viewOtherButton.Text = "&Open";
        viewOtherButton.Size = viewThisButton.Size;
        viewOtherButton.Location = new Point(ClientSize.Width - DockPadding.Right - viewOtherButton.Width,
                                            otherPicture.Top);
        viewOtherButton.Tag = otherPlayer;
        viewOtherButton.Click += new EventHandler(OpenThing);
        viewOtherButton.Visible = otherPlayer != null;

        otherLabel.Location = new Point(otherPicture.Right + 8,
                                        otherPicture.Top);

        otherCashLabel.Location = new Point(otherPicture.Right + 8,
                                            otherLabel.Bottom + 8);

        otherCash.Location = new Point(otherCashLabel.Right + 4,
                                       otherCashLabel.Top);
        otherCash.Width = ClientSize.Width - DockPadding.Right - otherCash.Left;

        MenuItem otherOpenMenuItem = new MenuItem("&Open", new EventHandler(ShowWindowCommand));
        otherOpenMenuItem.DefaultItem = true;
        otherContextMenu = new ContextMenu(new MenuItem[] {
            otherOpenMenuItem,
        });

        Label otherPropertiesLabel = new Label();
        otherPropertiesLabel.Text = "Properties:";
        otherPropertiesLabel.TextAlign = ContentAlignment.MiddleLeft;
        otherPropertiesLabel.Location = new Point(otherPicture.Left,
                                                  otherPicture.Bottom + 8);
        otherPropertiesLabel.Size = thisPropertiesLabel.Size;
        otherProperties = new ListView();
        otherProperties.Columns.Add("Name", -2, HorizontalAlignment.Left);
        otherProperties.FullRowSelect = true;
        otherProperties.HeaderStyle = ColumnHeaderStyle.Nonclickable;
        otherProperties.View = View.Details;
        otherProperties.DoubleClick += new EventHandler(ShowWindowCommand);
        otherProperties.Location = new Point(otherPicture.Left,
                                             otherPropertiesLabel.Bottom + 4);
        otherProperties.Size = thisProperties.Size;
        otherProperties.MultiSelect = false;
        otherProperties.SelectedIndexChanged += new EventHandler(UpdateOtherContextMenu);

        Label otherCardsLabel = new Label();
        otherCardsLabel.Text = "Cards:";
        otherCardsLabel.TextAlign = ContentAlignment.MiddleLeft;
        otherCardsLabel.Location = new Point(otherPicture.Left,
                                             otherProperties.Bottom + 8);
        otherCardsLabel.Size = thisCardsLabel.Size;
        otherCards = new ListView();
        otherCards.Columns.Add("Name", -2, HorizontalAlignment.Left);
        otherCards.FullRowSelect = true;
        otherCards.HeaderStyle = ColumnHeaderStyle.Nonclickable;
        otherCards.View = View.Details;
        otherCards.DoubleClick += new EventHandler(ShowWindowCommand);
        otherCards.Location = new Point(otherPicture.Left,
                                        otherCardsLabel.Bottom + 4);
        otherCards.Size = thisCards.Size;
        otherCards.MultiSelect = false;
        otherCards.SelectedIndexChanged += new EventHandler(UpdateOtherContextMenu);

        int x = ClientSize.Width / 2 - 4;

        Button cancelButton = new Button();
        CancelButton = cancelButton;
        if (cancellable) {
            // no reason, so it can be cancelled
            cancelButton.Text = "Cancel";
            cancelButton.Click += new EventHandler(CancelCommand);
        } else {
            // can't be cancelled
            cancelButton.Text = "Claim Bankruptcy";
            cancelButton.Click += new EventHandler(BankruptCommand);
        }
        ShrinkWrap(cancelButton);
        x -= cancelButton.Width;
        cancelButton.Location = new Point(x, thisCards.Bottom + 8);
        x -= 8;

        agreeButton = new Button();
        agreeButton.Text = "&Agree";
        ShrinkWrap(agreeButton);
        x -= agreeButton.Width;
        agreeButton.Location = new Point(x, thisCards.Bottom + 8);
        agreeButton.Enabled = false;
        agreeButton.Click += new EventHandler(AgreeCommand);
        x -= 8;

        finishButton = new Button();
        finishButton.Text = "&Finish";
        ShrinkWrap(finishButton);
        x -= finishButton.Width;
        finishButton.Location = new Point(x, thisCards.Bottom + 8);
        finishButton.Click += new EventHandler(FinishCommand);

        AcceptButton = finishButton;

        otherStatus = new Label(); 
        otherStatus.Text = "";
        otherStatus.TextAlign = ContentAlignment.MiddleCenter;
        otherStatus.UseMnemonic = false;
        otherStatus.Location = new Point(otherCards.Left, otherCards.Bottom + 8);
        otherStatus.Size = new Size(otherCards.Width, finishButton.Height);
        
        Controls.AddRange(new Control[] {
            caption, viewButton,
            thisPicture, thisLabel, viewThisButton, thisCashLabel, thisCash,
            thisPropertiesLabel, thisProperties, thisCardsLabel, thisCards,
            agreeButton, finishButton, cancelButton, otherStatus,
            otherPicture, otherLabel, viewOtherButton, otherCashLabel, otherCash,
            otherPropertiesLabel, otherProperties, otherCardsLabel, otherCards,
        });
        ActiveControl = finishButton;

        ClientSize = new Size(ClientSize.Width, agreeButton.Bottom + DockPadding.Bottom);

        if (visible)
            Show();
    }


    // client -> server

    public void RequestSetThisCash(uint c) {
        Network network = (Owner as MonopolyForm).Network;
        if (network == null) return;
        network.SendMessage(new PIMP_TRANSACTION_SET_CASH(id, c));
    }

    public void RequestAddThisProperty(Property p) {
        Network network = (Owner as MonopolyForm).Network;
        if (network == null) return;
        network.SendMessage(new PIMP_TRANSACTION_ADD_PROPERTY(id, Convert.ToByte(p.id)));
    }

    public void RequestRemoveThisProperty(Property p) {
        Network network = (Owner as MonopolyForm).Network;
        if (network == null) return;
        network.SendMessage(new PIMP_TRANSACTION_REMOVE_PROPERTY(id, Convert.ToByte(p.id)));
    }

    public void RequestAddThisCard(Card c) {
        Network network = (Owner as MonopolyForm).Network;
        if (network == null) return;
        network.SendMessage(new PIMP_TRANSACTION_ADD_CARD(id, Convert.ToByte(c.id)));
    }

    public void RequestRemoveThisCard(Card c) {
        Network network = (Owner as MonopolyForm).Network;
        if (network == null) return;
        network.SendMessage(new PIMP_TRANSACTION_REMOVE_CARD(id, Convert.ToByte(c.id)));
    }

    public void RequestThisFinish() {
        Network network = (Owner as MonopolyForm).Network;
        if (network == null) return;
        network.SendMessage(new PIMP_TRANSACTION_FINISH(id));
    }

    public void RequestThisAgree() {
        Network network = (Owner as MonopolyForm).Network;
        if (network == null) return;
        network.SendMessage(new PIMP_TRANSACTION_AGREE(id));
    }

    public void RequestThisReopen() {
        Network network = (Owner as MonopolyForm).Network;
        if (network == null) return;
        network.SendMessage(new PIMP_TRANSACTION_REOPEN(id));
    }

    public void RequestThisCancel() {
        Network network = (Owner as MonopolyForm).Network;
        if (network == null) return;
        network.SendMessage(new PIMP_TRANSACTION_CANCEL(id));
    }

    public void RequestThisBankrupt() {
        Network network = (Owner as MonopolyForm).Network;
        if (network == null) return;
        network.SendMessage(new PIMP_BANKRUPT_TRANSACTION(id));
    }


    // server -> client

    public void EnsureVisible() {
        if (!Visible)
            Show();
    }

    public void CannotSetCash(uint c) {
        EnsureVisible();
        MessageBox.Show(this,
                        String.Format("You do not have enough cash to pay ${0}.", c),
                        "Kaspar",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
    }

    public void SetThisCash(uint c) {
        thisCash.Value = c;
        thisCash.Font = new Font(thisCash.Font, FontStyle.Regular);
        cashChanged = false;
    }

    public void SetOtherCash(uint c) {
        otherCash.Text = c.ToString();
    }

    public void AddThisProperty(Property p) {
        ListViewItem item = new ListViewItem(p.Name);
        item.Tag = p;
        thisProperties.Items.Add(item);
        thisPropertiesMap.Add(p.id, item);
    }

    public void RemoveThisProperty(Property p) {
        ListViewItem item = thisPropertiesMap[p.id] as ListViewItem;
        if (item != null)
            item.Remove();
    }

    public void AddThisCard(Card c) {
        ListViewItem item = new ListViewItem(c.text);
        item.Tag = c;
        thisCards.Items.Add(item);
        thisCardsMap.Add(c.id, item);
    }

    public void RemoveThisCard(Card c) {
        ListViewItem item = thisCardsMap[c.id] as ListViewItem;
        if (item != null)
            item.Remove();
    }

    public void AddOtherProperty(Property p) {
        ListViewItem item = new ListViewItem(p.Name);
        item.Tag = p;
        otherProperties.Items.Add(item);
        otherPropertiesMap.Add(p.id, item);
    }

    public void RemoveOtherProperty(Property p) {
        ListViewItem item = otherPropertiesMap[p.id] as ListViewItem;
        if (item != null)
            item.Remove();
    }

    public void AddOtherCard(Card c) {
        ListViewItem item = new ListViewItem(c.text);
        item.Tag = c;
        otherCards.Items.Add(item);
        otherCardsMap.Add(c.id, item);
    }

    public void RemoveOtherCard(Card c) {
        ListViewItem item = otherCardsMap[c.id] as ListViewItem;
        if (item != null)
            item.Remove();
    }

    public void ThisFinish() {
        finishButton.Enabled = false;
        thisState = State.Finished;
        if (otherState > State.Editing) {
            agreeButton.Enabled = true;
            AcceptButton = agreeButton;
        } else {
            AcceptButton = null;
        }
    }

    public void OtherFinish() {
        otherStatus.Text = "Ready to seal the deal.";
        otherState = State.Finished;
        if (thisState == State.Finished) {
            agreeButton.Enabled = true;
            AcceptButton = agreeButton;
        }
    }

    public void ThisAgree() {
        finishButton.Enabled = false;
        agreeButton.Enabled = false;
        thisState = State.Agreed;
        AcceptButton = null;
    }

    public void OtherAgree() {
        otherStatus.Text = "Deal agreed.";
        otherState = State.Agreed;
    }

    public void ThisReopen() {
        EnsureVisible();
        finishButton.Enabled = true;
        agreeButton.Enabled = false;
        AcceptButton = finishButton;
        thisState = State.Editing;
        if (otherState == State.Agreed) {
            otherState = State.Finished;
            otherStatus.Text = "Ready to seal the deal.";
        }
    }

    public void OtherReopen() {
        EnsureVisible();
        otherStatus.Text = "";
        otherState = State.Editing;
        if (thisState == State.Agreed) {
            thisState = State.Finished;
            agreeButton.Enabled = false;
            AcceptButton = null;
        }
    }

    public void Finalised() {
        ForceClose();
    }

    public void Cancelled() {
        ForceClose();
    }

    public void Unacceptable(uint c) {
        MessageBox.Show(this,
                        String.Format("That is not an acceptable deal. The bank wants exactly ${0}.", c),
                        "Kaspar",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
    }

    public void WouldReduceNetWorth(Transaction otherTransaction) {
        EnsureVisible();
        MessageBox.Show(this,
                        "You cannot reduce your net worth while you owe someone else money.",
                        "Kaspar",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
    }

    public void PropertyHasHouse(Property p) {
        String s;
        if (p.CurrentHouses == 0) {
            s = "{2} hotel{4}";
        } else if (p.CurrentHotels == 0) {
            s = "{1} house{3}";
        } else {
            s = "{1} house{3} and {2} hotel{4}";
        }
        MessageBox.Show(this,
                        String.Format("{0} has " + s + ". You may not trade properties with buildings.",
                                      p.Name, p.CurrentHouses, p.CurrentHotels,
                                      (p.CurrentHouses == 1 ? "" : "s"),
                                      (p.CurrentHotels == 1 ? "" : "s")),
                        "Kaspar",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
    }

    public void NotBankrupt(Transaction other) {
        if (other == null)
            MessageBox.Show(this,
                            "You can afford this. Mortgage some properties if required.",
                            "Kaspar",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
        else
            MessageBox.Show(this,
                            "You must complete earlier transactions before claiming bankruptcy on later ones.",
                            "Kaspar",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
    }

    public void CannotCancel() {
        MessageBox.Show(this,
                        "You may not cancel this transaction.",
                        "Kaspar",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
    }

    public bool IgnoreUnusual = false;
    public void Unusual() {
        if (!IgnoreUnusual)
            EnsureVisible();
    }


    // UI

    public void OpenThing(Object sender, EventArgs e) {
        ((sender as Button).Tag as Thing).ShowWindow(Owner as MonopolyForm);
    }

    public void UpdatePropertyContextMenu(Object sender, EventArgs e) {
        if (thisProperties.SelectedItems.Count == 0) {
            thisProperties.ContextMenu = null;
            return;
        }
        Property property = thisProperties.SelectedItems[0].Tag as Property;
        propertyMortgageMenuItem.Enabled = !property.Mortgaged;
        propertyMortgageMenuItem.Visible = !property.Mortgaged;
        propertyUnmortgageMenuItem.Enabled = property.Mortgaged;
        propertyUnmortgageMenuItem.Visible = property.Mortgaged;
        propertySellMenuItem.Enabled = (property.CurrentHotels > 0 || property.CurrentHouses > 0);
        thisProperties.ContextMenu = propertiesContextMenu;
    }

    public void UpdateCardContextMenu(Object sender, EventArgs e) {
        if (thisCards.SelectedItems.Count == 0)
            thisCards.ContextMenu = null;
        else 
            thisCards.ContextMenu = cardsContextMenu;
    }

    public void UpdateOtherContextMenu(Object sender, EventArgs e) {
        ListView x = sender as ListView;
        if (x.SelectedItems.Count == 0)
            x.ContextMenu = null;
        else 
            x.ContextMenu = otherContextMenu;
    }

    public void ShowWindowCommand(Object sender, EventArgs e) {
        if (sender is MenuItem)
            sender = ((sender as MenuItem).Parent as ContextMenu).SourceControl;
        if ((sender as ListView).SelectedItems.Count == 0) return;
        ((sender as ListView).SelectedItems[0].Tag as Thing).ShowWindow(Owner as MonopolyForm);
    }

    public void MortgageCommand(Object sender, EventArgs e) {
        if (thisProperties.SelectedItems.Count == 0) return;
        Property p = thisProperties.SelectedItems[0].Tag as Property;
        MonopolyForm f = (MonopolyForm)Owner;
        if (f.Network != null)
            f.Network.SendMessage(new PIMP_MORTGAGE_PROPERTY(Convert.ToByte(Convert.ToByte(p.id))));
    }

    public void UnmortgageCommand(Object sender, EventArgs e) {
        if (thisProperties.SelectedItems.Count == 0) return;
        Property p = thisProperties.SelectedItems[0].Tag as Property;
        MonopolyForm f = (MonopolyForm)Owner;
        if (f.Network != null)
            f.Network.SendMessage(new PIMP_UNMORTGAGE_PROPERTY(Convert.ToByte(Convert.ToByte(p.id))));
    }

    public void SellCommand(Object sender, EventArgs e) {
        if (thisProperties.SelectedItems.Count == 0) return;
        Property p = thisProperties.SelectedItems[0].Tag as Property;
        int a = p.CurrentHotels > 0 ? 0 : 1;
        int b = p.CurrentHotels > 0 ? 1 : 0;
        MonopolyForm f = (MonopolyForm)Owner;
        if (f.Network != null)
            f.Network.SendMessage(new PIMP_PURCHASE_HOUSES(new byte[] { Convert.ToByte(Convert.ToByte(p.id)) },
                                                           new sbyte[] { Convert.ToSByte(-a) },
                                                           new sbyte[] { Convert.ToSByte(-b) }));
    }

    public void RemoveCommand(Object sender, EventArgs e) {
        if (sender is MenuItem)
            sender = ((sender as MenuItem).Parent as ContextMenu).SourceControl;
        if ((sender as ListView).SelectedItems.Count == 0) return;
        Thing x = ((sender as ListView).SelectedItems[0].Tag as Thing);
        if (x is Property)
            RequestRemoveThisProperty(x as Property);
        else
            RequestRemoveThisCard(x as Card);
    }

    public void FinishCommand(Object sender, EventArgs e) {
        UpdateCashIfChanged();
        RequestThisFinish();
    }

    public void AgreeCommand(Object sender, EventArgs e) {
        UpdateCashIfChanged();
        RequestThisAgree();
    }

    public void CancelCommand(Object sender, EventArgs e) {
        RequestThisCancel();
    }

    public void BankruptCommand(Object sender, EventArgs e) {
        UpdateCashIfChanged();
        RequestThisBankrupt();
    }

    protected override void OnClosing(CancelEventArgs e) {
        // totally abort the attempt to close the form
        if (Visible) {
            e.Cancel = true;
            RequestThisCancel();
        } else {
            base.OnClosing(e);
        }
    }

    private bool calledOnClosed;

    public void ForceClose() {
        Hide();
        calledOnClosed = false;
        Close();
        if (!calledOnClosed)
            OnClosed(new EventArgs());
    }

    protected override void OnClosed(EventArgs e) {
        base.OnClosed(e);
        calledOnClosed = true;
    }

    public void DragEnterProperties(Object sender, DragEventArgs e) {
        if (e.Data.GetDataPresent(typeof(Property)))
            e.Effect = DragDropEffects.Link;
    }

    public void DragDropProperties(Object sender, DragEventArgs e) {
        RequestAddThisProperty(e.Data.GetData(typeof(Property)) as Property);
    }

    public void DragEnterCards(Object sender, DragEventArgs e) {
        if (e.Data.GetDataPresent(typeof(Card)))
            e.Effect = DragDropEffects.Link;
    }

    public void DragDropCards(Object sender, DragEventArgs e) {
        RequestAddThisCard(e.Data.GetData(typeof(Card)) as Card);
    }

    public void EnteredCash(Object sender, EventArgs e) {
        if (thisState > State.Editing)
            RequestThisReopen();
    }

    public void LeftCash(Object sender, EventArgs e) {
        UpdateCashIfChanged();
    }

    protected bool cashChanged = false;

    public void CashChanged(Object sender, EventArgs e) {
        cashChanged = true;
        thisCash.Font = new Font(thisCash.Font, FontStyle.Bold);
        if (thisState > State.Editing)
            RequestThisReopen();
    }

    protected void UpdateCashIfChanged() {
        if (cashChanged)
            RequestSetThisCash(Convert.ToUInt32(thisCash.Value));
    }

}
