open Expecto
open Main.TruncateTags

let tests =
    let inputArgs =
        [| "v0.0.010 v0.0.009 v0.0.008 v0.0.007 v0.0.006 v0.0.005 v0.0.004 v0.0.003 v0.0.002 v0.0.001" |]

    let valuesToTake = 5

    testList
        "Truncated tags tests"
        [ test "one string" {
              let expected = inputArgs.[0].Split ' ' |> Array.take valuesToTake

              Expect.equal (getTruncatedTags valuesToTake inputArgs) expected "Does not work with one string"
          }

          test "many string" {
              let inputArgs = inputArgs.[0].Split ' '

              let expected = inputArgs |> Array.take valuesToTake

              Expect.equal (getTruncatedTags valuesToTake inputArgs) expected "Does not work with many strings"
          }

          test "insufficient one string" {
              let inputArgs = [| "v0.0.003 v0.0.002 v0.0.001" |]

              let expected = inputArgs.[0].Split ' '

              Expect.equal
                  (getTruncatedTags valuesToTake inputArgs)
                  expected
                  "Does not work with insufficient one string"
          }

          test "insufficient many strings" {
              let inputArgs = [| "v0.0.003 v0.0.002 v0.0.001" |].[0].Split ' '

              let expected = inputArgs

              Expect.equal
                  (getTruncatedTags valuesToTake inputArgs)
                  expected
                  "Does not work with insufficient many strings"
          } ]
    |> testLabel "getTruncatedTags"

[<EntryPoint>]
let main args = runTestsWithCLIArgs [] args tests
