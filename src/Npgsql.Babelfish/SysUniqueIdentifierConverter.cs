using System;
using System.Runtime.CompilerServices;
using Npgsql.Internal;
using Npgsql.Internal.Postgres;

namespace Npgsql.Babelfish;

sealed class SysUniqueIdentifierConverter : PgBufferedConverter<Guid>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(16);
        return format is DataFormat.Binary;
    }

    protected override Guid ReadCore(PgReader reader)
    {
        Span<byte> buffer = stackalloc byte[16];
        reader.ReadBytes(buffer);

        // Reverse Data1 (4 bytes)
        Swap(buffer, 0, 3);
        Swap(buffer, 1, 2);

        // Reverse Data2 (2 bytes)
        Swap(buffer, 4, 5);

        // Reverse Data3 (2 bytes)
        Swap(buffer, 6, 7);

        return new Guid(buffer);
    }

    protected override void WriteCore(PgWriter writer, Guid value)
    {
        Span<byte> buffer = stackalloc byte[16];
        value.TryWriteBytes(buffer);

        // Reverse Data1 (4 bytes)
        Swap(buffer, 0, 3);
        Swap(buffer, 1, 2);

        // Reverse Data2 (2 bytes)
        Swap(buffer, 4, 5);

        // Reverse Data3 (2 bytes)
        Swap(buffer, 6, 7);

        writer.WriteBytes(buffer);
    }

    public override bool CanHandle(PgTypeId pgTypeId)
    {
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Swap(Span<byte> buffer, int i, int j)
    {
        var temp = buffer[i];
        buffer[i] = buffer[j];
        buffer[j] = temp;
    }
}

