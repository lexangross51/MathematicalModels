using System.Linq;
using System.Reactive;
using System.Windows.Forms;
using BicubicHermite.Core.Graphics.Objects;
using ReactiveUI;

namespace BicubicHermite.ViewModels;

public class MainViewModel : ReactiveObject
{
    private Point[]? _loadedPoints;
    private double[]? _loadedValues;
    
    public Point[]? LoadedPoints
    {
        get => _loadedPoints;
        set => this.RaiseAndSetIfChanged(ref _loadedPoints, value);
    }

    public double[]? LoadedValues
    {
        get => _loadedValues;
        set => this.RaiseAndSetIfChanged(ref _loadedValues, value);
    }
    
    public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenModelCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveModelCommand { get; }
    public ReactiveCommand<Unit, Unit> BuildMeshCommand { get; }
    public ReactiveCommand<Unit, Unit> DeleteMeshCommand { get; }
    public ReactiveCommand<Unit, Unit> BuildSplineCommand { get; }
    public ReactiveCommand<Unit, Unit> DeleteSplineCommand { get; }

    public MainViewModel()
    {
        LoadDataCommand = ReactiveCommand.Create<Unit>(_ =>
        {
            using var openFileWindow = new OpenFileDialog();
        
            if (openFileWindow.ShowDialog() != DialogResult.OK) return;
        
            var pair = FilesWorking.LoadData(openFileWindow.FileName);
            LoadedPoints = pair.Points.ToArray();
            LoadedValues = pair.Values.ToArray();
        });
    }
}