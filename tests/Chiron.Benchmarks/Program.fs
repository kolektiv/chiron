module Chiron.Benchmarks

open BenchmarkDotNet
open BenchmarkDotNet.Tasks
open Chiron
open Chiron.Operators
open Jil
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

[<BenchmarkTask (jitVersion = BenchmarkJitVersion.RyuJit)>]
type ChironVsJsonNet () =

    let simpleRecord =
        { Id = 42
          Name = "Andrew"
          Address = "Brighton"
          Scores = [| 12; 20; 45 |] }

    let simpleRecordJson =
        """{"address":"Brighton","id":42,"name":"Andrew","scores":[12,20,45]}"""

    [<Benchmark ("Chiron Serialization")>]
    member __.ChironSerialize () =
        (Json.serialize >> Json.format) simpleRecord

    [<Benchmark ("Chiron Deserialization")>]
    member __.ChironDeserialize () : SimpleRecord =
        (Json.parse >> Json.deserialize) simpleRecordJson

    [<Benchmark ("Jil Serialization")>]
    member __.JilSerialize () =
        JSON.Serialize simpleRecord

    [<Benchmark ("Jil Deserialization")>]
    member __.JilDeserialize () : SimpleRecord =
        JSON.Deserialize<SimpleRecord> simpleRecordJson

    [<Benchmark ("Json.Net Serialization")>]
    member __.JsonNetSerialize () =
        JsonConvert.SerializeObject simpleRecord

    [<Benchmark ("Json.Net Deserialization")>]
    member __.JsonNetDeserialize () : SimpleRecord =
        JsonConvert.DeserializeObject<SimpleRecord> simpleRecordJson

(* Main *)

[<EntryPoint>]
let main _ =

    let runner = new BenchmarkRunner ()
    let reports = runner.Run<ChironVsJsonNet> ()
    let _ = System.Console.ReadLine ()

    0