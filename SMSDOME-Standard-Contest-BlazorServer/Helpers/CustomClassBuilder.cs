using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SMSDOME_Standard_Contest_BlazorServer.Helpers
{
    public partial class CustomClassBuilder
    {
        AssemblyName asemblyName;
        public CustomClassBuilder(string ClassName)
        {
            this.asemblyName = new AssemblyName(ClassName);
        }
        public object CreateObject(string[] propertyNames, Type[] types, bool[] isRequired, string[] columnLabels, string[] regexPatterns)
        {
            if (propertyNames.Length != types.Length ||
                propertyNames.Length != isRequired.Length ||
                propertyNames.Length != columnLabels.Length ||
                propertyNames.Length != regexPatterns.Length)
            {
                throw new ArgumentException("All input arrays must have the same length.");
            }

            var typeBuilder = CreateClass();
            CreateConstructor(typeBuilder);

            for (int i = 0; i < propertyNames.Length; i++)
            {
                var attributes = new List<CustomAttributeBuilder>();

                if (!string.IsNullOrEmpty(columnLabels[i]))
                {
                    var displayAttribute = new CustomAttributeBuilder(
                        typeof(DisplayNameAttribute).GetConstructor(new[] { typeof(string) })!,
                        new object[] { columnLabels[i] }
                    );
                    attributes.Add(displayAttribute);
                }

                if (isRequired[i])
                {
                    var requiredAttribute = new CustomAttributeBuilder(
                        typeof(RequiredAttribute).GetConstructor(Type.EmptyTypes)!,
                        new object[] { },
                        new[] { typeof(RequiredAttribute).GetProperty(nameof(RequiredAttribute.ErrorMessage))! },
                        new object[] { $"{columnLabels[i]} is a required field!" }
                    );
                    attributes.Add(requiredAttribute);
                }

                if (!string.IsNullOrEmpty(regexPatterns[i]))
                {
                    var regexAttribute = new CustomAttributeBuilder(
                        typeof(RegularExpressionAttribute).GetConstructor(new[] { typeof(string) })!,
                        new object[] { regexPatterns[i] },
                        new[] { typeof(RegularExpressionAttribute).GetProperty(nameof(RegularExpressionAttribute.ErrorMessage))! },
                        new object[] { $"{columnLabels[i]} has an invalid format!" }
                    );
                    attributes.Add(regexAttribute);
                }

                CreateProperty(typeBuilder, propertyNames[i], types[i], attributes);
            }

            Type dynamicType = typeBuilder.CreateType()!;
            return Activator.CreateInstance(dynamicType)!;
        }

        private TypeBuilder CreateClass()
        {
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(this.asemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder typeBuilder = moduleBuilder.DefineType(this.asemblyName.FullName
                                , TypeAttributes.Public |
                                TypeAttributes.Class |
                                TypeAttributes.AutoClass |
                                TypeAttributes.AnsiClass |
                                TypeAttributes.BeforeFieldInit |
                                TypeAttributes.AutoLayout
                                , null);
            return typeBuilder;
        }
        private void CreateConstructor(TypeBuilder typeBuilder)
        {
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
        }
        private void CreateProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType, List<CustomAttributeBuilder> lstCustomAttributeBuilders)
        {
            FieldBuilder fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
            if(lstCustomAttributeBuilders.Count > 0)
            {
                foreach(var customAttributeBuilder in lstCustomAttributeBuilders)
                {
                    propertyBuilder.SetCustomAttribute(customAttributeBuilder);
                }
            }
            MethodBuilder getPropMthdBldr = typeBuilder.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
            ILGenerator getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            MethodBuilder setPropMthdBldr = typeBuilder.DefineMethod("set_" + propertyName,
                  MethodAttributes.Public |
                  MethodAttributes.SpecialName |
                  MethodAttributes.HideBySig,
                  null, new[] { propertyType });

            ILGenerator setIl = setPropMthdBldr.GetILGenerator();
            Label modifyProperty = setIl.DefineLabel();
            Label exitSet = setIl.DefineLabel();

            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }
    }
}
