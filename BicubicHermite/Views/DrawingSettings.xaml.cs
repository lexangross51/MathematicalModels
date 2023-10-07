using System;

namespace BicubicHermite.Views;

public partial class DrawingSettings : IDisposable
{
    private bool _isDisposed;
    
    public DrawingSettings()
    {
        InitializeComponent();
    }

    private void Dispose(bool disposing)
    {
        if (!disposing || _isDisposed) return;
        _isDisposed = true;
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}