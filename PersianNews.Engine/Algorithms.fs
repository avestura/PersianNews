namespace PersianNews.Engine

module TfIdf =
    open System.Collections.Generic
    open Newtonsoft.Json
    open System.IO
    open System

    type TfDb = Dictionary<AsrIranCategories, Dictionary<string, int>>
    type DfDocHolder = Dictionary<string, HashSet<AsrDocument>>
    type DfDb = Dictionary<string, int>

    // W w,d = tf w,d * log N/df w,d
    // We have 20 categories
    let N = 15000.0

    let TfStore = TfDb ()
    let DfStore = DfDocHolder ()

    let mutable DfLoad = DfDb ()
    let mutable TfLoad = TfDb ()

    let LoadDb () =
        TfLoad <- JsonConvert.DeserializeObject<TfDb>(File.ReadAllText (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Data/TfIdf/tf.json")))
        DfLoad <- JsonConvert.DeserializeObject<DfDb>(File.ReadAllText (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Data/TfIdf/df.json")))

    let Tf word doc =
        if TfLoad.ContainsKey(doc) && TfLoad.[doc].ContainsKey(word) then TfLoad.[doc].[word]
        else 0

    let Df word =
        if DfLoad.ContainsKey(word) then DfLoad.[word]
        else 0

    let CalculateTfIdf word doc =
        let innerTf = Tf word doc
        let innerDf =  Df word
        let idf =
            if innerDf <> 0 then Math.Log (N / (float innerDf))
            else 0.0

        (float innerTf) * idf

module AgencyTfIdf =
    open System.Collections.Generic
    open Newtonsoft.Json
    open System.IO
    open System

    type TfDb = Dictionary<NewsAgency, Dictionary<string, int>>
    type DfDocHolder = Dictionary<string, HashSet<FormalDocument>>
    type DfDb = Dictionary<string, int>

    // W w,d = tf w,d * log N/df w,d
    // We have 20 categories
    let N = 15000.0

    let TfStore = TfDb ()
    let DfStore = DfDocHolder ()

    let mutable DfLoad = DfDb ()
    let mutable TfLoad = TfDb ()

    let LoadDb () =
        TfLoad <- JsonConvert.DeserializeObject<TfDb>(File.ReadAllText (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Data/AgencyTfIdf/tf.json")))
        DfLoad <- JsonConvert.DeserializeObject<DfDb>(File.ReadAllText (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Data/AgencyTfIdf/df.json")))

    let Tf word doc =
        if TfLoad.ContainsKey(doc) && TfLoad.[doc].ContainsKey(word) then TfLoad.[doc].[word]
        else 0

    let Df word =
        if DfLoad.ContainsKey(word) then DfLoad.[word]
        else 0

    let CalculateTfIdf word doc =
        let innerTf = Tf word doc
        let innerDf =  Df word
        let idf =
            if innerDf <> 0 then Math.Log (N / (float innerDf))
            else 0.0

        (float innerTf) * idf