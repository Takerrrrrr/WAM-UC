using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ut64configurator
{
    internal class EventTest
    {
        public delegate void NumManipulationHandler();
        public event NumManipulationHandler ChangeNumSendCmd;
        public event NumManipulationHandler ChangeNumDataSaveStart;
        public event NumManipulationHandler ChangeNumDataSaveStop;
        public event NumManipulationHandler ChangeNumDataSaveEmpty;
        private Thread _eventThread;
        public void SetValueInSeparateThread(int flag)  //setValue
        {
            if (ChangeNumSendCmd != null || ChangeNumDataSaveStart != null || ChangeNumDataSaveStop != null || ChangeNumDataSaveEmpty != null)
            {
                if (flag == 1)
                {
                    ChangeNumSendCmd();
                }
                else if (flag == 2)
                {
                    ChangeNumDataSaveStart();
                }
                else if (flag == 3)
                {
                    ChangeNumDataSaveStop();
                }
                else
                {
                    ChangeNumDataSaveEmpty();
                }
            }

        }
        public void setValue(int flag) //setValue  SetValueInSeparateThread
        {
            // Create a new thread to run the event
            if (_eventThread == null || !_eventThread.IsAlive)
            {
                _eventThread = new Thread(new ThreadStart(() =>
                {
                    SetValueInSeparateThread(flag);
                }));
                // Start the thread
                _eventThread.Start();
            }



        }


    }



}
