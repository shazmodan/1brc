// The task is to write a Java program which reads the file, 
// calculates the min, mean, and max temperature value per weather station, 
// and emits the results on stdout like this (i.e. sorted alphabetically by station name, 
// and the result values per station in the format <min>/<mean>/<max>, rounded to one fractional digit):

open System
open System.Collections.Generic

type Station = string
type Min = float
type Count = float
type Mean = float
type Max = float
type Temperature = float

let printResult(dict: Dictionary<string, Min * Mean * Max * Count>) =
    dict
    |> Seq.sortBy (fun keyValue -> keyValue.Key)
    |> Seq.iter(fun (keyValue) ->
        let (min, mean, max, _) = keyValue.Value
        printfn $"%s{keyValue.Key};%.1f{min};%.1f{Math.Round(mean, 1)};%.1f{max}")

//TODO: Check if struct(min, mean, max) instead of normal tuples (allocated on stack instead) is faster.
// let a = (1, "hej")          // System.Tuple<int,string>, heap
// let b = struct (1, "hej")   // System.ValueTuple<int,string>, stack (usually)
let parseLine (line: string) : Station * Temperature  =
    let semiColonIndex = line.IndexOf ';'
    line.Substring(0, semiColonIndex), line.Substring(semiColonIndex + 1) |> float

let addLineToDict (resultDict: Dictionary<string, Min * Mean * Max * Count>) (line: string) : Dictionary<string, Min * Mean * Max * Count> =
    let station, newTemperature = parseLine line

    match resultDict.ContainsKey station with
    | false -> 
        resultDict.[station] <- (newTemperature, newTemperature, newTemperature, 1.0)
        resultDict
    | true -> 
        let (existingMin, existingMean, existingMax, existingCount) = resultDict.[station]
        let newCount = existingCount + 1.0
        let newMin = min existingMin newTemperature
        let newMax = max existingMax newTemperature
        let newMean = (existingMean + newTemperature) / newCount

        resultDict.[station] <- (newMin, newMean, newMax, newCount)
        resultDict

//TODO: Prob not readalllines, one a time is best
let parse (path: string) : Dictionary<string, Min * Mean * Max * Count> =
    // let initMap : Map<string, Min * Mean * Max * Count> = Map.empty
    let initDict : Dictionary<string, Min * Mean * Max * Count> = Dictionary()
    
    let lines: string array = System.IO.File.ReadAllLines path

    (initDict, lines)
    ||> Array.fold (fun accMap line -> addLineToDict accMap line)


[<EntryPoint>]
let main args =
    let stopWatch = System.Diagnostics.Stopwatch.StartNew()

    if args.Length = 0 then
        printfn "Missing file path"
        1
    else
        let path: string = args[0]
        let resultDict = parse path

        printResult resultDict

        stopWatch.Stop()
        printfn "%fms" stopWatch.Elapsed.TotalMilliseconds

        0

// Initial time: 1559.331800ms
// Using Dictionary instead of Map: 1158.244300ms