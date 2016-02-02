using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Reflection;
using System.Reflection.Emit;
using ANDREICSLIB.ClassExtras;
using PropertyAttributes = System.Reflection.PropertyAttributes;

namespace SQLAutoJoin
{
    public static class DynamicSQL
    {
        public static List<Dictionary<string, object>> DynamicSQlQueryToDict(this Database database, string sql,
            params object[] parameters)
        {
            var l = (((DbRawSqlQuery) DynamicSqlQuery(database, sql, parameters)).ToListAsync()).Result;
            var ret = new List<Dictionary<string, object>>();
            foreach (var d in l)
            {
                var dd = ExpandoObjectExtras.ToDictionary(d);
                ret.Add(dd);
            }
            return ret;
        }

        public static dynamic DynamicSqlQuery(this Database database, string sql, params object[] parameters)
        {
            var builder = createTypeBuilder(
                "MyDynamicAssembly", "MyDynamicModule", "MyDynamicObject");

            using (IDbCommand command = database.Connection.CreateCommand())
            {
                try
                {
                    database.Connection.Open();
                    command.CommandText = sql;
                    command.CommandTimeout = command.Connection.ConnectionTimeout;
                    foreach (var param in parameters)
                    {
                        command.Parameters.Add(param);
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        var schema = reader.GetSchemaTable();

                        foreach (DataRow row in schema.Rows)
                        {
                            var name = (string) row["ColumnName"];
                            var type = (Type) row["DataType"];
                            if (type != typeof (string) && (bool) row.ItemArray[schema.Columns.IndexOf("AllowDbNull")])
                            {
                                type = typeof (Nullable<>).MakeGenericType(type);
                            }
                            createAutoImplementedProperty(builder, name, type);
                        }
                    }
                }
                finally
                {
                    database.Connection.Close();
                    command.Parameters.Clear();
                }
            }

            var resultType = builder.CreateType();

            return database.SqlQuery(resultType, sql, parameters);
        }

        private static TypeBuilder createTypeBuilder(
            string assemblyName, string moduleName, string typeName)
        {
            var typeBuilder = AppDomain
                .CurrentDomain
                .DefineDynamicAssembly(new AssemblyName(assemblyName),
                    AssemblyBuilderAccess.Run)
                .DefineDynamicModule(moduleName)
                .DefineType(typeName, TypeAttributes.Public);
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
            return typeBuilder;
        }

        private static void createAutoImplementedProperty(
            TypeBuilder builder, string propertyName, Type propertyType)
        {
            const string PrivateFieldPrefix = "m_";
            const string GetterPrefix = "get_";
            const string SetterPrefix = "set_";

            // Generate the field.
            var fieldBuilder = builder.DefineField(
                string.Concat(PrivateFieldPrefix, propertyName),
                propertyType, FieldAttributes.Private);

            // Generate the property
            var propertyBuilder = builder.DefineProperty(
                propertyName, PropertyAttributes.HasDefault, propertyType, null);

            // Property getter and setter attributes.
            var propertyMethodAttributes =
                MethodAttributes.Public | MethodAttributes.SpecialName |
                MethodAttributes.HideBySig;

            // Define the getter method.
            var getterMethod = builder.DefineMethod(
                string.Concat(GetterPrefix, propertyName),
                propertyMethodAttributes, propertyType, Type.EmptyTypes);

            // Emit the IL code.
            // ldarg.0
            // ldfld,_field
            // ret
            var getterILCode = getterMethod.GetILGenerator();
            getterILCode.Emit(OpCodes.Ldarg_0);
            getterILCode.Emit(OpCodes.Ldfld, fieldBuilder);
            getterILCode.Emit(OpCodes.Ret);

            // Define the setter method.
            var setterMethod = builder.DefineMethod(
                string.Concat(SetterPrefix, propertyName),
                propertyMethodAttributes, null, new[] {propertyType});

            // Emit the IL code.
            // ldarg.0
            // ldarg.1
            // stfld,_field
            // ret
            var setterILCode = setterMethod.GetILGenerator();
            setterILCode.Emit(OpCodes.Ldarg_0);
            setterILCode.Emit(OpCodes.Ldarg_1);
            setterILCode.Emit(OpCodes.Stfld, fieldBuilder);
            setterILCode.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getterMethod);
            propertyBuilder.SetSetMethod(setterMethod);
        }
    }
}