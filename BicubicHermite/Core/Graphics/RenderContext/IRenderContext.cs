namespace BicubicHermite.Core.Graphics.Objects.RenderContext;

public interface IRenderContext
{
    void AddObject(IBaseObject obj);
    void DrawObjects();
    void UpdateView();
    int[] GetNewViewport(ScreenSize newScreenSize);
    void Clear();
}