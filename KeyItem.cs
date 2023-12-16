namespace OpenKeyboard
{
    public struct KeyItem
    {
        public int code;
        public bool isUpLast;
        public string extendCode;

        public KeyItem(int c, bool isLast) { code = c; isUpLast = isLast; extendCode = null; }//func
        public KeyItem(int c) { code = c; isUpLast = false; extendCode = null; }//func
        public KeyItem(string exCode) { code = 0; isUpLast = false; extendCode = exCode; }//func
    }//func
}//ns
