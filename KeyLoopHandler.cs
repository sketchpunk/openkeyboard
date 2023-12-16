using System.Timers;

namespace OpenKeyboard
{
    public abstract class KeyLoopHandler
    {
        private static KeyboardCommand mKBCommand;
        private static Timer mTimer = null;
        private static bool mIsTimerOn = false;

        public static void EndKeypress() { StopTimer(); }//func
        public static void BeginKeypress(KeyboardCommand cmd)
        {
            if (mIsTimerOn) return;
            vKeyboard.ProcessCommand(cmd);

            mKBCommand = cmd;
            StartTimer();
        }//for

        private static void StopTimer() { mTimer.Stop(); mIsTimerOn = false; }
        private static void StartTimer()
        {
            if (mTimer == null)
            {
                mTimer = new Timer();
                mTimer.Interval = 200;
                mTimer.Elapsed += new ElapsedEventHandler(onTick);
            }
            else if (mIsTimerOn) return;

            mTimer.Start();
            mIsTimerOn = true;
        }//func

        private static void onTick(object sender, ElapsedEventArgs e) { vKeyboard.ProcessCommand(mKBCommand); }
    }//cls
}//ns
