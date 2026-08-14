namespace GHelper.Helpers
{
    public static class AsyncHelper
    {
        // Polls `condition` at `intervalMs` until it's true, instead of guessing a fixed delay.
        // Always bounded by `timeoutMs` as a safety net for hardware that never reports the expected state.
        public static async Task<bool> PollUntilAsync(Func<bool> condition, int intervalMs, int timeoutMs)
        {
            var deadline = Environment.TickCount64 + timeoutMs;

            while (Environment.TickCount64 < deadline)
            {
                if (condition()) return true;
                await Task.Delay(intervalMs);
            }

            return condition();
        }
    }
}
