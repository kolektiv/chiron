module Chiron.Benchmarks

open BenchmarkDotNet
open BenchmarkDotNet.Tasks
open Chiron
open Chiron.Operators
open Newtonsoft.Json

(* Examples *)

type SimpleRecord =
    { Id: int
      Name: string
      Address: string
      Scores: int array }

    static member ToJson (x: SimpleRecord) =
            Json.write "id" x.Id
         *> Json.write "name" x.Name
         *> Json.write "address" x.Address
         *> Json.write "scores" x.Scores

    static member FromJson (_: SimpleRecord) =
            fun id name address scores ->
                { Id = id
                  Name = name
                  Address = address
                  Scores = scores }
        <!> Json.read "id"
        <*> Json.read "name"
        <*> Json.read "address"
        <*> Json.read "scores"

(* Benchmarks *)

[<BenchmarkTask (platform = BenchmarkPlatform.AnyCpu,
                 jitVersion = BenchmarkJitVersion.LegacyJit,
                 framework = BenchmarkFramework.V452)>]
type ChironVsJsonNet () =

    let simpleRecord =
        { Id = 42
          Name = "Andrew"
          Address = "Brighton"
          Scores = [| 12; 20; 45 |] }

    [<Benchmark>]
    member __.ChironSerialize () =
        (Json.serialize >> Json.format) simpleRecord

    [<Benchmark>]
    member __.JsonNetSerialize () =
        JsonConvert.SerializeObject simpleRecord

(* Main *)

[<EntryPoint>]
let main _ =

    let runner = new BenchmarkRunner ()
    let reports = runner.Run<ChironVsJsonNet> ()

    0