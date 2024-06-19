open System
open System.IO
open FSharp.Configuration
open FsToolkit.ErrorHandling

/// Helper functions to deal with GitHub environment.
module GitHubHelpers =
    let private VALUES_TO_TAKE = "VALUES_TO_TAKE"
    let private WORKFLOW_KEY = "WORKFLOW_KEY"

    let private tryInt (value: string) : Result<int, string> =
        try
            value |> int |> Ok
        with _exn ->
            Error $"{value} cannot be converted to int"

    let getValuesToTake () : Result<int, string> =
        let valuesToTake = Environment.GetEnvironmentVariable VALUES_TO_TAKE

        match String.IsNullOrEmpty valuesToTake with
        | true -> Error $"{VALUES_TO_TAKE} must not be null or empty"
        | false -> valuesToTake |> tryInt

    let getWorkflowKey () : Result<string, string> =
        let workflowKey = Environment.GetEnvironmentVariable WORKFLOW_KEY

        match String.IsNullOrEmpty workflowKey with
        | true -> Error $"{WORKFLOW_KEY} must not be null or empty"
        | false -> Ok workflowKey

module Validations =
    let validateTagsAreNotEmpty (tags: string array) : Result<unit, string> =
        match Array.isEmpty tags with
        | true -> Error "No tags found for this repository"
        | false -> Ok()

type RETURN_CODE =
    | SUCCESS = 0
    | FAIL = 1

let getWorkflowNewPath () =
    Path.Combine [| Directory.GetCurrentDirectory(); "workflow.new.yml" |]

type Workflow = YamlConfig<"workflow.temp.yml">

let handleMainResult (mainResult: Result<string, string>) : int =
    match mainResult with
    | Ok filePath ->
        printfn $"[SUCCESS] New options generated on path: {filePath}!"
        int RETURN_CODE.SUCCESS
    | Error err ->
        eprintfn $"[ERROR] {err}"
        int RETURN_CODE.FAIL

[<EntryPoint>]
let main (args: string array) : int =
    let TEMP_WORKFLOW_KEY = "TEMP_WORKFLOW_KEY%%"

    let workflow = Workflow()

    let workflowNewPath = getWorkflowNewPath ()

    result {
        let! valuesToTake = GitHubHelpers.getValuesToTake ()
        let! workflowKey = GitHubHelpers.getWorkflowKey ()

        let tags =
            match valuesToTake = -1 with
            | true -> args
            | false -> args |> Array.truncate valuesToTake
            |> Array.map (fun s -> s.Split(' ') |> Array.filter (String.IsNullOrEmpty >> not))
            |> Array.concat // flat

        do! Validations.validateTagsAreNotEmpty tags

        // remove temp values
        do workflow.on.workflow_dispatch.inputs.``TEMP_WORKFLOW_KEY%%``.options.Clear()

        // write the options from the repository tags
        do
            tags
            |> Array.iter workflow.on.workflow_dispatch.inputs.``TEMP_WORKFLOW_KEY%%``.options.Add

        // store this new workflow configuration
        do workflow.Save workflowNewPath

        // replace the TEMP_WORKFLOW_KEY by the key name informed by the user
        let workflowNewContent = File.ReadAllText workflowNewPath
        let workflowNewContent' = workflowNewContent.Replace(TEMP_WORKFLOW_KEY, workflowKey)
        do File.WriteAllText(workflowNewPath, workflowNewContent')

        // next -> use yq to merge the workflow with updated options to the already existent workflow

        return (workflowNewPath)
    }
    |> handleMainResult
