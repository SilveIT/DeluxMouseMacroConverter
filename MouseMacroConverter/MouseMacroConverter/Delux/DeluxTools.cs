namespace MouseMacroConverter.Delux
{
    public static unsafe class DeluxTools
    {
        public static void EncryptMacro(ref byte[] buffer)
        {
            var v2 = 0;
            var v3 = 185;
            do
            {
                fixed (byte* p = &buffer[v2])
                    EncryptRotate((uint*)p);
                v2 += 8; //v2 += 2;
                --v3;
            }
            while (v3 > 0);
        }

        public static void DecryptMacro(ref byte[] buffer)
        {
            var v4 = 0;
            var v5 = 185;
            do
            {
                fixed (byte* p = &buffer[v4])
                    DecryptRotate((uint*)p);
                v4 += 8; //v4 += 2; 
                --v5;
            }
            while (v5 > 0);
        }

        private static ulong EncryptRotate(uint* buffer)
        {
            // ReSharper disable JoinDeclarationAndInitializer
            uint result; // eax
            uint v2; // ecx
            uint v3; // edx
            uint v4; // esi

            result = *buffer;
            v2 = buffer[1];
            v3 = 0;
            v4 = 32;
            do
            {
                v3 -= 0x61C88647;
                result += (v3 + v2) ^ (65570 + 16 * v2) ^ (6353952u + (v2 >> 5));
                v2 += (v3 + result) ^ (3 + 16 * result) ^ (4 + (result >> 5));
                --v4;
            } while (v4 > 0);
            *buffer = result;
            buffer[1] = v2;
            return result;
            // ReSharper restore JoinDeclarationAndInitializer
        }

        private static ulong DecryptRotate(uint* buffer)
        {
            // ReSharper disable JoinDeclarationAndInitializer
            uint v1; // ecx
            uint result; // eax
            uint v3; // edx
            int v4; // esi

            v1 = *buffer;
            result = buffer[1];
            v3 = 0xC6EF3720;
            v4 = 32;
            do
            {
                result -= (v3 + v1) ^ (3 + 16 * v1) ^ (4 + (v1 >> 5));
                v1 -= (v3 + result) ^ (65570 + 16 * result) ^ (6353952u + (result >> 5));
                v3 += 0x61C88647;
                --v4;
            } while (v4 > 0);
            *buffer = v1;
            buffer[1] = result;
            return result;
            // ReSharper restore JoinDeclarationAndInitializer
        }
    }
}