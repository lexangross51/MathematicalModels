namespace BicubicHermite.ViewModels;

public struct TableRecord
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Value { get; set; }
    public double Spline { get; set; }
    public double AbsError { get; set; }
}