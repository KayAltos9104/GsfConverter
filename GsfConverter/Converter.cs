using System.Globalization;

namespace GsfConverter;

public static class Converter
{
    public static List<(double X, double Y, double Z)> ReadStructure (string path)
    {
        var pointsList = new List<(double X, double Y, double Z)> ();
        string[] lines = File.ReadAllLines(path);
        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Split(", ");
            pointsList.Add((float.Parse(
                line[1], new CultureInfo("en-us")), 
                float.Parse(line[2], new CultureInfo("en-us")), 
                float.Parse(line[3], new CultureInfo("en-us")
                )
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