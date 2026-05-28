// -*- Mode: Java -*-

// XXX This assumes a pretty well behaved server. If the server starts
// sending messages at odd times, e.g. PIMP_STATE_POT in the middle of
// the game without an associated PIMP_STATE_BOARD, then the results
// are undefined. (Although it should never crash.)
//
// In theory, Ctrl+L (refresh) should reset the state if necessary.

using System;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
using Microsoft.Win32;

class Kaspar {
    // Entry point
    [STAThread] // need this to use OLE/drag and drop features.
    public static void Main() {
        // Create the main application window
        MonopolyForm form = new MonopolyForm();
        // Run the applications message pump
        Application.Run(form);
    }
}

public enum OptionType { Play, Observe, Rejoin }

// User Interface
public class MonopolyForm : Form {

    // Protected UI elements
    protected MonopolyBoard board;
    protected Panel controlPanel;
    protected Panel controlWelcomePanel;
    protected Panel controlYourTurnPanel;
    protected Panel controlYourTurnAgainPanel;
    protected Panel controlJailWithDicePanel;
    protected Panel controlJailWithoutDicePanel;
    protected Panel controlJailWithoutDiceNorHousesPanel;
    protected Panel controlJailPayBailPanel;
    protected Panel controlWaitingPanel;
    protected Panel controlWaitingOnTransactionPanel;
    protected Panel controlPropertySalePanel;
    protected Panel controlPropertyAuctionPanel;
    protected Panel controlOtherPlayerPropertySalePanel;
    protected Panel controlIncomeTaxPanel;

    protected Label waitingHeader;
    protected Label waitingText;
    protected Label turnTimerText;
    protected Timer turnTimerTimer;
    protected StatusBar statusBar;
    protected StatusBarPanel statusGamePanel;
    protected StatusBarPanel statusHousePanel;
    protected StatusBarPanel statusHotelPanel;
    protected StatusBarPanel statusNetworkPanel;
    protected Timer networkTimer; // network related timeouts
    protected RegistryKey key;
    protected NotifyIcon turnNotifier;
    protected Bitmap piece64Images;
    protected Bitmap piece16Images;
    protected ListView playerListView;
    protected ContextMenu boardContextMenu;
    protected MenuItem boardOpenMenuItem;
    protected MenuItem boardClaimRentMenuItem;
    protected MenuItem boardClaimSalaryMenuItem;
    protected MenuItem boardMortgageMenuItem;
    protected MenuItem boardUnmortgageMenuItem;
    protected MenuItem boardSellHouseMenuItem;
    protected MenuItem boardDividerMenuItem;
    protected ContextMenu playerContextMenu;
    protected MenuItem playerOpenMenuItem;
    protected MenuItem playerTradeMenuItem;
    protected MenuItem playerKickMenuItem;
    protected MenuItem playerTransferMenuItem;
    protected MenuItem DisconnectMenuItem;
    protected MenuItem SwitchPlayMenuItem;
    protected MenuItem TransferMenuItem;
    protected MenuItem ShowHouseBuyingFormMenuItem;
    protected MenuItem ShowPayRentTransactionMenuItem;
    protected MenuItem ShowClaimRentTransactionMenuItem;
    protected MenuItem AFKMenuItem;
    protected MenuItem RefreshMenuItem;
    protected MenuItem StatusBarMenuItem;
    protected NumericUpDown auctionBid;
    protected Button auctionBidButton;
    protected Button auctionNoBidButton;
    protected Button payRentButton;
    protected MenuItem debugMenuItem;
    protected MenuItem debugThrowDice;
    protected MenuItem debugThrowDiceSlowly;
    protected MenuItem debugBuyProperty;
    protected MenuItem debugAuctionProperty;
    protected MenuItem debugClaimGo;
    protected MenuItem debugClaimRent;
    protected MenuItem debugPayRent;
    protected MenuItem debugPayTax;
    protected MenuItem debugViolateTrademark;

    protected Panel lastShownPanel;
    protected HouseBuyingForm houseBuyingForm;
    protected LogForm logForm;
    protected Network network; // accessed by the player form
    public Network Network { get { return network; } }
    protected HybridDictionary transactions;
    protected HybridDictionary pendingTransactionBits;

    protected int gutterSize = 200;
    protected int numPieces;

    protected String server = "monopoly.damowmow.com"; // default
    protected int port = 13220; // default
    protected String playerName; // defaults to registry
    protected int playerPiece = 0; // defaults to registry
    protected uint game = 0; // not really used, but kept up to date
    protected int playerID = 0; // defaults to registry
    protected uint playerPassword = 0; // defaults to registry
    protected OptionType joinType = OptionType.Play; // default

    protected HybridDictionary players;
    protected Player thisPlayer;
    protected Player currentPlayer_;
    protected Player currentPlayer {
        get { return currentPlayer_; }
        set {
            if (currentPlayer_ != null)
                currentPlayer_.Item.Font = new Font(currentPlayer_.Item.Font, FontStyle.Regular);
            currentPlayer_ = value;
            if (currentPlayer_ != null)
                currentPlayer_.Item.Font = new Font(currentPlayer_.Item.Font, FontStyle.Bold);
        }
    }
    protected Player latestPlayer;
    public Player LatestPlayer { get { return latestPlayer; } }
    protected Transaction blockingTransaction;
    protected Transaction hiddenRentClaimTransaction;
    protected Transaction jailTransaction;
    protected Property currentProperty;

    protected Property[] properties;
    protected Card[] cards;

    protected Button NewControlButton(String caption, EventHandler handler) {
        Button button = new Button();
        button.Text = caption;
        button.Dock = DockStyle.Top;
        button.Font = new Font("Tahoma", 14, FontStyle.Bold);
        button.Height = Convert.ToByte(button.Font.Height * 2.5F);
        button.Click += handler;
        return button;
    }

    protected Label NewControlTitleLabel(String caption) {
        Label title = new Label();
        title.Text = caption;
        title.UseMnemonic = false;
        title.TextAlign = ContentAlignment.TopCenter;
        title.Dock = DockStyle.Top;
        title.Font = new Font("Tahoma", 20, FontStyle.Bold);
        title.Height = title.PreferredHeight;
        return title;
    }

    protected Label NewControlShortLabel(String caption) {
        Label label = new Label();
        label.Text = caption;
        label.UseMnemonic = false;
        label.TextAlign = ContentAlignment.TopCenter;
        label.Dock = DockStyle.Top;
        return label;
    }

    protected Label NewControlMediumLabel(String caption) {
        Label label = NewControlShortLabel(caption);
        label.Height = label.PreferredHeight * 5;
        return label;
    }

    protected Label NewControlFullLabel(String caption) {
        Label label = NewControlShortLabel(caption);
        label.Dock = DockStyle.Fill;
        return label;
    }

    protected Panel NewControlSplitterPanel() {
        Panel splitterPanel = new Panel();
        splitterPanel.Dock = DockStyle.Top;
        splitterPanel.Height = 8;
        return splitterPanel;
    }

    protected Panel NewControlPanel(String label, String comment, String question,
                                    Control[] controls) {
        Panel controlPanel = new Panel();
        controlPanel.Dock = DockStyle.Fill;
        controlPanel.AutoScroll = true; // XXX to really use this we have to stop docking controls
        Label title = NewControlTitleLabel(label);
        if (controls != null) {
            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.DockPadding.Left = 8;
            buttonPanel.DockPadding.Right = 8;
            int tabindex = controls.Length;
            foreach (Control c in controls) {
                c.TabIndex = --tabindex;
                buttonPanel.Controls.Add(NewControlSplitterPanel());
                buttonPanel.Controls.Add(NewControlSplitterPanel());
                buttonPanel.Controls.Add(c);
            }
            controlPanel.Controls.Add(buttonPanel);
        }
        controlPanel.Controls.Add(NewControlShortLabel(question));
        Label commentLabel = NewControlMediumLabel(comment);
        controlPanel.Tag = commentLabel;
        controlPanel.Controls.AddRange(new Control[] {
            commentLabel,
            NewControlSplitterPanel(),
            title,
        });
        controlPanel.Visible = false;
        return controlPanel;
    }

    public MonopolyForm() : base() {
        Text = "Kaspar";
        Icon = new Icon(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("monopoly.ico"));

        key = Registry.CurrentUser.OpenSubKey(@"Software\Ian Hickson\Kaspar", true);
        if (key != null) {
            try {
                playerID = Convert.ToInt32(key.GetValue("id", 0));
                playerPassword = Convert.ToUInt32(key.GetValue("password", 0));
                playerName = key.GetValue("name", SystemInformation.UserName).ToString();
                playerPiece = Convert.ToInt32(key.GetValue("piece", 0));
            } catch {
                // ignore errors
            }
        } else {
            // use defaults
            playerID = 0;
            playerPassword = 0;
            playerName = SystemInformation.UserName;
            playerPiece = 0;
            // try creating the registry key
            try {
                key = Registry.CurrentUser;
                key.CreateSubKey(@"Software\Ian Hickson\Kaspar");
                key = key.OpenSubKey(@"Software\Ian Hickson\Kaspar", true);
            } catch {
                // never mind
                key = null;
            }
        }

        ToolTip tooltips = new ToolTip();

        statusBar = new StatusBar();
        statusBar.ShowPanels = true;
        statusBar.Dock = DockStyle.Bottom;
        if (key != null)
            statusBar.Visible = Convert.ToBoolean(key.GetValue("status bar", true));
        statusGamePanel = new StatusBarPanel();
        statusGamePanel.AutoSize = StatusBarPanelAutoSize.Contents;
        statusGamePanel.MinWidth = gutterSize;
        statusBar.Panels.Add(statusGamePanel);
        statusNetworkPanel = new StatusBarPanel();
        statusNetworkPanel.AutoSize = StatusBarPanelAutoSize.Spring;
        statusBar.Panels.Add(statusNetworkPanel);
        statusHousePanel = new StatusBarPanel();
        statusHousePanel.AutoSize = StatusBarPanelAutoSize.Contents;
        statusHousePanel.Text = "0";
        statusBar.Panels.Add(statusHousePanel);
        statusHotelPanel = new StatusBarPanel();
        statusHotelPanel.AutoSize = StatusBarPanelAutoSize.Contents;
        statusHotelPanel.Text = "0";
        statusBar.Panels.Add(statusHotelPanel);

        DisconnectMenuItem = new MenuItem("&Disconnect", new EventHandler(DisconnectCommand));
        SwitchPlayMenuItem = new MenuItem("&Become Player", new EventHandler(SwitchPlayCommand));
        TransferMenuItem = new MenuItem("&Transfer to Bank...", new EventHandler(TransferCommand));
        ShowClaimRentTransactionMenuItem = new MenuItem("Open &Rent Claim", new EventHandler(ShowClaimRentTransactionCommand));
        ShowPayRentTransactionMenuItem = new MenuItem("Open &Rent Payment", new EventHandler(ShowPayRentTransactionCommand));
        ShowHouseBuyingFormMenuItem = new MenuItem("&Sell Houses...", new EventHandler(ShowHouseBuyingFormCommand));
        AFKMenuItem = new MenuItem("Away From &Keyboard", new EventHandler(AFKCommand), Shortcut.CtrlA);
        RefreshMenuItem = new MenuItem("&Refresh", new EventHandler(RefreshCommand), Shortcut.CtrlL);
        StatusBarMenuItem = new MenuItem("Status &Bar", new EventHandler(StatusBarCommand));
        StatusBarMenuItem.Checked = statusBar.Visible;

        MenuItem propertiesDebugMenuItem = new MenuItem("&Own Properties");
        debugViolateTrademark = new MenuItem("&Violate Trademark", new EventHandler(DebugViolateTrademarkCommand), Shortcut.CtrlV);

        debugMenuItem = new MenuItem("&Debug", new MenuItem[] {
            new MenuItem("&Toggle Debug", new EventHandler(ShowDebugCommand), Shortcut.CtrlShiftD),
            new MenuItem("-"),
            new MenuItem("Add &Animated Text", new EventHandler(DebugAddAnimatedTextCommand), Shortcut.CtrlShiftA),
            new MenuItem("Add &Card", new EventHandler(DebugAddCardCommand)),
            propertiesDebugMenuItem,
            debugViolateTrademark,
            new MenuItem("-"),
            debugThrowDice = new MenuItem("Throw &Dice Automatically", new EventHandler(ToggleMenuItem)),
            debugThrowDiceSlowly = new MenuItem("Throw Dice Slo&wly", new EventHandler(ToggleMenuItem)),
            debugClaimGo = new MenuItem("Claim &Salary Automatically", new EventHandler(ToggleMenuItem)),
            debugBuyProperty = new MenuItem("&Purchase Automatically", new EventHandler(ToggleMenuItem)),
            debugAuctionProperty = new MenuItem("A&uction Automatically", new EventHandler(ToggleMenuItem)),
            debugClaimRent = new MenuItem("Claim &Rent Automatically", new EventHandler(ToggleMenuItem)),
            debugPayRent = new MenuItem("Pa&y Rent Automatically", new EventHandler(ToggleMenuItem)),
            debugPayTax = new MenuItem("Pay Ta&x Automatically", new EventHandler(ToggleMenuItem)),
            new MenuItem("-"),
            AFKMenuItem,
        });
        if (key != null)
            debugMenuItem.Visible = Convert.ToBoolean(key.GetValue("debug", false));
        else
            debugMenuItem.Visible = false;

        Menu = new MainMenu(new MenuItem[] {
            new MenuItem("&File", new MenuItem[] {
                new MenuItem("&Connect...",
                             new EventHandler(ConnectCommand), Shortcut.CtrlO),
                new MenuItem("R&econnect...",
                             new EventHandler(ReconnectCommand), Shortcut.CtrlShiftO),
                DisconnectMenuItem,
                new MenuItem("-"),
                new MenuItem("E&xit",
                             new EventHandler(ExitCommand), Shortcut.CtrlQ),
            }),
            new MenuItem("&Game", new MenuItem[] {
                SwitchPlayMenuItem,
                TransferMenuItem,
                ShowHouseBuyingFormMenuItem,
                ShowClaimRentTransactionMenuItem,
                ShowPayRentTransactionMenuItem,
                new MenuItem("-"),
                RefreshMenuItem,
            }),
            new MenuItem("&View", new MenuItem[] {
                StatusBarMenuItem,
                new MenuItem("&Rotate",
                             new EventHandler(RotateCommand), Shortcut.CtrlR),
                new MenuItem("-"),
                new MenuItem("&Log", new EventHandler(LogCommand)),
            }),
            debugMenuItem,
        });

        int statusBarHeight = statusBar.Visible ? statusBar.Height : 0;
        ClientSize = new Size(gutterSize * 2, gutterSize + statusBarHeight);
        MinimumSize = Size;
        Rectangle r = SystemInformation.WorkingArea;
        if (r.Height > r.Width) {
            // portrait screen
            ClientSize = new Size((int)(r.Width * 0.8),
                                  (int)(r.Width * 0.8 - gutterSize + statusBarHeight));
        } else {
            // landscape screen
            ClientSize = new Size((int)((r.Height + gutterSize) * 0.8),
                                  (int)((r.Height + gutterSize) * 0.8 - gutterSize + statusBarHeight));
        }
        StartPosition = FormStartPosition.CenterScreen;

        boardClaimRentMenuItem = new MenuItem("&Claim Rent", new EventHandler(ClaimRentCommand));
        boardClaimRentMenuItem.DefaultItem = true;
        boardOpenMenuItem = new MenuItem("&Open", new EventHandler(ShowPropertyWindowCommand));
        boardClaimSalaryMenuItem = new MenuItem("&Claim Salary", new EventHandler(ClaimSalaryCommand));
        boardClaimSalaryMenuItem.DefaultItem = true;
        boardMortgageMenuItem = new MenuItem("&Mortgage", new EventHandler(MortgageCommand));
        boardUnmortgageMenuItem = new MenuItem("&Unmortgage", new EventHandler(UnmortgageCommand));
        boardSellHouseMenuItem = new MenuItem("&Sell House", new EventHandler(SellHouseCommand));
        boardDividerMenuItem = new MenuItem("-");
        boardContextMenu = new ContextMenu(new MenuItem[] {
            boardClaimRentMenuItem,
            boardOpenMenuItem,
            boardClaimSalaryMenuItem,
            boardMortgageMenuItem,
            boardUnmortgageMenuItem,
            boardSellHouseMenuItem,
            boardDividerMenuItem,
            new MenuItem("&Rotate Board",
                         new EventHandler(RotateCommand), Shortcut.CtrlR),
        });
        boardContextMenu.Popup += new EventHandler(SquareSetPopupMenu);

        board = new MonopolyBoard();
        if (key != null) {
            board.Text = key.GetValue("board text", Text).ToString();
            debugViolateTrademark.Checked = board.Text == "Monopoly";
        } else
            board.Text = Text;
        board.Dock = DockStyle.Fill;
        board.SquareHover += new MonopolyBoard.SquareEventHandler(SquareHover);
        board.SquareDoubleClick += new MonopolyBoard.SquareEventHandler(SquareDoubleClick);
        board.ContextMenu = boardContextMenu;

        statusHousePanel.Icon = new Icon(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("house.ico"));;
        statusHotelPanel.Icon = new Icon(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("hotel.ico"));;

        //board.GetHouseIcon(16);
        //board.GetHotelIcon(16);

        piece64Images = new Bitmap(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("pieces.png"));
        piece16Images = new Bitmap(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("pieces-16.png"));
        numPieces = piece16Images.Width / piece16Images.Height;

        playerOpenMenuItem = new MenuItem("&Open", new EventHandler(ShowPlayerWindowCommand));
        playerTradeMenuItem = new MenuItem("&Trade with", new EventHandler(TradePlayerCommand));
        playerKickMenuItem = new MenuItem("&Kick", new EventHandler(KickPlayerCommand));
        playerTransferMenuItem = new MenuItem("&Transfer everything to", new EventHandler(TransferPlayerCommand));
        playerContextMenu = new ContextMenu(new MenuItem[] {
            playerOpenMenuItem, playerTradeMenuItem,
            playerKickMenuItem, playerTransferMenuItem
        });
        playerContextMenu.MenuItems[0].DefaultItem = true;
        playerContextMenu.Popup += new EventHandler(UpdatePlayerListViewMenu);

        Panel propertiesQuickView = new Panel();
        propertiesQuickView.Height = 16;
        propertiesQuickView.DockPadding.Top = 8;
        propertiesQuickView.DockPadding.Bottom = 8;
        propertiesQuickView.Dock = DockStyle.Bottom;

        payRentButton = new Button();
        payRentButton.Text = ""; // filled in later
        //payRentButton.UseMnemonic = false;
        payRentButton.Dock = DockStyle.Bottom;
        payRentButton.Click += new EventHandler(payRent);
        payRentButton.Height = Convert.ToInt32(payRentButton.Font.Height * 2.5F);
        payRentButton.Visible = false;

        playerListView = new ListView();
        playerListView.Columns.Add("Name", -2, HorizontalAlignment.Left);
        playerListView.Columns.Add("Cash", -2, HorizontalAlignment.Left);
        playerListView.FullRowSelect = true;
        playerListView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
        playerListView.SmallImageList = new ImageList();
        playerListView.SmallImageList.ImageSize = new Size(piece16Images.Height, piece16Images.Height);
        for (int x = 0; x < piece16Images.Width; x += piece16Images.Height) {
            r = new Rectangle(x, 0, piece16Images.Height, piece16Images.Height);
            playerListView.SmallImageList.Images.Add(piece16Images.Clone(r, PixelFormat.Format32bppArgb));
        }
        playerListView.View = View.Details;
        playerListView.SelectedIndexChanged += new EventHandler(UpdatePlayerListViewMenu);
        playerListView.DoubleClick += new EventHandler(ShowPlayerWindowCommand);
        playerListView.Height = Font.Height * 10;
        playerListView.Dock = DockStyle.Bottom;
        playerListView.MultiSelect = false;

        controlWelcomePanel = new Panel();
        controlWelcomePanel.Dock = DockStyle.Fill;
        controlWelcomePanel.Controls.AddRange(new Control[] {
            NewControlFullLabel("To connect to a Kaspar server, press Ctrl+O.\n\nThe game will automatically begin once two players have joined."),
            NewControlSplitterPanel(),
            NewControlTitleLabel("Welcome"),
        });

        turnTimerText = new Label();
        turnTimerText.Text = "";
        turnTimerText.UseMnemonic = false;
        turnTimerText.TextAlign = ContentAlignment.MiddleCenter;
        turnTimerText.Height = turnTimerText.PreferredHeight;
        turnTimerText.Dock = DockStyle.Bottom;

        turnTimerTimer = new Timer();
        turnTimerTimer.Tick += new EventHandler(ClockTick);
        turnTimerTimer.Interval = 1000;
        turnTimerTimer.Start();

        controlWaitingPanel = new Panel();
        controlWaitingPanel.Dock = DockStyle.Fill;
        controlWaitingPanel.Controls.AddRange(new Control[] {
            turnTimerText,
            waitingText = NewControlFullLabel(""),
            NewControlSplitterPanel(),
            waitingHeader = NewControlTitleLabel("Waiting"),
        });

        controlYourTurnPanel =
            NewControlPanel("Your Turn",
                            "It is now your turn.",
                            "Do you want to:",
                            new Control[] {
                                NewControlButton("Buy Houses",
                                                 new EventHandler(DoBuyHouses)),
                                NewControlButton("Throw Dice",
                                                 new EventHandler(DoThrowDice)),
                            });

        controlYourTurnAgainPanel =
            NewControlPanel("Your Turn",
                            "It is your turn again, you rolled doubles.",
                            "When you are ready:",
                            new Control[] {
                                NewControlButton("Throw Dice",
                                                 new EventHandler(DoThrowDice)),
                            });

        controlJailWithDicePanel =
            NewControlPanel("Your Turn",
                            "", // filled later
                            "Do you want to:",
                            new Control[] {
                                NewControlButton("Buy Houses",
                                                 new EventHandler(DoBuyHouses)),
                                NewControlButton("Pay Bail",
                                                 new EventHandler(DoJailPayBail)),
                                NewControlButton("Throw Dice",
                                                 new EventHandler(DoJailThrowDice)),
                            });

        controlJailWithoutDicePanel =
            NewControlPanel("Your Turn",
                            "You are in jail but must now pay bail.",
                            "When you are ready:",
                            new Control[] {
                                NewControlButton("Buy Houses",
                                                 new EventHandler(DoBuyHouses)),
                                NewControlButton("Pay Bail",
                                                 new EventHandler(DoJailPayBail)),
                            });

        controlJailWithoutDiceNorHousesPanel =
            NewControlPanel("Your Turn",
                            "You are in jail but must now pay bail.",
                            "When you are ready:",
                            new Control[] {
                                NewControlButton("Pay Bail",
                                                 new EventHandler(DoJailPayBail)),
                            });

        controlJailPayBailPanel =
            NewControlPanel("Your Turn",
                            "You are in jail and must pay bail to continue.",
                            "Do you want to:",
                            new Control[] {
                                NewControlButton("Show Transaction",
                                                 new EventHandler(DoOpenJailTransaction)),
                                NewControlButton("Use Card",
                                                 new EventHandler(DoJailBailUseCard)),
                                NewControlButton("Pay Cash",
                                                 new EventHandler(DoJailBailPayCash)),
                            });

        controlWaitingOnTransactionPanel =
            NewControlPanel("Your Turn",
                            "You must complete this transaction to continue.",
                            "You may, if you like:",
                            new Control[] {
                                NewControlButton("Show Transaction",
                                                 new EventHandler(DoOpenCurrentTransaction)),
                            });

        controlPropertySalePanel =
            NewControlPanel("Your Turn",
                            "", // filled in later
                            "Do you want to:",
                            new Control[] {
                                NewControlButton("View Property",
                                                 new EventHandler(DoShowPropertyWindow)),
                                NewControlButton("Auction Property",
                                                 new EventHandler(DoAuctionProperty)),
                                NewControlButton("Purchase Property",
                                                 new EventHandler(DoBuyProperty)),
                            });

        controlOtherPlayerPropertySalePanel =
            NewControlPanel("Waiting",
                            "", // filled in later
                            "You may, if you like:",
                            new Control[] {
                                NewControlButton("View Property",
                                                 new EventHandler(DoShowPropertyWindow)),
                            });

        controlIncomeTaxPanel =
            NewControlPanel("Your Turn",
                            "You must pay income tax.",
                            "Do you want to:",
                            new Control[] {
                                NewControlButton("Pay 10%",
                                                 new EventHandler(DoIncomeTaxPay10)),
                                NewControlButton("Pay $200",
                                                 new EventHandler(DoIncomeTaxPay200)),
                            });

        Label auctionLabel = new Label();
        auctionLabel.Text = "&Bid: $";
        auctionLabel.Width = auctionLabel.PreferredWidth;
        auctionLabel.Dock = DockStyle.Left;
        auctionLabel.TextAlign = ContentAlignment.MiddleLeft;
        auctionLabel.TabIndex = 1;
        auctionBid = new NumericUpDown();
        auctionBid.Minimum = UInt32.MinValue;
        auctionBid.Maximum = UInt32.MaxValue;
        auctionBid.Dock = DockStyle.Fill;
        auctionBid.TabIndex = 2;
        Panel auctionBidPanel = new Panel();
        auctionBidPanel.Dock = DockStyle.Top;
        auctionBidPanel.DockPadding.Left = 8;
        auctionBidPanel.DockPadding.Right = 8;
        auctionBidPanel.Height = Math.Max(auctionLabel.PreferredHeight,
                                          auctionBid.Height);
        auctionBidPanel.Controls.AddRange(new Control[] {
            auctionBid, auctionLabel
        });

        controlPropertyAuctionPanel =
            NewControlPanel("Auction",
                            "", // filled in later
                            "Do you want to:",
                            new Control[] {
                                NewControlButton("View Property",
                                                 new EventHandler(DoShowPropertyWindow)),
                                auctionNoBidButton = NewControlButton("Don't Bid",
                                                                    new EventHandler(DoNoBidProperty)),
                                auctionBidButton = NewControlButton("Bid on This Property",
                                                                      new EventHandler(DoBidProperty)),
                                auctionBidPanel,
                            });

        controlPanel = new Panel();
        controlPanel.Dock = DockStyle.Fill;
        // XXX when resizing the control panel, resize all the panels
        // inside it to be the same height and don't make the inner
        // panels be docked
        controlPanel.Controls.AddRange(new Control[] {
            controlWelcomePanel,
            controlWaitingPanel,
            controlYourTurnPanel,
            controlYourTurnAgainPanel,
            controlJailWithDicePanel,
            controlJailWithoutDicePanel,
            controlJailWithoutDiceNorHousesPanel,
            controlJailPayBailPanel,
            controlWaitingOnTransactionPanel,
            controlPropertySalePanel,
            controlOtherPlayerPropertySalePanel,
            controlPropertyAuctionPanel,
            controlIncomeTaxPanel,
        });

        Panel sidebarPanel = new Panel();
        sidebarPanel.Width = gutterSize;
        sidebarPanel.DockPadding.All = 8;
        sidebarPanel.Dock = DockStyle.Right;
        sidebarPanel.Controls.AddRange(new Control[] {
            controlPanel,
            payRentButton,
            propertiesQuickView,
            playerListView,
        });

        Panel boardPanel = new Panel();
        boardPanel.Dock = DockStyle.Fill;
        boardPanel.BorderStyle = BorderStyle.Fixed3D;
        boardPanel.Controls.AddRange(new Control[] {board});

        Controls.AddRange(new Control[] {
            boardPanel, sidebarPanel, statusBar,
        });

        playerListView.Columns[0].Width = Convert.ToInt32(playerListView.ClientRectangle.Size.Width * 0.7F); // XXX when the number of players gets above the height, it adds a scrollbar without resizing the columns so it also adds a horizontal scrollbar. Bummer.

        properties = new Property[] {
            new Property(00, 0, 60, 50, 30, 2, 10, 30, 90, 160, 250, 0, board, 1),
            new Property(01, 0, 60, 50, 30, 4, 20, 60, 180, 320, 450, 0, board, 3),
            new Property(02, 1, 200, 0, 100, 25, 50, 100, 200, 0, 0, 8, board, 5),
            new Property(03, 0, 100, 50, 50, 6, 30, 90, 270, 400, 550, 1, board, 6),
            new Property(04, 0, 100, 50, 50, 6, 30, 90, 270, 400, 550, 1, board, 8),
            new Property(05, 0, 120, 50, 60, 8, 40, 100, 300, 450, 600, 1, board, 9),
            new Property(06, 0, 140, 100, 70, 10, 50, 150, 450, 625, 750, 2, board, 11),
            new Property(07, 2, 150, 0, 75, 4, 10, 0, 0, 0, 0, 9, board, 12),
            new Property(08, 0, 140, 100, 70, 10, 50, 150, 450, 625, 750, 2, board, 13),
            new Property(09, 0, 160, 100, 80, 12, 60, 180, 500, 700, 900, 2, board, 14),
            new Property(10, 1, 200, 0, 100, 25, 50, 100, 200, 0, 0, 8, board, 15),
            new Property(11, 0, 180, 100, 90, 14, 70, 200, 550, 750, 950, 3, board, 16),
            new Property(12, 0, 180, 100, 90, 14, 70, 200, 550, 750, 950, 3, board, 18),
            new Property(13, 0, 200, 100, 100, 16, 80, 220, 600, 800, 1000, 3, board, 19),
            new Property(14, 0, 220, 150, 110, 18, 90, 250, 700, 875, 1050, 4, board, 21),
            new Property(15, 0, 220, 150, 110, 18, 90, 250, 700, 875, 1050, 4, board, 23),
            new Property(16, 0, 240, 150, 120, 20, 100, 300, 750, 925, 1100, 4, board, 24),
            new Property(17, 1, 200, 0, 100, 25, 50, 100, 200, 0, 0, 8, board, 25),
            new Property(18, 0, 260, 150, 130, 22, 110, 330, 800, 975, 1150, 5, board, 26),
            new Property(19, 0, 260, 150, 130, 22, 110, 330, 800, 975, 1150, 5, board, 27),
            new Property(20, 2, 150, 0, 75, 4, 10, 0, 0, 0, 0, 9, board, 28),
            new Property(21, 0, 280, 150, 140, 24, 120, 360, 850, 1025, 1200, 5, board, 29),
            new Property(22, 0, 300, 200, 150, 26, 130, 390, 900, 1100, 1275, 6, board, 31),
            new Property(23, 0, 300, 200, 150, 26, 130, 390, 900, 1100, 1275, 6, board, 32),
            new Property(24, 0, 320, 200, 160, 28, 150, 450, 1000, 1200, 1400, 6, board, 34),
            new Property(25, 1, 200, 0, 100, 25, 50, 100, 200, 0, 0, 8, board, 35),
            new Property(26, 0, 350, 200, 175, 35, 175, 500, 1100, 1300, 1500, 7, board, 37),
            new Property(27, 0, 400, 200, 200, 50, 200, 600, 1400, 1700, 2000, 7, board, 39),
        };

        foreach (Property p in properties) {
            propertiesDebugMenuItem.MenuItems.Add(new MenuItem(p.id.ToString(), new EventHandler(DebugToggleOwnCommand)));
        }

        // XXX this should _so_ be a custom control... :-)
        foreach (Property p in properties) {
            // the card
            Panel q = new Panel();
            q.Tag = p;
            q.BackColor = Color.White;
            q.BorderStyle = BorderStyle.FixedSingle;
            q.Width = 26;
            q.Height = 32;
            q.Visible = false;
            q.VisibleChanged += new EventHandler(QuickViewRelayout);
            q.Click += new EventHandler(OpenThing);
            q.MouseDown += new MouseEventHandler(QuickViewDrag);
            tooltips.SetToolTip(q, p.Name);
            if (p.type == 0) {
                // the color bit on the card
                Panel c = new Panel();
                c.Tag = p;
                c.Dock = DockStyle.Top;
                c.Height = Convert.ToInt32(q.Height * board.ColorHeight);
                c.BackColor = p.BackColor;
                c.Click += new EventHandler(OpenThing);
                c.MouseDown += new MouseEventHandler(QuickViewDrag);
                tooltips.SetToolTip(c, p.Name);
                q.Controls.Add(c);
            } else {
                // the "R" or "E" or "W"
                Label l = new Label();
                l.Tag = p;
                l.Dock = DockStyle.Fill;
                l.TextAlign = ContentAlignment.MiddleCenter;
                switch (p.id) {
                case 7:
                    l.Text = "E";
                    l.Font = new Font("Times New Roman", q.Height / 2, FontStyle.Bold);
                    break;
                case 20:
                    l.Text = "W";
                    l.Font = new Font("Times New Roman", q.Height / 2, FontStyle.Bold);
                    break;
                default:
                    l.Text = "R";
                    l.Font = new Font("Times New Roman", q.Height / 2, FontStyle.Bold | FontStyle.Italic);
                    break;
                }
                l.Click += new EventHandler(OpenThing);
                l.MouseDown += new MouseEventHandler(QuickViewDrag);
                tooltips.SetToolTip(l, p.Name);
                q.Controls.Add(l);
            }
            // the container with the padding
            p.QuickView = q;
            propertiesQuickView.Controls.Add(q);
        }

        cards = new Card[] {
            new Card(00, 6, 0, 0, -1, "Advance to Go and collect $400."),
            new Card(01, 6, 0, 24, -1, "Advance to Illinois Avenue. If you pass Go, collect $200."),
            new Card(02, 10, 0, 2, -1, "Advance token to nearest Utility. If unowned, you may buy it from the Bank. If owned, throw dice and pay the owner ten times the amount thrown."),
            new Card(03, 10, 0, 1, -1, "Advance token to the nearest Railroad and pay owner twice the rental to which he/she is otherwise entitled. If Railroad is unowned, you may buy it from the Bank."),
            new Card(04, 10, 0, 1, -1, "Advance token to the nearest Railroad and pay owner twice the rental to which he/she is otherwise entitled. If Railroad is unowned, you may buy it from the Bank."),
            new Card(05, 6, 0, 11, -1, "Advance to St. Charles Place. If you pass Go, collect $200."),
            new Card(06, 2, 0, 50, -1, "Bank pays you dividend of $50."),
            new Card(07, 1, 0, -1, -1, "Get out of Jail free. This card may be kept until needed or sold."),
            new Card(08, 9, 0, 3, -1, "Go back 3 spaces."),
            new Card(09, 12, 0, -1, -1, "Go directly to Jail. Do not pass Go, do not collect $200."),
            new Card(10, 14, 0, 25, 100, "Make general repairs on all your property. For each house pay $25, for each hotel $100."),
            new Card(11, 3, 0, 15, -1, "You eat a supersize burrito. Pay $15."), // "Pay poor tax of $15."
            new Card(12, 6, 0, 5, -1, "Take a ride on the Reading. If you pass Go collect $200."),
            new Card(13, 6, 0, 39, -1, "Take a walk on the Boardwalk: advance token to Boardwalk."),
            new Card(14, 5, 0, 50, -1, "You have been elected chairman of the board. Pay each player $50."),
            new Card(15, 2, 0, 150, -1, "Your building and loan matures. Collect $150."),
            new Card(16, 6, 1, 0, -1, "Advance to Go and collect $400."),
            new Card(17, 2, 1, 200, -1, "Bank error in your favor. Collect $200."),
            new Card(18, 3, 1, 50, -1, "Your monthly TiVo subscription is due. Pay $50."), // "Doctor's fee. Pay $50."
            new Card(19, 2, 1, 45, -1, "From eBay auction you get $45."),  // "From sale of stock you get $45."
            new Card(20, 1, 1, -1, -1, "Get out of Jail free. This card may be kept until needed or sold."),
            new Card(21, 12, 1, -1, -1, "Go to Jail. Do not pass Go, do not collect $200."),
            new Card(22, 4, 1, 50, -1, "Grand opera opening. Collect $50 from every player for opening night seats."),
            new Card(23, 2, 1, 20, -1, "Life insurance matures, collect $20."), // Income tax refund, collect $20.
            new Card(24, 2, 1, 100, -1, "You release software only 5 months after the due date. Collect $100 bonus from shocked manager."), // "Life insurance matures, collect $100."
            new Card(25, 3, 1, 100, -1, "You contract SARS. Pay hospital $100."), // "Pay hospital $100."
            new Card(26, 3, 1, 150, -1, "You go to a restaurant with Brendan. Your share of the bill is $150."), // "Pay school tax of $150."
            new Card(27, 2, 1, 25, -1, "Receive $25 for \"services\"."), // "Receive for services $25"
            new Card(28, 2, 1, 100, -1, "You answer a particularly difficult question on Google Answers. Collect $100."), // "Christmas fund matures, collect $100."
            new Card(29, 14, 1, 40, 115, "You must upgrade the Internet connectivity in all your properties. For each house pay $40, for each hotel $115."), // "You are assessed for street repairs. Pay $40 per house, $115 per hotel."
            new Card(30, 2, 1, 10, -1, "You have won second prize in a beauty contest. Collect $10."),
            new Card(31, 2, 1, 100, -1, "You inherit $100."),
        };

        houseBuyingForm = new HouseBuyingForm(this, properties);
        houseBuyingForm.Closed += new EventHandler(DoneBuyingHouses);
        houseBuyingForm.Confirm += new HouseBuyingForm.ConfirmEventHandler(ConfirmedBuyHouses);

        logForm = new LogForm(this);

        transactions = new HybridDictionary(4);
        pendingTransactionBits = new HybridDictionary(2);

        turnNotifier = new NotifyIcon();
        turnNotifier.Icon = Icon;
        turnNotifier.Text = "It is your turn.";
        turnNotifier.Click += new EventHandler(turnNotifierClicked);

        networkTimer = new Timer();
        networkTimer.Tick += new EventHandler(Startup);
        networkTimer.Interval = 1;
        networkTimer.Start();

        players = new HybridDictionary(8);

    }

    protected void ShowDebugCommand(Object sender, EventArgs e) {
        debugMenuItem.Visible = !debugMenuItem.Visible;
    }
    
    protected void DebugToggleOwnCommand(Object sender, EventArgs e) {
        int i = Convert.ToInt32((sender as MenuItem).Text);
        Property p = properties[i];
        p.Owner = thisPlayer;
    }
    
    protected int debugTestSquare = 0;
    protected void DebugAddAnimatedTextCommand(Object sender, EventArgs e) {
        if (debugTestSquare >= MonopolyBoard.SquareNames.Length)
            debugTestSquare = 0;
        board.AddAnimatedTextSquare("TEST", Color.Yellow, debugTestSquare++);
    }
    
    protected void DebugAddCardCommand(Object sender, EventArgs e) {
        int i = new Random().Next(cards.Length);
        board.Card = cards[i];
        cards[i].ShowWindow(this);
    }

    protected void ToggleMenuItem(Object sender, EventArgs e) {
        (sender as MenuItem).Checked =! (sender as MenuItem).Checked;
    }

    protected void DebugViolateTrademarkCommand(Object sender, EventArgs e) {
        if (debugViolateTrademark.Checked) {
            board.Text = Text;
        } else {
            board.Text = "Monopoly";
        }
        if (key != null)
            key.SetValue("board text", board.Text);
        debugViolateTrademark.Checked = board.Text == "Monopoly";
    }

    protected void OpenThing(Object sender, EventArgs e) {
        ((sender as Control).Tag as Thing).ToggleWindow(this);
    }

    protected void QuickViewRelayout(Object sender, EventArgs e) {
        Point l = new Point(4, 8);
        Size s = new Size(0, 16);
        (sender as Control).Parent.SuspendLayout();
        foreach (Property p in properties) {
            Control panel = p.QuickView;
            if (panel.Visible) {
                if (l.X + panel.Width > (sender as Control).Parent.Width) {
                    l.Y += panel.Height + 4;
                    l.X = 4;
                }
                panel.Location = l;
                s.Width += panel.Width + 4;
                if (l.X == 4) {
                    s.Height += panel.Height + 4;
                }
                l.X += panel.Width + 4;
            }
        }
        (sender as Control).Parent.Size = s;
        (sender as Control).Parent.ResumeLayout();
    }

    protected void QuickViewDrag(Object sender, MouseEventArgs e) {
        if (e.Button == MouseButtons.Left) {
            // let's roll
            DoDragDrop((sender as Control).Tag, DragDropEffects.Link);
        }
    }

    class PropertySquareComparer : IComparer {
        public int Compare(Object x, Object y) {
            int a = (x as Property).square;
            int b = (int)y;
            if (a < b)
                return -1;
            else if (a > b)
                return 1;
            else
                return 0;
        }
    }

    protected Property GetPropertyFromSquare(int square) {
        return GetProperty(Array.BinarySearch(properties, square, new PropertySquareComparer()));
    }

    protected Property GetProperty(int property) {
        if (property < 0 || property >= properties.Length)
            return null;
        return properties[property];
    }

    protected Card GetCard(int card) {
        if (card < 0 || card >= cards.Length)
            return null;
        return cards[card];
    }

    protected Transaction AddTransaction(uint id, String s, Object r, uint a, Player p1, Player p2, bool c, bool show) {
        Transaction transaction = new Transaction(id, s, r, a, p1, p2, c, this, show);
        transactions.Add(transaction.id, transaction);
        transaction.Closed += new EventHandler(RemoveTransaction);
        if (p2 != null && pendingTransactionBits[p2.ID] != null) {
            if (pendingTransactionBits[p2.ID] is Property)
                transaction.RequestAddThisProperty(pendingTransactionBits[p2.ID] as Property);
            else // is Card
                transaction.RequestAddThisCard(pendingTransactionBits[p2.ID] as Card);
            pendingTransactionBits.Remove(p2.ID);
        }
        return transaction;
    }

    protected void RemoveTransaction(Object sender, EventArgs e) {
        transactions.Remove((sender as Transaction).id);
    }

    public bool AddToPendingTransactionList(Player p, Thing thing) {
        foreach (Transaction t in transactions.Values) {
            if (t.otherPlayer == p) {
                if (thing is Property)
                    t.RequestAddThisProperty(thing as Property);
                else
                    t.RequestAddThisCard(thing as Card);
                return false;
            }
        }
        pendingTransactionBits[p.ID] = thing;
        return true;
    }

    protected bool FindPiece(int piece) {
        foreach (Player p in players.Values)
            if (p.Piece == piece - 1)
                return true;
        return false;
    }

    protected int GetSparePiece(int id) { // (GetFreePiece)
        if (id == playerID &&
            playerPiece > 0 &&
            playerPiece <= numPieces)
            return playerPiece;
        int max = 0;
        while (true) {
            for (int trial = 0; trial < numPieces; ++trial) {
                int count = 0;
                foreach (Player p in players.Values)
                    if (p.Piece == trial)
                        ++count;
                if (count <= max)
                    return trial;
            }
            ++max;
        }
    }

    protected Player AddPlayer(int id, String name, int piece, bool playing, bool isUs) {
        return AddPlayer(id, name, piece, playing, 0, 0, false, isUs);
    }

    protected Player AddPlayer(int id, String name, int piece, bool playing,
                               uint cash, int square, bool jail, bool isUs) {
        if (players[id] != null)
            throw new ArgumentOutOfRangeException();
        if (piece <= 0 || piece > numPieces || FindPiece(piece))
            piece = GetSparePiece(id);
        else
            piece = piece-1;
        Player player = new Player(id, name, piece, playing, cash, square, jail, isUs, board, piece64Images.Clone(new Rectangle(piece * piece64Images.Height, 0, piece64Images.Height, piece64Images.Height), PixelFormat.Format32bppArgb));
        players.Add(player.ID, player);
        playerListView.Items.Add(player.Item);
        if (playing)
            board.AddPiece(player.ID, player.Image, player.Square, player.InJail);
        return player;
    }

    protected Player GetPlayer(int id) {
        foreach (Player p in players.Values) {
            if (p.ID == id)
                return p;
        }
        return null;
    }

    protected int NumPlayers() {
        int count = 0;
        foreach (Player p in players.Values) {
            if (p.Playing)
                ++count;
        }
        return count;
    }

    protected void MakePlayerIntoObserver(Player p, bool updatePiece) {
        if (!p.Playing) return;
        p.Playing = false;
        p.Item.ImageIndex = -1;
        p.Item.ListView.Invalidate();
        if (updatePiece)
            board.RemovePiece(p.ID);
        if (p == thisPlayer && p == currentPlayer)
            ShowPanel(controlWelcomePanel);
    }

    protected void MakeObserverIntoPlayer(Player p, bool updatePiece) {
        if (p.Playing) return;
        p.Playing = true;
        p.Item.ImageIndex = p.Piece;
        p.Item.ListView.Invalidate();
        if (updatePiece)
            board.AddPiece(p.ID, p.Image, p.Square, p.InJail);
    }

    protected void ShowPanel(Panel p) {
        foreach (Control c in p.Parent.Controls)
            if (c is Panel)
                c.Visible = c == p;
        lastShownPanel = p;
        p.Invalidate(true);
        p.Update();
    }

    protected void disableStartOfTurnButtons() {
        controlYourTurnPanel.Enabled = false;
        controlJailWithDicePanel.Enabled = false;
        controlJailWithoutDicePanel.Enabled = false;
    }

    protected void enableStartOfTurnButtons(Object sender, EventArgs e) {
        board.AnimationsDone -= new EventHandler(enableStartOfTurnButtons);
        controlYourTurnPanel.Enabled = true;
        controlJailWithDicePanel.Enabled = true;
        controlJailWithoutDicePanel.Enabled = true;
        if (debugThrowDice.Checked && debugThrowDiceSlowly.Checked) {
            System.Threading.Thread.Sleep(50);
            if (lastShownPanel == controlYourTurnPanel) {
                DoThrowDice(null, new EventArgs());
            } else if (lastShownPanel == controlJailWithDicePanel) {
                DoJailThrowDice(null, new EventArgs());
            } else if (lastShownPanel == controlJailWithoutDicePanel) {
                DoJailPayBail(null, new EventArgs());
            }
            // else something odd going on, just do nothing
        }        
    }

    protected void Startup(Object sender, EventArgs e) {
        networkTimer.Stop();
        networkTimer.Tick -= new EventHandler(Startup);
        ShowConnectDialog(false);
        // we used to check the return value and quit if it was false (cancelled)
        // but whatever, it seems that if the user wants to just look at the board
        // he should be able to. It is a pretty board...
    }

    protected void notifyUserItIsHisTurn() {
        if (ActiveForm != this)
            turnNotifier.Visible = true;
    }

    protected void turnNotifierClicked(Object sender, EventArgs e) {
        WindowState = FormWindowState.Normal;
        Activate();
    }

    override protected void OnActivated(EventArgs e) {
        base.OnActivated(e);
        turnNotifier.Visible = false;
    }

    override protected void OnMenuStart(EventArgs e) {
        base.OnMenuStart(e);
        DisconnectMenuItem.Enabled = network != null;
        SwitchPlayMenuItem.Enabled = (thisPlayer != null) && (!thisPlayer.Playing);
        TransferMenuItem.Enabled = (thisPlayer != null) && (thisPlayer.Playing);
        ShowHouseBuyingFormMenuItem.Enabled = (thisPlayer != null) && (thisPlayer.Playing);
        ShowPayRentTransactionMenuItem.Enabled = payRentButton.Visible;
        ShowClaimRentTransactionMenuItem.Enabled = hiddenRentClaimTransaction != null;
        AFKMenuItem.Enabled = network != null;
        RefreshMenuItem.Enabled = network != null;
    }

    override protected void OnSizeChanged(EventArgs e) {
        base.OnSizeChanged(e);
        statusBar.SizingGrip = WindowState == FormWindowState.Normal;
    }

    protected void UpdatePlayerListViewMenu(Object sender, EventArgs e) {
        if (playerListView.SelectedItems.Count > 0) {
            Player p = (Player)playerListView.SelectedItems[0].Tag;
            playerTradeMenuItem.Enabled = (thisPlayer != null) && (thisPlayer.Playing) && (p.Playing) && (p != thisPlayer);
            playerKickMenuItem.Enabled = (p == currentPlayer) && (p != thisPlayer);
            playerTransferMenuItem.Enabled = (thisPlayer != null) && (thisPlayer.Playing) && (p != thisPlayer);
            playerListView.ContextMenu = playerContextMenu;
        } else {
            playerListView.ContextMenu = null;
        }
    }

    protected void ShowPlayerWindowCommand(Object sender, EventArgs e) {
        if (playerListView.SelectedIndices.Count > 0) {
            Player p = (Player)playerListView.SelectedItems[0].Tag;
            if (p != null)
                p.ShowWindow(this);
        }
    }

    protected void TradePlayerCommand(Object sender, EventArgs e) {
        if (playerListView.SelectedIndices.Count > 0) {
            Player p = (Player)playerListView.SelectedItems[0].Tag;
            if (p != null && p != thisPlayer && network != null)
                network.SendMessage(new PIMP_TRANSACTION_REQUEST_TRADE(Convert.ToByte(p.ID)));
        }
    }

    protected void KickPlayerCommand(Object sender, EventArgs e) {
        if (playerListView.SelectedIndices.Count > 0) {
            Player p = (Player)playerListView.SelectedItems[0].Tag;
            if (p != null && network != null)
                network.SendMessage(new PIMP_KICK(Convert.ToByte(p.ID)));
        }
    }

    protected void TransferPlayerCommand(Object sender, EventArgs e) {
        if (playerListView.SelectedIndices.Count > 0) {
            Player p = (Player)playerListView.SelectedItems[0].Tag;
            if (p != null && network != null) {
                if (MessageBox.Show(this,
                                    String.Format("Are you sure you want to transfer all your assets to {0}?", p.Name),
                                    "Kaspar", MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question,
                                    MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    network.SendMessage(new PIMP_TRANSFER(Convert.ToByte(p.ID)));
            }
        }
    }

    protected void ShowPropertyWindowCommand(Object sender, EventArgs e) {
        Property p = GetPropertyFromSquare(board.FocussedSquare);
        if (p != null)
            p.ShowWindow(this);
    }

    protected void ClaimRentCommand(Object sender, EventArgs e) {
        Property p = GetPropertyFromSquare(board.FocussedSquare);
        if (p != null && latestPlayer != null && network != null)
            network.SendMessage(new PIMP_CLAIM_RENT(Convert.ToByte(latestPlayer.ID),
                                                    Convert.ToByte(p.id)));
    }

    protected void ClaimSalaryCommand(Object sender, EventArgs e) {
        if (network != null)
            network.SendMessage(new PIMP_CLAIM_GO(0));
    }

    protected void MortgageCommand(Object sender, EventArgs e) {
        if (network == null) return;
        Property p = GetPropertyFromSquare(board.FocussedSquare);
        if (p == null) return;
        network.SendMessage(new PIMP_MORTGAGE_PROPERTY(Convert.ToByte(Convert.ToByte(p.id))));
    }

    protected void UnmortgageCommand(Object sender, EventArgs e) {
        if (network == null) return;
        Property p = GetPropertyFromSquare(board.FocussedSquare);
        if (p == null) return;
        network.SendMessage(new PIMP_UNMORTGAGE_PROPERTY(Convert.ToByte(Convert.ToByte(p.id))));
    }

    protected void SellHouseCommand(Object sender, EventArgs e) {
        if (network == null) return;
        Property p = GetPropertyFromSquare(board.FocussedSquare);
        if (p == null) return;
        int a = p.CurrentHotels > 0 ? 0 : 1;
        int b = p.CurrentHotels > 0 ? 1 : 0;
        network.SendMessage(new PIMP_PURCHASE_HOUSES(new byte[] { Convert.ToByte(Convert.ToByte(p.id)) },
                                                     new sbyte[] { Convert.ToSByte(-a) },
                                                     new sbyte[] { Convert.ToSByte(-b) }));
    }

    protected void SwitchPlayCommand(Object sender, EventArgs e) {
        if (network != null)
            network.SendMessage(new PIMP_SWITCH_PLAY());
    }

    protected void TransferCommand(Object sender, EventArgs e) {
        if (network == null) return;
        if (MessageBox.Show(this,
                            "Are you sure you want to transfer all your assets to the bank?",
                            "Kaspar", MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            network.SendMessage(new PIMP_TRANSFER(0));
    }

    protected void ShowPayRentTransactionCommand(Object sender, EventArgs e) {
        if (payRentButton.Visible)
            (payRentButton.Tag as Transaction).Show();
    }

    protected void ShowClaimRentTransactionCommand(Object sender, EventArgs e) {
        if (hiddenRentClaimTransaction != null)
            hiddenRentClaimTransaction.Show();
    }

    protected void AFKCommand(Object sender, EventArgs e) {
        if (network == null) return;
        network.SendMessage(new PIMP_AFK());
    }

    protected void RefreshCommand(Object sender, EventArgs e) {
        if (network == null) return;
        network.SendMessage(new PIMP_REQUEST_STATE());
        Waiting("Please Wait", "Updating board with latest game information...");
    }

    protected void StatusBarCommand(Object sender, EventArgs e) {
        statusBar.Visible = !statusBar.Visible;
        StatusBarMenuItem.Checked = statusBar.Visible;
        if (key != null)
            key.SetValue("status bar", statusBar.Visible);
    }

    protected void DoThrowDice(Object sender, EventArgs e) {
        if (network == null) return;
        network.SendMessage(new PIMP_THROW_DICE());
        Waiting("Your Turn", "Dice thrown.");
    }

    protected void DoBuyHouses(Object sender, EventArgs e) {
        controlWaitingPanel.Tag = lastShownPanel;
        Waiting("Your Turn", "Purchasing houses and hotels.\n\nClick \"Confirm\" on the house purchase window when you are ready to continue with your turn.");
        houseBuyingForm.ShowWindow(thisPlayer);
    }

    protected void ShowHouseBuyingFormCommand(Object sender, EventArgs e) {
        if (!houseBuyingForm.Visible) {
            controlWaitingPanel.Tag = null;
            houseBuyingForm.ShowWindow(thisPlayer);
        }
    }

    protected void DoneBuyingHouses(Object sender, EventArgs e) {
        if (controlWaitingPanel.Tag != null)
            ShowPanel((Panel)(controlWaitingPanel.Tag));
    }

    protected void ConfirmedBuyHouses(Object sender,
                                      HouseBuyingForm.ConfirmEventArgs e) {
        if (network == null) return;
        network.SendMessage(new PIMP_PURCHASE_HOUSES(e.ids, e.houses, e.hotels));
    }


    protected void DoJailPayBail(Object sender, EventArgs e) {
        if (network == null) return;
        network.SendMessage(new PIMP_JAIL_PAY_BAIL());
        Waiting("Your Turn", "Paying bail.");
    }

    protected void DoJailBailPayCash(Object sender, EventArgs e) {
        if (network == null) return;
        jailTransaction.RequestThisAgree();
    }

    protected void DoJailBailUseCard(Object sender, EventArgs e) {
        if (network == null) return;
        if (thisPlayer.Cards.Count > 0) {
            jailTransaction.RequestSetThisCash(0);
            foreach (Card card in thisPlayer.Cards.Values) {
                jailTransaction.RequestAddThisCard(card);
                break; // XXX is there a simpler way?
            }
            jailTransaction.RequestThisFinish();
            jailTransaction.RequestThisAgree();
        } else {
            jailTransaction.Show();
            MessageBox.Show(this,
                            "You do not have a get out of jail free card.",
                            "Kaspar",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
        }
    }

    protected void DoJailThrowDice(Object sender, EventArgs e) {
        if (network == null) return;
        network.SendMessage(new PIMP_JAIL_ROLL_DICE());
        Waiting("Your Turn", "Dice thrown.");
    }

    protected void DoOpenCurrentTransaction(Object sender, EventArgs e) {
        blockingTransaction.Show();
    }

    protected void DoOpenJailTransaction(Object sender, EventArgs e) {
        jailTransaction.Show();
    }

    protected void DoBuyProperty(Object sender, EventArgs e) {
        if (network == null || currentProperty == null) return;
        network.SendMessage(new PIMP_BUY_PROPERTY());
        Waiting("Your Turn", "Buying " + currentProperty.Name + ".");
    }

    protected void DoAuctionProperty(Object sender, EventArgs e) {
        if (network == null || currentProperty == null) return;
        network.SendMessage(new PIMP_AUCTION_PROPERTY());
        Waiting("Your Turn", "Putting " + currentProperty.Name + " up for auction.");
    }

    protected void DoBidProperty(Object sender, EventArgs e) {
        if (network == null) return;
        network.SendMessage(new PIMP_BID(Convert.ToUInt32(auctionBid.Value)));
    }

    protected void DoNoBidProperty(Object sender, EventArgs e) {
        if (network == null) return;
        network.SendMessage(new PIMP_NO_BID());
    }

    protected void DoShowPropertyWindow(Object sender, EventArgs e) {
        if (currentProperty == null) return;
        currentProperty.ShowWindow(this);
    }

    protected void DoIncomeTaxPay200(Object sender, EventArgs e) {
        if (network == null) return;
        network.SendMessage(new PIMP_TAX_PAY_FLAT_FEE());
        Waiting("Your Turn", "Paying $200.");
    }

    protected void DoIncomeTaxPay10(Object sender, EventArgs e) {
        if (network == null) return;
        network.SendMessage(new PIMP_TAX_PAY_TEN_PERCENT());
        Waiting("Your Turn", "Paying 10%.");
    }

    protected void ConnectCommand(Object sender, EventArgs e) {
        ShowConnectDialog(false);
    }

    protected void ReconnectCommand(Object sender, EventArgs e) {
        ShowConnectDialog(true);
    }

    protected void DisconnectCommand(Object sender, EventArgs e) {
        if (network == null) return;
        network.SendMessage(new PIMP_AFK());
        Disconnect();
    }

    protected bool ShowConnectDialog(bool reconnect) {
        ConnectForm form = new ConnectForm();
        bool resume = false;
        try {
            if (network != null) {
                System.Threading.Monitor.Enter(network);
                resume = true;
            }
            form.Option = reconnect ? OptionType.Rejoin : OptionType.Play;
            form.Host = server;
            form.Port = port;
            form.PlayerName = playerName;
            form.Piece = playerPiece;
            form.Password = playerPassword;
            form.PlayerID = playerID;
            form.ReconnectIfPossible = true;
            if (form.ShowDialog(this) == DialogResult.OK) {
                joinType = form.Option;
                server = form.Host;
                port = form.Port;
                game = 0;
                if (!form.ReconnectIfPossible && key != null) {
                    key.DeleteValue(server+":"+port+"/game", false);
                    key.DeleteValue(server+":"+port+"/id", false);
                    key.DeleteValue(server+":"+port+"/password", false);
                }
                if (form.Option == OptionType.Rejoin) {
                    // leave piece and playerName for now
                    playerID = form.PlayerID;
                    playerPassword = form.Password;
                } else {
                    playerName = form.PlayerName;
                    playerPiece = form.Piece;
                    if (key != null) {
                        key.SetValue("name", playerName);
                        key.SetValue("piece", playerPiece);
                    }
                }
                if (resume) {
                    System.Threading.Monitor.Exit(network);
                    resume = false; // XXX doesn't cancel the monitor
                }
                Connect(server, port);
                return true;
            }
            return false;
        } finally {
            if (resume)
                System.Threading.Monitor.Exit(network);
            form.Dispose();
        }
    }

    protected void ExitCommand(Object sender, EventArgs e) {
        Close();
    }

    protected void RotateCommand(Object sender, EventArgs e) {
        switch (board.Rotation) {
        case MonopolyBoard.Angle.d0:
            board.Rotation = MonopolyBoard.Angle.d270;
            break;
        case MonopolyBoard.Angle.d90:
            board.Rotation = MonopolyBoard.Angle.d0;
            break;
        case MonopolyBoard.Angle.d180:
            board.Rotation = MonopolyBoard.Angle.d90;
            break;
        case MonopolyBoard.Angle.d270:
            board.Rotation = MonopolyBoard.Angle.d180;
            break;
        }
    }

    protected void LogCommand(Object sender, EventArgs e) {
        logForm.Show();
    }

    // When the app closes, dispose of the game object
    protected override void OnClosed(EventArgs e) {
        Disconnect();
        base.OnClosed(e);
    }

    protected void SquareHover(Object sender, MonopolyBoard.SquareEventArgs e) {
        Property p = GetPropertyFromSquare(e.Square);
        String s = "";
        if (p != null && p.Owner != null)
            s = " (" + p.Owner.Name + ")";
        statusGamePanel.Text = board.GetSquareName(e.Square) + s;
    }

    protected void SquareDoubleClick(Object sender, MonopolyBoard.SquareEventArgs e) {
        if (e.Square == 0) {
            ClaimSalaryCommand(sender, new EventArgs());
        } else {
            Property p = GetPropertyFromSquare(board.FocussedSquare);
            if (p != null) {
                ClaimRentCommand(sender, new EventArgs());
            }
        }
    }

    protected void SquareSetPopupMenu(Object sender, EventArgs e) {
        bool go = false;
        bool property = false;
        Property p = null;
        if (board.FocussedSquare == 0) {
            go = true;
        } else {
            p = GetPropertyFromSquare(board.FocussedSquare);
            if (p != null)
                property = true;
        }
        boardClaimRentMenuItem.Visible = property;
        boardOpenMenuItem.Visible = property;
        boardClaimSalaryMenuItem.Visible = go;
        boardDividerMenuItem.Visible = property || go;
        boardClaimRentMenuItem.Enabled = network != null;
        boardClaimSalaryMenuItem.Enabled = network != null;
        boardMortgageMenuItem.Enabled = property && !p.Mortgaged
            && (p.CurrentHouses == 0 && p.CurrentHotels == 0)
            && thisPlayer != null && p.Owner == thisPlayer;
        boardMortgageMenuItem.Visible = property && !p.Mortgaged
            && thisPlayer != null && p.Owner == thisPlayer;
        boardUnmortgageMenuItem.Enabled = property && p.Mortgaged
            && thisPlayer != null && p.Owner == thisPlayer;
        boardUnmortgageMenuItem.Visible = boardUnmortgageMenuItem.Enabled;
        boardSellHouseMenuItem.Enabled = property
            && (p.CurrentHouses > 0 || p.CurrentHotels > 0)
            && thisPlayer != null && p.Owner == thisPlayer;
        boardSellHouseMenuItem.Visible = property
            && (p.type == 0)
            && thisPlayer != null && p.Owner == thisPlayer;
    }

    protected void Connect(String host, int port) {
        Disconnect(); // just in case
        network = new Network(host, port, this);
        network.ReceiveMessage += new Network.ReceiveMessageEventHandler(ReceiveMessage);
        network.NetworkError += new Network.StringEventHandler(NetworkError);
        network.NetworkFatalError += new Network.StringEventHandler(NetworkFatalError);
        network.NetworkConnected += new EventHandler(NetworkConnected);
        network.NetworkDisconnected += new EventHandler(NetworkDisconnected);
        statusNetworkPanel.Text = String.Format("Connecting to {0}:{1}...", host, port);
        network.Start();
    }

    protected void Disconnect() {
        if (network != null) {
            network.ReceiveMessage -= new Network.ReceiveMessageEventHandler(ReceiveMessage);
            network.NetworkError -= new Network.StringEventHandler(NetworkError);
            network.NetworkFatalError -= new Network.StringEventHandler(NetworkFatalError);
            network.NetworkConnected -= new EventHandler(NetworkConnected);
            network.NetworkDisconnected -= new EventHandler(NetworkDisconnected);
            network.Dispose();
            network = null;
        }
        Reset();
    }

    protected void Reset() {
        // now clear everything out
        thisPlayer = null;
        currentPlayer = null;
        latestPlayer = null;
        currentProperty = null;
        foreach (Property p in properties)
            p.Clear();
        foreach (Card c in cards)
            c.Clear();
        foreach (Player p in players.Values)
            p.Clear();
        houseBuyingForm.Hide();
        foreach (Transaction t in transactions.Values) {
            t.Closed -= new EventHandler(RemoveTransaction);
            t.ForceClose();
        }
        transactions.Clear();
        players.Clear();
        playerListView.Items.Clear();
        board.Clear();
        startOfTurnTime = DateTime.MinValue;

        // update user
        statusNetworkPanel.Text = "Disconnected";
        statusHousePanel.Text = "0";
        statusHotelPanel.Text = "0";
        ShowPanel(controlWelcomePanel);
    }

    public void NetworkConnected(Object sender, EventArgs e) {
        statusNetworkPanel.Text = "Connected (awaiting game state)";
        logForm.AddEntry("Connected to ", server, ":", port.ToString());
    }

    public void NetworkDisconnected(Object sender, EventArgs e) {
        logForm.AddEntry("Disconnected");
        Reset();
    }

    public void Waiting(String s2) {
        Waiting("Waiting", s2);
    }

    public void Waiting(String s1, String s2) {
        waitingHeader.Text = s1;
        waitingText.Text = s2;
        turnTimerText.Text = "";
        ShowPanel(controlWaitingPanel);
    }

    protected DateTime startOfTurnTime = DateTime.MinValue;

    public void ClockTick(Object sender, EventArgs e) {
        String s = "";
        if (startOfTurnTime != DateTime.MinValue) {
            TimeSpan delta = DateTime.UtcNow - startOfTurnTime;
            if (delta.Days > 1)
                s += delta.Days + " days";
            else
            if (delta.Days == 1)
                s += delta.Days + " day";
            if (delta.Hours > 0 && s != "")
                s += ", ";
            if (delta.Hours > 1)
                s += delta.Hours + " hours";
            else
            if (delta.Hours == 1)
                s += delta.Hours + " hour";
            if (delta.Minutes > 0 && s != "")
                s += ", ";
            if (delta.Minutes > 1)
                s += delta.Minutes + " minutes";
            else
            if (delta.Minutes == 1)
                s += delta.Minutes + " minute";
        }
        if (turnTimerText.Text != s)
            turnTimerText.Text = s;
    }

    public void payRent(Object sender, EventArgs e) {
        ((sender as Button).Tag as Transaction).RequestThisAgree();
        payRentButton.Enabled = false;
    }

    public void payRentHideButton(Object sender, EventArgs e) {
        (sender as Transaction).VisibleChanged -= new EventHandler(payRentHideButton);
        (sender as Transaction).Closed -= new EventHandler(payRentHideButton);
        payRentButton.Visible = false;
    }

    public void rentClaimTransactionClosed(Object sender, EventArgs e) {
        (sender as Transaction).VisibleChanged -= new EventHandler(rentClaimTransactionClosed);
        (sender as Transaction).Closed -= new EventHandler(rentClaimTransactionClosed);
        hiddenRentClaimTransaction = null;
    }

    public void jailTransactionClosed(Object sender, EventArgs e) {
        (sender as Transaction).Closed -= new EventHandler(jailTransactionClosed);
        jailTransaction = null;
        Waiting("Your Turn", "Paying bail.");
    }

    public void ReceiveMessage(Object sender, Network.ReceiveMessageEventArgs e) {
        switch (e.Message.GetMessageType()) {
        case PIMP_HANDSHAKE_ACKNOWLEDGE.MessageType: {
            PIMP_HANDSHAKE_ACKNOWLEDGE details = (PIMP_HANDSHAKE_ACKNOWLEDGE)e.Message;
            game = details.Field1;
            // Try to rejoin if we can (unless user explicitly gave a rejoin id)
            if (key != null && joinType != OptionType.Rejoin) {
                // get details
                int lastGame = Convert.ToInt32(key.GetValue(server+":"+port+"/game", 0));
                int playerID = Convert.ToInt32(key.GetValue(server+":"+port+"/id", 0));
                uint password = Convert.ToUInt32(key.GetValue(server+":"+port+"/password", 0));
                if (game == lastGame && playerID != 0) {
                    // we have a key for this game
                    network.SendMessage(new PIMP_REJOIN(Convert.ToByte(playerID),
                                                        password));
                    break;
                }
            }
            switch (joinType) {
            case OptionType.Play:
                network.SendMessage(new PIMP_JOIN(Convert.ToByte(playerPiece), true, playerName));
                break;
            case OptionType.Observe:
                network.SendMessage(new PIMP_JOIN(Convert.ToByte(playerPiece), false, playerName));
                break;
            case OptionType.Rejoin:
                network.SendMessage(new PIMP_REJOIN(Convert.ToByte(playerID), playerPassword));
                break;
            }
            break;
        }
        case PIMP_QUERY_JOIN_PLAY.MessageType: {
            PIMP_QUERY_JOIN_PLAY details = (PIMP_QUERY_JOIN_PLAY)e.Message;
            QueryCandidiate(details.Field1, details.Field2, true);
            logForm.AddEntry(details.Field2, " wanted to play.");
            break;
        }
        case PIMP_QUERY_JOIN_OBSERVE.MessageType: {
            PIMP_QUERY_JOIN_OBSERVE details = (PIMP_QUERY_JOIN_OBSERVE)e.Message;
            QueryCandidiate(details.Field1, details.Field2, false);
            logForm.AddEntry(details.Field2, " wanted to observe.");
            break;
        }
        case PIMP_JOIN_PENDING.MessageType: {
            PIMP_JOIN_PENDING details = (PIMP_JOIN_PENDING)e.Message;
            Waiting("Waiting for other players to vote...");
            break;
        }
        case PIMP_JOIN_REFUSED.MessageType: {
            PIMP_JOIN_REFUSED details = (PIMP_JOIN_REFUSED)e.Message;
            statusNetworkPanel.Text = String.Format("{0} did not win the vote",
                                                    details.Field2);
            logForm.AddEntry(details.Field2, " did not win the vote.");
            break;
        }
        case PIMP_ERROR_TOO_MANY_USERS.MessageType: {
            MessageBox.Show(this,
                            "There are too many players in the game already.",
                            "Kaspar",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
            logForm.AddEntry("Too many users were already connected to the server.");
            Disconnect();
            break;
        }
        case PIMP_ERROR_NAME_IN_USE.MessageType: {
            if (GetTextForm.GetText(this, "Kaspar",
                                    "That name is already in use.",
                                    "&Name:", ref playerName)) {
                switch (joinType) {
                case OptionType.Play:
                    network.SendMessage(new PIMP_JOIN(Convert.ToByte(playerPiece), true, playerName));
                    break;
                case OptionType.Observe:
                default: // not that we should ever hit _this_ case label
                    network.SendMessage(new PIMP_JOIN(Convert.ToByte(playerPiece), false, playerName));
                    break;
                }
            } else {
                Disconnect();
            }
            break;
        }
        case PIMP_ERROR_NOT_WELCOME.MessageType: {
            if (thisPlayer == null) {
                DialogResult result = MessageBox.Show(this,
                                                      "The players already in the game voted to not let you in.",
                                                      "Kaspar",
                                                      MessageBoxButtons.OK,
                                                      MessageBoxIcon.Error);
                logForm.AddEntry("The other players voted not to let you join the server.");
                Disconnect();
            } else {
                DialogResult result = MessageBox.Show(this,
                                                      "The majority of the other players do not want you to play. You may continue observing, though.",
                                                      "Kaspar",
                                                      MessageBoxButtons.OK,
                                                      MessageBoxIcon.Error);
                logForm.AddEntry("The other players voted not to let you play in the game.");
            }
            break;
        }
        case PIMP_WELCOME_DETAILS.MessageType: {
            PIMP_WELCOME_DETAILS details = (PIMP_WELCOME_DETAILS)e.Message;
            playerID = details.Field1;
            playerPassword = details.Field2;
            joinType = OptionType.Rejoin;
            if (key != null) {
                // remember this for future reference
                try {
                    key.SetValue("id", playerID);
                    key.SetValue("password", playerPassword);
                    key.SetValue(server+":"+port+"/game", game);
                    key.SetValue(server+":"+port+"/id", playerID);
                    key.SetValue(server+":"+port+"/password", playerPassword);
                } catch {
                    // ignore errors
                }
            }
            thisPlayer = AddPlayer(playerID, playerName, playerPiece, true, true);
            logForm.AddEntry("Joined the server as ", thisPlayer, ".");
            break;
        }
        case PIMP_WELCOME_PLAYER.MessageType: {
            PIMP_WELCOME_PLAYER details = (PIMP_WELCOME_PLAYER)e.Message;
            Player p = AddPlayer(details.Field1, details.Field3, details.Field2, true, false);
            logForm.AddEntry(p, " joined the game.");
            break;
        }
        case PIMP_WELCOME_OBSERVER.MessageType: {
            PIMP_WELCOME_OBSERVER details = (PIMP_WELCOME_OBSERVER)e.Message;
            Player p = AddPlayer(details.Field1, details.Field3, details.Field2, false, false);
            logForm.AddEntry(p, " started observing the game.");
            break;
        }
        case PIMP_WELCOME_BACK.MessageType: {
            PIMP_WELCOME_BACK details = (PIMP_WELCOME_BACK)e.Message;
            if (key != null && joinType == OptionType.Rejoin) {
                // remember this for future reference
                try {
                    key.SetValue(server+":"+port+"/game", game);
                    key.SetValue(server+":"+port+"/id", playerID);
                    key.SetValue(server+":"+port+"/password", playerPassword);
                } catch {
                    // ignore errors
                }
            }
            thisPlayer = AddPlayer(details.Field1, details.Field3, details.Field2, details.Field4, true);
            logForm.AddEntry("Rejoined the server as ", thisPlayer, ".");
            break;
        }
        case PIMP_ERROR_WRONG_PASSWORD.MessageType: {
            if (key != null && joinType != OptionType.Rejoin) {
                // We tried to rejoin and failed
                switch (joinType) {
                case OptionType.Play:
                    network.SendMessage(new PIMP_JOIN(Convert.ToByte(playerPiece), true, playerName));
                    break;
                case OptionType.Observe:
                default: // shouldn't get here
                    network.SendMessage(new PIMP_JOIN(Convert.ToByte(playerPiece), false, playerName));
                    break;
                }
            } else {
                // They tried to rejoin and failed
                MessageBox.Show(this, "Either this is no longer the same game, or you are no longer recognised on this server.",
                                "Kaspar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                logForm.AddEntry("Server did not recognise you.");
                Disconnect();
            }
            break;
        }
        case PIMP_PING.MessageType: {
            PIMP_PING details = (PIMP_PING)e.Message;
            network.SendMessage(new PIMP_PONG(details.Field1));
            break;
        }
        case PIMP_LINK_DEAD.MessageType: {
            PIMP_LINK_DEAD details = (PIMP_LINK_DEAD)e.Message;
            Player p = GetPlayer(details.Field1);
            if (p != null) {
                statusNetworkPanel.Text = p.Name + " has gone link dead";
                logForm.AddEntry(p, " went link dead.");
                // XXX change their icon?
            }
            break;
        }
        case PIMP_ERROR_CANNOT_TRANSFER_DURING_TRANSACTION.MessageType: {
            PIMP_ERROR_CANNOT_TRANSFER_DURING_TRANSACTION details = (PIMP_ERROR_CANNOT_TRANSFER_DURING_TRANSACTION)e.Message;
            if (details.Field1 == 0) {
                if (currentPlayer == thisPlayer) {
                    MessageBox.Show(this,
                                    "You cannot transfer while you owe someone money. Roll the dice quickly before they notice, then transfer.",
                                    "Kaspar", MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                } else {
                    MessageBox.Show(this,
                                    "You cannot transfer while you owe someone money. Wait for the next person to roll the dice, or click the Bankruptcy button if they claim their rent.",
                                    "Kaspar", MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            } else {
                // details.Field1 == transaction
                MessageBox.Show(this,
                                "You cannot transfer while you owe someone money. If you cannot afford the transaction, use the Bankruptcy button.",
                                "Kaspar", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            break;
        }
        case PIMP_PLAYER_BECAME_OBSERVER_KICKED.MessageType: {
            PIMP_PLAYER_BECAME_OBSERVER_KICKED details = (PIMP_PLAYER_BECAME_OBSERVER_KICKED)e.Message;
            Player p = GetPlayer(details.Field1);
            MakePlayerIntoObserver(p, true);
            if (p == thisPlayer) {
                statusNetworkPanel.Text = "You kicked and are now an observer";
            } else {
                statusNetworkPanel.Text = p.Name + " was kicked and is now an observer";
            }
            logForm.AddEntry(p, " was kicked and is now an observer.");
            break;
        } 
       case PIMP_PLAYER_TAKEOVER.MessageType: {
            PIMP_PLAYER_TAKEOVER details = (PIMP_PLAYER_TAKEOVER)e.Message;
            Player to = GetPlayer(details.Field1);
            Player from = GetPlayer(details.Field2);
            if (from != null && to != null && from != to) {
                if (!to.Playing) {
                    to.Piece = from.Piece;
                    to.Image = from.Image;
                    to.Item.ImageIndex = from.Item.ImageIndex;
                    from.Item.ImageIndex = -1;
                    to.MoveImmediately(from.Square, from.InJail);
                    to.Playing = true;
                    from.Playing = false;
                    playerListView.Invalidate();
                    board.RenumberPiece(from.ID, to.ID);
                    if (currentPlayer == from)
                        currentPlayer = to;
                    if (latestPlayer == from)
                        latestPlayer = to;
                } else {
                    MakePlayerIntoObserver(from, true);
                }
                if (to == thisPlayer)
                    statusNetworkPanel.Text = "You have taken over from " + from.Name;
                else if (from == thisPlayer)
                    statusNetworkPanel.Text = to.Name + " has taken over from you";
                else
                    statusNetworkPanel.Text = to.Name + " has taken over from " + from.Name;
            }
            logForm.AddEntry(from, " transferred all assets to ", to);
            break;
        }
       case PIMP_OBSERVER_BECAME_PLAYER.MessageType: {
            PIMP_OBSERVER_BECAME_PLAYER details = (PIMP_OBSERVER_BECAME_PLAYER)e.Message;
            Player p = GetPlayer(details.Field1);
            p.MoveImmediately(0, false);
            MakeObserverIntoPlayer(p, true);
            if (p == thisPlayer) 
                statusNetworkPanel.Text = "You have become a player";
            else
                statusNetworkPanel.Text = p.Name + " has become a player";
            logForm.AddEntry(p, " became a player.");
            break;
        }
        case PIMP_PLAYER_BECAME_OBSERVER_TRANSFER.MessageType: {
            PIMP_PLAYER_BECAME_OBSERVER_TRANSFER details = (PIMP_PLAYER_BECAME_OBSERVER_TRANSFER)e.Message;
            Player p = GetPlayer(details.Field1);
            if (p.Playing) {
                MakePlayerIntoObserver(p, true);
                if (p == thisPlayer)
                    statusNetworkPanel.Text = "You have given up";
                else
                    statusNetworkPanel.Text = p.Name + " has given up";
            }
            break;
        }
        case PIMP_PLAYER_BECAME_OBSERVER_BANKRUPT.MessageType: {
            PIMP_PLAYER_BECAME_OBSERVER_BANKRUPT details = (PIMP_PLAYER_BECAME_OBSERVER_BANKRUPT)e.Message;
            Player p = GetPlayer(details.Field1);
            MakePlayerIntoObserver(p, true);
            if (p == thisPlayer) 
                statusNetworkPanel.Text = "You have gone bankrupt!";
            else
                statusNetworkPanel.Text = p.Name + " has gone bankrupt!";
            logForm.AddEntry(p, " went bankrupt.");
            break;
        }
        case PIMP_PLAYER_WON.MessageType: {
            PIMP_PLAYER_WON details = (PIMP_PLAYER_WON)e.Message;
            Player p = GetPlayer(details.Field1);
            if (p == thisPlayer)
                Waiting("Well Done", "You won!");
            else
                Waiting("Game Over", p.Name + " won!");
            startOfTurnTime = DateTime.MinValue;
            logForm.AddEntry(p, " won!");
            break;
        }
        case PIMP_STATE_BOARD.MessageType: {
            PIMP_STATE_BOARD details = (PIMP_STATE_BOARD)e.Message;
            int id = details.Field1;
            statusNetworkPanel.Text = "Connected (receiving game state)";
            if (id != 0) {
                MessageBox.Show(this,
                                "This server isn't playing on a standard US monopoly board.",
                                "Kaspar",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                Disconnect();
            } else {
                Waiting("Please Wait", "Updating board with latest game information...\n\nBoard");
                board.AnimationEnabled = false;
                foreach (Property p in properties)
                    p.ownerSetDuringState = false;
                foreach (Card c in cards)
                    c.ownerSetDuringState = false;
                foreach (Transaction t in transactions.Values) {
                    t.Closed -= new EventHandler(RemoveTransaction);
                    t.ForceClose();
                }
                transactions.Clear();
            }
            break;
        }
        case PIMP_STATE_PLAYER.MessageType: {
            PIMP_STATE_PLAYER details = (PIMP_STATE_PLAYER)e.Message;
            Waiting("Please Wait", "Updating board with latest game information...\n\nPlayers\n" + details.Field3);
            Player p = GetPlayer(details.Field1);
            if (p == null) {
                p = AddPlayer(details.Field1, details.Field3, details.Field2,
                              true, details.Field5, details.Field4,
                              details.Field6 > 0, false);
            } else {
                if (p.Playing) {
                    p.Cash = details.Field5;
                    p.MoveImmediately(details.Field4, details.Field6 > 0);
                } else {
                    MakeObserverIntoPlayer(p, true);
                }
            }
            break;
        }
        case PIMP_STATE_OBSERVER.MessageType: {
            PIMP_STATE_OBSERVER details = (PIMP_STATE_OBSERVER)e.Message;
            Waiting("Please Wait", "Updating board with latest game information...\n\nObservers\n" + details.Field3);
            Player p = GetPlayer(details.Field1);
            if (p == null) {
                p = AddPlayer(details.Field1, details.Field3, details.Field2,
                              false, false);
            } else {
                if (p.Playing) {
                    MakePlayerIntoObserver(p, true);
                }
            }
            break;
        }
        case PIMP_STATE_PROPERTY.MessageType: {
            PIMP_STATE_PROPERTY details = (PIMP_STATE_PROPERTY)e.Message;
            if (details.Field1 >= properties.Length) break; // ZZZ payload error
            Property property = GetProperty(details.Field1);
            Waiting("Please Wait", "Updating board with latest game information...\n\nProperties\n" + property.Name);
            property.Owner = GetPlayer(details.Field2);
            property.Mortgaged = details.Field3;
            property.SetHouses(details.Field4, details.Field5);
            property.ownerSetDuringState = true;
            break;
        }
        case PIMP_STATE_CARD.MessageType: {
            PIMP_STATE_CARD details = (PIMP_STATE_CARD)e.Message;
            if (details.Field1 >= cards.Length) break; // ZZZ payload error
            Card card = cards[details.Field1];
            Waiting("Please Wait", "Updating board with latest game information...\n\nCards\n"
                    + (card.pile == 0 ? "Chance" : "Community Chest"));
            card.Owner = GetPlayer(details.Field2);
            card.ownerSetDuringState = true;
            break;
        }
        case PIMP_STATE_POT.MessageType: {
            PIMP_STATE_POT details = (PIMP_STATE_POT)e.Message;
            board.Pot = details.Field1;
            foreach (Property p in properties)
                if (!p.ownerSetDuringState)
                    p.Owner = null;
            foreach (Card c in cards)
                if (!c.ownerSetDuringState)
                    c.Owner = null;
            board.AnimationEnabled = true;
            if (NumPlayers() > 1) {
                Waiting("Please Wait", "Updating board with latest game information...\n\nStatus");
            } else {
                ShowPanel(controlWelcomePanel);
            }
            statusNetworkPanel.Text = "Connected";
            break;
        }
        case PIMP_START_OF_TURN.MessageType: {
            PIMP_START_OF_TURN details = (PIMP_START_OF_TURN)e.Message;
            Player p = GetPlayer(details.Field1);
            if (p == null) break; // ZZZ payload error
            startOfTurnTime = DateTime.UtcNow;
            currentPlayer = p;
            currentProperty = null;
            if (board.Card != null &&
                (board.Card.Picker == p || board.Card.Owner == p)) {
                board.Card = null;
            }
            if (p == thisPlayer) {
                if (details.Field2) {
                    if (board.IsAnimating) {
                        disableStartOfTurnButtons();
                        board.AnimationsDone += new EventHandler(enableStartOfTurnButtons);
                    }
                    ShowPanel(controlYourTurnPanel);
                    if (debugThrowDice.Checked && (!debugThrowDiceSlowly.Checked || !board.IsAnimating)) {
                        System.Threading.Thread.Sleep(25);
                        DoThrowDice(null, new EventArgs());
                    } else 
                        notifyUserItIsHisTurn();
                }
            } else {
                if (details.Field2)
                    Waiting(String.Format("It is now {0}'s turn.\n\nDouble click on properties on the board to claim rent.", p.Name));
                else
                    Waiting(String.Format("It is now {0}'s turn.\n\n{0} is in jail.", p.Name));
            }
            logForm.AddEntry("Start of turn: ", p);
            break;
        }
        case PIMP_DICE_ROLLED.MessageType: {
            PIMP_DICE_ROLLED details = (PIMP_DICE_ROLLED)e.Message;
            Player p = GetPlayer(details.Field1);
            if (p == null) break; // ZZZ payload error
            if (latestPlayer != p) {
                for (int i = 0; i < MonopolyBoard.SquareNames.Length; ++i)
                    board.SetState(i, MonopolyBoard.SquareState.Highlighted, false);
                latestPlayer = p;
            }
            board.SetDice(details.Field2, details.Field3);
            if (p == thisPlayer) {
                Waiting("Your Turn", String.Format("You rolled a {0} and a {1}, totalling {2}.", details.Field2, details.Field3, details.Field2 + details.Field3));
            } else {
                Waiting(String.Format("{0} is playing.\n\n{0} rolled a {1} and a {2}, totalling {3}.", p.Name, details.Field2, details.Field3, details.Field2 + details.Field3));
            }
            logForm.AddEntry(p, " rolled a ", details.Field2.ToString(), " and a ", details.Field3.ToString(), ".");
            break;
        }
        case PIMP_THREE_DOUBLES.MessageType: {
            PIMP_THREE_DOUBLES details = (PIMP_THREE_DOUBLES)e.Message;
            Player p = GetPlayer(details.Field1);
            if (p == null) break; // ZZZ payload error
            if (p == thisPlayer) {
                board.AddAnimatedMessage("You got three doubles", Color.Lime);
            } else {
                board.AddAnimatedMessage(String.Format("{0} got three doubles", p.Name), Color.Lime);
            }
            logForm.AddEntry(p, " rolled three doubles.");
            break;
        }
        case PIMP_DICE_MOVED_PLAYER.MessageType: {
            PIMP_DICE_MOVED_PLAYER details = (PIMP_DICE_MOVED_PLAYER)e.Message;
            Player p = GetPlayer(details.Field1);
            if (p == null) break; // ZZZ payload error
            String s = board.GetSquareName(details.Field2);
            p.Square = details.Field2;
            logForm.AddEntry(p, " moved to ", s, ".");
            break;
        }
        case PIMP_CARD_MOVED_PLAYER.MessageType: {
            PIMP_CARD_MOVED_PLAYER details = (PIMP_CARD_MOVED_PLAYER)e.Message;
            Player p = GetPlayer(details.Field1);
            if (p == null) break; // ZZZ payload error
            Card c = GetCard(details.Field2);
            String s = board.GetSquareName(details.Field3);
            p.Square = details.Field3;
            logForm.AddEntry(p, " moved to ", s, " because of a card: ", c);
            // XXX what about the card?
            break;
        }
        case PIMP_SQUARE_MOVED_PLAYER.MessageType: {
            PIMP_SQUARE_MOVED_PLAYER details = (PIMP_SQUARE_MOVED_PLAYER)e.Message;
            Player p = GetPlayer(details.Field1);
            if (p == null) break; // ZZZ payload error
            p.Square = details.Field3;
            String s1 = board.GetSquareName(details.Field2);
            String s2 = board.GetSquareName(details.Field3);
            logForm.AddEntry(s1, " moved ", p, " to ", s2, ".");
            break;
        }
        case PIMP_PLAYER_PASSING_BY_SQUARE.MessageType: {
            PIMP_PLAYER_PASSING_BY_SQUARE details = (PIMP_PLAYER_PASSING_BY_SQUARE)e.Message;
            Player p = GetPlayer(details.Field1);
            if (p == null) break; // ZZZ payload error
            board.MovePiece(p.ID, details.Field2, p.InJail);
            if (debugClaimGo.Checked & p == thisPlayer && details.Field2 == 0)
                ClaimSalaryCommand(null, new EventArgs());
            break;
        }
        case PIMP_PLAYER_LANDING_ON_SQUARE.MessageType: {
            PIMP_PLAYER_LANDING_ON_SQUARE details = (PIMP_PLAYER_LANDING_ON_SQUARE)e.Message;
            Player p = GetPlayer(details.Field1);
            if (p == null) break; // ZZZ payload error
            board.SetState(details.Field2, MonopolyBoard.SquareState.Highlighted, true);
            board.MovePiece(p.ID, details.Field2, p.InJail);
            if (debugClaimGo.Checked && p == thisPlayer && details.Field2 == 0)
                ClaimSalaryCommand(null, new EventArgs());
            Property r = GetPropertyFromSquare(details.Field2);
            if (debugClaimRent.Checked && r != null && p != thisPlayer && r.Owner == thisPlayer && !r.Mortgaged) {
                network.SendMessage(new PIMP_CLAIM_RENT(Convert.ToByte(p.ID), Convert.ToByte(r.id)));
            }
            break;
        }
        case PIMP_GOING_TO_JAIL.MessageType: {
            PIMP_GOING_TO_JAIL details = (PIMP_GOING_TO_JAIL)e.Message;
            Player p = GetPlayer(details.Field1);
            if (p == null) break; // ZZZ payload error
            p.Square = details.Field2; // (should be redundant)
            p.InJail = true; // shouldn't need a MovePiece because this will be followed by a LANDING_ON
            logForm.AddEntry(p, " went to Jail.");
            break;
        }
        case PIMP_WAITING_FOR_TRANSACTION.MessageType: {
            PIMP_WAITING_FOR_TRANSACTION details = (PIMP_WAITING_FOR_TRANSACTION)e.Message;
            Player p = GetPlayer(details.Field1);
            if (p == null) break; // ZZZ payload error
            if (p == thisPlayer) {
                blockingTransaction = (Transaction)(transactions[details.Field2]);
                if (blockingTransaction == null) break; // ZZZ payload error
                if (blockingTransaction != jailTransaction)
                    ShowPanel(controlWaitingOnTransactionPanel);
                // else showed another panel earlier
                notifyUserItIsHisTurn();
            } else {
                Waiting(String.Format("It is now {0}'s turn.\n\n{0} is completing a transaction.", p.Name));
            }
            break;
        }
        case PIMP_RENT_COLLECTION_MORATORIUM.MessageType: {
            PIMP_RENT_COLLECTION_MORATORIUM details = (PIMP_RENT_COLLECTION_MORATORIUM)e.Message;
            for (int i = 0; i < MonopolyBoard.SquareNames.Length; ++i)
                board.SetState(i, MonopolyBoard.SquareState.Highlighted, false);
            latestPlayer = null;
            logForm.AddEntry("Someone missed a rent or Go money claim.");
            break;
        }
        case PIMP_ERROR_INVALID_RENT_CLAIM.MessageType: {
            PIMP_ERROR_INVALID_RENT_CLAIM details = (PIMP_ERROR_INVALID_RENT_CLAIM)e.Message;
            MessageBox.Show(this,
                            "You are not owed rent on that property at the moment.",
                            "Kaspar", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
            break;
        }
        case PIMP_ERROR_INVALID_GO_CLAIM.MessageType: {
            PIMP_ERROR_INVALID_GO_CLAIM details = (PIMP_ERROR_INVALID_GO_CLAIM)e.Message;
            statusNetworkPanel.Text = "You are not entitled to Go money at the moment.";
            break;
        }
        case PIMP_MONEY_BEING_HELD_IN_ESCROW.MessageType: {
            PIMP_MONEY_BEING_HELD_IN_ESCROW details = (PIMP_MONEY_BEING_HELD_IN_ESCROW)e.Message;
            Player p = GetPlayer(details.Field1);
            if (p == null) break; // ZZZ payload error
            if (p == thisPlayer) {
                statusNetworkPanel.Text = String.Format("You will receive ${0} once you have completed pending transactions", details.Field3);
            } else {
                statusNetworkPanel.Text = String.Format("{0} will receive ${1} once they have completed pending transactions", p.Name, details.Field3);
            }
            break;
        }
        case PIMP_ROLL_AGAIN.MessageType: {
            PIMP_ROLL_AGAIN details = (PIMP_ROLL_AGAIN)e.Message;
            Player p = GetPlayer(details.Field1);
            if (p == null) break; // ZZZ payload error
            currentProperty = null;
            if (p == thisPlayer) {
                ShowPanel(controlYourTurnAgainPanel);
                if (debugThrowDice.Checked) {
                    System.Threading.Thread.Sleep(25);
                    DoThrowDice(null, new EventArgs());
                } else 
                    notifyUserItIsHisTurn();
            } else {
                Waiting(String.Format("It is still {0}'s turn.\n\nDouble click on properties on the board to claim rent.", p.Name));
            }
            break;
        }
        case PIMP_PROPERTY_SALE.MessageType: {
            PIMP_PROPERTY_SALE details = (PIMP_PROPERTY_SALE)e.Message;
            Player p = GetPlayer(details.Field1);
            Property r = GetProperty(details.Field2);
            uint c = details.Field3;
            if (p == null || r == null) break; // ZZZ payload error
            currentProperty = r;
            if (p == thisPlayer) {
                ((Label)(controlPropertySalePanel.Tag)).Text =
                    String.Format("You have been given a chance to purchase {0} for ${1}.", r.Name, c);
                ShowPanel(controlPropertySalePanel);
                if (debugBuyProperty.Checked)
                    DoBuyProperty(null, new EventArgs());
                else
                if (debugAuctionProperty.Checked)
                    DoAuctionProperty(null, new EventArgs());
                else
                    notifyUserItIsHisTurn();
            } else {
                ((Label)(controlOtherPlayerPropertySalePanel.Tag)).Text =
                    String.Format("{0} has been given a chance to purchase {1} for ${2}.", p.Name, r.Name, c);
                ShowPanel(controlOtherPlayerPropertySalePanel);
            }
            logForm.AddEntry(p, " has the opportunity to buy ", r, " for $", c, ".");
            break;
        }
        case PIMP_ERROR_PROPERTY_TOO_EXPENSIVE.MessageType: {
            PIMP_ERROR_PROPERTY_TOO_EXPENSIVE details = (PIMP_ERROR_PROPERTY_TOO_EXPENSIVE)e.Message;
            Property r = GetProperty(details.Field1);
            uint c = details.Field2;
            if (r == null) break; // ZZZ payload error
            ((Label)(controlPropertySalePanel.Tag)).Text =
                String.Format("You have been given a chance to purchase {0} for ${1}.", r.Name, c);
            ShowPanel(controlPropertySalePanel);
            if (debugAuctionProperty.Checked)
                DoAuctionProperty(null, new EventArgs());
            else
                notifyUserItIsHisTurn();
            MessageBox.Show(this,
                            String.Format("You can't afford {0}, it costs ${1}.",
                                          r.Name, c),
                            "Kaspar", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
            break;
        }
        case PIMP_ERROR_PROPERTY_WOULD_REDUCE_NET_WORTH.MessageType: {
            PIMP_ERROR_PROPERTY_WOULD_REDUCE_NET_WORTH details = (PIMP_ERROR_PROPERTY_WOULD_REDUCE_NET_WORTH)e.Message;
            Property r = GetProperty(details.Field1);
            if (r == null) break; // ZZZ payload error
            Transaction t = transactions[details.Field2] as Transaction;
            if (t == null) break; // ZZZ payload error
            ((Label)(controlPropertySalePanel.Tag)).Text =
                String.Format("You have been given a chance to purchase {0} for ${1}.", r.Name, r.price);
            ShowPanel(controlPropertySalePanel);
            notifyUserItIsHisTurn();
            MessageBox.Show(this,
                            "You cannot reduce your net worth while you owe someone else money.",
                            "Kaspar",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
            break;
        }
        case PIMP_PROPERTY_AUCTION.MessageType: {
            PIMP_PROPERTY_AUCTION details = (PIMP_PROPERTY_AUCTION)e.Message;
            Property r = GetProperty(details.Field1);
            if (r == null) break; // ZZZ payload error
            if (thisPlayer.Playing) {
                currentProperty = r;
                ((Label)(controlPropertyAuctionPanel.Tag)).Text =
                    String.Format("{0} has gone up for auction.\n\nNo-one has bid yet.", r.Name);
                auctionBid.Value = 10;
                auctionBidButton.Enabled = true;
                auctionNoBidButton.Enabled = true;
                auctionNoBidButton.Tag = (Object)(true);
                ShowPanel(controlPropertyAuctionPanel);
                if (debugAuctionProperty.Checked)
                    DoNoBidProperty(null, new EventArgs());
                else
                if (debugBuyProperty.Checked)
                    DoBidProperty(null, new EventArgs());
                else
                    notifyUserItIsHisTurn();
            } else {
                Waiting("Auction", String.Format("{0} has gone up for auction.\n\nNo-one has bid yet.", r.Name));
            }
            logForm.AddEntry(currentPlayer, " auctioned ", r, ".");
            break;
        }
        case PIMP_PROPERTY_AUCTION_BID.MessageType: {
            PIMP_PROPERTY_AUCTION_BID details = (PIMP_PROPERTY_AUCTION_BID)e.Message;
            Player p = GetPlayer(details.Field1);
            if (p == null) break; // ZZZ payload error
            if (currentProperty == null) break; // ZZZ unexpected message
            if (auctionBid.Value <= details.Field2)
                auctionBid.Value = details.Field2 + 10;
            if (p == thisPlayer) {
                ((Label)(controlPropertyAuctionPanel.Tag)).Text =
                    String.Format("{0} has gone up for auction.\n\nYou have the highest bid, ${1}.", currentProperty.Name, details.Field2);
                auctionBidButton.Enabled = false;
                auctionNoBidButton.Enabled = false;
                auctionNoBidButton.Tag = (Object)(true);
            } else if (thisPlayer.Playing) {
                ((Label)(controlPropertyAuctionPanel.Tag)).Text =
                    String.Format("{0} has gone up for auction.\n\n{1} has bid ${2}.", currentProperty.Name, p.Name, details.Field2);
                if (!auctionBidButton.Enabled) // been out bid
                    notifyUserItIsHisTurn();
                auctionBidButton.Enabled = true;
                auctionNoBidButton.Enabled = (bool)(auctionNoBidButton.Tag);
            } else {
                Waiting("Auction", String.Format("{0} has gone up for auction.\n\n{1} has bid ${2}.", currentProperty.Name, p.Name, details.Field2));
            }
            logForm.AddEntry(p, " bid $", details.Field2, ".");
            break;
        }
        case PIMP_PROPERTY_AUCTION_NO_BID.MessageType: {
            PIMP_PROPERTY_AUCTION_NO_BID details = (PIMP_PROPERTY_AUCTION_NO_BID)e.Message;
            Player p = GetPlayer(details.Field1);
            if (p == null) break; // ZZZ payload error
            if (currentProperty == null) break; // ZZZ unexpected message
            if (p == thisPlayer) {
                auctionBidButton.Enabled = true;
                auctionNoBidButton.Enabled = false;
                auctionNoBidButton.Tag = (Object)(false);
            } else {
                statusNetworkPanel.Text = String.Format("{0} is out of the bidding", p.Name);
            }
            logForm.AddEntry(p, " stepped out of the bidding.");
            // XXX we should so list this somewhere on the panel
            break;
        }
        case PIMP_PROPERTY_AUCTION_WON.MessageType: {
            PIMP_PROPERTY_AUCTION_WON details = (PIMP_PROPERTY_AUCTION_WON)e.Message;
            Player p = GetPlayer(details.Field1);
            if (p == null) break; // ZZZ payload error
            if (currentProperty == null) break; // ZZZ unexpected message
            String s;
            if (p == thisPlayer)
                s = String.Format("You won the auction for {0}.", currentProperty.Name);
            else
                s = String.Format("{0} won the auction for {1}.", p.Name, currentProperty.Name);
            Waiting(s);
            statusNetworkPanel.Text = s;
            logForm.AddEntry(p, " won the auction for ", currentProperty, ".");
            currentProperty = null;
            break;
        }
        case PIMP_PROPERTY_AUCTION_VOID.MessageType: {
            PIMP_PROPERTY_AUCTION_VOID details = (PIMP_PROPERTY_AUCTION_VOID)e.Message;
            if (currentProperty == null) break; // ZZZ unexpected message
            String s = String.Format("Nobody bid for {0}", currentProperty.Name);
            Waiting(s);
            statusNetworkPanel.Text = s;
            logForm.AddEntry("Nobody won the auction for ", currentProperty, ".");
            currentProperty = null;
            break;
        }
        case PIMP_ERROR_HOUSES_NOT_OWNED.MessageType: {
            PIMP_ERROR_HOUSES_NOT_OWNED details = (PIMP_ERROR_HOUSES_NOT_OWNED)e.Message;
            Property r = GetProperty(details.Field1);
            if (r == null) break; // ZZZ payload error
            // shouldn't ever happen, but we never know
            MessageBox.Show(this,
                            String.Format("You do not own {0}, so you cannot build on it.", r.Name),
                            "Kaspar",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
            break;
        }
        case PIMP_ERROR_HOUSES_NEED_MONOPOLY.MessageType: {
            PIMP_ERROR_HOUSES_NEED_MONOPOLY details = (PIMP_ERROR_HOUSES_NEED_MONOPOLY)e.Message;
            Property r = GetProperty(details.Field1);
            if (r == null) break; // ZZZ payload error
            // shouldn't ever happen, but we never know
            MessageBox.Show(this,
                            String.Format("You must own all the properties in a color group to build houses on any of its properties. You do not own {0}.", r.Name),
                            "Kaspar",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
            break;
        }
        case PIMP_ERROR_HOUSES_MORTGAGED.MessageType: {
            PIMP_ERROR_HOUSES_MORTGAGED details = (PIMP_ERROR_HOUSES_MORTGAGED)e.Message;
            Property r = GetProperty(details.Field1);
            if (r == null) break; // ZZZ payload error
            // shouldn't ever happen, but we never know
            MessageBox.Show(this,
                            String.Format("You cannot build on {0}, it is mortgaged.", r.Name),
                            "Kaspar",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
            break;
        }
        case PIMP_ERROR_HOUSES_NOT_COLOUR_GROUP.MessageType: {
            PIMP_ERROR_HOUSES_NOT_COLOUR_GROUP details = (PIMP_ERROR_HOUSES_NOT_COLOUR_GROUP)e.Message;
            Property r = GetProperty(details.Field1);
            if (r == null) break; // ZZZ payload error
            // shouldn't ever happen, but we never know
            MessageBox.Show(this,
                            String.Format("You cannot build on {0}, it isn't a colour group.", r.Name),
                            "Kaspar",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
            break;
        }
        case PIMP_ERROR_HOUSES_NOT_YOUR_TURN.MessageType: {
            PIMP_ERROR_HOUSES_NOT_YOUR_TURN details = (PIMP_ERROR_HOUSES_NOT_YOUR_TURN)e.Message;
            MessageBox.Show(this,
                            "You cannot build when it isn't the start of your turn.",
                            "Kaspar",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
            break;
        }
        case PIMP_ERROR_HOUSES_TOO_EXPENSIVE.MessageType: {
            PIMP_ERROR_HOUSES_TOO_EXPENSIVE details = (PIMP_ERROR_HOUSES_TOO_EXPENSIVE)e.Message;
            MessageBox.Show(this,
                            String.Format("You do not have enough money to complete the house buildings you requested. It would cost ${0}.", details.Field1),
                            "Kaspar",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
            break;
        }
        case PIMP_ERROR_HOUSES_WOULD_REDUCE_NET_WORTH.MessageType: {
            PIMP_ERROR_HOUSES_WOULD_REDUCE_NET_WORTH details = (PIMP_ERROR_HOUSES_WOULD_REDUCE_NET_WORTH)e.Message;
            Transaction t = transactions[details.Field1] as Transaction;
            if (t == null) break; // ZZZ payload error
            MessageBox.Show(this,
                            "You cannot reduce your net worth while you owe someone else money.",
                            "Kaspar",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
            break;
        }
        case PIMP_ERROR_HOUSES_NO_HOUSE_PIECES_LEFT.MessageType: {
            PIMP_ERROR_HOUSES_NO_HOUSE_PIECES_LEFT details = (PIMP_ERROR_HOUSES_NO_HOUSE_PIECES_LEFT)e.Message;
            String houses;
            switch (details.Field1) {
            case 0: houses = String.Format("no houses", details.Field1); break;
            case 1: houses = String.Format("only one house", details.Field1); break;
            default: houses = String.Format("only {0} houses", details.Field1); break;
            }
            String hotels;
            switch (details.Field2) {
            case 0: hotels = String.Format("no hotels", details.Field2); break;
            case 1: hotels = String.Format("one hotel", details.Field2); break;
            default: hotels = String.Format("{0} hotels", details.Field2); break;
            }
            MessageBox.Show(this,
                            String.Format("There are not enough houses or hotels available. (There are {0} and {1} left.)",
                                          houses, hotels),
                            "Kaspar",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
            break;
        }
        case PIMP_ERROR_HOUSES_CANNOT_BUILD_THAT_NUMBER.MessageType: {
            PIMP_ERROR_HOUSES_CANNOT_BUILD_THAT_NUMBER details = (PIMP_ERROR_HOUSES_CANNOT_BUILD_THAT_NUMBER)e.Message;
            Property r = GetProperty(details.Field1);
            if (r == null) break; // ZZZ payload error
            MessageBox.Show(this,
                            String.Format("You cannot build that number of houses or hotels on {0}.", r.Name),
                            "Kaspar",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
            break;
        }
        case PIMP_ERROR_HOUSES_UNBALANCED.MessageType: {
            PIMP_ERROR_HOUSES_UNBALANCED details = (PIMP_ERROR_HOUSES_UNBALANCED)e.Message;
            Property r = GetProperty(details.Field1);
            if (r == null) break; // ZZZ payload error
            MessageBox.Show(this,
                            String.Format("You cannot build that number of houses or hotels, it would make the houses unbalanced around {0}.", r.Name),
                            "Kaspar",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
            break;
        }
        case PIMP_DELTA_HOUSES_PURCHASED.MessageType: {
            PIMP_DELTA_HOUSES_PURCHASED details = (PIMP_DELTA_HOUSES_PURCHASED)e.Message;
            Player p = GetPlayer(details.Field1);
            Property r = GetProperty(details.Field2);
            if (r == null) break; // ZZZ payload error
            r.SetHouses(details.Field3, details.Field4);
            if (p == null) {
                if (details.Field3 > 0 || details.Field4 > 0) {
                    logForm.AddEntry("The bank placed ",
                                     String.Format("{0} house{2} and {1} hotel{3}",
                                                   details.Field3,
                                                   details.Field4,
                                                   details.Field3 == 1 ? "" : "s",
                                                   details.Field4 == 1 ? "" : "s"),
                                     " on ", r);
                } else {
                    logForm.AddEntry("The bank removed all the houses and hotels on ", r);
                }
            } else {
                if (p != thisPlayer) {
                    String s;
                    if (details.Field4 == 0) {
                        s = "{0} now has {2} house{4} on {1}";
                    } else if (details.Field3 == 0) {
                        s = "{0} now has {3} hotel{5} on {1}";
                    } else {
                        s = "{0} now has {2} house{4} and {3} hotel{5} on {1}";
                    }
                    statusNetworkPanel.Text =
                        String.Format(s, p.Name, r.Name, details.Field3,
                                      details.Field4,
                                      details.Field3 == 1 ? "" : "s",
                                      details.Field4 == 1 ? "" : "s");
                }
                logForm.AddEntry(p, " placed ",
                                 String.Format("{0} house{2} and {1} hotel{3}",
                                               details.Field3,
                                               details.Field4,
                                               details.Field3 == 1 ? "" : "s",
                                               details.Field4 == 1 ? "" : "s"),
                                 " on ", r);
            }
            break;
        }
        case PIMP_DELTA_PROPERTY_MORTGAGED.MessageType: {
            PIMP_DELTA_PROPERTY_MORTGAGED details = (PIMP_DELTA_PROPERTY_MORTGAGED)e.Message;
            Property r = GetProperty(details.Field1);
            if (r == null) break; // ZZZ payload error
            r.Mortgaged = true;
            logForm.AddEntry(r, " is now mortgaged.");
            break;
        }
        case PIMP_DELTA_PROPERTY_UNMORTGAGED.MessageType: {
            PIMP_DELTA_PROPERTY_UNMORTGAGED details = (PIMP_DELTA_PROPERTY_UNMORTGAGED)e.Message;
            Property r = GetProperty(details.Field1);
            if (r == null) break; // ZZZ payload error
            r.Mortgaged = false;
            logForm.AddEntry(r, " is no longer mortgaged.");
            break;
        }
        case PIMP_ERROR_MORTGAGE_TOO_EXPENSIVE.MessageType: {
            PIMP_ERROR_MORTGAGE_TOO_EXPENSIVE details = (PIMP_ERROR_MORTGAGE_TOO_EXPENSIVE)e.Message;
            Property r = GetProperty(details.Field1);
            if (r == null) break; // ZZZ payload error
            MessageBox.Show(this,
                            String.Format("You do not have enough money to unmortgage {0}. You need ${1}.", r.Name, details.Field2),
                            "Kaspar",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
            break;
        }
        case PIMP_SQUARE_GIVES_CASH.MessageType: {
            PIMP_SQUARE_GIVES_CASH details = (PIMP_SQUARE_GIVES_CASH)e.Message;
            Player p = GetPlayer(details.Field1);
            if (p == null) break; // ZZZ payload error
            if (p == thisPlayer) {
                board.AddAnimatedTextSquare("$" + details.Field3, Color.Yellow, details.Field2);
                statusNetworkPanel.Text = String.Format("You got ${1} from {2}", p.Name,
                                                        details.Field3, board.GetSquareName(details.Field2));
            } else {
                board.AddAnimatedTextSquare("$" + details.Field3, Color.Silver, details.Field2);
                statusNetworkPanel.Text = String.Format("{0} got ${1} from {2}", p.Name,
                                                        details.Field3, board.GetSquareName(details.Field2));
            }
            logForm.AddEntry(board.GetSquareName(details.Field2), " gave $", details.Field3, " to ", p, ".");
            break;
        }
        case PIMP_SQUARE_TAKES_CASH.MessageType: {
            PIMP_SQUARE_TAKES_CASH details = (PIMP_SQUARE_TAKES_CASH)e.Message;
            Player p = GetPlayer(details.Field1);
            if (p == null) break; // ZZZ payload error
            if (p == thisPlayer) {
                board.AddAnimatedTextSquare("$" + details.Field3, Color.Red, details.Field2);
                statusNetworkPanel.Text = String.Format("{2} took ${1} from you", p.Name,
                                                        details.Field3, board.GetSquareName(details.Field2));
            } else {
                board.AddAnimatedTextSquare("$" + details.Field3, Color.Gray, details.Field2);
                statusNetworkPanel.Text = String.Format("{2} took ${1} from {0}", p.Name,
                                                        details.Field3, board.GetSquareName(details.Field2));
            }
            logForm.AddEntry(board.GetSquareName(details.Field2), " took $", details.Field3, " from ", p, ".");
            break;
        }
        case PIMP_SQUARE_GIVES_CARD.MessageType: {
            PIMP_SQUARE_GIVES_CARD details = (PIMP_SQUARE_GIVES_CARD)e.Message;
            Player p = GetPlayer(details.Field1);
            if (p == null) break; // ZZZ payload error
            Card c = GetCard(details.Field3);
            if (c == null) break; // ZZZ payload error
            logForm.AddEntry(board.GetSquareName(details.Field2), " gave ", p, " a card: ", c);
            break;
        }
        case PIMP_SQUARE_TAKES_CARD.MessageType: {
            PIMP_SQUARE_TAKES_CARD details = (PIMP_SQUARE_TAKES_CARD)e.Message;
            Player p = GetPlayer(details.Field1);
            if (p == null) break; // ZZZ payload error
            Card c = GetCard(details.Field3);
            if (c == null) break; // ZZZ payload error
            logForm.AddEntry(board.GetSquareName(details.Field2), " took a card from ", p, ": ", c);
            break;
        }
        case PIMP_CONSTRUCTION_TAKES_CASH.MessageType: {
            PIMP_CONSTRUCTION_TAKES_CASH details = (PIMP_CONSTRUCTION_TAKES_CASH)e.Message;
            Player p = GetPlayer(details.Field1);
            if (p == null) break; // ZZZ payload error
            logForm.AddEntry(p, " payed $", details.Field2, " for house and hotel construction.");
            break;
        }
        case PIMP_DESTRUCTION_GIVES_CASH.MessageType: {
            PIMP_DESTRUCTION_GIVES_CASH details = (PIMP_DESTRUCTION_GIVES_CASH)e.Message;
            Player p = GetPlayer(details.Field1);
            if (p == null) break; // ZZZ payload error
            logForm.AddEntry(p, " received $", details.Field2, " from house and hotel destruction.");
            break;
        }
        case PIMP_PLAYER_CLAIMED_RENT.MessageType: {
            PIMP_PLAYER_CLAIMED_RENT details = (PIMP_PLAYER_CLAIMED_RENT)e.Message;
            Player p1 = GetPlayer(details.Field1);
            if (p1 == null) break; // ZZZ payload error
            Player p2 = GetPlayer(details.Field2);
            if (p2 == null) break; // ZZZ payload error
            Property p = GetProperty(details.Field3);
            if (p == null) break; // ZZZ payload error
            uint a = details.Field4;
            if (p1 == thisPlayer) {
                statusNetworkPanel.Text = String.Format("You claimed ${1} from {2} for landing on {3}",
                                                        p1.Name, a, p2.Name, p.Name);
                board.AddAnimatedTextSquare("Claimed", Color.Yellow, p.square);
            } else if (p2 == thisPlayer) {
                statusNetworkPanel.Text = String.Format("{0} claimed ${1} from you for landing on {3}",
                                                        p1.Name, a, p2.Name, p.Name);
            } else {
                statusNetworkPanel.Text = String.Format("{0} claimed ${1} from {2} for landing on {3}",
                                                        p1.Name, a, p2.Name, p.Name);
            }
            logForm.AddEntry(p1, " claimed $", a, " rent from ", p2, " for landing on ", p, ".");
            break;
        }
        case PIMP_PLAYER_CLAIMED_GO.MessageType: {
            PIMP_PLAYER_CLAIMED_GO details = (PIMP_PLAYER_CLAIMED_GO)e.Message;
            Player p = GetPlayer(details.Field1);
            if (p == null) break; // ZZZ payload error
            // (field 2 is the square, which we don't care about)
            uint a = details.Field3;
            if (p == thisPlayer) {
                statusNetworkPanel.Text = String.Format("You claimed salary of ${1}",
                                                        p.Name, a);
            } else {
                statusNetworkPanel.Text = String.Format("{0} claimed salary of ${1}",
                                                        p.Name, a);
            }
            logForm.AddEntry(p, " claimed Go money.");
            break;
        }
        case PIMP_TAX_SELECT_OPTION.MessageType: {
            PIMP_TAX_SELECT_OPTION details = (PIMP_TAX_SELECT_OPTION)e.Message;
            Player p = GetPlayer(details.Field1);
            if (p == null) break; // ZZZ payload error
            if (p == thisPlayer) {
                ShowPanel(controlIncomeTaxPanel);
                if (debugPayTax.Checked) {
                    if (thisPlayer.Cash < 2000 && thisPlayer.Properties.Count == 0)
                        DoIncomeTaxPay10(null, new EventArgs());
                    else
                        DoIncomeTaxPay200(null, new EventArgs());
                } else 
                    notifyUserItIsHisTurn();
            } else {
                Waiting(String.Format("It is now {0}'s turn.\n\n{0} must pay income tax.", p.Name));
            }
            break;
        }
        case PIMP_JAIL_PAY_OR_ROLL.MessageType: {
            PIMP_JAIL_PAY_OR_ROLL details = (PIMP_JAIL_PAY_OR_ROLL)e.Message;
            Player p = GetPlayer(details.Field1);
            if (p == null) break; // ZZZ payload error
            if (p == thisPlayer) {
                ((Label)(controlJailWithDicePanel.Tag)).Text =
                    String.Format("You are jail and can throw dice for another {0} turn{1}.", details.Field2, details.Field2 == 1 ? "" : "s");
                if ((details.Field2 > 0 || details.Field3) && board.IsAnimating) {
                    disableStartOfTurnButtons();
                    board.AnimationsDone += new EventHandler(enableStartOfTurnButtons);
                }
                if (details.Field2 > 0)
                    ShowPanel(controlJailWithDicePanel);
                else if (details.Field3)
                    ShowPanel(controlJailWithoutDicePanel);
                else
                    ShowPanel(controlJailWithoutDiceNorHousesPanel);
                if (debugThrowDice.Checked && details.Field2 > 0) {
                    System.Threading.Thread.Sleep(25);
                    DoJailThrowDice(null, new EventArgs());
                } else 
                if (debugThrowDice.Checked && details.Field2 == 0) {
                    System.Threading.Thread.Sleep(25);
                    DoJailPayBail(null, new EventArgs());
                } else
                    notifyUserItIsHisTurn();
            } else {
                Waiting(String.Format("It is now {0}'s turn.\n\n{0} is in jail and can throw dice for another {1} turn{2}.", p.Name, details.Field2, details.Field2 == 1 ? "" : "s"));
            }
            break;
        }
        case PIMP_JAIL_FREE.MessageType: {
            PIMP_JAIL_FREE details = (PIMP_JAIL_FREE)e.Message;
            Player p = GetPlayer(details.Field1);
            if (p == null) break; // ZZZ payload error
            if (latestPlayer != p) {
                for (int i = 0; i < MonopolyBoard.SquareNames.Length; ++i)
                    board.SetState(i, MonopolyBoard.SquareState.Highlighted, false);
                latestPlayer = p;
            }
            p.InJail = false;
            board.MovePiece(p.ID, p.Square, p.InJail);
            // XXX message?
            logForm.AddEntry(p, " got out of Jail.");
            break;
        }
        case PIMP_DELTA_POT.MessageType: {
            PIMP_DELTA_POT details = (PIMP_DELTA_POT)e.Message;
            board.Pot = details.Field1;
            // XXX message?
            logForm.AddEntry("The pot is now $", details.Field1, ".");
            break;
        }
        case PIMP_TRANSACTION_TRADE_REQUESTED.MessageType: {
            PIMP_TRANSACTION_TRADE_REQUESTED details = (PIMP_TRANSACTION_TRADE_REQUESTED)e.Message;
            Player p = GetPlayer(details.Field2);
            if (p == null) break; // ZZZ payload error
            String s = String.Format("Trading with {0}.", p.Name);
            AddTransaction(details.Field1, s, null, 0, thisPlayer, p, true, true);
            break;
        }
        case PIMP_TRANSACTION_RENT_REQUESTED.MessageType: {
            PIMP_TRANSACTION_RENT_REQUESTED details = (PIMP_TRANSACTION_RENT_REQUESTED)e.Message;
            Player p = GetPlayer(details.Field2);
            if (p == null) break; // ZZZ payload error
            Property r = GetProperty(details.Field3);
            if (r == null) break; // ZZZ payload error
            Player o = GetPlayer(details.Field4);
            if (o == null) break; // ZZZ payload error
            uint a = details.Field5;
            if (o == p) {
                // they own it
                Transaction transaction = AddTransaction(details.Field1, String.Format("{0} has requested that you pay the rent of ${1} for landing on {2}.", p.Name, a, r.Name), r, a, thisPlayer, p, o != p, payRentButton.Visible);
                if (!transaction.Visible) {
                    payRentButton.Text = String.Format("{0}\n Pay ${1} to {2}",
                                                       r.Name, a, p.Name);
                    payRentButton.Tag = transaction;
                    payRentButton.Enabled = true;
                    payRentButton.Visible = true;
                    transaction.VisibleChanged += new EventHandler(payRentHideButton);
                    transaction.Closed += new EventHandler(payRentHideButton);
                    if (debugPayRent.Checked) {
                        payRent(payRentButton, new EventArgs());
                    }
                }
            } else {
                // presumably we own it
                statusNetworkPanel.Text = String.Format("You have claimed ${0} from {1} for landing on {2}", a, p.Name, r.Name);
                Transaction transaction = AddTransaction(details.Field1, String.Format("You have requested that {0} pay the rent of ${1} for landing on {2}.", p.Name, a, r.Name), r, a, thisPlayer, p, o != p, hiddenRentClaimTransaction != null);
                if (!transaction.Visible) {
                    hiddenRentClaimTransaction = transaction;
                    transaction.VisibleChanged += new EventHandler(rentClaimTransactionClosed);
                    transaction.Closed += new EventHandler(rentClaimTransactionClosed);
                }
            }
            break;
        }
        case PIMP_TRANSACTION_CARD_REQUESTED.MessageType: {
            PIMP_TRANSACTION_CARD_REQUESTED details = (PIMP_TRANSACTION_CARD_REQUESTED)e.Message;
            Player p = GetPlayer(details.Field2);
            if (p == null) break; // ZZZ payload error
            Card c = GetCard(details.Field3);
            if (c == null) break; // ZZZ payload error
            Player o = GetPlayer(details.Field4);
            if (o == null) break; // ZZZ payload error
            uint a = details.Field5;
            String s;
            if (o == p) {
                // they are due the cash
                s = String.Format("You owe ${0} to {1} because of a card.", a, p.Name);
            } else {
                // presumably, we are due the cash
                s = String.Format("{0} owes you ${1} because of a card.", p.Name, a);
            }
            AddTransaction(details.Field1, s, c, a, thisPlayer, p, o != p, true);
            break;
        }
        case PIMP_TRANSACTION_SQUARE_REQUESTED.MessageType: {
            PIMP_TRANSACTION_SQUARE_REQUESTED details = (PIMP_TRANSACTION_SQUARE_REQUESTED)e.Message;
            int s = details.Field2;
            uint a = details.Field3;
            AddTransaction(details.Field1, String.Format("{0} wants ${1}.", board.GetSquareName(s), a),
                           s, a, thisPlayer, null, false, true);
            break;
        }
        case PIMP_TRANSACTION_BANK_REQUESTED.MessageType: {
            PIMP_TRANSACTION_BANK_REQUESTED details = (PIMP_TRANSACTION_BANK_REQUESTED)e.Message;
            uint a = details.Field2;
            AddTransaction(details.Field1, String.Format("Bank wants ${0}.", a),
                           false, a, thisPlayer, null, false, true);
            break;
        }
        case PIMP_TRANSACTION_JAIL_REQUESTED.MessageType: {
            PIMP_TRANSACTION_JAIL_REQUESTED details = (PIMP_TRANSACTION_JAIL_REQUESTED)e.Message;
            uint a = details.Field2;
            jailTransaction = AddTransaction(details.Field1,
                                             String.Format("To get out of jail you must pay bail. Bail is either ${0} or a \"Get Out Of Jail Free\" card.", a),
                                             true, a, thisPlayer, null, false, false);
            jailTransaction.Closed += new EventHandler(jailTransactionClosed);
            ShowPanel(controlJailPayBailPanel);
            if (debugPayTax.Checked)
                DoJailBailPayCash(null, new EventArgs());
            break;
        }
        case PIMP_ERROR_TRANSACTION_TOO_EXPENSIVE.MessageType: {
            PIMP_ERROR_TRANSACTION_TOO_EXPENSIVE details = (PIMP_ERROR_TRANSACTION_TOO_EXPENSIVE)e.Message;
            Transaction t = transactions[details.Field1] as Transaction;
            if (t == null) return;
            t.CannotSetCash(details.Field2);
            break;
        }
        case PIMP_TRANSACTION_CASH_SET.MessageType: {
            PIMP_TRANSACTION_CASH_SET details = (PIMP_TRANSACTION_CASH_SET)e.Message;
            Transaction t = transactions[details.Field1] as Transaction;
            if (t == null) return;
            t.SetThisCash(details.Field2);
            break;
        }
        case PIMP_TRANSACTION_OTHER_CASH_SET.MessageType: {
            PIMP_TRANSACTION_OTHER_CASH_SET details = (PIMP_TRANSACTION_OTHER_CASH_SET)e.Message;
            Transaction t = transactions[details.Field1] as Transaction;
            if (t == null) return;
            t.SetOtherCash(details.Field2);
            break;
        }
        case PIMP_ERROR_TRANSACTION_PROPERTY_NOT_OWNED.MessageType: break; // XXX
        case PIMP_TRANSACTION_PROPERTY_ADDED.MessageType: {
            PIMP_TRANSACTION_PROPERTY_ADDED details = (PIMP_TRANSACTION_PROPERTY_ADDED)e.Message;
            Transaction t = transactions[details.Field1] as Transaction;
            if (t == null) return;
            Property p = GetProperty(details.Field2);
            if (p == null) return;
            t.AddThisProperty(p);
            break;
        }
        case PIMP_TRANSACTION_PROPERTY_REMOVED.MessageType: {
            PIMP_TRANSACTION_PROPERTY_REMOVED details = (PIMP_TRANSACTION_PROPERTY_REMOVED)e.Message;
            Transaction t = transactions[details.Field1] as Transaction;
            if (t == null) return;
            Property p = GetProperty(details.Field2);
            if (p == null) return;
            t.RemoveThisProperty(p);
            break;
        }
        case PIMP_TRANSACTION_OTHER_PROPERTY_ADDED.MessageType: {
            PIMP_TRANSACTION_OTHER_PROPERTY_ADDED details = (PIMP_TRANSACTION_OTHER_PROPERTY_ADDED)e.Message;
            Transaction t = transactions[details.Field1] as Transaction;
            if (t == null) return;
            Property p = GetProperty(details.Field2);
            if (p == null) return;
            t.AddOtherProperty(p);
            break;
        }
        case PIMP_TRANSACTION_OTHER_PROPERTY_REMOVED.MessageType: {
            PIMP_TRANSACTION_OTHER_PROPERTY_REMOVED details = (PIMP_TRANSACTION_OTHER_PROPERTY_REMOVED)e.Message;
            Transaction t = transactions[details.Field1] as Transaction;
            if (t == null) return;
            Property p = GetProperty(details.Field2);
            if (p == null) return;
            t.RemoveOtherProperty(p);
            break;
        }
        case PIMP_ERROR_TRANSACTION_CARD_NOT_OWNED.MessageType: break; // XXX
        case PIMP_TRANSACTION_CARD_ADDED.MessageType: {
            PIMP_TRANSACTION_CARD_ADDED details = (PIMP_TRANSACTION_CARD_ADDED)e.Message;
            Transaction t = transactions[details.Field1] as Transaction;
            if (t == null) return;
            Card c = GetCard(details.Field2);
            if (c == null) return;
            t.AddThisCard(c);
            break;
        }
        case PIMP_TRANSACTION_CARD_REMOVED.MessageType: {
            PIMP_TRANSACTION_CARD_REMOVED details = (PIMP_TRANSACTION_CARD_REMOVED)e.Message;
            Transaction t = transactions[details.Field1] as Transaction;
            if (t == null) return;
            Card c = GetCard(details.Field2);
            if (c == null) return;
            t.RemoveThisCard(c);
            break;
        }
        case PIMP_TRANSACTION_OTHER_CARD_ADDED.MessageType: {
            PIMP_TRANSACTION_OTHER_CARD_ADDED details = (PIMP_TRANSACTION_OTHER_CARD_ADDED)e.Message;
            Transaction t = transactions[details.Field1] as Transaction;
            if (t == null) return;
            Card c = GetCard(details.Field2);
            if (c == null) return;
            t.AddOtherCard(c);
            break;
        }
        case PIMP_TRANSACTION_OTHER_CARD_REMOVED.MessageType: {
            PIMP_TRANSACTION_OTHER_CARD_REMOVED details = (PIMP_TRANSACTION_OTHER_CARD_REMOVED)e.Message;
            Transaction t = transactions[details.Field1] as Transaction;
            if (t == null) return;
            Card c = GetCard(details.Field2);
            if (c == null) return;
            t.RemoveOtherCard(c);
            break;
        }
        case PIMP_TRANSACTION_FINISHED.MessageType: {
            PIMP_TRANSACTION_FINISHED details = (PIMP_TRANSACTION_FINISHED)e.Message;
            Transaction t = transactions[details.Field1] as Transaction;
            if (t == null) return;
            t.ThisFinish();
            break;
        }
        case PIMP_TRANSACTION_OTHER_FINISHED.MessageType: {
            PIMP_TRANSACTION_OTHER_FINISHED details = (PIMP_TRANSACTION_OTHER_FINISHED)e.Message;
            Transaction t = transactions[details.Field1] as Transaction;
            if (t == null) return;
            t.OtherFinish();
            break;
        }
        case PIMP_ERROR_TRANSACTION_NOT_SUITABLE.MessageType: {
            PIMP_ERROR_TRANSACTION_NOT_SUITABLE details = (PIMP_ERROR_TRANSACTION_NOT_SUITABLE)e.Message;
            Transaction t = transactions[details.Field1] as Transaction;
            if (t == null) return;
            t.Unacceptable(details.Field2);
            break;
        }
        case PIMP_TRANSACTION_REOPENED.MessageType: {
            PIMP_TRANSACTION_REOPENED details = (PIMP_TRANSACTION_REOPENED)e.Message;
            Transaction t = transactions[details.Field1] as Transaction;
            if (t == null) return;
            t.ThisReopen();
            break;
        }
        case PIMP_TRANSACTION_OTHER_REOPENED.MessageType: {
            PIMP_TRANSACTION_OTHER_REOPENED details = (PIMP_TRANSACTION_OTHER_REOPENED)e.Message;
            Transaction t = transactions[details.Field1] as Transaction;
            if (t == null) return;
            t.OtherReopen();
            break;
        }
        case PIMP_ERROR_TRANSACTION_PROPERTY_HAS_HOUSE.MessageType: {
            PIMP_ERROR_TRANSACTION_PROPERTY_HAS_HOUSE details = (PIMP_ERROR_TRANSACTION_PROPERTY_HAS_HOUSE)e.Message;
            Transaction t = transactions[details.Field1] as Transaction;
            if (t == null) return;
            Property p = GetProperty(details.Field2);
            if (p == null) return;
            t.PropertyHasHouse(p);
            break;
        }
        case PIMP_ERROR_TRANSACTION_WOULD_REDUCE_NET_WORTH.MessageType: {
            PIMP_ERROR_TRANSACTION_WOULD_REDUCE_NET_WORTH details = (PIMP_ERROR_TRANSACTION_WOULD_REDUCE_NET_WORTH)e.Message;
            Transaction t1 = transactions[details.Field1] as Transaction;
            if (t1 == null) return; // ZZZ payload error
            Transaction t2 = transactions[details.Field2] as Transaction;
            if (t2 == null) return; // ZZZ payload error
            t1.WouldReduceNetWorth(t2);
            break;
        }
        case PIMP_TRANSACTION_AGREED.MessageType: {
            PIMP_TRANSACTION_AGREED details = (PIMP_TRANSACTION_AGREED)e.Message;
            Transaction t = transactions[details.Field1] as Transaction;
            if (t == null) return;
            t.ThisAgree();
            break;
        }
        case PIMP_TRANSACTION_OTHER_AGREED.MessageType: {
            PIMP_TRANSACTION_OTHER_AGREED details = (PIMP_TRANSACTION_OTHER_AGREED)e.Message;
            Transaction t = transactions[details.Field1] as Transaction;
            if (t == null) return;
            t.OtherAgree();
            break;
        }
        case PIMP_TRANSACTION_FINALISED.MessageType: {
            PIMP_TRANSACTION_FINALISED details = (PIMP_TRANSACTION_FINALISED)e.Message;
            Transaction t = transactions[details.Field1] as Transaction;
            if (t == null) return;
            t.Finalised();
            if (t == blockingTransaction) {
                blockingTransaction = null;
                turnNotifier.Visible = false;
            }
            break;
        }
        case PIMP_ERROR_TRANSACTION_NOT_BANKRUPT.MessageType: {
            PIMP_ERROR_TRANSACTION_NOT_BANKRUPT details = (PIMP_ERROR_TRANSACTION_NOT_BANKRUPT)e.Message;
            Transaction t = transactions[details.Field1] as Transaction;
            if (t == null) return;
            Transaction t2 = transactions[details.Field2] as Transaction;
            t.NotBankrupt(t2);
            break;
        }
        case PIMP_TRANSACTION_CANCELLED.MessageType: {
            PIMP_TRANSACTION_CANCELLED details = (PIMP_TRANSACTION_CANCELLED)e.Message;
            Transaction t = transactions[details.Field1] as Transaction;
            if (t == null) return;
            t.Cancelled();
            blockingTransaction = null;
            break;
        }
        case PIMP_TRANSACTION_UNUSUAL.MessageType: {
            PIMP_TRANSACTION_UNUSUAL details = (PIMP_TRANSACTION_UNUSUAL)e.Message;
            Transaction t = transactions[details.Field1] as Transaction;
            if (t == null) return;
            t.Unusual();
            break;
        }
        case PIMP_ERROR_TRANSACTION_CANNOT_BE_CANCELLED.MessageType: {
            PIMP_ERROR_TRANSACTION_CANNOT_BE_CANCELLED details = (PIMP_ERROR_TRANSACTION_CANNOT_BE_CANCELLED)e.Message;
            Transaction t = transactions[details.Field1] as Transaction;
            if (t == null) return;
            t.CannotCancel();
            break;
        }
        case PIMP_GOT_CARD.MessageType: {
            PIMP_GOT_CARD details = (PIMP_GOT_CARD)e.Message;
            Player p = GetPlayer(details.Field1);
            if (p == null) break; // ZZZ payload error
            int s = details.Field2;
            Card c = GetCard(details.Field3);
            if (c == null) break; // ZZZ payload error
            c.Picker = p;
            board.Card = c;
            logForm.AddEntry(p, " picked up a card at ", board.GetSquareName(s), ": ", c);
            break;
        }
        case PIMP_DELTA_CASH.MessageType: {
            PIMP_DELTA_CASH details = (PIMP_DELTA_CASH)e.Message;
            Player p1 = GetPlayer(details.Field1);
            Player p2 = GetPlayer(details.Field2);
            uint c = details.Field3;
            if (p1 != null)
                p1.Cash -= c;
            if (p2 != null)
                p2.Cash += c;
            logForm.AddEntry("$", c, " went from ", p1, " to ", p2, ".");
            String p1Name, p2Name;
            if (p1 == null) break;
            if (p2 == null) break;
            if (p1 == thisPlayer)
                p1Name = "you";
            else
                p1Name = p1.Name;
            if (p2 == thisPlayer)
                p2Name = "You";
            else
                p2Name = p2.Name;
            statusNetworkPanel.Text = String.Format("{0} got ${1} from {2}",
                                                    p2Name, c, p1Name);
            break;
        }
        case PIMP_DELTA_PROPERTY.MessageType: {
            PIMP_DELTA_PROPERTY details = (PIMP_DELTA_PROPERTY)e.Message;
            Player p1 = GetPlayer(details.Field1);
            Player p2 = GetPlayer(details.Field2);
            Property p = GetProperty(details.Field3);
            if (p == null) return; // ZZZ payload error
            p.Owner = p2;
            // XXX animate
            logForm.AddEntry(p, " went from ", p1, " to ", p2, ".");
            break;
        }
        case PIMP_DELTA_CARD.MessageType: {
            PIMP_DELTA_CARD details = (PIMP_DELTA_CARD)e.Message;
            Player p1 = GetPlayer(details.Field1);
            Player p2 = GetPlayer(details.Field2);
            Card c = GetCard(details.Field3);
            if (c == null) return; // ZZZ payload error
            c.Owner = p2;
            // XXX animate
            logForm.AddEntry("A card went from ", p1, " to ", p2, ": ", c);
            break;
        }
        case PIMP_DELTA_BANK.MessageType: {
            PIMP_DELTA_BANK details = (PIMP_DELTA_BANK)e.Message;
            statusHousePanel.Text = details.Field1.ToString();
            statusHotelPanel.Text = details.Field2.ToString();
            break;
        }
        default:
            statusNetworkPanel.Text = e.Message.ToString() + " unexpected";
            network.SendMessage(new PIMP_ERROR_UNEXPECTED_MESSAGE(e.Message.GetMessageType()));
            break;
        }
    }

    public void QueryCandidiate(uint candidate, String name, bool playing) {
        DialogResult result = MessageBox.Show(this, String.Format(
                                      "Would you like to let {0} {1} this game?",
                                          name, playing ? "play in" : "observe"),
                                              "Kaspar",
                                              MessageBoxButtons.YesNo,
                                              MessageBoxIcon.Question,
                                              MessageBoxDefaultButton.Button1);
        switch (result) {
        case DialogResult.Yes:
            network.SendMessage(new PIMP_ACCEPT_JOIN(candidate));
            break;
        case DialogResult.No:
            network.SendMessage(new PIMP_REFUSE_JOIN(candidate));
            break;
        }
    }

    public void NetworkError(Object sender, Network.StringEventArgs e) {
        statusNetworkPanel.Text = e.Data;
        /*
        MessageBox.Show(this,
                        "An error occurred with our connection to the server.\n\n"
                        + e.Data + ".",
                        "Kaspar",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
        */
    }

    private bool fatalErrorInProgress = false;
    public void NetworkFatalError(Object sender, Network.StringEventArgs e) {
        if (fatalErrorInProgress)
            return;
        try {
            network.Thread.Suspend();
            fatalErrorInProgress = true;
            DialogResult result = MessageBox.Show(this,
                                                  "An error occurred with our connection to the server.\n\n"
                                                  + e.Data + ".",
                                                  "Kaspar",
                                                  MessageBoxButtons.RetryCancel,
                                                  MessageBoxIcon.Error,
                                                  MessageBoxDefaultButton.Button1);
            network.Thread.Resume();
            if (result == DialogResult.Cancel)
                Disconnect();
        } finally {
            fatalErrorInProgress = false;
        }
    }

}
