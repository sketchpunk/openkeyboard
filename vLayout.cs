using System;
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpenKeyboard
{
    public abstract class vLayout
    {
        public static FontFamily mIconFont = new FontFamily(new Uri("pack://application:,,,/fonts/#FontAwesome"), "./#FontAwesome");

        public static double defaultfsize = 24;

        public static bool Load(string fName, Grid uiGrid, Window uiWindow)
        {
            try
            {
                //..........................................
                //Load up Layout XML
                XmlDocument xml = new XmlDocument();
                xml.Load(RootPath("Layouts\\" + fName + ".xml"));

                XmlElement root = xml.DocumentElement;
                if (root.ChildNodes.Count == 0) return false;

                //..........................................
                //Set window size and position
                double sHeight = SystemParameters.WorkArea.Height;
                double sWidth = SystemParameters.WorkArea.Width;

                uiWindow.Width = double.Parse(root.GetAttribute("width"));
                uiWindow.Height = double.Parse(root.GetAttribute("height"));

                var fsize = root.GetAttribute("fsize");

                if (!String.IsNullOrEmpty(fsize))
                    defaultfsize = double.Parse(fsize);

                switch (root.GetAttribute("vpos"))
                {
                    case "top": uiWindow.Top = 20; break;
                    case "center": uiWindow.Top = (sHeight - uiWindow.Height) / 2; break;
                    case "bottom": uiWindow.Top = sHeight - uiWindow.Height - 20; break;
                }//switch

                switch (root.GetAttribute("hpos"))
                {
                    case "left": uiWindow.Left = 20; break;
                    case "center": uiWindow.Left = (sWidth - uiWindow.Width) / 2; break;
                    case "right": uiWindow.Left = sWidth - uiWindow.Width - 20; break;
                }//switch

                //..........................................
                string sMargin = root.GetAttribute("margin");
                if (!String.IsNullOrEmpty(sMargin))
                {
                    String[] aryMargin = sMargin.Split(',');
                    if (aryMargin.Length == 4)
                    {
                        uiGrid.Margin = new Thickness(
                            int.Parse(aryMargin[0])
                            , int.Parse(aryMargin[1])
                            , int.Parse(aryMargin[2])
                            , int.Parse(aryMargin[3])
                       );
                    }//if
                }//if

                //..........................................
                //Reset UI Grid
                uiGrid.Children.Clear();
                uiGrid.RowDefinitions.Clear();
                uiGrid.ColumnDefinitions.Clear();

                //Create all the rows on the main UI Grid
                for (int i = 0; i < root.ChildNodes.Count; i++) uiGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

                //..........................................
                //Reset UI Grid
                int iRow = 0, iKey = 0;
                Grid rGrid;

                foreach (XmlNode row in root.ChildNodes)
                {
                    //Create Key Row Container
                    rGrid = CreateGrid();
                    Grid.SetRow(rGrid, iRow);
                    Grid.SetColumn(rGrid, 0);
                    uiGrid.Children.Add(rGrid);

                    //Create Keys
                    iKey = 0;
                    double gLen = 0;
                    string sgLen = "";
                    foreach (XmlElement key in row.ChildNodes)
                    {
                        sgLen = key.GetAttribute("weight");
                        gLen = (String.IsNullOrEmpty(sgLen)) ? 1 : Double.Parse(sgLen);

                        rGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(gLen, GridUnitType.Star) });
                        rGrid.Children.Add(CreateButton(key, iKey));
                        iKey++;
                    }//for
                    iRow++;
                }//for

                return true;
            }
            catch (Exception e)
            {
                vLogger.Exception("vLayout.Load", e, fName);
            }//try

            return false;
        }//func

        private static Grid CreateGrid()
        {
            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            return grid;
        }//func

        private static vButton CreateButton(XmlElement elm, int col)
        {
            string code, shCode, shText
                , title = elm.GetAttribute("text")
                , fsize = elm.GetAttribute("fsize");

            vButton btn = new vButton() { };
            Grid.SetRow(btn, 0);
            Grid.SetColumn(btn, col);
            btn.FontFamily = mIconFont;
            btn.Title = title;
            if (!String.IsNullOrEmpty(fsize))
                btn.FontSize = double.Parse(fsize);
            else
                btn.FontSize = defaultfsize;

            switch (elm.Name)
            {
                //.........................................
                case "key":
                    code = elm.GetAttribute("code");
                    shCode = elm.GetAttribute("shcode");
                    shText = elm.GetAttribute("shtext");

                    if (!String.IsNullOrEmpty(code)) btn.KBCommand.KBKeys = code.Split(' ');
                    if (!String.IsNullOrEmpty(shCode)) btn.KBCommand.KBShKeys = shCode.Split(' ');
                    if (!String.IsNullOrEmpty(shText)) btn.ShiftText = shText;

                    btn.KBCommand.SendString = elm.GetAttribute("string");
                    btn.KBCommand.shSendString = elm.GetAttribute("shstring");

                    btn.PreviewMouseLeftButtonDown += BtnTouch_Down;
                    btn.PreviewMouseLeftButtonUp += BtnTouch_Up;
                    btn.PreviewTouchDown += BtnTouch_Down;
                    btn.PreviewTouchUp += BtnTouch_Up;

                    break;
                //.........................................
                case "menu":
                    ContextMenu menu = new ContextMenu();
                    KeyboardCommand kbCmd;
                    MenuItem mItem;

                    foreach (XmlElement itm in elm.ChildNodes)
                    {
                        kbCmd = new KeyboardCommand();
                        title = itm.GetAttribute("text");
                        code = itm.GetAttribute("code");

                        if (!String.IsNullOrEmpty(code)) kbCmd.KBKeys = code.Split(' ');
                        kbCmd.SendString = itm.GetAttribute("string");

                        mItem = new MenuItem() { Header = title, Tag = kbCmd };
                        mItem.Click += OnMenuClick;
                        menu.Items.Add(mItem);
                    }//for

                    btn.ContextMenu = menu;
                    btn.Click += OnMenuButtonPress;
                    break;
            }//switch

            return btn;
        }//func

        //public static void OnButtonPress(Object sender, RoutedEventArgs e) { vKeyboard.ProcessCommand((sender as vButton).KBCommand); }//func
        private static void BtnTouch_Down(Object sender, EventArgs e) { KeyLoopHandler.BeginKeypress((sender as vButton).KBCommand); }//func
        private static void BtnTouch_Up(Object sender, EventArgs e) { KeyLoopHandler.EndKeypress(); }//func

        public static void OnMenuClick(object sender, RoutedEventArgs e) { vKeyboard.ProcessCommand((KeyboardCommand)(sender as MenuItem).Tag); }
        public static void OnMenuButtonPress(Object sender, RoutedEventArgs e) { (sender as vButton).ContextMenu.IsOpen = true; }//func

        private static string RootPath(string relativePath)
        {
            string rtn = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            if (!rtn.EndsWith("\\")) rtn += "\\";

            return rtn + relativePath;
        }//func

        public static string[] GetLayoutList()
        {
            string[] rtn = System.IO.Directory.GetFiles(RootPath("Layouts"), "*.xml");

            for (int i = 0; i < rtn.Length; i++) rtn[i] = System.IO.Path.GetFileNameWithoutExtension(rtn[i]);

            return rtn;
        }//func
    }//cls
}//ns
