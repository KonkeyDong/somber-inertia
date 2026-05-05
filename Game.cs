using Raylib_cs;

namespace SomberInertia;

public static class Game
{
    public static int FrameCount { get; private set; } = 0;
    public static float DeltaTime { get; private set; } = 0f;

    // Call this once per frame in your main loop
    public static void Update()
    {
        FrameCount++;
        DeltaTime = Raylib.GetFrameTime();
    }

    // Optional helper methods
    public static bool EveryNthFrame(int n) => FrameCount % n == 0;
}