namespace PersianNews.Engine

open Newtonsoft.Json
open System.Collections.Generic

type NewsItem = {
    NewsAgency : string
    Category : string
    Title : string
    RuTitr : string
    Subtitle : string
    Body : string
    BodyHtml : string
    Tags : string list
}

type JsonItem = {
    [<JsonProperty "_id">]
    Id : string
    NewsAgency : string
    NewsLink : string
    Date : string
    NewsPath : string
    NewsPathLinks : Dictionary<string, string>
    Title : string
    RuTitr : string
    Subtitle : string
    Body : string
    BodyHtml : string
    Tags : Dictionary<string, string>
}

