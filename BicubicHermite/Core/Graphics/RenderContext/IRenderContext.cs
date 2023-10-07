using BicubicHermite.Core.Graphics.Objects;
using BicubicHermite.Core.Graphics.Objects.RenderContext;

namespace BicubicHermite.Core.Graphics.RenderContext;

public interface IRenderContext
{
    void AddObject(IBaseObject obj);
    void DeleteObject(IBaseObject obj);
    void DrawObjects();
    void UpdateView();
    int[] GetNewViewport(ScreenSize newScreenSize);
}