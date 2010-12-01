﻿module FSharp.Javascript.Compiler

open System
open System.IO
open System.Linq
open System.CodeDom.Compiler
open Microsoft.FSharp.Compiler.CodeDom
open System.Reflection

let CompileFSharpString(str, assemblies, output) =
        use pro = new FSharpCodeProvider()
        let opt = CompilerParameters()
        opt.OutputAssembly <- output
        //opt.CompilerOptions <- [for a in assemblies -> "-r:\"" + a + "\""] |> String.concat " "
        assemblies |> Seq.iter (fun a -> opt.ReferencedAssemblies.Add(a) |> ignore)
        let res = pro.CompileAssemblyFromSource( opt, [|str|] )
        let errors = [for a in res.Errors -> a]
        if errors.Length = 0 then 
             (errors, Some(FileInfo(res.PathToAssembly)))
        else (errors, None)

let (++) v1 v2   = Path.Combine(v1, v2)
let assemblyDirectory = Path.GetDirectoryName(Assembly.GetCallingAssembly().CodeBase).Remove(0, 6)
let defaultAsms() = [|(assemblyDirectory ++ "FSharp.Javascript.dll")|]
let randomFile directory = directory ++ Path.GetRandomFileName() + ".dll"

type System.CodeDom.Compiler.CodeCompiler with 
    static member CompileFSharpString (str, directory, ?assemblies, ?output) =
        let assemblies  = defaultArg assemblies (defaultAsms())
        let output      = defaultArg output (randomFile directory)
        CompileFSharpString(str, assemblies, output)

let compile (input:string) (directory:string) =
    let errors,fileInfo = CodeCompiler.CompileFSharpString(input, directory)
    if fileInfo.IsSome then
        let assemData = File.ReadAllBytes(fileInfo.Value.FullName)
        let assem = Assembly.Load(assemData)
        let moduleType = assem.GetExportedTypes().[0]
        let ast = FSharp.Javascript.ModuleCompiler.getAstFromType moduleType
        let javascript = FSharp.Javascript.Printer.getJavascript ast

        File.Delete(fileInfo.Value.FullName)
        (errors, Some(javascript))
    else
        (errors, None)