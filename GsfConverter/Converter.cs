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
    
    public static List<(int X, int Y, int Z, int state)> ADConvert (
        List<(double X, double Y, double Z)> points, 
        int multiplier
        )
    {
        int discretizationCoefficient = multiplier;
       
        List<(int X, int Y, int Z, int state)> discretePoints = new List<(int X, int Y, int Z, int state)>();
        double max = points.Max(c => Math.Max(c.X, Math.Max(c.Y, c.Z)));
        double maxNew = (max * discretizationCoefficient);
        double xMax = 0;
        double yMax = 0;
        double zMax = 0;
        //foreach (var p in points)
        //{
        //    if (xMax < p.X)
        //        xMax = p.X;
        //    if (yMax < p.Y)
        //        yMax = p.Y;
        //    if (zMax < p.Z)
        //        zMax = p.Z;
        //}
        int[,,] tempArray = new int[
            (int)(points.Max(p => p.X) * discretizationCoefficient),
            (int)(points.Max(p => p.Y) * discretizationCoefficient),
            (int)(points.Max(p => p.Z) * discretizationCoefficient)];
        //int[,,] tempArray = new int[
        //    (int)(xMax * discretizationCoefficient),
        //    (int)(yMax * discretizationCoefficient),
        //    (int)(zMax * discretizationCoefficient)];

        //int ratio = (int)(maxNew/max);
        double ratio = max / maxNew;
        ratio = 0.001;
        double step = 0.005;
        foreach (var point in points)
        {
            int X = (int)(point.X * discretizationCoefficient);
            int Y = (int)(point.Y * discretizationCoefficient);
            int Z = (int)(point.Z * discretizationCoefficient);
            for (double x = point.X; x < point.X + step; x+=ratio)
            {
                for (double y = point.Y; y < point.Y + step; y += ratio)
                {
                    for (double z = point.Z; z < point.Z + step; z+=ratio)
                    {
                        if (x * discretizationCoefficient >= tempArray.GetLength(0) || 
                            y * discretizationCoefficient >= tempArray.GetLength(1) || 
                            z * discretizationCoefficient >= tempArray.GetLength(2))
                            continue;
                        tempArray[(int)(
                            x * discretizationCoefficient), 
                            (int)(y * discretizationCoefficient),
                            (int)(z * discretizationCoefficient)] = 1;
                        //discretePoints.Add((
                        //    (int)(x * discretizationCoefficient), 
                        //    (int)(y * discretizationCoefficient), 
                        //    (int)(z * discretizationCoefficient)));
                    }
                }
            }

            
            //discretePoints.Add((X, Y, Z));

        }           
        for (int z = 0; z < tempArray.GetLength(2); z++)
            for (int y = 0; y < tempArray.GetLength(1); y++)
                for (int x = 0; x < tempArray.GetLength(0); x++)
                {
                    if (tempArray[x, y, z] == 1)
                        discretePoints.Add((x, y, z, 1));
                    else if (tempArray[x, y, z] == 0)
                        discretePoints.Add((x, y, z, -10));
                }
        return discretePoints;
    }

    public static void SaveToGsf(string path, List<(int X, int Y, int Z, int state)> points)
    {
        StreamWriter sw = new StreamWriter(path);
        
        sw.WriteLine("Structure");
        foreach (var p in points)
        {
            sw.WriteLine(string.Join("\t", new string[] {p.X.ToString(),p.Y.ToString(),
                                p.Z.ToString(), p.state.ToString()}));
        }
                    
        sw.Close();
    }
}