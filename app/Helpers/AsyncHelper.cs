namespace GHelper.Helpers
{
    public static class AsyncHelper
    {
        // Polls `condition` at `intervalMs` until it's true, instead of guessing a fixed delay.
        // Always bounded by `timeoutMs` as a safety net for hardware that never reports the expected state.
        // `token` lets an in-flight poll bail out immediately when superseded by a newer request,
        // instead of running to completion (or timeout) against a target that's already stale.
        public static Task<bool> PollUntilAsync(Func<bool> condition, int intervalMs, int timeoutMs, CancellationToken token = default) =>
            PollUntilAsync(_ => Task.FromResult(condition()), intervalMs, timeoutMs, token);

        // Same as above, for conditions that are themselves genuinely async (e.g. awaiting a
        // real process/service operation) rather than a cheap synchronous state check.
        public static async Task<bool> PollUntilAsync(Func<CancellationToken, Task<bool>> condition, int intervalMs, int timeoutMs, CancellationToken token = default)
        {
            var deadline = Environment.TickCount64 + timeoutMs;

            while (Environment.TickCount64 < deadline)
            {
                token.ThrowIfCancellationRequested();
                if (await condition(token)) return true;
                await Task.Delay(intervalMs, token);
            }

            return await condition(token);
        }
    }
}
