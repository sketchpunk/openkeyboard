using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace OpenKeyboard{
	public abstract class vLayout{
		public static void Load(string fName,Grid uiGrid,Window uiWindow){
			//..........................................
			//Load up Layout XML
			XmlDocument xml = new XmlDocument();
			xml.Load(RootPath("Layouts\\" + fName + ".xml"));

			XmlElement root = xml.DocumentElement;
			if(root.ChildNodes.Count == 0) return;

			//..........................................
			//Set window size and position
			double sHeight = SystemParameters.WorkArea.Height;
            double sWidth = SystemParameters.WorkArea.Width;
			
			uiWindow.Width = double.Parse(root.GetAttribute("width"));
			uiWindow.Height = double.Parse(root.GetAttribute("height"));

			switch(root.GetAttribute("vpos")){
				case "top": uiWindow.Top = 20; break;
				case "center": uiWindow.Top = (sHeight - uiWindow.Height) / 2; break;
				case "bottom": uiWindow.Top = sHeight - uiWindow.Height - 20; break;
			}//switch

			switch(root.GetAttribute("hpos")){
				case "left": uiWindow.Left = 20; break;
				case "center": uiWindow.Left = (sWidth - uiWindow.Width) / 2; break;
				case "right": uiWindow.Left = sWidth - uiWindow.Width - 20; break;
			}//switch

			//..........................................
			//Reset UI Grid
			uiGrid.Children.Clear();
			uiGrid.RowDefinitions.Clear();
			uiGrid.ColumnDefinitions.Clear();

			//Create all the rows on the main UI Grid
			for(int i=0; i < root.ChildNodes.Count; i++) uiGrid.RowDefinitions.Add(new RowDefinition(){ Height=new GridLength(1,GridUnitType.Star) });

			//..........................................
			//Reset UI Grid
			int iRow=0,iKey=0;
			Grid rGrid;

			foreach(XmlNode row in root.ChildNodes){
				//Create Key Row Container
				rGrid = CreateGrid();
				Grid.SetRow(rGrid,iRow);
				Grid.SetColumn(rGrid,0);
				uiGrid.Children.Add(rGrid);

				//Create Keys
				iKey=0;
				double gLen = 0;
				string sgLen = "";
				foreach(XmlElement key in row.ChildNodes){
					sgLen = key.GetAttribute("weight");
					gLen = (String.IsNullOrEmpty(sgLen))?1:Double.Parse(sgLen);

					rGrid.ColumnDefinitions.Add(new ColumnDefinition(){ Width=new GridLength(gLen,GridUnitType.Star) });
					rGrid.Children.Add(CreateButton(key,iKey));
					iKey++;
				}//for
				iRow++;
			}//for
		}//func

		private static Grid CreateGrid(){
			Grid grid = new Grid();
			grid.RowDefinitions.Add(new RowDefinition(){ Height=new GridLength(1,GridUnitType.Star) });
			return grid;
		}//func

		private static vButton CreateButton(XmlElement elm,int col){
			string code = elm.GetAttribute("code");
			string shCode = elm.GetAttribute("shcode");
			string shText = elm.GetAttribute("shtext");
			string title = elm.GetAttribute("text");
			
			vButton btn = new vButton(){};
			Grid.SetRow(btn,0);
			Grid.SetColumn(btn,col);

			if(!String.IsNullOrEmpty(code)) btn.KBKeys = code.Split(' ');
			if(!String.IsNullOrEmpty(shCode)) btn.KBShKeys = shCode.Split(' ');
			if(!String.IsNullOrEmpty(shText)) btn.ShiftText = shText;

			btn.Content = title;
			btn.SendString = elm.GetAttribute("string");
			btn.shSendString = elm.GetAttribute("shstring");
			btn.Click += vKeyboard.OnKeyPress;
			return btn;
		}//func

		private static string RootPath(string relativePath){
			string rtn = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
			if(!rtn.EndsWith("\\")) rtn += "\\";

			return rtn + relativePath;
		}//func
	
		public static string[] GetLayoutList(){
            string [] rtn = System.IO.Directory.GetFiles(RootPath("Layouts"), "*.xml");

            for(int i=0; i < rtn.Length; i++) rtn[i] = System.IO.Path.GetFileNameWithoutExtension(rtn[i]);

            return rtn; 
		}//func
	}//cls
}//ns
