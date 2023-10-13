using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Windows;
using BicubicHermite.Core.Graphics.Colorbar;
using BicubicHermite.Core.Graphics.Objects.ColorMap;
using BicubicHermite.Core.Graphics.Objects.Contour;
using BicubicHermite.Core.Graphics.Objects.Mesh;
using BicubicHermite.Core.Graphics.Objects.RenderContext;
using BicubicHermite.Core.Graphics.Palette;
using BicubicHermite.ViewModels;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TriangleNet;
using TriangleNet.Meshing;
using Mesh = BicubicHermite.Core.Graphics.Objects.Mesh.Mesh;
using Point = BicubicHermite.Core.Graphics.Objects.Point;

namespace BicubicHermite;

public partial class MainWindow : IViewFor<MainViewModel>
{
    private ColorMap? _colorMap;
    private Contour? _contour;
    private Mesh? _mesh;
    
    [Reactive] public double XCursorPosition { get; private set;}
    [Reactive] public double YCursorPosition { get; private set;}
    
    public MainWindow()
    {
        InitializeComponent();
        
        IMesh? contourMesh = null!;
        double[] values = null!;
        ViewModel = new MainViewModel();
        DataContext = ViewModel;

        this.WhenActivated(disposables =>
        {
            this.WhenAnyValue(view => view.ViewModel!.LoadedPoints)
                .WhereNotNull()
                .Subscribe(data =>
                {
                    GraphicControl.AddObject(new Scatter(data, 2));
                })
                .DisposeWith(disposables);

            this.WhenAnyValue(view => view.ViewModel!.AreaRectangle)
                .WhereNotNull()
                .Subscribe(rect =>
                {
                    GraphicControl.AddObject(rect);
                })
                .DisposeWith(disposables);

            this.WhenAnyValue(view => view.ViewModel!.RegularMesh)
                .Subscribe(mesh =>
                {
                    if (mesh == null && _mesh != null)
                    {
                        GraphicControl.DeleteObject(_mesh!);
                        _mesh = null;
                    }
                    else if (mesh == null) {}
                    else
                    {
                        _mesh = mesh;
                        GraphicControl.AddObject(_mesh);
                    }
                })
                .DisposeWith(disposables);

            this.WhenAnyValue(view => view.ViewModel!.Spline)
                .Subscribe(spline =>
                {
                    if (spline == null && _colorMap != null)
                    {
                        GraphicControl.DeleteObject(_colorMap!);
                        GraphicControl.DeleteObject(_contour!);

                        _colorMap = null;
                        _contour = null;
                    }
                    else if (spline == null) { }
                    else
                    {
                        var x = ViewModel.RegularMesh!.Points.DistinctBy(p => p.X).ToArray();
                        var y = ViewModel.RegularMesh!.Points.DistinctBy(p => p.Y).ToArray();

                        var nx = ViewModel.RegularMesh.AbscissaPointsCount * ViewModel.PointsFactor;
                        var ny = ViewModel.RegularMesh.OrdinatePointsCount * ViewModel.PointsFactor;

                        var hx = (x[^1].X - x[0].X) / (nx - 1);
                        var hy = (y[^1].Y - y[0].Y) / (ny - 1);

                        var points = new List<Point>();

                        for (int i = 0; i < ny; i++)
                        {
                            for (int j = 0; j < nx; j++)
                            {
                                points.Add(new Point((double)(x[0].X + j * hx)!, (double)(y[0].Y + i * hy)!));
                            }
                        }

                        var linePoints = new List<Point>();
                        for (int j = 0; j < nx; j++)
                        {
                            linePoints.Add(new Point((double)(x[0].X + j * hx)!, 6.0));
                        }
                        
                        var line = spline.CalculateAtPoints(linePoints);
                        FilesWorking.WriteData("graph", linePoints, line);
                        
                        values = spline.CalculateAtPoints(points).ToArray();
                        var delaunay = new TriangleNet.Meshing.Algorithm.Dwyer();
                    
                        contourMesh = delaunay.Triangulate(MathHelper.ToTriangleNetVertices(points), new Configuration());
                        _contour = new Contour(contourMesh, values, 30);
                        _colorMap = new ColorMap(_contour.Mesh, values, Palette.RainbowReverse);
                    
                        GraphicControl.AddObject(_colorMap);
                        GraphicControl.AddObject(_contour);
                        
                        var colorbar = new Colorbar(values, Palette.Rainbow)
                        {
                            VerticalAlignment = VerticalAlignment.Bottom,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            Margin = new Thickness(0, 0, 0, 5)
                        };
                        
                        GraphicControl.MainGrid.Children.Add(colorbar);
                    }
                })
                .DisposeWith(disposables);

            this.WhenAnyValue(view => view.ViewModel!.DrawIsolines)
                .Subscribe(drawIsolines =>
                {
                    if (_contour == null) return;
                    
                    if (drawIsolines)
                        GraphicControl.AddObject(_contour!);
                    else
                        GraphicControl.DeleteObject(_contour!);
                });
            
            this.WhenAnyValue(view => view.ViewModel!.IsolinesCount)
                .Subscribe(isolinesCount =>
                {
                    if (_contour == null) return;

                    GraphicControl.DeleteObject(_contour!);
                    _contour = new Contour(contourMesh, values, isolinesCount);
                    GraphicControl.AddObject(_contour);
                });
            
            this.WhenAnyValue(view => view.ViewModel!.DrawColorbar)
                .Subscribe(drawColorbar =>
                {
                    if (_contour == null) return;
                    
                    if (drawColorbar)
                    {
                        var colorbar = new Colorbar(values, Palette.Rainbow)
                        {
                            VerticalAlignment = VerticalAlignment.Bottom,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            Margin = new Thickness(0, 0, 0, 5)
                        };
                        
                        GraphicControl.MainGrid.Children.Add(colorbar);
                    }
                    else
                        GraphicControl.MainGrid.Children.Remove(GraphicControl.MainGrid.Children[^1]);
                });
        });
        
        GraphicControl.WhenAnyValue(p => p.XCursorPosition, p => p.YCursorPosition)
            .Subscribe(pair =>
            {
                XCursorPosition = pair.Item1;
                YCursorPosition = pair.Item2;
            });
    }

    private void MainWindowOnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        GraphicControl.OnChangeSize(new ScreenSize
        {
            Width = Width - MainGrid.ColumnDefinitions[1].ActualWidth - 30,
            Height = Height - StatusBar.ActualHeight - 59,
        });
    }

    private void MainGridOnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        GraphicControl.OnChangeSize(new ScreenSize
        {
            Width = Width - MainGrid.ColumnDefinitions[1].ActualWidth - 30,
            Height = Height - StatusBar.ActualHeight - 59,
        });
    }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => throw new Exception("");
    }

    public MainViewModel? ViewModel { get; set; }
}