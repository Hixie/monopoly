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

// XXX automatically scroll to bottom if already at bottom
// XXX remember scroll position when resizing
// XXX add more log messages

public class LogForm : Form {

    Panel entries;
    MonopolyForm parent;

    public LogForm(MonopolyForm parent) : base() {
        this.parent = parent;
        Text = "Game Log";
        MinimumSize = new Size(Convert.ToInt32(Font.Size * 25),
                               Convert.ToInt32(Font.Size * 15));
        ClientSize = new Size(Convert.ToInt32(Font.Size * 50),
                              Convert.ToInt32(Font.Size * 30));
        FormBorderStyle = FormBorderStyle.SizableToolWindow;
        MinimizeBox = false;
        MaximizeBox = false;
        ShowInTaskbar = false;
        DockPadding.All = 8;
        entries = new Panel();
        entries.Dock = DockStyle.Fill;
        entries.BorderStyle = BorderStyle.Fixed3D;
        entries.AutoScroll = true;
        entries.BackColor = Color.FromKnownColor(KnownColor.Window);
        Controls.Add(entries);
        parent.AddOwnedForm(this);
    }

    protected override void OnClosing(CancelEventArgs e) {
        if (Visible) {
            // we don't want user triggered closing to dispose us
            e.Cancel = true;
            Hide();
        } else {
            base.OnClosing(e);
        }
    }

    protected int y = 0;

    public void AddEntry(params Object[] items) {
        // items contains a list of Strings interspersed with either
        // Properties, Cards, Players, or ints (for squares)
        LinkLabel label = new LinkLabel();
        label.AutoSize = true;
        label.UseMnemonic = false;
        label.Location = new Point(entries.AutoScrollPosition.X + 0,
                                   entries.AutoScrollPosition.Y + y);
        label.Text = DateTime.Now.ToString("HH:mm ");
        int links = 0;
        int count = 0;
        foreach (Object item in items) {
            ++count;
            if (item is String) {
                label.Text += item as String;
            } else if (item is Property) {
                AddLink(label, (item as Property).Name, item);
                ++links;
            } else if (item is Card) {
                AddLink(label, (item as Card).text, item);
                ++links;
            } else if (item is Player) {
                AddLink(label, (item as Player).Name, item);
                ++links;
            } else if (item == null) {
                if (count == 1)
                    label.Text += "The bank";
                else
                    label.Text += "the bank";
            } else {
                label.Text += item.ToString();
            }
            // else, error.
        } 
        if (links == 0) {
            label.LinkArea = new LinkArea(0, 0);
        }
        label.LinkClicked += new LinkLabelLinkClickedEventHandler(LinkClicked);
        y += label.PreferredHeight;
        entries.Controls.Add(label);
    }

    protected void AddLink(LinkLabel label, String s, Object item) {
        int index = label.Text.Length;
        label.Text += s;
        label.Links.Add(index, s.Length, item);
    }

    private void LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
        if (e.Link.LinkData is Property) {
            (e.Link.LinkData as Property).ShowWindow(parent);
        } else if (e.Link.LinkData is Card) {
            (e.Link.LinkData as Card).ShowWindow(parent);
        } else if (e.Link.LinkData is Player) {
            (e.Link.LinkData as Player).ShowWindow(parent);
        }
    }

}
