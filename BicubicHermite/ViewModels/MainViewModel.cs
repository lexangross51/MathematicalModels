using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Forms;
using BicubicHermite.Core.Graphics.Objects;
using BicubicHermite.Core.Graphics.Objects.Mesh;
using BicubicHermite.Core.Graphics.Palette;
using BicubicHermite.Core.SplineCalculator.Mesh;
using BicubicHermite.Core.SplineCalculator.Spline;
using BicubicHermite.Views;
using BicubicHermiteSpline.Spline;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace BicubicHermite.ViewModels;

public class MainViewModel : ReactiveObject
{
    private List<PracticeData>? _data;
    private readonly SplineBuilder _splineBuilder = new();
    [Reactive] public HermiteBicubicSpline? Spline { get; private set; }
    [Reactive] public double? Alpha { get; set; } = 1e-06;
    [Reactive] public double? Omega { get; set; } = 1.0;
    [Reactive] public int? PointsFactor { get; set; } = 4;
    [Reactive] public double Residual { get; set; }
    
    [Reactive] public Point[]? LoadedPoints { get; set; }
    [Reactive] public double[]? LoadedValues { get; set; }
    [Reactive] public Rectangle? AreaRectangle { get; set; }

    [Reactive] public int? XSplits { get; set; } = 10;
    [Reactive] public int? YSplits { get; set; } = 10;
    [Reactive] public int? MeshRefinement { get; set; } = 0;
    [Reactive] public Mesh? RegularMesh { get; set; }

    [Reactive] public bool DrawIsolines { get; set; } = true;
    [Reactive] public int IsolinesCount { get; set; } = 30;
    [Reactive] public bool DrawColorbar { get; set; } = false;

    public ObservableCollection<double> Residuals { get; } = new();
    public ReactiveCommand<Unit, Unit> LoadData { get; }
    // public ReactiveCommand<Unit, Unit> OpenModel { get; }
    // public ReactiveCommand<Unit, Unit> SaveModel { get; }
    public ReactiveCommand<Unit, Unit> BuildMesh { get; }
    public ReactiveCommand<Unit, Unit> BuildSpline { get; }
    public ReactiveCommand<Unit, Unit> DeleteSpline { get; }
    public ReactiveCommand<Unit, Unit> DeleteMesh { get; }
    public ReactiveCommand<Unit, Unit> DrawingSettingsOpenWindow { get; }
    public ReactiveCommand<Unit, Unit> AddResidualToList { get; }

    public MainViewModel()
    {
        this.WhenAnyValue(x => x.LoadedPoints)
            .WhereNotNull()
            .Subscribe(data =>
            {
                var pair = MathHelper.MakeAreaForData(data, 2);
                AreaRectangle = new Rectangle(pair.LeftBottom, pair.RightTop);
            });
        
        LoadData = ReactiveCommand.Create<Unit>(_ =>
        {
            using var openFileWindow = new OpenFileDialog();
        
            if (openFileWindow.ShowDialog() != DialogResult.OK) return;
        
            var pair = FilesWorking.LoadData(openFileWindow.FileName);
            LoadedPoints = pair.Points.ToArray();
            LoadedValues = pair.Values.ToArray();
        });

        BuildMesh = ReactiveCommand.Create<Unit>(_ =>
        {
            var meshParameters = new MeshParameters
            {
                LeftBottom = AreaRectangle!.LeftBottom,
                RightTop = AreaRectangle!.RightTop,
                XSplits = XSplits!.Value,
                YSplits = YSplits!.Value,
                Refinement = MeshRefinement!.Value
            };

            var meshBuilder = new MeshBuilder(meshParameters);
            RegularMesh = meshBuilder.Build();

        }, this.WhenAnyValue(
            x => x.XSplits,
            x => x.YSplits,
            x => x.MeshRefinement,
            x => x.LoadedPoints,
            (xs, ys, mr, lp)
                => xs is > 0 && ys is > 0 && mr is >= 0 && lp != null));

        BuildSpline = ReactiveCommand.Create<Unit>(_ =>
            {
                _data ??= new List<PracticeData>();
                _data.Clear();
                
                for (int i = 0; i < LoadedValues!.Length; i++)
                {
                    _data.Add(new PracticeData
                    {
                        X = LoadedPoints![i].X,
                        Y = LoadedPoints![i].Y,
                        Value = LoadedValues[i],
                    });
                }

                _splineBuilder.SetData(RegularMesh!, _data);
                _splineBuilder.AlphaRegulator = Alpha!.Value;
                _splineBuilder.OmegaWeight = Omega!.Value;
                Spline = _splineBuilder.Build();
                DrawIsolines = true;
                Residual = Spline.CalculateResidual(_data);

            }, canExecute: this.WhenAnyValue(
                    x => x.RegularMesh)
                .Select(mesh => mesh != null)
        );
        
        DeleteMesh = ReactiveCommand.Create<Unit>(_ =>
        {
            RegularMesh = null;
            
        }, this.WhenAnyValue(x => x.RegularMesh)
            .Select(mesh => mesh != null));

        DeleteSpline = ReactiveCommand.Create<Unit>(_ =>
        {
            Spline = null;
            
        }, this.WhenAnyValue(x => x.Spline)
            .Select(spline => spline != null));

        DrawingSettingsOpenWindow = ReactiveCommand.Create(() =>
        {
            using var window = new DrawingSettings();
            window.DataContext = this;
            window.Show();

        }, this.WhenAnyValue(x => x.Spline)
            .Select(spline => spline != null));

        AddResidualToList = ReactiveCommand.Create(() =>
        {
            Residuals.Add(Residual);
        });
    }
}