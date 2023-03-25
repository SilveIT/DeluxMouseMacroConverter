using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BinarySilvelizerX.Extensions;
using MouseMacroConverter.Delux;

namespace MouseMacroConverter
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            WireDragDrop(Controls);
        }

        // ReSharper disable once InconsistentNaming
        public static byte LOBYTE(int dwValue) => (byte) (dwValue & 0xFF);

        // ReSharper disable once InconsistentNaming
        public static byte HIBYTE(int dwValue) => (byte) (dwValue >> 8);

        // ReSharper disable once InconsistentNaming
        public static ushort LOWORD(uint nValue) => (ushort) (nValue & 0xFFFF);

        // ReSharper disable once InconsistentNaming
        public static ushort HIWORD(uint nValue) => (ushort) (nValue >> 16);

        private void WireDragDrop(Control.ControlCollection ctls)
        {
            foreach (Control ctl in ctls)
            {
                ctl.AllowDrop = true;
                ctl.DragEnter += MainForm_DragEnter;
                ctl.DragDrop += MainForm_DragDrop;
                WireDragDrop(ctl.Controls);
            }
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Move;
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (var filepath in files) 
                ProcessFile(filepath);
        }

        private string GetWeirdTimeInfo(int t)
        {
            //var lb = LOBYTE(999);
            //var wh = ((15 & 0xF) << 12);
            ////var t = LOBYTE(999) | ((((15 & 0xF) << 12) | (55 | (44 << 6))) << 8);
            //var t = 15828480;
            //rtbLog.Text += "T: " + t + " lb: " + lb + "; wh: " + wh + "\r\n";
            var rMs = LOBYTE(t);
            var rS = HIBYTE(t) & 0x3F;
            var rM = (byte)((t >> 8 >> 6) & 0x3F);
            var rH = (byte)((t >> 8 >> 12) & 0xF);
            //rtbLog.Text += "Final: MS: " + rMs + "; S: " + rS + "; M: " + rM + "; H:" + rH + "\r\n";
            //var rt = LOBYTE(rMs) | ((((rH & 0xF) << 12) | (rS | (rM << 6))) << 8);
            //rtbLog.Text += "RT: " + rt + "\r\n";
            return "H: " + rH + "; M: " + rM + "; S: " + rS + "; M: " + rMs;
        }

        private void FixMacro(DeluxMacro macro)
        {
            int pos = macro.Name.IndexOf('\0');
            if (pos >= 0)
                macro.Name = macro.Name.Substring(0, pos);
        }

        private string GetActionString(byte action)
        {
            if (Enum.IsDefined(typeof(DeluxAction), action))
                return ((DeluxAction) action).ToString();
                
            if (Enum.IsDefined(typeof(Keys), (int)action))
                return "Key " + (Keys) action;

            return "Unknown 0x" + action.ToString("X");
        }

        private void ProcessFile(string filepath)
        {
            rtbLog.Text += filepath + "\r\n";
            var bytes = File.ReadAllBytes(filepath);
            //rtbLog.Text += bytes.ToHex();
            DeluxTools.DecryptMacro(ref bytes);
            //rtbLog.Text += bytes.ToHex();

            var macro = (DeluxMacro)bytes;
            FixMacro(macro);

            rtbLog.Text += "Key: 0x" + macro.Key.ToString("X") + 
                           "\r\nWeirdTime: " + GetWeirdTimeInfo(macro.WeirdTime) + 
                           //"\r\nUnk0: " + macro.Unk0 + 
                           //"\r\nUnk1: " + macro.Unk1 +
                           //"\r\nUnk2: " + macro.Unk2 +
                           "\r\nName: " + macro.Name.Replace("\0", "") +
                           //"\r\nUnk3: " + macro.Unk3 +
                           //"\r\nUnk4: " + macro.Unk4 +
                           //"\r\nUnk5: " + macro.Unk5 +
                           "\r\n";

            var resS = "";
            foreach (var i in macro.Instructions.TakeWhile(i => i.Action != 0))
            {
                resS += "-------------------------------------------\r\n";
                resS += "Action: " + 
                        GetActionString(i.Action) + 
                        (i.Action == 0xF0 || i.Action == 0xF1 || i.Action == 0xF2 || i.Action < 0xF0 ? i.Pressed > 0 ? ", DOWN" : ", UP" : "") + 
                        (i.Param != 0 ? ", " + i.Param : "") + 
                        "\r\n";
                if (i.DelayAfter != 0)
                    resS += "Delay: " + i.DelayAfter + "\r\n";
            }
            resS += "-------------------------------------------" + "\r\n";

            rtbLog.Text += resS;
            //byte[] res = macro;
            //rtbLog.Text += "RESULT:\r\n" + res.ToHex() + "\r\n";

            //DeluxTools.EncryptMacro(ref res);
            //File.WriteAllBytes(filepath.Replace(".mcf", "_OUR.mcf"), res);
            //rtbLog.Text += "ENCRYPTED\r\n";
            //rtbLog.Text += bytes.ToHex();
        }
    }
}
