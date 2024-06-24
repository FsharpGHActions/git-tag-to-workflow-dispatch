namespace Main

module TruncateTags =

    open System

    let getTruncatedTags (valuesToTake: int) (args: string array) : string array =
        let truncate (valuesToTake: int) (args: string array) : string array =
            match valuesToTake = -1 with
            | true -> args
            | false -> args |> Array.truncate valuesToTake

        args
        |> Array.map (fun s -> s.Split(' ') |> Array.filter (String.IsNullOrEmpty >> not))
        |> Array.concat // flat
        |> truncate valuesToTake
