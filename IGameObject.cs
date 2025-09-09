using System.Text;

internal interface IGameObject
{
    void Update(double deltaTime);
    void Render(StringBuilder frameBuffer);
}