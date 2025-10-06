using System;
using Npgsql.Internal;
using Npgsql.Internal.Postgres;

namespace Npgsql.Babelfish;

sealed class BabelfishTypeInfoResolverFactory : PgTypeInfoResolverFactory
{
    public override IPgTypeInfoResolver CreateResolver() => new Resolver();
    public override IPgTypeInfoResolver? CreateArrayResolver() => new ArrayResolver();

    class Resolver : IPgTypeInfoResolver
    {
        protected static DataTypeName SysUniqueIdentifierDataTypeName => new("sys.uniqueidentifier");

        TypeInfoMappingCollection? _mappings;
        protected TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new());

        public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        {
            var typeInfo = Mappings.Find(type, dataTypeName, options);
            return typeInfo;
        }

        static TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings)
        {
            mappings.AddResolverStructType<Guid>("pg_catalog.uuid",
                static (options, mapping, _) => {
                    return mapping.CreateInfo(options,
                        new BabelfishPgConverterResolver<Guid>(
                            new SysUniqueIdentifierConverter(),
                            options.GetCanonicalTypeId(SysUniqueIdentifierDataTypeName)), true);
                }, isDefault: true);

            return mappings;
        }

        public static BabelfishPgConverterResolver<Guid> CreateResolver(PgTypeId pgTypeId) => new(
            new SysUniqueIdentifierConverter(),
            pgTypeId
        );
    }

    sealed class BabelfishPgConverterResolver<T> : PgConverterResolver<T>
    {
        private readonly PgConverter<T> _converter;
        private readonly PgTypeId _pgTypeId;

        internal BabelfishPgConverterResolver(
            PgConverter<T> converter,
            PgTypeId pgTypeId)
        {
            _converter = converter;
            _pgTypeId = pgTypeId;
        }

        public override PgConverterResolution? Get(T? value, PgTypeId? expectedPgTypeId)
        {
            return GetDefault(expectedPgTypeId);
        }

        public override PgConverterResolution GetDefault(PgTypeId? pgTypeId)
        {
            return new PgConverterResolution(_converter, _pgTypeId);
        }
    }

    sealed class ArrayResolver : Resolver, IPgTypeInfoResolver
    {
        TypeInfoMappingCollection? _mappings;
        new TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new(base.Mappings));

        public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => Mappings.Find(type, dataTypeName, options);

        static TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings)
        {
            mappings.AddResolverStructArrayType<Guid>("pg_catalog.uuid");
            return mappings;
        }
    }
}
