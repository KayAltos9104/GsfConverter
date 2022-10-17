using System.Globalization;

namespace GsfConverter;

public static class Converter
{
    public static List<(double X, double Y, double Z)> ReadStructure (string path)
    {
        var pointsList = new List<(double X, double Y, double Z)> ();
        string[] lines = File.ReadAllLines(path);
        float xMin = 0;
        float yMin = 0;
        float zMin = 0;

        // Да, это мерзотный дубляж, но мне лень. Работает - и хорошо :-)

        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Split(",");
            if (float.Parse(line[1].Trim(), new CultureInfo("en-us")) < xMin)
                xMin = float.Parse(line[1].Trim(), new CultureInfo("en-us"));
            if (float.Parse(line[2].Trim(), new CultureInfo("en-us")) < yMin)
                yMin = float.Parse(line[2].Trim(), new CultureInfo("en-us"));
            if (float.Parse(line[3].Trim(), new CultureInfo("en-us")) < zMin)
                zMin = float.Parse(line[3].Trim(), new CultureInfo("en-us"));
        }
        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Split(",");
            pointsList.Add((float.Parse(
                line[1].Trim(), new CultureInfo("en-us"))-xMin, 
                float.Parse(line[2].Trim(), new CultureInfo("en-us")) - yMin, 
                float.Parse(line[3].Trim(), new CultureInfo("en-us")) - zMin
                ));
        }
        return pointsList;
    }
    
    public static List<(int X, int Y, int Z)> ADConvert (
        List<(double X, double Y, double Z)> points, 
        int multiplier
        )
    {
        int discretizationCoefficient = multiplier;
       
        List<(int X, int Y, int Z)> discretePoints = new List<(int X, int Y, int Z)>();
        
        foreach (var point in points)
        {
            int X = (int)(point.X * discretizationCoefficient);
            int Y = (int)(point.Y * discretizationCoefficient);
            int Z = (int)(point.Z * discretizationCoefficient);                
            discretePoints.Add((X, Y, Z));
        }           
        return discretePoints;
    }

    public static void SaveToGsf(string path, List<(int X, int Y, int Z)> points)
    {
        StreamWriter sw = new StreamWriter(path);
        
        sw.WriteLine("Structure");
        foreach (var p in points)
        {
            sw.WriteLine(string.Join("\t", new string[] {p.X.ToString(),p.Y.ToString(),
                                p.Z.ToString(), "1"}));
        }
                    
        sw.Close();
    }
}