using WindowsInput;

namespace Sygnalista_A.LocalLibrary.MouseKeyPasterino;

public static class Mouse
{
    public async static Task Move(int x, int y)
    {
        Cursor.Position = new Point(x, y);
    }

    public async static Task MoveLeftClick(int x, int y)
    {
        InputSimulator inputSimulator = new();
        await Move(x, y);
        inputSimulator.Mouse.LeftButtonClick();
    }

    public async static Task MouseClickNewInfo(int x, int y, int length)
    {
        await MoveLeftClick(x + (8 * length) + 200, y);
    }

    private static void BlockMouseInput()
    {
        Cursor.Hide();
    }

    private static void UnblockMouseInput()
    {
        Cursor.Show();
    }
}
