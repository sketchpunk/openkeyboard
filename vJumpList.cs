using System;
using System.Windows;
using System.Windows.Shell;

namespace OpenKeyboard{
    
    public class vJumpList{
        private JumpList mList = null;

        public string CategoryName { get; set; }
        public string AppPath { get; set; }

        public vJumpList(){
            mList = new JumpList();
            JumpList.SetJumpList(Application.Current, mList);

            CategoryName = "Category";
            AppPath = "";
        }//func

        public void Apply() { mList.Apply(); }

        public void AddTask(string title,string desc, string arg){
            mList.JumpItems.Add(new JumpTask() {
                Title           = title,
                Description     = desc,
                ApplicationPath = AppPath,
                Arguments       = arg,
                CustomCategory  = CategoryName
            });            
        }//func
    }//cls
}//ns
