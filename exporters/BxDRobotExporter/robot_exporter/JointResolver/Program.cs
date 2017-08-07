using System;
using EditorsLibrary;
using System.Windows.Forms;

static class Program
{

    [STAThread]
    public static void Main(String[] args)
    {
        SynthesisGUI GUI = new SynthesisGUI(true);
        Application.Run(GUI);
    }
}