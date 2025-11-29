// The task is to write a Java program which reads the file, 
// calculates the min, mean, and max temperature value per weather station, 
// and emits the results on stdout like this (i.e. sorted alphabetically by station name, 
// and the result values per station in the format <min>/<mean>/<max>, rounded to one fractional digit):

open System

type Station = string
type Min = float
type Count = float
type Mean = float
type Max = float
type Temperature = float

let printResult(map: Map<string, Min * Mean * Max * Count>) =
    map
    |> Map.iter (fun station (min, mean, max, _) -> 
        printfn $"%s{station};%.1f{min};%.1f{Math.Round(mean, 1)};%.1f{max}")


//TODO: Check if struct(min, mean, max) instead of normal tuples (allocated on stack instead) is faster.
// let a = (1, "hej")          // System.Tuple<int,string>, heap
// let b = struct (1, "hej")   // System.ValueTuple<int,string>, stack (usually)
let parseLine (line: string) : Station * Temperature  =
    let semiColonIndex = line.IndexOf ';'
    line.Substring(0, semiColonIndex), line.Substring(semiColonIndex + 1) |> float

let addLineToDict (resultMap: Map<string, Min * Mean * Max * Count>) (line: string) =
    let station, newTemperature = parseLine line

    let existingValuesOpt = resultMap.TryFind station

    match existingValuesOpt with
    | None -> 
        resultMap 
        |> Map.add station (newTemperature, newTemperature, newTemperature, 1.0)
    | Some (existingMin, existingMean, existingMax, existingCount) ->
        let newCount = existingCount + 1.0
        let newMin = min existingMin newTemperature
        let newMax = max existingMax newTemperature
        let newMean = (existingMean + newTemperature) / newCount

        resultMap |> Map.add station (newMin, newMean, newMax, newCount)

//TODO: Prob not readalllines, one a time is best
let parse (path: string) : Map<string, Min * Mean * Max * Count> =
    let initMap : Map<string, Min * Mean * Max * Count> = Map.empty
    
    let lines: string array = System.IO.File.ReadAllLines path

    (initMap, lines)
    ||> Array.fold (fun accMap line -> addLineToDict accMap line)


[<EntryPoint>]
let main args =
    if args.Length = 0 then
        printfn "Missing file path"
        1
    else
        let path: string = args[0]
        let resultMap = parse path

        printResult resultMap

        0