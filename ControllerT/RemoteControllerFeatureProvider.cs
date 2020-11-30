using ControllerT.Controllers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace ControllerT
{
    public class RemoteControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
          //  IReadOnlyList<System.Reflection.TypeInfo> Types = new List<System.Reflection.TypeInfo> { typeof(Aftor).GetTypeInfo(), typeof(Album).GetTypeInfo(), };

            var remoteCode = new HttpClient().GetStringAsync("https://gist.githubusercontent.com/filipw/9311ce866edafde74cf539cbd01235c9/raw/6a500659a1c5d23d9cfce95d6b09da28e06c62da/types.txt").GetAwaiter().GetResult();
            if (remoteCode != null)
            {
                var compilation = CSharpCompilation.Create("DynamicAssembly", new[] { CSharpSyntaxTree.ParseText(remoteCode) },
                    new[] {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(RemoteControllerFeatureProvider).Assembly.Location)
                    },
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                using (var ms = new MemoryStream())
                {
                    var emitResult = compilation.Emit(ms);

                    if (!emitResult.Success)
                    {
                        // handle, log errors etc
                        Debug.WriteLine("Compilation failed!");
                        return;
                    }

                    ms.Seek(0, SeekOrigin.Begin);
                    var assembly = Assembly.Load(ms.ToArray());
                    var candidates = assembly.GetExportedTypes();
                    System.Type[] d = new System.Type[2];
                 //   var aa = new Aftor();
                 //   d[0] = aa.GetType();
                  //  var a = aa.GetType();
                 //   var a1111 = System.Activator.CreateInstance<Aftor>();

                    //  var a11 =  new GetType(){ Name = "Aftor", FullName = "ControllerT.Aftor" };
                    var rra = candidates.Select(x => x).ToList();

                    foreach (var entityType in IncludedEntities.Types)
                    {
                        //        feature.Controllers.Add(typeof(GenController<>).MakeGenericType(candidate).GetTypeInfo());

                        var controllerType = typeof(GenController<>).MakeGenericType(entityType.AsType()).GetTypeInfo();
                                         feature.Controllers.Add(controllerType);     
                    }
                }
            }
        }
    }
    public static class IncludedEntities { public static IReadOnlyList<System.Reflection.TypeInfo> Types = new List<System.Reflection.TypeInfo> { typeof(Album).GetTypeInfo()/*, typeof(Boo).GetTypeInfo()*/, }; }
}


    //public static class IncludedEntities
    //{ public static IReadOnlyList<System.Reflection.TypeInfo> Types = new List<System.Reflection.TypeInfo> { typeof(Animals).GetTypeInfo(), typeof(Insects).GetTypeInfo(), }; }

    //public class GenericControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    //{
    //    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature) { 
    // // Get the list of entities that we want to support for the generic controller     
    //             foreach (var entityType in IncludedEntities.Types)       
    //              {     
    //                var typeName = entityType.Name + "Controller";               
    //                 // Check to see if there is a "real" controller for this class     
    //                   if (!feature.Controllers.Any(t => t.Name == typeName))      
    //                  {                // Create a generic controller for this type    
    //                      var controllerType = typeof(GenericController<>).MakeGenericType(entityType.AsType()).GetTypeInfo();
    //                       feature.Controllers.Add(controllerType);     
    //                  }
    //              }
    //    }

    //}


