namespace PersianNews.Engine
open System
open System.Collections.Generic
open System.Linq

module JsonLinesParser =
    open Newtonsoft.Json

    let ToLines (lines:string) = lines.Split([|'\n'|], StringSplitOptions.RemoveEmptyEntries)

    let ParseLines (jsonl:string) =
        seq{
            let lines = ToLines jsonl
            for str in lines do yield JsonConvert.DeserializeObject<'a>(str)
        }

module DataCleanser =
    open System.Text.RegularExpressions
    open System.Net
    let WrappedRegexPattern = @"(\(((.*){0}(.*))\))"
    let CreateWrappedRegex (inner:string) = String.Format(WrappedRegexPattern, inner)
    let ConcatPatterns = Seq.reduce (fun a b -> String.Format ("{0}|{1}", a, b))
    let RemoveTitleAnnotation str =
        let innerWords = [|"+"; @"\+"; "تصویر"; "عکس"; "فیلم"; "ویدئو"; "ویدیو"|]
        let pattern = innerWords |> Array.map CreateWrappedRegex |> ConcatPatterns
        let regex = Regex pattern
        regex.Replace (str, "")

    let CleanseJsonItem (jsonItem : JsonItem) =
        let j = jsonItem
        {
            NewsAgency = j.NewsAgency
            Category = Seq.last (j.NewsPathLinks.Keys)
            Title = RemoveTitleAnnotation j.Title
            RuTitr  = j.RuTitr
            Subtitle = j.Subtitle
            Body = WebUtility.HtmlDecode j.Body
            BodyHtml = WebUtility.HtmlDecode j.BodyHtml
            Tags = j.Tags.Keys |> Seq.toList
        }

    let CleanseJsonItems(jsonItems : JsonItem seq) =
        jsonItems |> Seq.map (fun x -> CleanseJsonItem x)