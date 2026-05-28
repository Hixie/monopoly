// -*- Mode: Java -*-

using System;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
using Microsoft.Win32;

public class DialogForm : Form {

    public DialogForm() : base() {
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MinimizeBox = false;
        MaximizeBox = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
    }

    static protected Panel WrapWithLabel(Control control, String caption) {
        Panel panel = new Panel();
        panel.DockPadding.All = 4;
        Label label = new Label();
        label.Text = caption + " ";
        label.Width = label.PreferredWidth;
        label.Dock = DockStyle.Left;
        label.TextAlign = ContentAlignment.MiddleLeft;
        label.TabIndex = 1;
        control.Dock = DockStyle.Fill;
        control.TabIndex = 2;
        panel.Height = Math.Max(label.PreferredHeight, control.Height)
            + panel.DockPadding.Top + panel.DockPadding.Bottom;
        panel.Width = label.PreferredWidth + control.Width
            + panel.DockPadding.Left + panel.DockPadding.Right;
        panel.Controls.AddRange(new Control[] {control, label});
        return panel;
    }

    static protected void NormalizeWrappedLabelWidths(Control[] controls) {
        int max = 0;
        foreach (Control p in controls)
            if (p.Controls.Count == 2 && p.Controls[1].Width > max)
                max = p.Controls[1].Width;
        foreach (Control p in controls)
            if (p.Controls.Count == 2)
                p.Controls[1].Width = max;
    }

}

public class ConnectForm : DialogForm {

    protected TextBox host;
    public String Host {
        get { return host.Text; }
        set { host.Text = value; }
    }

    protected NumericUpDown port;
    public int Port {
        get { return Convert.ToInt32(port.Value); }
        set { port.Value = value; }
    }

    protected TextBox playerName;
    public String PlayerName {
        get { return playerName.Text; }
        set { playerName.Text = value; }
    }

    protected ComboBox piece;
    public int Piece {
        get { return piece.SelectedIndex; }
        set {
            try {
                piece.SelectedIndex = value;
            } catch (ArgumentOutOfRangeException) {
                // do nothing
            }
        }
    }

    protected NumericUpDown playerID;
    public int PlayerID {
        get { return Convert.ToInt32(playerID.Value); }
        set { playerID.Value = value; }
    }

    protected NumericUpDown password;
    public uint Password {
        get { return Convert.ToUInt32(password.Value); }
        set { password.Value = value; }
    }

    protected CheckBox reconnectIfPossible;
    public bool ReconnectIfPossible {
        get { return reconnectIfPossible.Checked; }
        set { reconnectIfPossible.Checked = value; }
    }

    protected Panel joinPanel;
    protected Panel rejoinPanel;

    protected RadioButton optionPlay;
    protected RadioButton optionObserve;
    protected RadioButton optionRejoin;
    public OptionType Option {
        get {
            if (optionPlay.Checked) {
                return OptionType.Play;
            } else if (optionObserve.Checked) {
                return OptionType.Observe;
            } else {
                return OptionType.Rejoin;
            }
        }
        set {
            switch (value) {
            case OptionType.Play: optionPlay.Checked = true; break;
            case OptionType.Observe: optionObserve.Checked = true; break;
            case OptionType.Rejoin: optionRejoin.Checked = true; break;
            }
        }
    }

    public ConnectForm() : base() {

        // Controls

        Text = "Connect";

        host = new TextBox();
        Panel hostPanel = WrapWithLabel(host, "&Server:");

        port = new NumericUpDown();
        port.Minimum = UInt16.MinValue;
        port.Maximum = UInt16.MaxValue;
        Panel portPanel = WrapWithLabel(port, "Por&t:");

        optionPlay = new RadioButton();
        optionPlay.Text = "Pl&ayer";
        optionPlay.CheckedChanged += new EventHandler(ConnectAsChanged);
        optionObserve = new RadioButton();
        optionObserve.Text = "&Observer";
        optionObserve.CheckedChanged += new EventHandler(ConnectAsChanged);
        Panel optionSeparator = new Panel();
        optionRejoin = new RadioButton();
        optionRejoin.Text = "&Rejoin";
        optionRejoin.CheckedChanged += new EventHandler(ConnectAsChanged);

        playerName = new TextBox();
        Panel playerNamePanel = WrapWithLabel(playerName, "&Name:");

        piece = new ComboBox();
        piece.DropDownStyle = ComboBoxStyle.DropDownList;
        piece.Items.AddRange(new String[] {
            "Any",
            "Bag of money",
            "Battleship", // "Boat"
            "Cannon",
            "Dog",
            "Horse & Rider", // "Horse"
            "Iron",
            "Race Car", // "Car"
            "Shoe",
            "Thimble",
            "Top Hat",
            "Wheelbarrow",
        });
        piece.Sorted = true;
        Panel piecePanel = WrapWithLabel(piece, "&Piece:");

        reconnectIfPossible = new CheckBox();
        reconnectIfPossible.Text = "A&utomatically reconnect if possible";
        reconnectIfPossible.Dock = DockStyle.Fill;
        Panel reconnectIfPossiblePanel = new Panel();
        reconnectIfPossiblePanel.DockPadding.Left = 8;
        reconnectIfPossiblePanel.Height = reconnectIfPossible.Height +
            reconnectIfPossiblePanel.DockPadding.Top;
        reconnectIfPossiblePanel.Controls.Add(reconnectIfPossible);

        playerID = new NumericUpDown();
        playerID.Minimum = Byte.MinValue;
        playerID.Maximum = Byte.MaxValue;
        Panel playerIDPanel = WrapWithLabel(playerID, "Player &ID:");

        password = new NumericUpDown();
        password.Minimum = UInt32.MinValue;
        password.Maximum = UInt32.MaxValue;
        Panel passwordPanel = WrapWithLabel(password, "Pass&word:");
        
        Button ok = new Button();
        ok.Text = "OK";
        ok.DialogResult = DialogResult.OK;
        AcceptButton = ok;

        Button cancel = new Button();
        cancel.Text = "&Cancel";
        cancel.DialogResult = DialogResult.Cancel;
        CancelButton = cancel;

        // Layout

        Size = new Size(Font.Height * 30, Convert.ToInt32(Font.Height * 16));

        Panel rootPanel = new Panel();
        rootPanel.Dock = DockStyle.Fill;
        rootPanel.DockPadding.All = 4;

        Panel hostPortPanel = new Panel();
        hostPortPanel.Dock = DockStyle.Top;
        hostPortPanel.Height = Math.Max(hostPanel.Height, portPanel.Height);
        hostPortPanel.TabIndex = 1;
        hostPanel.Dock = DockStyle.Fill;
        portPanel.Width = Convert.ToInt32(Width * 0.3);
        portPanel.Dock = DockStyle.Right;
        hostPortPanel.Controls.AddRange(new Control[] { hostPanel, portPanel });

        Panel optionsOuterPanel = new Panel();
        optionsOuterPanel.DockPadding.All = 4;
        optionsOuterPanel.Dock = DockStyle.Left;
        optionsOuterPanel.Width = Math.Max(Math.Max(optionPlay.Width,
                                                    optionObserve.Width),
                                                    optionRejoin.Width) +
            optionsOuterPanel.DockPadding.Left +
            optionsOuterPanel.DockPadding.Right;
        optionsOuterPanel.TabIndex = 2;
        GroupBox optionsMiddleBox = new GroupBox();
        optionsMiddleBox.Text = "Connect as";
        optionsMiddleBox.Dock = DockStyle.Fill;
        Panel optionsInnerPanel = new Panel();
        optionsInnerPanel.DockPadding.Left = 8;
        optionsInnerPanel.Dock = DockStyle.Fill;
        optionPlay.Dock = DockStyle.Top;
        optionPlay.TabIndex = 1;
        optionObserve.Dock = DockStyle.Top;
        optionObserve.TabIndex = 2;
        optionSeparator.Dock = DockStyle.Top;
        optionSeparator.Height = 8;
        optionRejoin.Dock = DockStyle.Top;
        optionRejoin.TabIndex = 3;
        optionsInnerPanel.Controls.AddRange(new Control[] {
            optionRejoin, optionSeparator, optionObserve, optionPlay
        });
        optionsMiddleBox.Controls.Add(optionsInnerPanel);
        optionsOuterPanel.Controls.Add(optionsMiddleBox);

        Panel settingsOuterPanel = new Panel();
        settingsOuterPanel.Dock = DockStyle.Fill;
        settingsOuterPanel.DockPadding.All = 4;
        settingsOuterPanel.TabIndex = 3;
        GroupBox settingsMiddleBox = new GroupBox();
        settingsMiddleBox.Text = "Settings";
        settingsMiddleBox.Dock = DockStyle.Fill;
        Panel settingsInnerPanel = new Panel();
        settingsInnerPanel.Dock = DockStyle.Fill;
        settingsInnerPanel.DockPadding.Right = 4;
        NormalizeWrappedLabelWidths(new Control[] {
            playerNamePanel, piecePanel, playerIDPanel, passwordPanel,
        });
        joinPanel = new Panel();
        joinPanel.Dock = DockStyle.Fill;
        playerNamePanel.Dock = DockStyle.Top;
        playerNamePanel.TabIndex = 1;
        piecePanel.Dock = DockStyle.Top;
        piecePanel.TabIndex = 2;
        piecePanel.DockPadding.Bottom = 8;
        reconnectIfPossiblePanel.Dock = DockStyle.Top;
        reconnectIfPossiblePanel.TabIndex = 3;
        joinPanel.Controls.AddRange(new Control[] {
            reconnectIfPossiblePanel, piecePanel, playerNamePanel
        });
        rejoinPanel = new Panel();
        rejoinPanel.Dock = DockStyle.Fill;
        rejoinPanel.Hide();
        playerIDPanel.Dock = DockStyle.Top;
        playerIDPanel.TabIndex = 1;
        passwordPanel.Dock = DockStyle.Top;
        passwordPanel.TabIndex = 2;
        rejoinPanel.Controls.AddRange(new Control[] {
            playerIDPanel, passwordPanel
        });
        settingsInnerPanel.Controls.AddRange(new Control[] {
            joinPanel, rejoinPanel
        });
        settingsMiddleBox.Controls.Add(settingsInnerPanel);
        settingsOuterPanel.Controls.Add(settingsMiddleBox);

        Panel buttonsPanel = new Panel();
        buttonsPanel.Dock = DockStyle.Bottom;
        buttonsPanel.DockPadding.All = 4;
        buttonsPanel.Size = new Size(0,
                                     buttonsPanel.DockPadding.Top
                                     + ok.Height
                                     + buttonsPanel.DockPadding.Bottom);
        buttonsPanel.TabIndex = 4;
        Panel buttonSeparatorPanel = new Panel();
        buttonSeparatorPanel.Dock = DockStyle.Right;
        buttonSeparatorPanel.Size = new Size(8, 0);
        ok.Dock = DockStyle.Right;
        cancel.Dock = DockStyle.Right;
        buttonsPanel.Controls.AddRange(new Control[] {
            ok, buttonSeparatorPanel, cancel
        });

        rootPanel.Controls.AddRange(new Control[] {
            settingsOuterPanel,
            optionsOuterPanel,
            buttonsPanel,
            hostPortPanel,
        });

        Controls.Add(rootPanel);

        ActiveControl = ok;
    }

    protected void ConnectAsChanged(Object sender, EventArgs e) {
        if ((sender as RadioButton).Checked) {
            if (optionPlay.Checked || optionObserve.Checked) { 
                rejoinPanel.Hide();
                joinPanel.Show();
            } else {
                joinPanel.Hide();
                rejoinPanel.Show();
            }
        }
    }

}

public class GetTextForm : DialogForm {

    protected TextBox data;
    public String Data {
        get { return data.Text; }
        set { data.Text = value; }
    }

    static public bool GetText(IWin32Window parent, String caption, String message, String label, ref String value) {
        GetTextForm form = new GetTextForm(caption, message, label);
        form.Data = value;
        if (form.ShowDialog(parent) == DialogResult.OK) {
            value = form.Data;
            return true;
        }
        return false;
    }

    public GetTextForm(String a, String b, String c) : base() {

        Text = a;
        DockPadding.All = 8;

        Label message = new Label();
        message.UseMnemonic = false;
        message.Text = b;
        message.Dock = DockStyle.Top;

        data = new TextBox();
        Panel dataPanel = WrapWithLabel(data, c);
        dataPanel.Dock = DockStyle.Top;

        Button ok = new Button();
        ok.Text = "OK";
        ok.DialogResult = DialogResult.OK;
        AcceptButton = ok;

        Button cancel = new Button();
        cancel.Text = "&Cancel";
        cancel.DialogResult = DialogResult.Cancel;
        CancelButton = cancel;

        // Layout

        Size = new Size(Font.Height * 20, Convert.ToInt32(Font.Height * 10));

        Panel buttonsPanel = new Panel();
        buttonsPanel.Dock = DockStyle.Bottom;
        buttonsPanel.Size = new Size(0,
                                     buttonsPanel.DockPadding.Top
                                     + ok.Height
                                     + buttonsPanel.DockPadding.Bottom);
        Panel buttonSeparatorPanel = new Panel();
        buttonSeparatorPanel.Dock = DockStyle.Right;
        buttonSeparatorPanel.Size = new Size(8, 0);
        ok.Dock = DockStyle.Right;
        cancel.Dock = DockStyle.Right;
        buttonsPanel.Controls.AddRange(new Control[] {
            ok, buttonSeparatorPanel, cancel
        });

        Controls.AddRange(new Control[] {
            dataPanel, message, buttonsPanel
        });

        ActiveControl = data;
    }

}
