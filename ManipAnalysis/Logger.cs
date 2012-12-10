using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ManipAnalysis
{
    static class Logger
    {
        private delegate void myLogCallback(string text);
        private delegate void myLogCallbackArray(string[] text);
        
        public static ListBox theLogBox;

        static Logger()
        {
            theLogBox = new ListBox();
            theLogBox.FormattingEnabled = true;
            theLogBox.HorizontalScrollbar = true;
            theLogBox.HorizontalExtent = 10000;
            theLogBox.Location = new System.Drawing.Point(748, 25);
            theLogBox.Name = "listBox_Log";
            theLogBox.ScrollAlwaysVisible = true;
            theLogBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            theLogBox.Size = new System.Drawing.Size(422, 576);
            theLogBox.TabIndex = 6;
        }

        public static void writeToLog(string text)
        {
            if (theLogBox.InvokeRequired)
            {
                myLogCallback _writeToLog = new myLogCallback(writeToLog);
                theLogBox.Invoke(_writeToLog, new object[] { text });
            }
            else
            {
                theLogBox.Items.Add(
                                            "["
                                            + DateTime.Now.Year.ToString("0000")
                                            + "."
                                            + DateTime.Now.Month.ToString("00")
                                            + "."
                                            + DateTime.Now.Day.ToString("00")
                                            + "-"
                                            + DateTime.Now.Hour.ToString("00")
                                            + "."
                                            + DateTime.Now.Minute.ToString("00")
                                            + "."
                                            + DateTime.Now.Second.ToString("00")
                                            + "] "
                                            + text
                                    );
            }
        }

        public static void writeToLog(string[] text)
        {
            if (theLogBox.InvokeRequired)
            {
                myLogCallbackArray _writeToLog = new myLogCallbackArray(writeToLog);
                theLogBox.Invoke(_writeToLog, new object[] { text });
            }
            else
            {
                theLogBox.Items.Add(
                                            "["
                                            + DateTime.Now.Year.ToString("0000")
                                            + "."
                                            + DateTime.Now.Month.ToString("00")
                                            + "."
                                            + DateTime.Now.Day.ToString("00")
                                            + "-"
                                            + DateTime.Now.Hour.ToString("00")
                                            + "."
                                            + DateTime.Now.Minute.ToString("00")
                                            + "."
                                            + DateTime.Now.Second.ToString("00")
                                            + "]"
                                );
                theLogBox.Items.AddRange(text);
            }
        }

        public static string[] getLogText()
        {
            return theLogBox.Items.Cast<string>().ToArray();
        }

        public static void clearLogBox()
        {
            theLogBox.Items.Clear();
        }
    }
}
