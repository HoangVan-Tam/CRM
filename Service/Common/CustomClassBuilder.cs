﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Services.Common
{
    public partial class CustomClassBuilder
    {
        AssemblyName asemblyName;
        public CustomClassBuilder(string ClassName)
        {
            this.asemblyName = new AssemblyName(ClassName);
        }
        public object CreateObject(string[] PropertyNames, Type[] Types, bool[] IsRequireds, string[] ColumnLabels)
        {
            if (PropertyNames.Length != Types.Length)
            {
                Console.WriteLine("The number of property names should match their corresopnding types number");
            }

            TypeBuilder DynamicClass = this.CreateClass();
            this.CreateConstructor(DynamicClass);
            for (int ind = 0; ind < PropertyNames.Count(); ind++)
            {
                List<CustomAttributeBuilder> lstCustomAttributeBuilders = new List<CustomAttributeBuilder>();

                var columnLabel = ColumnLabels[ind];
                if (!string.IsNullOrEmpty(columnLabel))
                {
                    Type[] constructorParameters = new Type[] { typeof(string) };
                    var constructorInfo = typeof(DisplayNameAttribute).GetConstructor(constructorParameters);
                    CustomAttributeBuilder customAttribute = new CustomAttributeBuilder(constructorInfo, new object[] { columnLabel });
                    lstCustomAttributeBuilders.Add(customAttribute);
                }

                if (IsRequireds[ind] == true)
                {
                    Type[] constructorParameters = new Type[] { };
                    var constructorInfo = typeof(RequiredAttribute).GetConstructor(constructorParameters);
                    PropertyInfo errorMessageProperty = typeof(RequiredAttribute).GetProperty("ErrorMessage");
                    CustomAttributeBuilder customAttribute = new CustomAttributeBuilder(constructorInfo, new object[] {}, new PropertyInfo[] { errorMessageProperty }, new object[] { columnLabel + " is a required field!" });
                    lstCustomAttributeBuilders.Add(customAttribute);
                }
              

                CreateProperty(DynamicClass, PropertyNames[ind], Types[ind], lstCustomAttributeBuilders);
            }
            Type type = DynamicClass.CreateType();
                                                                                                                                                    
            return Activator.CreateInstance(type);
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
