using System;
using System.Windows;
using BicubicHermite.Core.Graphics.Objects.Mesh;
using BicubicHermite.Core.Graphics.Objects.RenderContext;
using BicubicHermite.ViewModels;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace BicubicHermite;

public partial class MainWindow : IViewFor<MainViewModel>
{
    [Reactive] public double XCursorPosition { get; set;}
    [Reactive] public double YCursorPosition { get; set;}
    
    public MainWindow()
    {
        InitializeComponent();

        ViewModel = new MainViewModel();
        DataContext = ViewModel;

        this.WhenAnyValue(view => view.ViewModel!.LoadedPoints)
            .Subscribe(data =>
            {
                if (data == null) return;
                
                GraphicControl.AddObject(new Scatter(data, 2));
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