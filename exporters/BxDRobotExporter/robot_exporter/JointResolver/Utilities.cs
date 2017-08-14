using Inventor;
using System.IO;
using System;
using System.Diagnostics;


public class Utilities
{
    public static Vector ToInventorVector(BXDVector3 v)
    {
        if (InventorManager.Instance == null) return null;
        return InventorManager.Instance.TransientGeometry.CreateVector(v.x, v.y, v.z);
    }

    public static BXDVector3 ToBXDVector(dynamic p)
    {
        return new BXDVector3(p.X, p.Y, p.Z);
    }

    public static string VectorToString(object pO)
    {
        if (pO is Vector)
        {
            Vector p = (Vector) pO;
            return (p.X + "," + p.Y + "," + p.Z);
        }
        else if (pO is UnitVector)
        {
            UnitVector p = (UnitVector) pO;
            return (p.X + "," + p.Y + "," + p.Z);
        }
        else if (pO is Point)
        {
            Point p = (Point) pO;
            return (p.X + "," + p.Y + "," + p.Z);
        }
        return "";
    }

    public static double BoxVolume(Box b)
    {
        double dx = b.MaxPoint.X - b.MinPoint.X;
        double dy = b.MaxPoint.Y - b.MinPoint.Y;
        double dz = b.MaxPoint.Z - b.MinPoint.Z;
        return dx * dy * dz;
    }
}
public class ExporterLogger
{
    public enum LoggerMode { DateTime, Precise }
    private LoggerMode mode;
    private Stopwatch Stopwatch;

    internal StreamWriter Writer;
    public ExporterLogger(LoggerMode mode = LoggerMode.DateTime)
    {
        string OutDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\Synthesis\\Exporters";
        if (!Directory.Exists(OutDir))
            Directory.CreateDirectory(OutDir);
        string FileName = "RobotExporter-" + DateTime.Now.ToString("MMM-dd-yyyy_hh.mm.ss") + ".log";
        Writer = new StreamWriter(OutDir + FileName);
        this.mode = mode;
        if (mode == LoggerMode.Precise)
        {
            Stopwatch = new Stopwatch();
            Stopwatch.Start();
        }
        Writer.WriteLine(DateTime.Now.ToString() + "\n");
    }

    /// <summary>
    /// Writes the given string to the <see cref="StreamWriter"/>. Log files will be located in %appdata%\Synthesis\Exporters
    /// </summary>
    /// <param name="output"></param>
    public void LogText(string output, bool WriteLine = true)
    {
        string Opening = (mode == LoggerMode.DateTime) ? DateTime.Now.ToString("DDD hh:mm:ss tt") : Stopwatch.ElapsedMilliseconds.ToString() + " ms";
        if (WriteLine)
            Writer.WriteLine(Opening + "> " + output);
        else
            Writer.Write(Opening + "> " + output);
    }
    public async void LogTextAsync(string output, bool WriteLine = true)
    {
        string Opening = (mode == LoggerMode.DateTime) ? DateTime.Now.ToString("DDD hh:mm:ss tt") : Stopwatch.ElapsedMilliseconds.ToString() + " ms";
        if (WriteLine)
            await Writer.WriteLineAsync(Opening + "> " + output);
        else
            await Writer.WriteAsync(Opening + "> " + output);
    }

    public void DisposeWriter()
    {
        Writer.Dispose();
    }
}
