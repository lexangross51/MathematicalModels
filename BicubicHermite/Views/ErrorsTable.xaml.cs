using System;

namespace BicubicHermite.Views;

public partial class ErrorsTable : IDisposable
{
    private bool _isDisposed;
    
    public ErrorsTable()
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