module Tests

open System
open System.IO
open Xunit

[<Fact>]
let ``Program produces the correct output - 10 lines`` () =
    let currentDir = __SOURCE_DIRECTORY__
    let testFilePath = Path.Combine(currentDir, "input_10.txt")

    // Setup and read program output.
    use sw = new StringWriter()
    Console.SetOut(sw)
    Program.main [| testFilePath |] |> ignore

    let actual = sw.ToString().TrimEnd()

    let expected =
        """Bridgetown;26.9;26.9;26.9
Bulawayo;8.9;8.9;8.9
Conakry;31.2;31.2;31.2
Cracow;12.6;12.6;12.6
Hamburg;12.0;12.0;12.0
Istanbul;6.2;14.6;23.0
Palembang;38.8;38.8;38.8
Roseau;34.4;34.4;34.4
St. John's;15.2;15.2;15.2"""
    
    Assert.Contains(expected, actual)
