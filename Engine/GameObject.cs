using System.Text;
using JetBrains.Annotations;

namespace Engine;

[MeansImplicitUse(ImplicitUseTargetFlags.WithInheritors)]
internal abstract class GameObject
{
    public abstract void Update(double deltaTime);
    public abstract void Render(StringBuilder frameBuffer);
}