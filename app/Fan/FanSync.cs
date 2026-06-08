namespace GHelper.Fan
{
    public static class FanSync
    {
        // Shifts all 8 speed bytes of a fan curve by delta, clamped to 0..100.
        // Curve layout: bytes 0..7 = temp points, bytes 8..15 = speed % at those points.
        public static byte[] AdjustSpeedBytes(byte[] curve, int delta)
        {
            var copy = (byte[])curve.Clone();
            if (copy.Length != 16) return copy;
            for (int i = 8; i < 16; i++)
            {
                int v = copy[i] + delta;
                if (v < 0) v = 0;
                if (v > 100) v = 100;
                copy[i] = (byte)v;
            }
            return copy;
        }
    }
}
