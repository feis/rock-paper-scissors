using System.Text;

internal interface IGameObject
{
    void Update(Game game, StringBuilder frameBuffer);
}