using System;
using System.Windows.Forms;


namespace StandaloneViewer
{
    public static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            LaunchParams.Load(args);
            Application.Run(new StandaloneViewerForm());
        }
    }
}